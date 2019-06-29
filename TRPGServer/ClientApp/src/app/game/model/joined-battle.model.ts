import { Formation } from "./formation.model";
import { ActiveEntities } from "./start-of-turn-data.model";

export class JoinedBattleArgs {
  public isAttacker: boolean;
  public joinedFormation: Formation;
  public activeEntities: ActiveEntities[];
}
