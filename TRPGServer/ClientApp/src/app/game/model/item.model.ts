import { ItemType } from "./item-type.model";
import { CharacterStats } from "./character-stats.model";
import { SecondaryStat } from "./secondary-stat.model";
import { Ability } from "./ability.model";
import { StatusEffect } from "./status-effect.model";

export class Item {
  public id: number;
  public name: string;
  public description: string;
  public iconUris: string[];
  public type: ItemType;
  public stats: CharacterStats;
  public secondaryStats: SecondaryStat;
  public isEquippable: boolean;
  public equippedAbilities: Ability[];
  public isConsumable: boolean;
  public consumableAbility: Ability;
  public consumableCharges: number;
  public destroyedWhenOutOfCharges: boolean;
  public selfAppliedStatusEffects: StatusEffect[];
  public appliedStatusEffects: StatusEffect[];
  public isStackable: boolean;
  public stackSize: number;
  public cost: number;
  public equipIconUri: string;
}
