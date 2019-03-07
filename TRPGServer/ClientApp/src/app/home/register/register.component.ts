import { Component } from "@angular/core";
import { AccountService } from "../../shared/services/account.service";
import { User } from "../../shared/model/user.model";

@Component({
  selector: 'app-login-form',
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  constructor(private account: AccountService) {

  }

  user = new User();

  onRegister = function () {
    this.account.register(this.user, '');
  }
}
