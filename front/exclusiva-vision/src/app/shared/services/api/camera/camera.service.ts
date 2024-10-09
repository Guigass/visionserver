import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Camera } from 'src/app/shared/models/camera.model';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CameraService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = `api/Cameras`;

  constructor() { }

  getCameras(): Observable<Camera[]> {
    return this.http.get<Camera[]>(this.apiUrl);
  }

  getCamera(id: string): Observable<Camera> {
    return this.http.get<Camera>(`${this.apiUrl}/${id}`);
  }

  createCamera(camera: Camera): Observable<Camera> {
    return this.http.post<Camera>(this.apiUrl, camera);
  }

  updateCamera(id: string, camera: Camera): Observable<Camera> {
    return this.http.put<Camera>(`${this.apiUrl}/${id}`, camera);
  }

  deleteCamera(id: string): Observable<Camera> {
    return this.http.delete<Camera>(`${this.apiUrl}/${id}`);
  }

  assignCameraClaim(cameraId: string, userId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${cameraId}/AssignClaim/${userId}`, {});
  }
}
