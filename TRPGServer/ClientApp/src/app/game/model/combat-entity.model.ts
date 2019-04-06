import { CharacterStats } from "./character-stats.model";
import { CharacterIconSet } from "./character.model";
import { DisplayableEntity } from "./display-entity.interface";
import { SecondaryStat } from "./secondary-stat.model";
import { Ability } from "./ability.model";
import { StatusEffect } from "./status-effect.model";

export class CombatEntity implements DisplayableEntity {
  public id: number;
  public name: string;
  public iconUris: CharacterIconSet;
  public groupId: number;
  public ownerName: string;
  public stats: CharacterStats;
  public secondaryStats: SecondaryStat;
  public abilities: Ability[];
  public statusEffects: StatusEffect[];
}
