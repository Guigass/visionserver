import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterModule } from '@angular/router';
import { IonMenu, IonRouterLink, IonSplitPane, IonContent, IonToolbar, IonImg, IonLabel, IonItem, IonFooter, IonPopover, IonIcon, IonRouterOutlet, IonList, IonHeader } from "@ionic/angular/standalone";

@Component({
  selector: 'app-admin-layout',
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.scss'],
  standalone: true,
  imports: [
    IonPopover,
    IonFooter,
    IonHeader,
    IonToolbar,
    IonImg,
    IonRouterOutlet,
    IonLabel,
    IonList,
    IonItem,
    IonIcon,
    IonContent,
    IonSplitPane,
    IonMenu,
    RouterLink,
    IonRouterLink,
  ]
})
export class AdminLayoutComponent {

  constructor() { }

  logout() {
    
  }

}
