import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { firstValueFrom } from 'rxjs';

export interface Dashboard {
  name: string;
  id: string;
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
    const session: any = await firstValueFrom(
      this.http.post(`${baseUrl}/api/session`, auth)
    );
    const headers = new HttpHeaders().set('X-Metabase-Session', session.id);
    const list: any = await firstValueFrom(
      this.http.get(`${baseUrl}/api/dashboard`, { headers })
    );

    return (list || [])
      .filter((d: any) => d.public_uuid !== null)
      .map((d: any) => ({
        name: d.name,
        id: d.public_uuid
      }));
  }
}
