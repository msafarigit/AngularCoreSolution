import { InjectionToken } from '@angular/core';

export const APP_CONFIG = new InjectionToken<AppConfig>('app.config');

export interface AppConfig {
  title: string;
  version: string;
  arcRestBaseUrl: string;
  arcBaseUrl: string;
  arcApiEndpoint: string;
  apiEndpoint: string;
}

export const APP_DI_CONFIG: AppConfig = {
  title: 'سامانه نیما',
  version: '3.0.1.0',
  arcRestBaseUrl: 'http://192.168.0.129:6080/arcgis/rest/services',
  arcBaseUrl: 'http://192.168.0.129/arcgis_js_v414_api/arcgis_js_api/library/4.14',
  arcApiEndpoint: 'api.arc.com',
  apiEndpoint: '/api/'
};
