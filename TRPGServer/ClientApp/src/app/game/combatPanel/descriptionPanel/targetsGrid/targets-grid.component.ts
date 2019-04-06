import { Component, Input } from "@angular/core";
import { FormationConstants } from "../../../model/formation.model";
import { Ability } from "../../../model/ability.model";

@Component({
  selector: "game-targets-grid",
  templateUrl: "./targets-grid.component.html",
  styleUrls: ["./targets-grid.component.css"]
})
export class TargetsGridComponent {
  @Input() ability: Ability;

  public getMaxGridSize(): number {
    let positions = FormationConstants.positions;
    let lastRow = positions[positions.length - 1];
    return lastRow[lastRow.length - 1];
  }

  public getFormationLimitAsArray(): number[] {
    return [0, 1];
  }

  public getPosition(formationIndex: number, position: number): number {
    return FormationConstants.maxFormationSize * formationIndex + position;
  }

  public getPositions(): number[][] {
    return FormationConstants.positions;
  }

  public getStyleForPosition(position: number): string {
    if (this.ability.targets == null) return "";
    if (this.ability.centerOfTargets === position && !this.ability.isPositionStatic) return "grid-node-center";
    if (this.ability.targets.indexOf(position) !== -1) {
      if (this.ability.isPositionStatic) return "grid-node-static";
      else return "grid-node-active";
    }
    else return "";
  }
}
