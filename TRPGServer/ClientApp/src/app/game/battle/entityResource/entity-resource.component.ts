import { Component, Input } from "@angular/core";
import { CombatEntity } from "../../model/combat-entity.model";

/**Component responsible for displaying a CombatEntity's resource values to the user. */
@Component({
  selector: "game-entity-resource",
  templateUrl: "./entity-resource.component.html",
  styleUrls: [
    "./entity-resource.component.css",
    "../../global-game-styles.css"
  ]
})
export class EntityResourceComponent {
  @Input() entity: CombatEntity;

  public getHealthStyles(): object {
    let widthPercent = Math.floor(this.entity.resources.currentHealth / this.entity.resources.maxHealth * 100);
    return {
      "background-color": "red",
      "width": widthPercent + "%"
    };
  }

  public getManaStyles(): object {
    let widthPercent = Math.floor(this.entity.resources.currentMana / this.entity.resources.maxMana * 100);
    return {
      "background-color": "blue",
      "width": widthPercent + "%"
    };
  }
}
