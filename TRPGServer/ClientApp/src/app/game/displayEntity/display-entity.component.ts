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

  /**Returns an array of string containing the uris of icons to display.*/
  public getIconArray(): string[] {
    let arr = CharacterIconSet.asArray(this.entity.iconUris);
    if (this.extraIcons != null) arr = arr.concat(this.extraIcons);
    return arr;
  }

  /**Calls the click handler whenever a click event is called from the template. */
  public onClick(): void {
    if (this.clickHandler != null) this.clickHandler(this.entity);
  }

  /**Returns an object containing the custom style for the DisplayEntityComponent. */
  public getEntityStyle(): any {
    let value: string;
    if (this.entity.resources.currentHealth > 0) value = "1.0";
    else value = "0.4";

    return {
      "opacity": value
    };
  }
}
