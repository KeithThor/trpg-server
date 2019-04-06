import { DamageTypes } from "./damage-types.model";

export class SecondaryStat {
  public healBonus: number;
  public healPercentageBonus: number;
  public critChance: number;
  public critChancePercentage: number;
  public critDamage: number;
  public critDamagePercentage: number;
  public damage: DamageTypes;
  public damagePercentage: DamageTypes;
  public armor: DamageTypes;
  public armorPercentages: DamageTypes;
  public resistances: DamageTypes;
  public bonusActionPoints: number;
  public bonusActionPointsPercentage: number;
  public actionPointCostReduction: number;
  public actionPointCostReductionPercentage: number;
  public manaCostReduction: number;
  public manaCostReductionPercentage: number;
}
