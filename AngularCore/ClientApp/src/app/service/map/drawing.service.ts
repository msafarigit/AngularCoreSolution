import { Injectable } from '@angular/core';
import { loadModules, setDefaultOptions } from 'esri-loader';
import esri = __esri; // Esri TypeScript Types - types/arcgis-js-api.

@Injectable()
export class DrawingService {

  readonly drawOptions = {
    Point: 'point',
    MultiPoint: 'multipoint',
    // Line: 'line',
    Polyline: 'polyline',
    Rectangle: 'rectangle',
    Polygon: 'polygon',
    // FreehandPolyline: 'freehandpolyline',
    // FreehandPolygon: 'freehandpolygon',
    // Arrow: 'arrow',
    // Triangle: 'triangle',
    Circle: 'circle',
    // Ellipse: 'ellipse'
    Move: 'move',
    Transform: 'transform',
    Reshape: 'reshape'
  };

  // drawOptions = {
  //   None: 'none',
  //   AutoCompletePolygon: 'auto-complete-polygon',
  //   Point: 'point',
  //   Line: 'line',
  //   Rectangle: 'rectangle',
  //   Polygon: 'polygon',
  //   Freehand: 'freehand',
  //   Arrow: 'arrow',
  //   LeftArrow: 'left-arrow',
  //   RightArrow: 'right-arrow',
  //   UpArrow: 'up-arrow',
  //   DownArrow: 'down-arrow',
  //   Triangle: 'triangle',
  //   Circle: 'circle',
  //   Ellipse: 'ellipse'
  // };

  toolbar: esri.SketchViewModel;

  constructor() { }

  async createToolbar(mapView: esri.MapView) {
    const [SketchViewModel] = await loadModules(['esri/widgets/Sketch/SketchViewModel']);
    this.toolbar = new SketchViewModel({ view: mapView });
  }

  async removeMapToolbarEventHandler(type: string) {
    const [on] = await loadModules(['dojo/on']);
    // Much like the DOM API, Dojo also provides a way to remove an event handler: _handle_.remove.
    // The return value of on is a simple object with a remove method, which will remove the event listener when called.
    // For example, if you wanted to have a one-time only event, you could do something like this:
    const handler = on(this.toolbar, type, () => handler.remove());
    let previousHandler;
    while ((previousHandler = handler.previous) !== undefined) {
      previousHandler.remove();
    }
  }

  async activateTool(drawOptions, pDrawEndFunction) {
    await this.removeMapToolbarEventHandler('draw-end');

    this.toolbar.create(drawOptions);
    // this.toolbar.on('draw-end', pDrawEndFunction);
  }

  deactivateTool() {
    // this.toolbar.deactivate();
  }
}
