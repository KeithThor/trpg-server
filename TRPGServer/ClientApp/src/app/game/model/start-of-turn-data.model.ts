import { CombatEntity } from "./combat-entity.model";

export class StartOfTurnData {
  public activeEntities: ActiveEntities[];
  public affectedEntities: CombatEntity[];
  public isDefendersTurn: boolean;
  public turnExpiration: number;
}

export class ActiveEntities {
  public formationId: number;
  public entityIds: number[];
}
