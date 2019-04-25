import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Router } from "@angular/router";
import { Injectable } from "@angular/core";
import { User } from "../model/user.model";
import { LoginData } from "../model/login-data.model";
import { JwtHelperService } from "@auth0/angular-jwt";
import { LocalStorageConstants } from "../../constants";

/** Service responsible for sending authentication requests to the server. */
@Injectable()
export class AccountService {
  constructor(private http: HttpClient,
    private router: Router) {

  }
  private jwtHelper = new JwtHelperService();

  /**Returns true if the client is currently authenticated. If the client is not authenticated, will redirect the
   * client to the login page.*/
  isAuthenticated(): boolean {
    if (sessionStorage.getItem(LocalStorageConstants.username) != null) {
      return !this.jwtHelper.isTokenExpired(
        sessionStorage.getItem(LocalStorageConstants.authToken));
    }

    return false;
  }

  /**
   * Logs a client in using the provided user data.
   * @param user The data of the user to log in.
   * @param redirectUrl The url to redirect to once the user has been authenticated.
   * @param errorHandler The function to call to handle the error in case there are any.
   */
  public login(user: User, redirectUrl: string, errorHandler?: (err: HttpErrorResponse) => void): void {
    let routerB = this.router;

    this.http.post<LoginData>("/api/account/login", user).subscribe({
      next(result) {
        sessionStorage.setItem(LocalStorageConstants.authToken, result.token);
        sessionStorage.setItem(LocalStorageConstants.username, result.username);
      },
      error(err: HttpErrorResponse) {
        if (errorHandler != null) errorHandler(err);
        else console.log(err);
      },
      complete() { routerB.navigate([redirectUrl]); }
    });
  }

  /**
   * Registers a new user using the provided user data.
   * @param user The data of the user to register.
   * @param redirectUrl The url to redirect to once the user has been created.
   * @param errorHandler The function to call to handle the error in case there are any.
   */
  public register(user: User, redirectUrl: string, errorHandler?: (err: HttpErrorResponse) => void): void {
    let routerB = this.router;

    this.http.post<LoginData>("/api/account/register", user).subscribe({
      next(result) {
        sessionStorage.setItem(LocalStorageConstants.authToken, result.token);
        sessionStorage.setItem(LocalStorageConstants.username, result.username);
      },
      error(err: HttpErrorResponse) {
        if (errorHandler != null) errorHandler(err);
        else console.log(err);
      },
      complete() { routerB.navigate([redirectUrl]); }
    });
  }

  /**Clears the user's JWT from session storage and redirects them to the home page. */
  logout(): void {
    sessionStorage.clear();
    this.router.navigate(['/']);
  }
}
