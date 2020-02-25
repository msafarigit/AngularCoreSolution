import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules, NoPreloading, ExtraOptions } from '@angular/router';

import { DelayPreloadingStrategy } from '@service/delay-preloading-strategy.service';

import { HomeComponent } from '@component/home/home.component';
import { CounterComponent } from '@component/counter/counter.component';
import { FetchDataComponent } from '@component/fetch-data/fetch-data.component';

const routerOptions: ExtraOptions = {
  scrollPositionRestoration: 'enabled',
  anchorScrolling: 'enabled',
  // scrollOffset: [0, 64],
  preloadingStrategy: DelayPreloadingStrategy
};

const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'counter', component: CounterComponent },
  { path: 'fetch-data', component: FetchDataComponent },
  {
    path: 'baseInfo', loadChildren: () => import('@component/baseInfo/base-info.module').then(m => m.BaseInfoModule),
    data: { preload: true, delay: 8000 }
  },
  {
    path: 'map', loadChildren: () => import('@component/map/map.module').then(m => m.MapModule),
    data: { preload: true, delay: 4000 }
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, routerOptions)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
