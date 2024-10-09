import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponse } from 'src/app/shared/models/api-response.modal';
import { Camera } from 'src/app/shared/models/camera.model';

@Injectable({
  providedIn: 'root'
})
export class CameraService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = `api/Cameras`;

  constructor() { }

  getCameras(): Observable<ApiResponse<Camera[]>> {
    return this.http.get<ApiResponse<Camera[]>>(this.apiUrl);
  }

  getCamera(id: string): Observable<ApiResponse<Camera>> {
    return this.http.get<ApiResponse<Camera>>(`${this.apiUrl}/${id}`);
  }

  createCamera(camera: Camera): Observable<ApiResponse<Camera>> {
    return this.http.post<ApiResponse<Camera>>(this.apiUrl, camera);
  }

  updateCamera(id: string, camera: Camera): Observable<ApiResponse<Camera>> {
    return this.http.put<ApiResponse<Camera>>(`${this.apiUrl}/${id}`, camera);
  }

  deleteCamera(id: string): Observable<ApiResponse<Camera>> {
    return this.http.delete<ApiResponse<Camera>>(`${this.apiUrl}/${id}`);
  }

  assignCameraClaim(cameraId: string, userId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${cameraId}/AssignClaim/${userId}`, {});
  }
}
