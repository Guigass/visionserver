import { Component } from '@angular/core';
import { IonApp, IonRouterOutlet } from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { create, person, library, add, albums, alertCircleOutline, apps, camera, chevronBack, chevronForward, close, ellipsisHorizontal, ellipsisVertical, exit, expand, eye, grid, home, hourglass, pause, pencil, play, reload, settings, trash, videocam, volumeHigh, volumeMute, createOutline, trashOutline } from 'ionicons/icons';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  standalone: true,
  imports: [IonApp, IonRouterOutlet],
})
export class AppComponent {
  constructor() {
    addIcons({
      play,
      pause,
      volumeHigh,
      volumeMute,
      expand,
      camera,
      reload,
      home,
      settings,
      apps,
      grid,
      videocam,
      albums,
      eye,
      hourglass,
      add,
      ellipsisVertical,
      pencil,
      trash,
      trashOutline,
      close,
      exit,
      alertCircleOutline,
      chevronBack,
      chevronForward,
      library,
      person,
      createOutline
    });
  }
}
