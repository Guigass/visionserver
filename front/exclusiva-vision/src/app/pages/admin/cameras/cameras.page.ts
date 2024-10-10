import { Component, inject, OnInit } from '@angular/core';
import { IonRouterLink, IonMenuButton, IonContent, IonHeader, IonTitle, IonToolbar, IonButtons, IonItem, IonList, IonLabel, IonFabButton, IonFab, IonIcon, IonItemSliding, IonItemOption, IonItemOptions } from '@ionic/angular/standalone';
import { CameraService } from 'src/app/shared/services/api/camera/camera.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-cameras',
  templateUrl: './cameras.page.html',
  styleUrls: ['./cameras.page.scss'],
  standalone: true,
  imports: [
    IonItemOptions, 
    IonItemOption, 
    IonItemSliding, 
    IonIcon, 
    IonFab, 
    IonFabButton, 
    IonLabel, 
    IonList, 
    IonItem, 
    IonMenuButton, 
    IonButtons, 
    IonContent, 
    IonHeader, 
    IonTitle, 
    IonToolbar, 
    IonRouterLink,
    RouterLink
  ]
})
export class CamerasPage {
  private readonly cameraService = inject(CameraService);

  cameras = toSignal(this.cameraService.getCameras().pipe(map(resp => resp.data)));

  constructor() { }
}
