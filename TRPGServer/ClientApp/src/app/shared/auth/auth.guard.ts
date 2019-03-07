import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router, NavigationExtras } from '@angular/router';
import { AccountService } from '../services/account.service';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(private account: AccountService,
              private router: Router) {

  }

  canActivate(next: ActivatedRouteSnapshot,
              state: RouterStateSnapshot): boolean {
    if (this.account.isAuthenticated()) {
      return true;
    }

    this.router.navigate(["/login"]);
    return false;
  }
}
