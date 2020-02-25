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
