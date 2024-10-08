import { DOCUMENT } from '@angular/common';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, debounceTime, fromEvent, Subscription, timer } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppFocusService {
  private hasFocus$: BehaviorSubject<boolean> = new BehaviorSubject(this.document.hasFocus());
  public hasFocus = this.hasFocus$.asObservable();

  private idle = !this.document.hasFocus();
  private isIdleSubscription!: Subscription;
  private isIdle$: BehaviorSubject<boolean> = new BehaviorSubject(this.idle);
  public isIdle = this.isIdle$.asObservable().pipe(debounceTime(1000));
  
  window;

  constructor(@Inject(DOCUMENT) private document: Document) { 
    this.window = document.defaultView;

    this.init();
  }

  init() {
    const win = document.defaultView;

    fromEvent(this.window!, 'blur', { passive: true })
    .pipe(debounceTime(250))
    .subscribe((evt: any) => {
      this.initIdleCount();

      this.hasFocus$.next(false);
    });

  fromEvent(this.window!, 'focus', { passive: true })
    .pipe(debounceTime(250))
    .subscribe((evt: any) => {
      this.isIdleSubscription?.unsubscribe();

      this.hasFocus$.next(true);

      if (this.idle) {
        this.idle = false;
        this.isIdle$.next(false);
      }
    });
  }

  private initIdleCount() {
    this.isIdleSubscription = timer(300000).subscribe(() => {
      this.idle = true;
      this.isIdle$.next(true);
    });
  }
}
