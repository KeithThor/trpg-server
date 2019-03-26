import { CharacterStats } from "./character-stats.model";
import { CharacterIconSet } from "./character.model";

export class CombatEntity {
  public id: number;
  public name: string;
  public iconUris: CharacterIconSet;
  public groupId: number;
  public ownerName: string;
  public stats: CharacterStats;
}
