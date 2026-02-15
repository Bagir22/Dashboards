import 'zone.js';
import { createApplication } from '@angular/platform-browser';
import { createCustomElement } from '@angular/elements';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideEventPlugins } from '@taiga-ui/event-plugins';
import { AppComponent } from './app/app.component';
import { provideHttpClient } from '@angular/common/http';
import { importProvidersFrom } from '@angular/core'; // Добавьте этот импорт

(async () => {
  try {
    const app = await createApplication({
      providers: [
        provideAnimations(),
        provideHttpClient(),
        provideEventPlugins(), // Используем модуль
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