import {
  Component,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges,
  ViewEncapsulation,
  inject,
  ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { TuiTabs } from '@taiga-ui/kit';

interface DashboardItem {
  name: string;
  mftid: string;
}

declare const __DASHBOARDS_CONFIG__: string;
declare const __METABASE_URL__: string;

@Component({
  selector: 'app-dashboards-root',
  standalone: true,
  imports: [CommonModule, TuiTabs],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AppComponent implements OnInit, OnChanges {
  @Input() config: string = '';
  @Input('metabase-url') metabaseUrl: string = '';

  private sanitizer = inject(DomSanitizer);
  private cdr = inject(ChangeDetectorRef);

  dashboards: DashboardItem[] = [];
  _activeIndex = 0;
  safeUrl?: SafeResourceUrl;

  get activeIndex(): number {
    return this._activeIndex;
  }

  set activeIndex(value: number) {
    this._activeIndex = value;
    this.updateIframeUrl();
  }

  ngOnInit() {
    this.processConfig();
  }

  ngOnChanges(changes: SimpleChanges) {
    this.processConfig();
  }

  private processConfig() {
    let rawConfig = this.config;

    if (!rawConfig) {
      try {
        rawConfig = __DASHBOARDS_CONFIG__;
      } catch (e) {
        console.warn(e);
      }
    }

    if (!rawConfig) {
      console.error('Dashboard configuration not found');
      return;
    }

    rawConfig = rawConfig.replace(/^"|"$/g, '');

    this.dashboards = this.parseEnvString(rawConfig);
    if (this.dashboards.length > 0) {
      this.updateIframeUrl();
    }
    this.cdr.detectChanges();
  }

  private updateIframeUrl() {
    const activeDash = this.dashboards[this.activeIndex];
    let baseUrl = this.metabaseUrl;

    if (!baseUrl) {
      try {
        baseUrl = __METABASE_URL__.replace(/^"|"$/g, '');
      } catch (e) {
        console.warn(e);
      }
    }

    if (activeDash?.mftid) {
      const rawUrl = `${baseUrl}/public/dashboard/${activeDash.mftid}`;
      this.safeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(rawUrl);
    }
  }

  private parseEnvString(str: string): DashboardItem[] {
    if (!str) return [];
    return str.split(';').reduce((acc: any[], item) => {
      const parts = item.split('=');
      if (parts.length < 2) return acc;

      const [rawKey, value] = [parts[0].trim(), parts[1].trim()];
      const [key, indexStr] = rawKey.split('__');
      const idx = parseInt(indexStr, 10);

      if (!acc[idx]) acc[idx] = {};

      const k = key.toLowerCase();
      if (k === 'name') acc[idx].name = value;
      else if (k === 'mftid') acc[idx].mftid = value;

      return acc;
    }, []).filter(d => d.name && d.mftid);
  }
}
