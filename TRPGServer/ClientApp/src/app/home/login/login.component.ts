import { Component } from "@angular/core";
import { AccountService } from "../../shared/services/account.service";
import { User } from "../../shared/model/user.model";

@Component({
  selector: 'app-login-form',
  templateUrl: './login.component.html'
})
export class LoginComponent {
  constructor(private account: AccountService) {

  }

  user = new User();

  onLogin = function () {
    this.account.login(this.user, '');
  }
}
