import { CharacterStats } from "./character-stats.model";
import { CharacterIconSet } from "./character.model";
import { DisplayableEntity } from "./display-entity.interface";

export class CombatEntity implements DisplayableEntity {
  public id: number;
  public name: string;
  public iconUris: CharacterIconSet;
  public groupId: number;
  public ownerName: string;
  public stats: CharacterStats;
}
