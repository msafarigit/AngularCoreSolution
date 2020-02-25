import { Injectable } from '@angular/core';
import { PreloadingStrategy, Route } from '@angular/router';
import { Observable, timer, of } from 'rxjs';
import { flatMap } from 'rxjs/operators'

@Injectable({
  providedIn: 'root'
})
export class DelayPreloadingStrategy implements PreloadingStrategy {

  constructor() { }

  // { path: 'baseInfo', loadChildren: () => import('@component/baseInfo/base-info.module').then(m => m.BaseInfoModule),
  //   data: { preload: true, delay: 5000 } }
  preload(route: Route, load: () => Observable<any>): Observable<any> {
    if (route.data && route.data.preload) {
      const delay: number = route.data.delay;
      return timer(delay).pipe(flatMap(_ => load()));
    } else {
      return of(null);
    }
  }
}

/*
There are two injector hierarchies in Angular:
  1- ModuleInjector hierarchy—configure a ModuleInjector in this hierarchy using an @NgModule() or @Injectable() annotation.
  2- ElementInjector hierarchy—created implicitly at each DOM element.
     An ElementInjector is empty by default unless you configure it in the providers property on @Directive() or @Component().

ModuleInjector
The ModuleInjector can be configured in one of two ways:
  1- Using the @Injectable() providedIn property to refer to @NgModule(), or root.
  2- Using the @NgModule() providers array.

Using the @Injectable() providedIn property is preferable to the @NgModule() providers array because with @Injectable() providedIn,
optimization tools can perform tree-shaking, which removes services that your app isn't using and results in smaller bundle sizes.
Tree-shaking is especially useful for a library because the application which uses the library may not have a need to inject it.
Read more about tree-shakable providers in DI Providers.

The @Injectable() decorator identifies a service class.
The providedIn property configures a specific ModuleInjector, here root, which makes the service available in the root ModuleInjector.
*/
