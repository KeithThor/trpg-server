import { CharacterStats } from "./character-stats.model";
import { ClassTemplate } from "./class-template.model";

export class CharacterTemplate {
  constructor() {
    this.allocatedStats = new CharacterStats();
    this.name = "";
  }

  public entityId: number;
  public name: string;
  public hairId: number;
  public baseId: number;
  public groupId: number;
  public classTemplateId: number;
  public allocatedStats: CharacterStats;
}

export class CharacterHair {
  public id: number;
  public name: string;
  public iconUri: string;
}

export class CharacterBase {
  constructor() {
    this.maxStats = new CharacterStats();
    this.bonusStats = new CharacterStats();
  }

  public id: number;
  public name: string;
  public iconUri: string;
  public maxStats: CharacterStats;
  public bonusStats: CharacterStats;
}

export class CharacterIconSet {
  public baseIconUri: string;
  public bootsIconUri: string;
  public hairIconUri: string;
  public cloakIconUri: string;
  public leftHandIconUri: string;
  public rightHandIconUri: string;
  public headIconUri: string;
  public glovesIconUri: string;
  public legsIconUri: string;
  public bodyIconUri: string;
  public extraIconUri: string;

  public static asArray(iconSet: CharacterIconSet): string[] {
    let arr: string[] = [];
    if (iconSet.cloakIconUri != null) arr.push(iconSet.cloakIconUri);
    if (iconSet.baseIconUri != null) arr.push(iconSet.baseIconUri);
    if (iconSet.legsIconUri != null) arr.push(iconSet.legsIconUri);
    if (iconSet.bootsIconUri != null) arr.push(iconSet.bootsIconUri);
    if (iconSet.bodyIconUri != null) arr.push(iconSet.bodyIconUri);
    if (iconSet.glovesIconUri != null) arr.push(iconSet.glovesIconUri);
    if (iconSet.hairIconUri != null) arr.push(iconSet.hairIconUri);
    if (iconSet.headIconUri != null) arr.push(iconSet.headIconUri);
    if (iconSet.leftHandIconUri != null) arr.push(iconSet.leftHandIconUri);
    if (iconSet.rightHandIconUri != null) arr.push(iconSet.rightHandIconUri);
    if (iconSet.extraIconUri != null) arr.push(iconSet.extraIconUri);
    return arr;
  }
}

export class CreateCharacterData {
  constructor() {
    this.hairs = [];
    this.bases = [];
    this.classTemplates = [];
  }

  public hairs: CharacterHair[];
  public bases: CharacterBase[];
  public classTemplates: ClassTemplate[];
}
