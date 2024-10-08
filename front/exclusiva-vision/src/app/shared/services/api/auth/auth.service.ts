import { NavController } from '@ionic/angular/standalone';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, debounceTime, map } from 'rxjs';
import { User } from 'src/app/shared/models/user.model';
import { StorageService } from '../../storage/storage.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Login } from 'src/app/shared/models/login.model';
import { Claim } from 'src/app/shared/models/claim.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly httpClient = inject(HttpClient);
  private readonly storage = inject(StorageService);
  private readonly jwtHelper = inject(JwtHelperService);
  private readonly navCtrl = inject(NavController);

  private isAuthenticatedBool = false;
  private isAuthenticated$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(this.isAuthenticatedBool);
  public isAuthenticated = this.isAuthenticated$.asObservable().pipe(debounceTime(10));

  private userObject?: User;
  private user$: BehaviorSubject<User> = new BehaviorSubject<User>(this.userObject!);
  public user = this.user$.asObservable().pipe(debounceTime(10));

  constructor() {
    this.init();
  }

  private init() {
    try {
      const uiat = this.storage.get('at');

      const isExpired = this.jwtHelper.isTokenExpired(uiat!);

      this.isAuthenticatedBool = !isExpired;
      this.isAuthenticated$.next(this.isAuthenticatedBool);

      if (!isExpired) {
        const user = this.storage.getObject<User>('ui');
        this.userObject = user;
        this.user$.next(user!);
      }
    } catch (error) {
      this.storage.clearAll();
    }
  }

  login(email: string, password: string, remember: boolean = false) {
    return this.httpClient.post('api/Auth/Login', { email, password }).pipe(
      map((login: any) => {

        this.saveLogin(login.data, remember);

        return login;
      })
    );
  }

  private saveLogin(login: Login, remember: boolean = false) {
    const typeStorage = remember ? 'local' : 'session';

    this.storage.set('at', login.accessToken, typeStorage);
    this.storage.setObject('ui', login.user, typeStorage);
    this.storage.setObject('cla', login.user.claims, typeStorage);

    this.isAuthenticatedBool = true;
    this.isAuthenticated$.next(this.isAuthenticatedBool);

    this.userObject = login.user;
    this.user$.next(this.userObject);
  }

  logOff() {
    this.isAuthenticated$.next(false);
    this.isAuthenticatedBool = false;

    this.user$.next(null!);
    this.userObject = undefined;

    this.storage.remove('at');
    this.storage.remove('ui');
    this.storage.remove('cla');

    this.navCtrl.navigateRoot('/');
  }

  checkAuthenticated(): boolean {
    try {
      const uiat = this.storage.get('at');

      const isExpired = this.jwtHelper.isTokenExpired(uiat!);

      this.isAuthenticatedBool = !isExpired;
      this.isAuthenticated$.next(!isExpired);

      return !isExpired;
    } catch (error) {
      this.storage.clearAll();
      this.navCtrl.navigateRoot('/');

      return false;
    }
  }

  public hasClaim(type: string, value: string): boolean {
    const claims = this.storage.getObject<Claim[]>('cla');

    if (this.isAuthenticatedBool && claims) {
      if(claims!.some(s => s.type === type && s.value.includes(value))) {
        return true;
      }
    }

    return false;
  }
}
