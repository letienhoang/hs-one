import {Injectable} from '@angular/core';
import {UserModel} from '../models/user.model';

const TOKEN_KEY = 'auth-token';
const REFRESHTOKEN_KEY = 'auth-refreshtoken';
const USER_KEY = 'auth-user';

@Injectable({
  providedIn: 'root',
})
export class TokenStorageService {
    constructor() { }
    signOut(): void {
        window.localStorage.clear();
    }

    public saveToken(token: string): void {
        window.localStorage.removeItem(TOKEN_KEY);
        window.localStorage.setItem(TOKEN_KEY, token);
        const user = this.getUser();
        if (user?.id) {
            this.saveUser({ ...user, accessToken: token });
        }
    }

    public getToken(): string | null {
        return window.localStorage.getItem(TOKEN_KEY);
    }

    public saveRefreshToken(refreshtoken: string): void {
        window.localStorage.removeItem(REFRESHTOKEN_KEY);
        window.localStorage.setItem(REFRESHTOKEN_KEY, refreshtoken);
    }

    public getRefreshToken(): string | null {
        return window.localStorage.getItem(REFRESHTOKEN_KEY);
    }

    public saveUser(user: any): void {
        window.localStorage.removeItem(USER_KEY);
        window.localStorage.setItem(USER_KEY, JSON.stringify(user));
    }

    public getUser(): UserModel | null {
        const token = window.localStorage.getItem(USER_KEY);
        if (!token)
            return null;
        const baseUrl = token.split('.')[1];
        const base64 = baseUrl.replace(/-/, '+').replace(/_/, '/');
        const user: UserModel = JSON.parse(this.b64DecodeUnicode(base64));
        return user;
    }

    private b64DecodeUnicode(str: string): string {
        return decodeURIComponent(Array.prototype.map.call(atob(str), (c: string) => {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
    }
}