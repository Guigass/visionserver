import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { IonRouterLink, IonSplitPane, IonContent, IonNote, IonIcon, IonItem, IonListHeader, IonList, IonLabel, IonRouterOutlet, IonMenu, IonImg, IonToolbar, IonHeader, IonAccordion, IonAccordionGroup, IonButton, IonFooter, IonPopover, IonCardHeader, IonCard, IonCardTitle, IonCardSubtitle, IonCardContent, IonText, IonBadge } from "@ionic/angular/standalone";
import { ClockComponent } from 'src/app/shared/components/clock/clock.component';
import { InternetStateIconComponent } from 'src/app/shared/components/internet-state-icon/internet-state-icon.component';

@Component({
  selector: 'app-app-layout',
  templateUrl: './app-layout.component.html',
  styleUrls: ['./app-layout.component.scss'],
  standalone: true,
  imports: [
    IonBadge,
    IonText,
    IonCardContent,
    IonCardSubtitle,
    IonCardTitle,
    IonCard,
    IonCardHeader,
    IonPopover,
    IonFooter,
    IonButton,
    IonAccordionGroup,
    IonAccordion,
    IonHeader,
    IonToolbar,
    IonImg,
    IonRouterOutlet,
    IonLabel,
    IonList,
    IonListHeader,
    IonItem,
    IonIcon,
    IonNote,
    IonContent,
    IonSplitPane,
    IonMenu,
    RouterModule,
    IonRouterLink,
    ClockComponent,
    InternetStateIconComponent
  ]
})
export class AppLayoutComponent implements OnInit {

  constructor() { }

  ngOnInit() { }

  logout() {

  }

}
