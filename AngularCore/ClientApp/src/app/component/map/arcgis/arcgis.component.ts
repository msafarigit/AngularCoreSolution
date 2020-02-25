import { Component, OnInit, ViewChild, ElementRef, Input, Output, EventEmitter, OnDestroy, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { LocalStorageService, LocalStorage } from 'ngx-webstorage';
import { loadCss, loadModules } from 'esri-loader';
import esri = __esri;
import { APP_CONFIG, AppConfig } from 'app/app.config';
import { ToastrService } from 'ngx-toastr';

import { CoordinateSystemConversionService } from '@service/map/coordinate-system-conversion.service';
import { MapUtilityService } from '@service/map/map-utility.service';
import { DrawingService } from '@service/map/drawing.service';

@Component({
  selector: 'app-arcgis',
  templateUrl: './arcgis.component.html',
  styleUrls: ['./arcgis.component.scss']
})
export class ArcGisComponent implements OnInit, OnDestroy {
  @Output() mapLoadedEvent = new EventEmitter<boolean>();

  @ViewChild('mapViewNode', { static: true }) private mapViewEl: ElementRef;

  @LocalStorage('mapWebMercatorExtent')
  extent;

  private _zoom = 10;
  private _center: Array<number> = [0.1278, 51.5074];
  private basemap: string;
  private loaded = false;
  private mapView: esri.MapView = null;

  get mapLoaded(): boolean {
    return this.loaded;
  }

  @Input()
  set zoom(zoom: number) {
    this._zoom = zoom;
  }

  get zoom(): number {
    return this._zoom;
  }

  @Input()
  set center(center: Array<number>) {
    this._center = center;
  }

  get center(): Array<number> {
    return this._center;
  }

  @Input()
  set mapBasemap(basemap: string) {
    this.basemap = basemap;
  }

  get mapBasemap(): string {
    return this.basemap;
  }

  constructor(private route: ActivatedRoute, private http: HttpClient, private toastr: ToastrService,
    private storageService: LocalStorageService, @Inject(APP_CONFIG) private config: AppConfig,
    private coordinateSystemConversion: CoordinateSystemConversionService, private mapUtility: MapUtilityService,
    private drawingService: DrawingService) {

    this.route.queryParamMap.subscribe(queryParams => {
      if (queryParams.has('OnlineBaseMap')) {
        this.basemap = 'osm';
      }
    });

    // this.route.paramMap.subscribe(params => {
    //   const date = params['startdate'];
    // });

    loadCss(`${this.config.arcBaseUrl}/dijit/themes/claro/claro.css`);
    loadCss(`${this.config.arcBaseUrl}/esri/themes/light/main.css`);
  }

  async getMapExtent(): Promise<esri.Extent> {
    if (this.extent) {
      const [Extent] = await loadModules(['esri/geometry/Extent']);
      return new Extent(JSON.parse(this.extent));
    }

    const extentUrl = `/Proxy/proxy.ashx?${this.config.arcRestBaseUrl}/SAMA/MapServer/?f=json`;
    const extentPromise = await this.http.get<{ fullExtent: esri.Extent, error }>(extentUrl).toPromise();
    if (extentPromise.error) {
      throw new Error(`Read extent from server failed, Error Code: ${extentPromise.error.code}`);
    }

    this.mapUtility.setMapSpatialReference(extentPromise.fullExtent.spatialReference);

    let bottomWgs, topWgs;
    if (!extentPromise.fullExtent.spatialReference.wkid) {
      bottomWgs = this.coordinateSystemConversion.lambertIranToWGS(extentPromise.fullExtent.xmin, extentPromise.fullExtent.ymin)
        .split(';')
        .map(a => Number(a));
      topWgs = this.coordinateSystemConversion.lambertIranToWGS(extentPromise.fullExtent.xmax, extentPromise.fullExtent.ymax)
        .split(';')
        .map(a => Number(a));
    } else if (extentPromise.fullExtent.spatialReference.wkid.toString().startsWith('326')) {
      const utmZone = Number(extentPromise.fullExtent.spatialReference.wkid.toString().replace('326', ''));
      bottomWgs = this.coordinateSystemConversion.utmToWGS(extentPromise.fullExtent.xmin, extentPromise.fullExtent.ymin, utmZone, 'N')
        .split(';')
        .map(a => Number(a));
      topWgs = this.coordinateSystemConversion.utmToWGS(extentPromise.fullExtent.xmax, extentPromise.fullExtent.ymax, utmZone, 'N')
        .split(';')
        .map(a => Number(a));
    }

    const [webMercatorUtils, Point, Extent] = await loadModules(['esri/geometry/support/webMercatorUtils', 'esri/geometry/Point', 'esri/geometry/Extent']);

    const bPoint = new Point(bottomWgs[0], bottomWgs[1]);
    const tPoint = new Point(topWgs[0], topWgs[1]);
    const bwPoint = webMercatorUtils.geographicToWebMercator(bPoint);
    const twPoint = webMercatorUtils.geographicToWebMercator(tPoint);

    return new Extent({
      xmin: bwPoint.x,
      ymin: bwPoint.y,
      xmax: twPoint.x,
      ymax: twPoint.y,
      spatialReference: { wkid: 102100 }
    });
  }

  async getMapViewProperties(): Promise<esri.MapViewProperties> {
    const [EsriMap] = await loadModules(['esri/Map']);

    const mapProperties: esri.MapProperties = {
      basemap: this.basemap,
      layers: await this.getLayers()
    };

    const map: esri.Map = new EsriMap(mapProperties);

    const mapViewProperties: esri.MapViewProperties = {
      container: this.mapViewEl.nativeElement,
      map,
      ui: {
        components: []
      }
    };

    mapViewProperties.extent = await this.getMapExtent();
    mapViewProperties.spatialReference = mapViewProperties.extent.spatialReference;
    return mapViewProperties;
  }

  async getLayers(): Promise<esri.LayerProperties[]> {
    const [MapImageLayer] = await loadModules(['esri/layers/MapImageLayer']);
    const layer = new MapImageLayer({
      id: 'OSM_Offline',
      url: `${this.config.arcRestBaseUrl}/OSM_Offline/MapServer/`
    });

    const mainLayer = new MapImageLayer({
      id: 'GISLayer',
      url: `${this.config.arcRestBaseUrl}/SAMA/MapServer/`
    });

    // layer.when(function() {
    //   this.mapView.goTo(layer.fullExtent);
    // });

    return [layer, mainLayer];
  }

  async initializeMapView() {
    try {
      const [esriConfig, urlUtils, EsriMapView] = await loadModules(['esri/config', 'esri/core/urlUtils', 'esri/views/MapView']);

      esriConfig.request.proxyUrl = '/Proxy/proxy.ashx';
      esriConfig.request.alwaysUseProxy = true;

      const mapViewProperties: esri.MapViewProperties = await this.getMapViewProperties();

      this.mapView = new EsriMapView(mapViewProperties);
      await this.mapView.when();

      this.mapUtility.setMapView(this.mapView);
      this.drawingService.createToolbar(this.mapView);

      return this.mapView;
    } catch (error) {
      this.toastr.error((error as Error).message, 'Error:');
    }
  }

  ngOnInit() {
    this.initializeMapView().then(mapView => {
      console.log('mapView ready: ', this.mapView);
      this.loaded = this.mapView.ready;
      this.mapLoadedEvent.emit(true);
    });
  }

  ngOnDestroy() {
    if (this.mapView) {
      this.mapView.container = null;
    }
  }
}
