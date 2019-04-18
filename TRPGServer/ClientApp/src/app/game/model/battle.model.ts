import { Formation } from "./formation.model";
import { ActionsPerFormation } from "./ActionsPerFormation";

export class Battle {
  public id: number;
  public attackers: Formation[];
  public defenders: Formation[];
  public actionsPerFormation: ActionsPerFormation;
  public isDefenderTurn: boolean;
  public round: number;
  public secondsLeftInTurn: number;
}
