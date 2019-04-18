import { Injectable } from "@angular/core";
import { PlayerStateConstants } from "../player-state-constants.static";
import { Router } from "@angular/router";

@Injectable()
export class StateHandlerService {
  constructor(private router: Router) {

  }

  public async handleStateAsync(state: string): Promise<void> {
    switch (state) {
      case PlayerStateConstants.inCombat:
        await this.router.navigateByUrl("/battle");
        break;
      case PlayerStateConstants.makeCharacter:
        await this.router.navigateByUrl("/create");
        break;
      case PlayerStateConstants.makeFormation:
        await this.router.navigateByUrl("/formation");
        break;
      default:
        break;
    }
  }
}
