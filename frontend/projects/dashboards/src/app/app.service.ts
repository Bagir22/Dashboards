import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

export interface Dashboard {
  name: string;
  mftid: string;
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

  public cleanUrl(url: any): string {
    if (!url) return '';
    return String(url).replace(/"/g, '').replace(/\/$/, '');
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
        mftid: d.public_uuid
      }));
  }
}
