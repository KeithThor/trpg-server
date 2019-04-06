import { Component, Input } from "@angular/core";
import { DisplayableEntity } from "../model/display-entity.interface";
import { CharacterIconSet } from "../model/character.model";

/** A low level component designed to provide a unified way to display game character entities.
 * Should be wrapped inside other components to provide custom functionality.*/
@Component({
  selector: "game-display-entity",
  templateUrl: "./display-entity.component.html",
  styleUrls: ["./display-entity.component.css"]
})
export class DisplayEntityComponent {
  constructor() {
    this.displayName = true;
  }
  @Input() entity: DisplayableEntity;
  @Input() displayName: boolean;
  @Input() clickHandler: (entity: DisplayableEntity) => void;
  @Input() extraIcons: string[];

  public getIconArray(): string[] {
    let arr = CharacterIconSet.asArray(this.entity.iconUris);
    if (this.extraIcons != null) arr.concat(this.extraIcons);
    return arr;
  }

  public onClick(): void {
    if (this.clickHandler != null) this.clickHandler(this.entity);
  }
}
