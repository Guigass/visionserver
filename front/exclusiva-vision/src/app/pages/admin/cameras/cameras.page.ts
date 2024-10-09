import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonMenuButton, IonContent, IonHeader, IonTitle, IonToolbar, IonButtons, IonItem, IonList, IonLabel } from '@ionic/angular/standalone';
import { CameraService } from 'src/app/shared/services/api/camera/camera.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';

@Component({
  selector: 'app-cameras',
  templateUrl: './cameras.page.html',
  styleUrls: ['./cameras.page.scss'],
  standalone: true,
  imports: [IonLabel, IonList, IonItem, IonMenuButton, IonButtons, IonContent, IonHeader, IonTitle, IonToolbar, CommonModule, FormsModule]
})
export class CamerasPage {
  private readonly cameraService = inject(CameraService);

  cameras = toSignal(this.cameraService.getCameras().pipe(map(resp => resp.data)));

  constructor() { }
}
