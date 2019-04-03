import { CharacterIconSet } from "./character.model";

export interface DisplayableEntity {
  id: number;
  name: string;
  iconUris: CharacterIconSet;
}
