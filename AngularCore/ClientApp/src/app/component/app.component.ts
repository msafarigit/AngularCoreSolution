import { Component, Inject } from '@angular/core';
import { APP_CONFIG, AppConfig } from 'app/app.config';
import { loadScript, setDefaultOptions } from 'esri-loader';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  host: {
    '[class]': '"row h-100"'
  },
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'app';

  constructor(@Inject(APP_CONFIG) private config: AppConfig) {
    setDefaultOptions({ url: this.config.arcBaseUrl });

    loadScript({
      dojoConfig: {
        baseUrl: `${this.config.arcBaseUrl}/dojo`,
        async: true
      }
    });
  }
}
