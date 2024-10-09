import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AlertController, IonToggle, IonSelect, IonSelectOption, IonContent, IonHeader, IonTitle, IonToolbar, IonList, IonLabel, IonItem, IonButton, IonText, IonItemDivider, IonNote, IonInput, LoadingController } from '@ionic/angular/standalone';
import { CameraService } from 'src/app/shared/services/api/camera/camera.service';

@Component({
  selector: 'app-camera-edit',
  templateUrl: './camera-edit.page.html',
  styleUrls: ['./camera-edit.page.scss'],
  standalone: true,
  imports: [
    IonSelect,
    IonInput,
    IonNote,
    IonSelectOption,
    IonItemDivider,
    IonText,
    ReactiveFormsModule,
    IonButton,
    IonItem,
    IonLabel,
    IonList,
    IonContent,
    IonHeader,
    IonTitle,
    IonToolbar,
    IonToggle
  ]
})
export class CameraEditPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly cameraService = inject(CameraService);
  private loadingCtrl = inject(LoadingController);
  private alertCtrl = inject(AlertController);

  cameraForm: FormGroup;

  constructor() {
    this.cameraForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      rtspUrl: ['', [Validators.required]],
      isActive: [true],
      width: [null, [Validators.min(1)]],
      height: [null, [Validators.min(1)]],
      framerate: [null, [Validators.min(1)]],
      bitrate: [null, [Validators.min(1)]],
      analyzeDuration: ['', [Validators.maxLength(5)]],
      probesize: [''],
      zeroLatency: [false],
      noBuffer: [false],
      gop: [null, [Validators.min(1)]],
      bufferSize: [null, [Validators.min(1)]],
      threads: [null, [Validators.min(1)]],
      vsync: [false],
      useServerTimestamps: [false],
      videoCodec: ['copy', [Validators.maxLength(30)]],
      preset: ['ultrafast', [Validators.maxLength(30)]],
      crf: [null, [Validators.min(0), Validators.max(51)]],
      audioEnabled: [false],
      audioCodec: ['', [Validators.maxLength(10)]],
      audioBitrate: [null, [Validators.min(1)]],
      audioChannels: [null, [Validators.min(1)]],
      hlsTime: [2, [Validators.required, Validators.min(1)]],
      hlsListSize: [5, [Validators.required, Validators.min(1)]],
      rtspTransport: ['udp', [Validators.required]],
    });
  }

  ngOnInit() {
  }

  async onSubmit() {
    this.cameraForm.markAllAsTouched();

    if (this.cameraForm.valid) {
      const loading = await this.loadingCtrl.create({ message: 'Salvando...' });

      const formData = this.cameraForm.value;

      this.cameraService.createCamera(formData).subscribe(async (resp) => {
        await loading.dismiss();

        const alert = await this.alertCtrl.create({
          header: 'Sucesso!',
          message: 'Camera salva com sucesso.',
          buttons: ['OK'],
        });

        alert.present();
      }, async (err) => {
        const alert = await this.alertCtrl.create({
          header: 'Erro!',
          message: 'Ocorreu um erro ao salvar a camera. Por favor, tente novamente.',
          buttons: ['OK'],
        });

        await alert.present();
      }).add(async () => {
        await loading.dismiss();
      });
    }
  }
}
