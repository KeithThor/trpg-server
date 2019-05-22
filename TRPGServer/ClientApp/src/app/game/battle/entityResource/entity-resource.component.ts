import { Component, Input } from "@angular/core";
import { CombatEntity } from "../../model/combat-entity.model";

/**Component responsible for displaying a CombatEntity's resource values to the user. */
@Component({
  selector: "game-entity-resource",
  templateUrl: "./entity-resource.component.html",
  styleUrls: ["./entity-resource.component.css"]
})
export class EntityResourceComponent {
  @Input() entity: CombatEntity;

  public getHealthStyles(): object {
    return {
      "background-color": "red"
    };
  }

  public getManaStyles(): object {
    return {
      "background-color": "blue"
    };
  }
}
