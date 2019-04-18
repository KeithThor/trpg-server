export class ResourceStats {
  constructor() {
    this.currentActionPoints = 0;
    this.currentHealth = 0;
    this.currentMana = 0;
    this.maxHealth = 0;
    this.maxMana = 0;
    this.unmodifiedMaxHealth = 0;
    this.unmodifiedMaxMana = 0;
  }

  public currentHealth: number;
  public maxHealth: number;
  public unmodifiedMaxHealth: number;
  public currentMana: number;
  public maxMana: number;
  public unmodifiedMaxMana: number;
  public currentActionPoints: number;
}
