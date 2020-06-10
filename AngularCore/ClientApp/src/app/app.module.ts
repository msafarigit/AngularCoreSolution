import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { CookieService } from 'ngx-cookie-service';
import { NgxWebstorageModule } from 'ngx-webstorage';
import { ToastrModule } from 'ngx-toastr';

import { APP_CONFIG, APP_DI_CONFIG } from './app.config';

import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from '@component/app.component';
import { HomeComponent } from '@component/home/home.component';
import { CounterComponent } from '@component/counter/counter.component';
import { FetchDataComponent } from '@component/fetch-data/fetch-data.component';
import { NavbarComponent } from './navbar/navbar.component';

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    BrowserAnimationsModule,
    NgbModule,
    AppRoutingModule,
    ToastrModule.forRoot({
      timeOut: 3000,
      positionClass: 'toast-bottom-right',
      preventDuplicates: true
    }),
    NgxWebstorageModule.forRoot({
      prefix: 'ngx-webstorage',
      separator: '|',
      caseSensitive: false
    }),
    FormsModule
  ],
  providers: [
    CookieService,
    { provide: APP_CONFIG, useValue: APP_DI_CONFIG }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

/*
url: https://www.typescriptlang.org/
Starting with ECMAScript 2015, JavaScript has a concept of modules. TypeScript shares this concept.
Modules are executed within their own scope, not in the global scope; this means that variables, functions, classes, etc.
declared in a module are not visible outside the module unless they are explicitly exported using one of the export forms.
Conversely, to consume a variable, function, class, interface, etc.
exported from a different module, it has to be imported using one of the import forms.

Module imports are resolved differently based on whether the module reference is relative or non-relative.
A relative import is one that starts with /, ./ or ../. Some examples include:
  import Entry from "./components/Entry";
Any other import is considered non-relative. Some examples include:
  import * as $ from "jquery";
  import { Component } from "@angular/core";
*/
