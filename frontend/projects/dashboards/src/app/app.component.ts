import { Component, Input, OnInit, inject, ChangeDetectorRef, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SafeResourceUrl } from '@angular/platform-browser';
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
  private readonly cdr = inject(ChangeDetectorRef);

  public dashboards: Dashboard[] = [];
  public activeIndex: number = 0;
  public safeUrl?: SafeResourceUrl;
  public isLoading: boolean = true;
  public searchQuery: string = '';

  public ngOnInit(): void {
    void this.initialize();
  }

  public get isAdmin(): boolean { return this.appService.isAdmin; }

  public get adminUrl(): string {
    return this.appService.getAdminUrl(this.baseUrl);
  }

  public get filteredDashboards(): Dashboard[] {
    const query = this.searchQuery.toLowerCase().trim();
    return query ? this.dashboards.filter(d => d.name.toLowerCase().includes(query)) : this.dashboards;
  }

  public get filteredActiveIndex(): number {
    const current = this.dashboards[this.activeIndex];
    return this.filteredDashboards.findIndex(d => d.id === current?.id);
  }

  public onSearchChange(): void {
    const filtered = this.filteredDashboards;
    if (filtered.length === 0) {
      this.safeUrl = undefined;
      return;
    }
    const currentInFiltered = filtered.find(d => d.id === this.dashboards[this.activeIndex]?.id);
    if (!currentInFiltered) {
      this.onTabClick(0);
    } else if (!this.safeUrl) {
      this.updateIframe();
    }
  }

  public onTabClick(index: number): void {
    const selected = this.filteredDashboards[index];
    if (!selected) return;

    const newIndex = this.dashboards.findIndex(d => d.id === selected.id);

    if (this.activeIndex === newIndex && this.safeUrl) return;

    this.activeIndex = newIndex;
    this.updateIframe();
  }

  private get baseUrl(): string {
    return this.appService.getBaseUrl(this.metabaseUrl, METABASE_URL);
  }

  private async initialize(): Promise<void> {
    if (!this.baseUrl) {
      this.isLoading = false;
      return;
    }
    const auth = this.appService.getAuthCredentials(METABASE_USER, METABASE_PASS);
    try {
      this.dashboards = await this.appService.fetchDashboards(this.baseUrl, auth);
      if (this.dashboards.length > 0) {
        this.activeIndex = 0;
        this.updateIframe();
      }
    } catch (e) {
      console.error('Ошибка загрузки:', e);
    } finally {
      this.isLoading = false;
      this.cdr.detectChanges();
    }
  }

  private updateIframe(): void {
    const active = this.dashboards[this.activeIndex];
    if (active) {
      this.safeUrl = this.appService.getSafeDashboardUrl(this.baseUrl, active.id);
      this.cdr.detectChanges();
    }
  }
}
