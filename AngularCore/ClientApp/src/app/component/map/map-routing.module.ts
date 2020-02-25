import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ArcGisComponent } from './arcgis/arcgis.component';

const routes: Routes = [
  { path: '', redirectTo: 'arcGis', pathMatch: 'full' },
  { path: 'arcGis', component: ArcGisComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MapRoutingModule { }
