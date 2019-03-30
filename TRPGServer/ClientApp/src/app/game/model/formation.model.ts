import { CombatEntity } from "./combat-entity.model";

export class Formation {
  public id: number;
  public name: string;
  public positions: CombatEntity[][];
}

export class FormationTemplate {
  public id: number;
  public positions: number[][];
  public name: string;
}
