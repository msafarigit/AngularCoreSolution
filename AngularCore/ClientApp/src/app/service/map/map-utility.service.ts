import { Injectable } from '@angular/core';
import { LocalStorageService, LocalStorage } from 'ngx-webstorage';
import { ToastrService } from 'ngx-toastr';
import { loadModules, setDefaultOptions } from 'esri-loader';
import esri = __esri; // Esri TypeScript Types - types/arcgis-js-api.
import { CoordinateSystemConversionService } from '@service/map/coordinate-system-conversion.service';

@Injectable()
export class MapUtilityService {

  @LocalStorage('mapUtmExtent')
  mapExtentFromLocalStorage;

  mapView: esri.MapView;
  mapSpatialReference: esri.SpatialReference = null;

  constructor(private toastr: ToastrService, private storageService: LocalStorageService,
    private coordinateSystemConversion: CoordinateSystemConversionService) { }

  setMapView(mapView: esri.MapView): MapUtilityService {
    this.mapView = mapView;
    return this;
  }

  setMapSpatialReference(spatialReference: esri.SpatialReference): MapUtilityService {
    this.mapSpatialReference = spatialReference;
    return this;
  }

  addMapEventHandler(type: string | string[], handler: (event: any) => void): void {
    this.mapView.on(type, handler);
  }

  async removeMapEventHandler(type: string) {
    const [on] = await loadModules(['dojo/on']);
    const handler = on(this.mapView, type, () => handler.remove());
    let previousHandler;
    while ((previousHandler = handler.previous) !== undefined) {
      previousHandler.remove();
    }
  }

  async removeMapClickHandler(): Promise<void> {
    await this.removeMapEventHandler('click');
  }

  async setMapClickHandler(handler: (event: esri.MapViewClickEvent) => void): Promise<void> {
    await this.removeMapClickHandler();
    this.addMapEventHandler('click', handler);
  }

  async removeMapPopupEventHandler(type: string) {
    const [on] = await loadModules(['dojo/on']);
    const handler = on(this.mapView.popup, type, () => handler.remove());
    let previousHandler;
    while ((previousHandler = handler.previous) !== undefined) {
      previousHandler.remove();
    }
  }

  async removeMapPopupSelectionChangeHandler(): Promise<void> {
    await this.removeMapPopupEventHandler('selection-change'); // TODO: warning
  }

  async setMapPopupSelectionChangeHandler(handler: (event: esri.PopupTriggerActionEvent) => void): Promise<void> {
    await this.removeMapPopupSelectionChangeHandler();
    this.mapView.popup.on('trigger-action', (event) => {
      if (event.action.id === 'selection-change') { // TODO: warning
        handler(event);
      }
    });
  }

  onMapExtentChange() {
    try {
      // if (typeof (Storage) === 'undefined') return;
      this.storageService.store('mapWebMercatorExtent', JSON.stringify(this.mapView.extent));

      // [deprecated = use nes method to set previos map extent]
      // store map extent on local storage (with UTM projection)
      // format: xmin, ymin, xmax, ymax
      // let extentForLocalStorage;
      // if (this.mapView.spatialReference.wkid === 102113 || this.mapView.spatialReference.wkid === 102100) {
      //   if (isNaN(+this.mapView.geographicExtent.xmin) || isNaN(+this.mapView.geographicExtent.ymin) ||
      //     isNaN(+this.mapView.geographicExtent.xmax) || isNaN(+this.mapView.geographicExtent.ymax))
      //     return;
      //   let Zone = { zone: 0 };
      //   var maxCoor = CoordinateSystemConversion.WGStoUTM(that.map.geographicExtent.xmax, that.map.geographicExtent.ymax, Zone)
      //     .split(',');
      //   var minCoor = CoordinateSystemConversion.WGStoUTM(that.map.geographicExtent.xmin, that.map.geographicExtent.ymin, Zone)
      //     .split(',');
      //   extentForLocalStorage = minCoor[0] + ',' + minCoor[1] + ',' + maxCoor[0] + ',' + maxCoor[1];
      // } else {
      //   if (!$.isNumeric(this.mapView.extent.xmax) ||
      //     !$.isNumeric(this.mapView.extent.ymax) ||
      //     !$.isNumeric(this.mapView.extent.xmin) ||
      //     !$.isNumeric(this.mapView.extent.ymin))
      //     return;

      //   extentForLocalStorage = this.mapView.extent.xmin + ','
      //     + this.mapView.extent.ymin + ','
      //     + this.mapView.extent.xmax + ','
      //     + this.mapView.extent.ymax;
      // }
      // localStorage.setItem('mapUtmExtent', extentForLocalStorage);
    } catch (error) {
      this.toastr.error((error as Error).message, 'Error:');
    }
  }

  private calculateMapUtmZone(): string {
    let wkid = '';
    if (this.mapSpatialReference) {
      wkid = this.mapSpatialReference.wkid.toString();
    }
    else {
      const gisLayer = this.mapView.map.layers.find((layer: esri.Layer) => layer.id === 'GISLayer');
      if (gisLayer.fullExtent.spatialReference.wkid) {
        wkid = gisLayer.fullExtent.spatialReference.wkid.toString();
      }
    }

    let mapUtmZone: string;
    if (wkid) {
      if (wkid.substring(0, 3) === '326')
        mapUtmZone = wkid.substring(wkid.length - 2, wkid.length);
    }
    else {
      mapUtmZone = 'LAMBERT';
    }
    return mapUtmZone;
  }

  private async calculateMapFullExtentInWGS(): Promise<esri.Extent> {
    const [Extent, SpatialReference] = await loadModules(['esri/geometry/Extent', 'esri/SpatialReference']);
    const gisLayerFullExtent = this.mapView.map.layers.find((layer: esri.Layer) => layer.id === 'GISLayer').fullExtent;

    const mapUtmZone = this.calculateMapUtmZone();
    let minCoor, maxCoor;
    if (mapUtmZone === 'LAMBERT') {
      minCoor = this.coordinateSystemConversion.lambertIranToWGS(gisLayerFullExtent.xmin, gisLayerFullExtent.ymin)
        .split(';');
      maxCoor = this.coordinateSystemConversion.lambertIranToWGS(gisLayerFullExtent.xmax, gisLayerFullExtent.ymax)
        .split(';');
    }
    else {
      minCoor = this.coordinateSystemConversion.utmToWGS(gisLayerFullExtent.xmin, gisLayerFullExtent.ymin, +mapUtmZone, 'N')
        .split(';');
      maxCoor = this.coordinateSystemConversion.utmToWGS(gisLayerFullExtent.xmax, gisLayerFullExtent.ymax, +mapUtmZone, 'N')
        .split(';');
    }

    return new Extent(minCoor[0], minCoor[1], maxCoor[0], maxCoor[1], new SpatialReference({ wkid: 4326 }));
  }

  async zoomToFullExtent(): Promise<void> {
    if (!this.mapView.spatialReference.wkid || this.mapView.spatialReference.wkid.toString().substring(0, 3) === '326') {
      this.mapView.extent = this.mapView.map.layers[0].fullExtent;
    }
    else {
      const mapFullExtentWgs: esri.Extent = await this.calculateMapFullExtentInWGS();
      this.mapView.extent = mapFullExtentWgs;
    }
  }
}
