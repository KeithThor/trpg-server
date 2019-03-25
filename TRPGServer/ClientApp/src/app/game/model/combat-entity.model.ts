import { CharacterStats } from "./character-stats.model";

export class CombatEntity {
  public id: number;
  public name: string;
  public iconUris: string[];
  public groupId: number;
  public ownerName: string;
  public stats: CharacterStats;
}
