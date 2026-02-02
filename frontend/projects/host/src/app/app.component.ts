import { Component, OnInit, CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core'; // 1. Добавили NO_ERRORS_SCHEMA
import { CommonModule } from '@angular/common';

interface RemoteConfig {
  name: string;
  path: string;
  url: string;
  mftid: string;
}

declare var process: { env: { [key: string]: string } };

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  remotes: RemoteConfig[] = [];
  activeMft: RemoteConfig | null = null;

  async ngOnInit() {
    const rawEnv = process.env['DASHBOARDS'] || '';
    this.remotes = this.parseComplexEnv(rawEnv);

    if (this.remotes.length > 0) {
      this.activeMft = this.remotes[0];
    }

    for (const mft of this.remotes) {
      if (!mft.url) continue;
      try {
        await this.loadRemoteScript(`${mft.url}/remoteEntry.js`);
        await this.loadRemoteScript(`${mft.url}/main.js`);

        await new Function(`return import('${mft.url}/remoteEntry.js')`)();

        console.log(`✅ Loaded: ${mft.name}`);
      } catch (err) {
        console.error(`❌ Error loading ${mft.name}:`, err);
      }
    }
  }

  private parseComplexEnv(str: string): RemoteConfig[] {
    if (!str) return [];
    const items = str.split(';').map(s => s.trim());
    const tempStorage: any = {};

    items.forEach(item => {
      const [rawKey, value] = item.split('=');
      if (!rawKey || !value) return;

      const [key, index] = rawKey.split('__');
      if (!tempStorage[index]) tempStorage[index] = {};

      const normalizedKey = key.toLowerCase();
      if (normalizedKey === 'remoteurl') tempStorage[index].url = value;
      else if (normalizedKey === 'mftid') tempStorage[index].mftid = value;
      else if (normalizedKey === 'name') tempStorage[index].name = value;
      else if (normalizedKey === 'path') tempStorage[index].path = value;
    });

    return Object.values(tempStorage) as RemoteConfig[];
  }


  private loadRemoteScript(url: string): Promise<void> {
    return new Promise((resolve, reject) => {
      if (document.querySelector(`script[src="${url}"]`)) return resolve();
      const script = document.createElement('script');
      script.src = url;
      script.type = 'text/javascript';
      script.async = true;
      script.onload = () => resolve();
      script.onerror = () => reject(new Error(`Script load error for ${url}`));
      document.head.appendChild(script);
    });
  }
}
