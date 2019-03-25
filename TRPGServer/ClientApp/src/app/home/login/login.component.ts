import { Component, OnInit } from "@angular/core";
import { AccountService } from "../../shared/services/account.service";
import { User } from "../../shared/model/user.model";
import { ActivatedRoute } from "@angular/router";

@Component({
  selector: 'app-login-form',
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {
  constructor(private account: AccountService,
    private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    let redirect = this.route.snapshot.queryParamMap.get("redirect");
    if (redirect != null) this.redirectUrl = redirect;
  }

  private redirectUrl: string = "";
  user = new User();

  public onLogin(): void {
    this.account.login(this.user, this.redirectUrl);
  }
}
