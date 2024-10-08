import { inject, Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Subscription, timer } from 'rxjs';
import { shareReplay, switchMap } from 'rxjs/operators';
import { AppFocusService } from '../app-focus/app-focus.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class InternetSpeedService implements OnDestroy {
  private readonly appFocusService = inject(AppFocusService);

  private speedSubject = new BehaviorSubject<string>('test');
  private subscription: Subscription | null = null;
  private idleSubscription: Subscription | null = null;

  private readonly testInterval = 15000;
  private testUrl = `/assets/logos/logo-vertical-negativo.png`

  speed$ = this.speedSubject.asObservable().pipe(shareReplay(1));

  constructor() {

    this.idleSubscription = this.appFocusService.isIdle.subscribe(isIdle => {
      if (isIdle) {
        this.speedSubject.next('not-monitoring');
        this.subscription?.unsubscribe();
      } else {
        this.startMonitoring();
      }
    });
  }

  private checkInternetSpeed(url: string): Promise<number | null> {
    return new Promise((resolve, reject) => {
      let startTime: number, endTime: number;
      const img = new Image();

      img.onload = () => {
        endTime = new Date().getTime();
        const duration = (endTime - startTime) / 1000; // tempo em segundos

        // Ignorar os primeiros 10ms para uma estimativa mais otimista
        const adjustedDuration = duration > 0.01 ? duration - 0.01 : duration;

        const fileSizeInBytes = 28700; // tamanho do arquivo em bytes
        const speedBps = (fileSizeInBytes * 8) / adjustedDuration; // bits por segundo

        // Adicionar um pequeno fator de otimização
        const optimisticFactor = 1.2;
        const speedKbps = (speedBps / 1024) * optimisticFactor; // Kilobits por segundo
        const speedMbps = speedKbps / 1024; // Megabits por segundo

        resolve(speedMbps);
      };

      img.onerror = () => {
        console.error("Erro ao carregar o arquivo.");
        resolve(null);
      };

      startTime = new Date().getTime();
      img.src = `${url}?cacheBuster=${startTime}`;
    });
  }

  private assessSpeed(speedMbps: number | null): string {
    const speedCategories = [
      { threshold: 0.5, label: "very-slow" },
      { threshold: 1, label: "slow" },
      { threshold: 5, label: "average" },
      { threshold: 10, label: "good" }
    ];

    if (speedMbps === null) {
      return "error";
    }

    for (const category of speedCategories) {
      if (speedMbps < category.threshold) {
        return category.label;
      }
    }

    return "fast";
  }

  private startMonitoring(): void {
    if (this.testUrl) {
      this.subscription = timer(0, this.testInterval).pipe(
        switchMap(() => this.checkInternetSpeed(this.testUrl))
      ).subscribe(speed => {
        const result = this.assessSpeed(speed);
        this.speedSubject.next(result);
      });
    } else {
      console.error('A URL de teste ainda não foi definida.');
    }
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
    this.idleSubscription?.unsubscribe();
  }
}
