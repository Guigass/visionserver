import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CameraEditPage } from './camera-edit.page';

describe('CameraEditPage', () => {
  let component: CameraEditPage;
  let fixture: ComponentFixture<CameraEditPage>;

  beforeEach(() => {
    fixture = TestBed.createComponent(CameraEditPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
