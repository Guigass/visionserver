import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { NgGlyph, NgIconComponent, provideIcons } from '@ng-icons/core';
import { matSignalCellular0Bar, matSignalCellular1Bar, matSignalCellular2Bar, matSignalCellular3Bar, matSignalCellular4Bar, matSignalCellularConnectedNoInternet0Bar } from '@ng-icons/material-icons/baseline';
import { Subscription } from 'rxjs';
import { InternetSpeedService } from '../../services/internet-speed/internet-speed.service';

@Component({
  selector: 'app-internet-state-icon',
  templateUrl: './internet-state-icon.component.html',
  styleUrls: ['./internet-state-icon.component.scss'],
  standalone: true,
  imports: [NgIconComponent, NgGlyph],
  providers: [provideIcons({ matSignalCellular0Bar, matSignalCellular1Bar, matSignalCellular2Bar, matSignalCellular3Bar, matSignalCellular4Bar, matSignalCellularConnectedNoInternet0Bar })],
})
export class InternetStateIconComponent implements OnDestroy {
  private readonly internetSpeedService = inject(InternetSpeedService);

  icon = signal('');
  color = signal('');
  speedText = signal('');

  internetSpeedSubscription: Subscription;
  constructor() {
    this.internetSpeedSubscription = this.internetSpeedService.speed$.subscribe((speed) => {
      switch (speed) {
        case 'error':
          this.icon.set('matSignalCellularConnectedNoInternet0Bar');
          this.color.set('red');
          this.speedText.set('Sem Conexão com o Servidor');
          break;
        case 'very-slow':
          this.icon.set('matSignalCellular0Bar');
          this.color.set('red');
          this.speedText.set('Conexão com o Servidor Muito Lenta');
          break;
        case 'slow':
          this.icon.set('matSignalCellular1Bar');
          this.color.set('red');
          this.speedText.set('Conexão com o Servidor Lenta');
          break;
        case 'average':
          this.icon.set('matSignalCellular2Bar');
          this.color.set('yellow');
          this.speedText.set('Conexão com o Servidor Média');
          break;
        case 'good':
          this.icon.set('matSignalCellular3Bar');
          this.color.set('green');
          this.speedText.set('Conexão com o Servidor Boa');
          break;
        case 'fast':
          this.icon.set('matSignalCellular4Bar');
          this.color.set('green');
          this.speedText.set('Conexão com o Servidor Rápida');
          break;
          case 'not-monitoring':
            this.icon.set('matSignalCellular4Bar');
            this.color.set('gray');
            this.speedText.set('Sem Monitoramento');
            break;
      }
    });

  }

  ngOnDestroy(): void {
    this.internetSpeedSubscription.unsubscribe();
  }
}
