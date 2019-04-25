import { Component, OnInit } from "@angular/core";
import { AccountService } from "../../shared/services/account.service";
import { User } from "../../shared/model/user.model";
import { ActivatedRoute } from "@angular/router";
import { HttpErrorResponse } from "@angular/common/http";

/**Component responsible for logging in the user. */
@Component({
  selector: 'app-login-form',
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {
  constructor(private account: AccountService,
    private route: ActivatedRoute) {
  }

  public errorMessage: string;

  ngOnInit(): void {
    let redirect = this.route.snapshot.queryParamMap.get("redirect");
    if (redirect != null) this.redirectUrl = redirect;
  }

  private redirectUrl: string = "";
  user = new User();

  /** Uses the AccountService to log in to the user's account. */
  public onLogin(): void {
    this.account.login(this.user, this.redirectUrl, this.onError.bind(this));
  }

  /**
   * Handler called if and when there is an error when logging in.
   * @param err
   */
  private onError(err: HttpErrorResponse): void {
    if (err.error instanceof ErrorEvent) {
      this.errorMessage = "Oops, there was an error. Please try again.";
    }
    else {
      if (err.status === 400) {
        if (err.statusText === "Wrong password") {
          this.errorMessage = "Password incorrect for the given username.";
        }
        else {
          this.errorMessage = "No users were found for the given username and password combination.";
        }
      }
    }
  }
}
