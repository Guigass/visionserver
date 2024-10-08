import { DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { IonCard, IonCardHeader, IonCardTitle } from "@ionic/angular/standalone";
import { Subscription, timer } from 'rxjs';


@Component({
  selector: 'app-clock',
  templateUrl: './clock.component.html',
  styleUrls: ['./clock.component.scss'],
  standalone: true,
  imports: [IonCardTitle, IonCardHeader, IonCard, DatePipe]
})
export class ClockComponent implements OnDestroy{
  clockSubscription: Subscription;

  clock = signal(new Date());

  constructor() { 
    this.clockSubscription = timer(0, 1000).subscribe(() => {
      this.clock.set(new Date());
    })
  }
  ngOnDestroy(): void {
    this.clockSubscription.unsubscribe();
  }
}
