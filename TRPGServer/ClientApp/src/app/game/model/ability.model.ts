import { Category } from "./category.model";
import { DamageTypes } from "./damage-types.model";
import { DamagePerStat } from "./damage-per-stat.model";
import { CharacterStats } from "./character-stats.model";
import { StatusEffect } from "./status-effect.model";

export class Ability {
  constructor() {
    this.name = "";
    this.description = "";
    this.targets = [];
    this.damage = new DamageTypes();
    this.damagePerStat = new DamagePerStat();
    this.healPerStat = new CharacterStats();
    this.percentDamage = new DamageTypes();
    this.appliedStatusEffects = [];
    this.selfAppliedStatusEffects = [];
    this.actionPointCost = 0;
    this.healthCost = 0;
    this.healthPercentCost = 0;
    this.manaCost = 0;
    this.manaPercentCost = 0;
    this.heal = 0;
    this.percentHeal = 0;
    this.isOffensive = false;
  }

  public id: number;
  public name: string;
  public description: string;
  public category: Category;
  public iconUris: string[];
  public targets: number[];
  public centerOfTargets: number;
  public isPointBlank: boolean;
  public isPositionStatic: boolean;
  public isOffensive: boolean;
  public canTargetBeBlocked: boolean;
  public isSpell: boolean;
  public isSkill: boolean;
  public actionPointCost: number;
  public healthCost: number;
  public healthPercentCost: number;
  public manaCost: number;
  public manaPercentCost: number;
  public damage: DamageTypes;
  public damagePerStat: DamagePerStat;
  public heal: number;
  public healPerStat: CharacterStats;
  public percentDamage: DamageTypes;
  public percentHeal: number;
  public appliedStatusEffects: StatusEffect[];
  public selfAppliedStatusEffects: StatusEffect[];
}
