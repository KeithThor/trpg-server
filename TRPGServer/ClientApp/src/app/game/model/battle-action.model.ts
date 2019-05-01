import { CombatEntity } from "./combat-entity.model";
import { Ability } from "./ability.model";

export class BattleAction {
  public actorId: number;
  public isDefending: boolean;
  public isUsingItem: boolean;
  public isFleeing: boolean;
  public abilityId: number;
  public targetPosition: number;
  public targetFormationId: number;
}

export class SuccessfulAction {
  public action: BattleAction;
  public affectedEntities: CombatEntity[];
  public ability: Ability;
  public actor: CombatEntity;
}
