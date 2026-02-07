import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

export interface Dashboard {
  name: string;
  mftid: string;
}

@Injectable({
  providedIn: 'root'
})
export class AppService {
  private http = inject(HttpClient);

  async fetchDashboards(baseUrl: string, auth: any): Promise<Dashboard[]> {
    const session: any = await firstValueFrom(
      this.http.post(`${baseUrl}/api/session`, auth)
    );

    const headers = new HttpHeaders().set('X-Metabase-Session', session.id);
    const list: any = await firstValueFrom(
      this.http.get(`${baseUrl}/api/dashboard`, { headers })
    );

    return list
      .filter((d: any) => d.public_uuid !== null)
      .map((d: any) => ({
        name: d.name,
        mftid: d.public_uuid
      }));
  }
}
