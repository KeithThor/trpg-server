import { CharacterStats } from "./character-stats.model";

export class CharacterTemplate {
  public entityId: number;
  public name: string;
  public hairId: number;
  public baseId: number;
  public groupId: number;
  public allocatedStats: CharacterStats;
}

export class CharacterHair {
  public id: number;
  public name: string;
  public iconUri: string;
}

export class CharacterBase {
  public id: number;
  public name: string;
  public iconUri: string;
  public maxStats: CharacterStats;
  public bonusStats: CharacterStats;
}

export class CreateCharacterData {
  public hairs: CharacterHair[];
  public bases: CharacterBase[];
}
