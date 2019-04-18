import { CombatEntity } from "./combat-entity.model";

export class Formation {
  public id: number;
  public name: string;
  public leaderId: number;
  public positions: CombatEntity[][];
}

export class FormationTemplate {
  public id: number;
  public positions: number[][];
  public name: string;
  public leaderId: number;
  public makeActive: boolean;
}

export class FormationConstants {
  public static readonly positions: number[][] = [
    [1, 2, 3],
    [4, 5, 6],
    [7, 8, 9]
  ];
  public static readonly maxColumns: number = 3;
  public static readonly maxRows: number = 3;
  public static readonly maxFormationSize: number = 9;
}
