import { CombatEntity } from "./combat-entity.model";

export class StartOfTurnData {
  public activeEntities: ActiveEntities[];
  public affectedEntities: CombatEntity[];
  public actionPointsChanged: ActionPointsChanged[];
  public isDefendersTurn: boolean;
  public turnExpiration: number;
}

export class ActiveEntities {
  public ownerId: string;
  public formationId: number;
  public entityIds: number[];
}

export class ActionPointData {
  public entityId: number;
  public currentActionPoints: number;
}

export class ActionPointsChanged {
  public formationId: number;
  public actionPointData: ActionPointData[];
}
