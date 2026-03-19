import 'zone.js';
import { createApplication } from '@angular/platform-browser';
import { createCustomElement } from '@angular/elements';
import { provideAnimations } from '@angular/platform-browser/animations';
import { NG_EVENT_PLUGINS } from '@taiga-ui/event-plugins';
import { AppComponent } from './app/app.component';
import { provideHttpClient } from '@angular/common/http';

(async () => {
  try {
    const app = await createApplication({
      providers: [
        provideAnimations(),
        provideHttpClient(),
        NG_EVENT_PLUGINS,
      ],
    });

    const dashboardElement = createCustomElement(AppComponent, {
      injector: app.injector,
    });

    if (!customElements.get('dashboards-mft')) {
      customElements.define('dashboards-mft', dashboardElement);
    }
  } catch (err) {
    console.error('Ошибка инициализации Web Component:', err);
  }
})();
