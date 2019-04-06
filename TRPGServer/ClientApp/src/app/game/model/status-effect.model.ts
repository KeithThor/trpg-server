import { DamageTypes } from "./damage-types.model";
import { DamagePerStat } from "./damage-per-stat.model";
import { CharacterStats } from "./character-stats.model";
import { SecondaryStat } from "./secondary-stat.model";

export class StatusEffect {
  public id: number;
  public name: string;
  public description: string;
  public iconUris: string[];
  public duration: number;
  public stackSize: number;
  public isMagical: boolean;
  public isDebuff: boolean;
  public damagePerTurn: DamageTypes;
  public damagePerStatPerTurn: DamagePerStat;
  public healPerTurn: number;
  public percentHealPerTurn: number;
  public healPerStatPerTurn: number;
  public modifiedStats: CharacterStats;
  public modifiedStatPercentages: CharacterStats;
  public modifiedSecondaryStats: SecondaryStat;
}
