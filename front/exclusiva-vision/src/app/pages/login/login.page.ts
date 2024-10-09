import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { IonInput, IonInputPasswordToggle, IonContent, IonHeader, IonTitle, IonToolbar, IonCardHeader, IonCard, IonCardTitle, IonCardContent, IonItem, IonButton, IonLabel, IonToggle } from '@ionic/angular/standalone';
import { NavController, AlertController, LoadingController } from '@ionic/angular/standalone';
import { AuthService } from 'src/app/shared/services/api/auth/auth.service';
import { take } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.scss'],
  standalone: true,
  imports: [
    IonInput,
    IonToggle, 
    IonLabel, 
    IonButton, 
    IonItem, 
    IonCardContent, 
    IonCardTitle, 
    IonCard, 
    IonCardHeader, 
    IonContent, 
    IonHeader, 
    IonTitle, 
    IonToolbar, 
    CommonModule, 
    FormsModule,
    IonInputPasswordToggle,
    ReactiveFormsModule
  ]
})
export class LoginPage {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly navController = inject(NavController);
  private readonly alertController = inject(AlertController);
  private readonly loadingController = inject(LoadingController);
  private readonly route = inject(ActivatedRoute);

  loginForm: FormGroup;

  redirectUrl: string | undefined;

  constructor() { 
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
      remember: [false]
    });

    this.redirectUrl = this.route.snapshot.queryParams['redirectUrl'] || '/';

    this.authService.isAuthenticated.pipe(take(1)).subscribe((isAuthenticated) => {
      if(isAuthenticated){
        this.navController.navigateRoot(['/']);
      }
    });
  }

  async onSubmit() {
    if (this.loginForm.valid) {
      const { email, password, remember } = this.loginForm.value;

      this.loadingController.create({
        message: 'Autenticando...',
      }).then(loading => {
        loading.present();

        this.authService.login(email, password, remember).subscribe(() => {
          if (this.redirectUrl) {
            this.navController.navigateRoot([this.redirectUrl]);
          } else {
            this.navController.navigateRoot(['/']);
          }
  
          this.loadingController.dismiss();
        }, () => {
          this.loadingController.dismiss();
  
          this.alertController.create({
            header: 'Ops!',
            message: 'E-mail ou senha inválidos.',
            buttons: ['OK']
          }).then(alert => alert.present());
        });
      });
    }
  }
}
