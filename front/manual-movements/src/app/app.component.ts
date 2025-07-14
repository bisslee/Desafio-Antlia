import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MovementMainComponent } from './components/movement-main/movement-main.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, MovementMainComponent],
  template: `
    <app-movement-main></app-movement-main>
  `,
  styles: []
})
export class AppComponent {
  title = 'manual-movements';
}
