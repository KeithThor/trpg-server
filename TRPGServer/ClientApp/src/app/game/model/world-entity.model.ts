import { CharacterIconSet } from "./character.model";

export class WorldEntity {
  id: number;
  ownerId: string;
  iconUris: CharacterIconSet;
  name: string;
}
