import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { firstValueFrom } from 'rxjs';
import JSZip from 'jszip';

export interface Dashboard {
  name: string;
  id: number;
  public_uuid: string;
}

export interface MetabaseAuth {
  username?: string;
  password?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AppService {
  private readonly http = inject(HttpClient);
  private readonly sanitizer = inject(DomSanitizer);

  public get isAdmin(): boolean {
    return localStorage.getItem('isAdmin') === 'true';
  }

  public getBaseUrl(inputUrl: string, globalVar: any): string {
    const raw = inputUrl || (typeof globalVar !== 'undefined' ? globalVar : '');
    return String(raw).replace(/"/g, '').replace(/\/$/, '');
  }

  public getAdminUrl(baseUrl: string): string {
    return `${baseUrl}/admin/`;
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
      .filter((d: any) => d.public_uuid !== null)
    /* .map((d: any) => ({
      name: d.name,
      id: d.id,
      public_uuid: d.public_uuid
    }));
    */
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
