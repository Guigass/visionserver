import { Injectable, NgZone } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  constructor(private ngZone: NgZone) {}

  private getStorage(type: 'local' | 'session' = 'local') {
    return type === 'local' ? localStorage : sessionStorage;
  }

  set(key: string, value: string, type: 'local' | 'session' = 'local') {
    if (typeof(Storage) === 'undefined') {
      return;
    }
  
    // Verifica se a chave já existe em algum storage
    const existsInOtherStorage = type === 'local' 
      ? sessionStorage.getItem(key) !== null 
      : localStorage.getItem(key) !== null;
  
    if (!existsInOtherStorage) {
      this.ngZone.runOutsideAngular(() => {
        this.getStorage(type).setItem(key, value);
      });
    } else {
      console.warn(`A chave "${key}" já está sendo usada no outro storage.`);
    }
  }
  
  setObject(key: string, object: any, type: 'local' | 'session' = 'local') {
    if (typeof(Storage) === 'undefined') {
      return;
    }
  
    // Verifica se a chave já existe em algum storage
    const existsInOtherStorage = type === 'local' 
      ? sessionStorage.getItem(key) !== null 
      : localStorage.getItem(key) !== null;
  
    if (!existsInOtherStorage) {
      this.ngZone.runOutsideAngular(() => {
        const ecObject = JSON.stringify(object);
        this.getStorage(type).setItem(key, ecObject);
      });
    } else {
      console.warn(`A chave "${key}" já está sendo usada no outro storage.`);
    }
  }

  remove(key: string) {
    if (typeof(Storage) === 'undefined') {
      return;
    }
  
    // Tenta remover do sessionStorage primeiro
    if (sessionStorage.getItem(key) !== null) {
      sessionStorage.removeItem(key);
    } else if (localStorage.getItem(key) !== null) {
      // Se não encontrou no sessionStorage, tenta no localStorage
      localStorage.removeItem(key);
    }
  }

  get(key: string): string {
    if (typeof(Storage) === 'undefined') {
      return '';
    }
  
    let value = sessionStorage.getItem(key);
    if (value === null) {
      value = localStorage.getItem(key);
    }
    return value ? value : '';
  }
  
  getObject<T>(key: string): T | undefined {
    if (typeof(Storage) === 'undefined') {
      return undefined;
    }
  
    let value = sessionStorage.getItem(key);
    if (value === null) {
      value = localStorage.getItem(key);
    }
  
    try {
      return value ? JSON.parse(value) : undefined;
    } catch (error) {
      sessionStorage.clear();
      localStorage.clear();
      
      return undefined;
    }
  }
  
  check(key: string): boolean {
    if (typeof(Storage) === 'undefined') {
      return false;
    }
  
    const hasValueInSession = !!sessionStorage.getItem(key);
    const hasValueInLocal = !!localStorage.getItem(key);
    return hasValueInSession || hasValueInLocal;
  }


  clearAll() {
    if (typeof(Storage) === 'undefined') {
      return;
    }
  
    sessionStorage.clear();
    localStorage.clear();
  }
}