import { DamageTypes } from "./damage-types.model";

export class DamagePerStat {
  public strength: DamageTypes = new DamageTypes();
  public dexterity: DamageTypes = new DamageTypes();
  public agility: DamageTypes = new DamageTypes();
  public intelligence: DamageTypes = new DamageTypes();
  public constitution: DamageTypes = new DamageTypes();

  public static asArray(damagePerStat: DamagePerStat): DamageTypes[] {
    return [
      damagePerStat.strength,
      damagePerStat.dexterity,
      damagePerStat.agility,
      damagePerStat.intelligence,
      damagePerStat.constitution
    ];
  }
}
