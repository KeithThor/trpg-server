import { DamageTypes } from "./damage-types.model";
import { DamagePerStat } from "./damage-per-stat.model";
import { CharacterStats } from "./character-stats.model";
import { SecondaryStat } from "./secondary-stat.model";

export class StatusEffect {
  constructor() {
    this.name = "";
    this.description = "";
    this.iconUris = [];
    this.duration = 1;
    this.stackSize = 1;
    this.damagePerTurn = new DamageTypes();
    this.damagePerStatPerTurn = new DamagePerStat();
    this.healPerTurn = 0;
    this.percentHealPerTurn = 0;
    this.healPerStatPerTurn = new CharacterStats();
    this.modifiedStats = new CharacterStats();
    this.modifiedStatPercentages = new CharacterStats();
    this.modifiedSecondaryStats = new SecondaryStat();
  }

  public id: number;
  public name: string;
  public description: string;
  public iconUris: string[];
  public duration: number;
  public stackSize: number;
  public isMagical: boolean;
  public isPermanent: boolean;
  public isDebuff: boolean;
  public isStunned: boolean;
  public isSilenced: boolean;
  public isRestricted: boolean;
  public damagePerTurn: DamageTypes;
  public damagePerStatPerTurn: DamagePerStat;
  public healPerTurn: number;
  public percentHealPerTurn: number;
  public healPerStatPerTurn: CharacterStats;
  public modifiedStats: CharacterStats;
  public modifiedStatPercentages: CharacterStats;
  public modifiedSecondaryStats: SecondaryStat;
}
