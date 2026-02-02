import { Component, OnInit, CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TuiRoot, TuiButton } from '@taiga-ui/core';
import { TuiTabs } from '@taiga-ui/kit';

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
  imports: [
    CommonModule,
    TuiRoot,
    TuiTabs,
    TuiButton
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  remotes: RemoteConfig[] = [];
  activeMft: RemoteConfig | null = null;
  metabaseUrl = process.env['METABASE_URL'] || 'http://localhost:3000';

  get metabaseAdminUrl(): string {
    return `${this.metabaseUrl}/admin/`;
  }

  get activeIndex(): number {
    return this.activeMft ? this.remotes.indexOf(this.activeMft) : 0;
  }

  set activeIndex(index: number) {
    this.activeMft = this.remotes[index];
  }

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
      } catch (err) {
        console.error(`Error loading ${mft.name}:`, err);
      }
    }
  }

  private parseComplexEnv(str: string): RemoteConfig[] {
    if (!str) return [];
    return str.split(';').reduce((acc: any[], item) => {
      const [rawKey, value] = item.split('=');
      if (!rawKey || !value) return acc;
      const [key, index] = rawKey.trim().split('__');
      const idx = parseInt(index, 10);
      if (!acc[idx]) acc[idx] = {};

      const k = key.toLowerCase();
      if (k === 'remoteurl') acc[idx].url = value;
      else if (k === 'mftid') acc[idx].mftid = value;
      else if (k === 'name') acc[idx].name = value;
      else if (k === 'path') acc[idx].path = value;
      return acc;
    }, []).filter(Boolean);
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
