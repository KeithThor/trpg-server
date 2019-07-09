import { BattleAction } from "./battle-action.model";

/**Represents an invalid action response from the server. */
export class InvalidAction {
  public action: BattleAction;
  public errorMessage: string;
}
