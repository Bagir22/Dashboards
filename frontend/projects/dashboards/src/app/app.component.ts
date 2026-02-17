import {
  Component,
  Input,
  OnInit,
  inject,
  ChangeDetectorRef,
  ViewEncapsulation
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { TuiButton, TuiTextfield } from '@taiga-ui/core';
import { TuiTabs } from '@taiga-ui/kit';
import { AppService, Dashboard } from './app.service';

declare const METABASE_URL: string;
declare const METABASE_USER: string;
declare const METABASE_PASS: string;

@Component({
  selector: 'app-dashboards-root',
  standalone: true,
  imports: [CommonModule, TuiTabs, TuiButton, FormsModule, TuiTextfield],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AppComponent implements OnInit {
  @Input('metabase-url') public metabaseUrl: string = '';

  private readonly appService = inject(AppService);
  private readonly sanitizer = inject(DomSanitizer);
  private readonly cdr = inject(ChangeDetectorRef);

  public dashboards: Dashboard[] = [];
  public activeIndex: number = 0;
  public safeUrl?: SafeResourceUrl;
  public isLoading: boolean = true;
  public searchQuery: string = '';

  public ngOnInit(): void {
    void this.initialize();
  }

  public get filteredDashboards(): Dashboard[] {
    const query = this.searchQuery.toLowerCase().trim();
    return query
      ? this.dashboards.filter(d => d.name.toLowerCase().includes(query))
      : this.dashboards;
  }

  public get isAdmin(): boolean {
    return this.appService.isAdmin;
  }

  public get adminUrl(): string {
    const baseUrl = this.appService.getBaseUrl(this.metabaseUrl, METABASE_URL);
    return this.appService.getAdminUrl(baseUrl);
  }

  public onTabClick(index: number): void {
    const selected = this.filteredDashboards[index];
    this.activeIndex = this.dashboards.findIndex(d => d.mftid === selected.mftid);
    this.updateIframe();
  }

  private async initialize(): Promise<void> {
    const baseUrl = this.appService.getBaseUrl(this.metabaseUrl, METABASE_URL);

    if (!baseUrl) {
      console.error('METABASE_URL не определен');
      this.isLoading = false;
      return;
    }

    const auth = this.appService.getAuthCredentials(METABASE_USER, METABASE_PASS);

    try {
      this.dashboards = await this.appService.fetchDashboards(baseUrl, auth);
      if (this.dashboards.length > 0) {
        this.updateIframe();
      }
    } catch (e) {
      console.error('Ошибка загрузки дашбордов:', e);
    } finally {
      this.isLoading = false;
      this.cdr.detectChanges();
    }
  }

  public updateIframe(): void {
    const active = this.dashboards[this.activeIndex];
    const baseUrl = this.appService.getBaseUrl(this.metabaseUrl, METABASE_URL);

    if (active?.mftid && baseUrl) {
      const url = `${baseUrl}/public/dashboard/${active.mftid}`;
      this.safeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
      this.cdr.detectChanges();
    }
  }
}
