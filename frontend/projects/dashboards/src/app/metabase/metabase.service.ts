import { Observable, map } from 'rxjs';
import { 
    MetabaseDashboard, 
    TabMap, 
    TabCard,
    MetabaseTab,
    MetabaseAuth,
    Dashboard
} from './metabase.model';
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { firstValueFrom } from 'rxjs';
import JSZip from 'jszip';

@Injectable({
    providedIn: 'root'
})
export class MetabaseService {
    private readonly http = inject(HttpClient);
    private readonly sanitizer = inject(DomSanitizer);

    /**
     * Получить дашборд по ID
     */
    public async getDashboard(baseUrl: string, id: number, auth: MetabaseAuth): Promise<Observable<MetabaseDashboard>> {
        const session: any = await firstValueFrom(this.http.post(`${baseUrl}/api/session`, auth));
        const headers = new HttpHeaders().set('X-Metabase-Session', session.id);

        return this.http.get<MetabaseDashboard>(`${baseUrl}/api/dashboard/${id}`, {headers});
    }

    /**
     * Получить карточки, сгруппированные по вкладкам
     */
    public async getGroupedCardsByTabs(baseUrl: string, dashboardId: number, auth: MetabaseAuth): Promise<Observable<TabMap>> {
        return (await this.getDashboard(baseUrl, dashboardId, auth)).pipe(
            map(dashboard => this.groupCardsByTab(dashboard))
        );
    }

    /**
     * Сгруппировать карточки по вкладкам
     */
    public groupCardsByTab(dashboardData: MetabaseDashboard): TabMap {
        const tabMap: TabMap = {};
        
        dashboardData.tabs.forEach((tab: MetabaseTab) => {
            tabMap[tab.id] = {
                name: tab.name,
                cards: []
            };
        });

        dashboardData.dashcards.forEach(dashcard => {
            const tabId = dashcard.dashboard_tab_id;
            
            if (tabMap[tabId]) {
                tabMap[tabId].cards.push({
                    id: dashcard.card.id,
                    name: dashcard.card.name,
                    display: dashcard.card.display,
                    description: dashcard.card.description
                });
            }
        });
        
        return tabMap;
    }

    /**
     * Получить карточки по имени вкладки
     */
    getCardsByTabName(dashboardData: MetabaseDashboard, tabName: string): TabCard[] | null {
        const tabMap = this.groupCardsByTab(dashboardData);
        
        const tabEntry = Object.entries(tabMap).find(([_, tabInfo]) => 
            tabInfo.name === tabName
        );
        
        return tabEntry ? tabEntry[1].cards : null;
    }

    public getSafeDashboardUrl(baseUrl: string, dashboardId: string): SafeResourceUrl {
        const url = `${baseUrl}/public/dashboard/${dashboardId}`;
        return this.sanitizer.bypassSecurityTrustResourceUrl(url);
    }

  public getAuthCredentials(userVar: any, passVar: any): MetabaseAuth {
    return {
      username: (typeof userVar !== 'undefined' ? userVar : '').replace(/"/g, ''),
      password: (typeof passVar !== 'undefined' ? passVar : '').replace(/"/g, '')
    };
  }

  public async fetchDashboards(baseUrl: string, auth: MetabaseAuth): Promise<Dashboard[]> {
    const session: any = await firstValueFrom(this.http.post(`${baseUrl}/api/session`, auth));
    const headers = new HttpHeaders().set('X-Metabase-Session', session.id);
    const list: any = await firstValueFrom(this.http.get(`${baseUrl}/api/dashboard`, { headers }));

    return (list || [])
      .filter((d: any) => d.public_uuid !== null);
  }

  public async downloadDashboardData(baseUrl: string, dashId: number, format: string, auth: MetabaseAuth) {
    const zip = new JSZip();
    const session: any = await firstValueFrom(this.http.post(`${baseUrl}/api/session`, auth));
    const headers = new HttpHeaders().set('X-Metabase-Session', session.id);

    const dash: any = await firstValueFrom(this.http.get(`${baseUrl}/api/dashboard/${dashId}`, { headers }));

    const promises = dash.dashcards
      .filter((dc: any) => dc.card_id)
      .map(async (dc: any) => {
        const blob: any = await firstValueFrom(
          this.http.post(`${baseUrl}/api/card/${dc.card_id}/query/${format}`, {}, { headers, responseType: 'blob' })
        );
        zip.file(`${dc.card.name}.${format}`, blob);
      });

    await Promise.all(promises);
    const content = await zip.generateAsync({ type: 'blob' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(content);
    link.download = `dashboard_${dashId}_export.zip`;
    link.click();
  }
}