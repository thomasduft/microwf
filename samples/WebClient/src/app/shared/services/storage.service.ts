import { Injectable } from '@angular/core';

import { ServicesModule } from './services.module';

@Injectable({
  providedIn: ServicesModule
})
export class StorageService {
  /**
   * Sets the session storage item.
   *
   */
  public setSessionItem<T>(key: string, item: T): void {
    sessionStorage.setItem(key, JSON.stringify(item));
  }

  /**
   * Gets the session storage item.
   *
   * @returns { T }
   */
  public getSessionItem<T>(key: string): T {
    return <T>JSON.parse(sessionStorage.getItem(key));
  }

  /**
   * Removes the session storage item.
   *
   */
  public removeSessionItem(key: string): void {
    sessionStorage.removeItem(key);
  }

  /**
   * Sets the local storage item.
   *
   */
  public setItem<T>(key: string, item: T): void {
    localStorage.setItem(key, JSON.stringify(item));
  }

  /**
   * Gets the local storage item.
   *
   * @returns { T }
   */
  public getItem<T>(key: string): T {
    return <T>JSON.parse(localStorage.getItem(key));
  }

  /**
   * Removes the local storage item.
   *
   */
  public removeItem(key: string): void {
    localStorage.removeItem(key);
  }
}
