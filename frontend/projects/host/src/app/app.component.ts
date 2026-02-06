import { Component, OnInit, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TuiRoot, TuiButton } from '@taiga-ui/core';

interface RemoteConfig {
  name: string;
  url: string;
  mftid: string;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, TuiRoot, TuiButton],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  metabaseUrl = process.env['METABASE_URL'];
  mftUrl = process.env['MFT_URL'];

  ngOnInit() {
    this.loadRemoteMft();
  }

  private loadRemoteMft() {
    const script = document.createElement('script');
    script.src = `${this.mftUrl}/main.js`;
    script.type = 'module';
    document.head.appendChild(script);
  }
}
