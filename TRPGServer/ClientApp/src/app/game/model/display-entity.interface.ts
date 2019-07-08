import { CharacterIconSet } from "./character.model";
import { ResourceStats } from "./ResourceStats";

export interface DisplayableEntity {
  id: number;
  name: string;
  iconUris: CharacterIconSet;
  resources: ResourceStats;
}
