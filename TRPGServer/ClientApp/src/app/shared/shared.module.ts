import { NgModule } from "@angular/core";
import { AuthGuard } from "./auth/auth.guard";
import { AccountService } from "./services/account.service";
import { HttpClientModule } from "@angular/common/http";
import { RouterModule } from "@angular/router";
import { ModuleWithProviders } from "@angular/compiler/src/core";

@NgModule({
  declarations: [

  ],

  imports: [
    HttpClientModule,
    RouterModule
  ],

  providers: [
    AccountService,
    AuthGuard
  ],

  exports: [

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