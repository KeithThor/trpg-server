import { Component } from "@angular/core";
import { AccountService } from "../../shared/services/account.service";
import { User } from "../../shared/model/user.model";
import { HttpErrorResponse } from "@angular/common/http";

/**Component responsible for creating a new user. */
@Component({
  selector: 'app-login-form',
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  constructor(private account: AccountService) {
  }

  public errorMessage: string;
  public user = new User();

  /**Uses the AccountService to register a new user. */
  public onRegister(): void {
    this.account.register(this.user, '', this.onError.bind(this));
  }

  /**
   * Handler called if and when there is an error with the request to register a new user.
   * @param err
   */
  private onError(err: HttpErrorResponse): void {
    if (err.error instanceof ErrorEvent) {
      this.errorMessage = "Oops, there was an error. Please try again.";
    }
    else {
      if (err.status === 400) {
        if (err.statusText === "Modelstate invalid") {
          this.errorMessage = "There was an error with the given username and password, please try again.";
        }
        else {
          this.errorMessage = "Oops, there was an error on our end. Please try again.";
        }
      }
      else if (err.status === 409) {
        this.errorMessage = "There is already another user with that username. Please choose another username.";
      }
    }
  }
}
