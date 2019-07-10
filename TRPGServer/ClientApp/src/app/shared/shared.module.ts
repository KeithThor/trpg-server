import { NgModule } from "@angular/core";
import { AuthGuard } from "./auth/auth.guard";
import { AccountService } from "./services/account.service";
import { HttpClientModule } from "@angular/common/http";
import { RouterModule } from "@angular/router";
import { ModuleWithProviders } from "@angular/compiler/src/core";
import { ConfirmationPromptComponent } from "./components/confirmationPrompt/confirmation-prompt.component";
import { CommonModule } from "@angular/common";
import { BrowserModule } from "@angular/platform-browser";

@NgModule({
  declarations: [
    ConfirmationPromptComponent
  ],

  imports: [
    HttpClientModule,
    RouterModule,
    CommonModule,
    BrowserModule
  ],

  providers: [
    AccountService,
    AuthGuard
  ],

  exports: [
    ConfirmationPromptComponent
  ]
})
export class SharedModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: SharedModule,
      providers: [
        AccountService,
        AuthGuard
      ]
    }
  }
}
