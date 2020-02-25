import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RTL } from '@progress/kendo-angular-l10n';
import { ToolBarModule } from '@progress/kendo-angular-toolbar';

import { MapRoutingModule } from './map-routing.module';
import { ArcGisComponent } from './arcgis/arcgis.component';
import { ToolbarComponent } from './toolbar/toolbar.component';

import { CoordinateSystemConversionService } from '@service/map/coordinate-system-conversion.service';
import { MapUtilityService } from '@service/map/map-utility.service';
import { DrawingService } from '@service/map/drawing.service';


@NgModule({
  declarations: [
    ArcGisComponent,
    ToolbarComponent
  ],
  imports: [
    CommonModule,
    ToolBarModule,
    MapRoutingModule
  ],
  providers: [
    { provide: RTL, useValue: true },
    CoordinateSystemConversionService,
    MapUtilityService,
    DrawingService
  ]
})
export class MapModule { }
