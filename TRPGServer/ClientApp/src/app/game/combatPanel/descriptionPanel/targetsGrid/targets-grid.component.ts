import { Component, Input } from "@angular/core";
import { FormationConstants } from "../../../model/formation.model";
import { Ability } from "../../../model/ability.model";
import { FormationTargeter } from "../../../services/formation-targeter.static";

@Component({
  selector: "game-targets-grid",
  templateUrl: "./targets-grid.component.html",
  styleUrls: ["./targets-grid.component.css"]
})
export class TargetsGridComponent {
  @Input() ability: Ability;
  @Input() entityPosition: number;
  @Input() flipGrid: boolean;

  private cachedAbilityId: number;
  private cachedEntityPosition: number;
  private cachedPositions: number[];

  /**Returns the max size for a Formation grid. */
  public getMaxGridSize(): number {
    return FormationConstants.maxRows * FormationConstants.maxColumns;
  }

  /**
   * Returns a position number taking into account multiple numbers of formations.
   * @param formationIndex The index of the current formation being iterated.
   * @param position The position in the current formation being iterated.
   */
  public getPosition(formationIndex: number, position: number): number {
    return FormationConstants.maxFormationSize * formationIndex + position;
  }

  /**Gets all the positions available in a single formation. */
  public getPositions(): number[][] {
    return FormationConstants.positions;
  }

  /**
   * Returns a string of the css class used to style a target grid node depending on the display ability's
   * targets and whether it is static.
   * @param position The position of the node corresponding to the ability's targets.
   */
  public getStyleForPosition(position: number): string {
    if (this.ability.targets == null) return "";

    // If grid shouldn't be flipped or the ability is offensive, but not when both
    if (!this.flipGrid !== this.ability.isOffensive) {
      // Offset to display on other grid
      if (position > FormationConstants.maxFormationSize) position -= FormationConstants.maxFormationSize;
      else position += FormationConstants.maxFormationSize;
    }

    if (this.ability.isPointBlank) {
      // If no entity position is set, use original ability targets
      if (this.entityPosition == null) {
        // Show grid-node-point-blank css if the targeted position is the center of targets and is included in the ability's target list
        if (position === this.ability.centerOfTargets
          && this.ability.targets.some(target => target === this.ability.centerOfTargets)) return "grid-node-point-blank";
        else if (this.ability.targets.indexOf(position) !== -1) return "grid-node-static";
      }
      // Show grid-node-point-blank css if the targeted position is the center of targets and is included in the ability's target list
      else {
        let positions = this.getTranslatedPositions();
        if (position === this.entityPosition && positions.some(target => target === this.entityPosition)) return "grid-node-point-blank";
        else if (positions.indexOf(position) !== -1) return "grid-node-static";
      }
      //else if (position === this.entityPosition
      //  && this.ability.targets.some(target => target === this.entityPosition)) return "grid-node-point-blank";

      //else if (this.getTranslatedPositions().indexOf(position) !== -1) return "grid-node-static";
    }
    // If center of a static positioned ability
    else if (this.ability.centerOfTargets === position && !this.ability.isPositionStatic) return "grid-node-center";
    // Position is inside target position
    else if (this.ability.targets.indexOf(position) !== -1) {
      if (this.ability.isPositionStatic) return "grid-node-static";
      else return "grid-node-active";
    }
    else return "";
  }

  /** Gets the translated positions for this ability using the entity's current position as the new center
   * of targets.*/
  public getTranslatedPositions(): number[] {
    if (this.ability == null || this.entityPosition == null) return null;
    if (this.cachedEntityPosition != null &&
      this.cachedEntityPosition === this.entityPosition &&
      this.ability.id === this.cachedAbilityId) {
      return this.cachedPositions;
    }
    else {
      let newCenter = this.entityPosition;
      this.cachedAbilityId = this.ability.id;
      this.cachedEntityPosition = this.entityPosition;
      this.cachedPositions = FormationTargeter.translate(this.ability.centerOfTargets, this.ability.targets, newCenter);
      return this.cachedPositions;
    }
  }
}
