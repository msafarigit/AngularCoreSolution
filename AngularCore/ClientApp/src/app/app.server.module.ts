import { NgModule } from '@angular/core';
import { ServerModule } from '@angular/platform-server';
// import { ModuleMapLoaderModule } from '@nguniversal/module-map-ngfactory-loader'; update to angular 10
import { AppComponent } from '@component/app.component';
import { AppModule } from './app.module';

@NgModule({
    // imports: [AppModule, ServerModule, ModuleMapLoaderModule], update to angular 10
    imports: [AppModule, ServerModule],
    bootstrap: [AppComponent]
})
export class AppServerModule { }
