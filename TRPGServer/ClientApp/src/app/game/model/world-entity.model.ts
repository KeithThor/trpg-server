import { CharacterIconSet } from "./character.model";
import { DisplayableEntity } from "./display-entity.interface";

export class WorldEntity implements DisplayableEntity {
  id: number;
  ownerId: string;
  iconUris: CharacterIconSet;
  name: string;
}
