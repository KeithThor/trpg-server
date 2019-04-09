import { CharacterStats } from "./character-stats.model";
import { SecondaryStat } from "./secondary-stat.model";
import { Ability } from "./ability.model";
import { Item } from "./item.model";

export class ClassTemplate {
  constructor() {
    this.name = "";
    this.description = "";
    this.stats = new CharacterStats();
    this.secondaryStats = new SecondaryStat();
    this.abilities = [];
    this.equippedItems = [];
  }

  public id: number;
  public name: string;
  public description: string;
  public stats: CharacterStats;
  public secondaryStats: SecondaryStat;
  public abilities: Ability[];
  public equippedItems: Item[];
}
