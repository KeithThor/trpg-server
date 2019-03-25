export class CharacterStats {
  public strength: number;
  public dexterity: number;
  public agility: number;
  public intelligence: number;
  public constitution: number;

  /**
   * Retrieves the value of a stat given the name of the stat.
   * @param statName The name of the stat to retrieve.
   */
  public getStat(statName: string): number {
    switch (statName) {
      case StatNames.strength:
        return this.strength;
      case StatNames.dexterity:
        return this.dexterity;
      case StatNames.agility:
        return this.agility;
      case StatNames.intelligence:
        return this.intelligence;
      case StatNames.constitution:
        return this.constitution;
      default:
        return null;
    }
  }
}

export class StatNames {
  public static readonly strength = "strength";
  public static readonly dexterity = "dexterity";
  public static readonly agility = "agility";
  public static readonly intelligence = "intelligence";
  public static readonly constitution = "constitution";
}
