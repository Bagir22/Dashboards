import 'zone.js';

import { createApplication } from '@angular/platform-browser';
import { createCustomElement } from '@angular/elements';
import { AppComponent } from './app/app.component';

(async () => {
  try {
    const app = await createApplication({
      providers: [
      ],
    });

    const dashboardElement = createCustomElement(AppComponent, {
      injector: app.injector,
    });

    if (!customElements.get('dashboard-mft')) {
      customElements.define('dashboard-mft', dashboardElement);
    }
  } catch (err) {
    console.log(err);
  }
})();
