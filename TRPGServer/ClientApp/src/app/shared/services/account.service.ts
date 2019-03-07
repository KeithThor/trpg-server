import { HttpClient } from "@angular/common/http";
import { Router } from "@angular/router";
import { Injectable } from "@angular/core";
import { User } from "../model/user.model";
import { LoginData } from "../model/login-data.model";
import { JwtHelperService } from "@auth0/angular-jwt";
import { LocalStorageConstants } from "../../constants";

@Injectable()
export class AccountService {
  constructor(private http: HttpClient,
    private router: Router) {

  }
  private jwtHelper = new JwtHelperService();

  isAuthenticated(): boolean {
    if (localStorage.getItem(LocalStorageConstants.username) != null) {
      return !this.jwtHelper.isTokenExpired(
        localStorage.getItem(LocalStorageConstants.authToken));
    }

    return false;
  }

  login(user: User, redirectUrl: string) {
    let routerB = this.router;

    this.http.post<LoginData>("/api/account/login", user).subscribe({
      next(result) {
        localStorage.setItem(LocalStorageConstants.authToken, result.token);
        localStorage.setItem(LocalStorageConstants.username, result.username);
          },
      error(err) { console.log(err); },
      complete() { routerB.navigate([redirectUrl]); }
    });
  }

  register(user: User, redirectUrl: string) {
    let routerB = this.router;

    this.http.post<LoginData>("/api/account/register", user).subscribe({
      next(result) {
        localStorage.setItem(LocalStorageConstants.authToken, result.token);
        localStorage.setItem(LocalStorageConstants.username, result.username);
      },
      error(err) { console.log(err); },
      complete() { routerB.navigate([redirectUrl]); }
    });
  }

  logout(): void {
    localStorage.clear();
  }
}
