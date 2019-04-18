import { CharacterStats } from "./character-stats.model";
import { CharacterIconSet } from "./character.model";
import { DisplayableEntity } from "./display-entity.interface";
import { SecondaryStat } from "./secondary-stat.model";
import { Ability } from "./ability.model";
import { StatusEffect } from "./status-effect.model";
import { Item } from "./item.model";
import { ResourceStats } from "./ResourceStats";

export class CombatEntity implements DisplayableEntity {
  constructor() {
    this.name = "";
    this.iconUris = new CharacterIconSet();
    this.stats = new CharacterStats();
    this.unmodifiedStats = new CharacterStats();
    this.growthPoints = new CharacterStats();
    this.secondaryStats = new SecondaryStat();
    this.resources = new ResourceStats();
    this.abilities = [];
    this.statusEffects = [];
    this.equippedItems = [];
  }

  public id: number;
  public name: string;
  public iconUris: CharacterIconSet;
  public groupId: number;
  public ownerName: string;
  public stats: CharacterStats;
  public unmodifiedStats: CharacterStats;
  public growthPoints: CharacterStats;
  public secondaryStats: SecondaryStat;
  public abilities: Ability[];
  public statusEffects: StatusEffect[];
  public equippedItems: Item[];
  public resources: ResourceStats;
  public isActive: boolean = false;
}
