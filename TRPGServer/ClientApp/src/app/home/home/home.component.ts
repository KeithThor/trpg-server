import { Component, OnInit } from '@angular/core';
import { LocalStorageConstants } from '../../constants';
import { AccountService } from '../../shared/services/account.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: [
    './home.component.css'
  ]
})
export class HomeComponent implements OnInit {
  constructor(private account: AccountService) {

  }
  ngOnInit(): void {
  }

  getUsername(): string {
    return sessionStorage.getItem(LocalStorageConstants.username);
  }

  isLoggedIn(): boolean {
    return this.account.isAuthenticated();
  }

  logOut(): void {
    return this.account.logout();
  }
}
