import { Component, Input } from "@angular/core";
import { CombatEntity } from "../../model/combat-entity.model";
import { CharacterIconSet } from "../../model/character.model";

@Component({
  selector: "game-edit-entity",
  templateUrl: "./edit-entity.component.html",
  styleUrls: ["./edit-entity.component.css"]
})
export class EditEntityComponent {
  @Input() entity: CombatEntity;
  @Input() clickHandler: (entity: CombatEntity) => void;

  public onClick(): void {
    this.clickHandler(this.entity);
  }

  public getIconArray(): string[] {
    return CharacterIconSet.asArray(this.entity.iconUris);
  }
}
