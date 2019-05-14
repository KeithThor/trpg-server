import { Formation } from "./formation.model";
import { ActiveEntities } from "./start-of-turn-data.model";

export class Battle {
  public id: number;
  public attackers: Formation[];
  public defenders: Formation[];
  public activeEntities: ActiveEntities[];
  public isDefenderTurn: boolean;
  public round: number;
  public secondsLeftInTurn: number;
}
