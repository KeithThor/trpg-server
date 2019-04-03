import { Component, Input } from "@angular/core";
import { Formation } from "../../model/formation.model";
import { DisplayableEntity } from "../../model/display-entity.interface";
import { Coordinate } from "../../model/coordinate.model";

/** A component that represents a Formation Grid. */
@Component({
  selector: "game-formation-grid",
  templateUrl: "./formation-grid.component.html",
  styleUrls: ["./formation-grid.component.css"]
})
export class FormationGridComponent {
  @Input() formation: Formation;
  @Input() specialIconsFunc: (entity: DisplayableEntity) => string[];
  @Input() nodeClickHandler: (entity: DisplayableEntity, position: Coordinate) => void;
  @Input() getNodeStateFunc: (entity: DisplayableEntity) => string;
  @Input() setHoveredEntityFunc: (entity: DisplayableEntity) => void;

  /**
   * Creates a new Coordinate object given an X and Y position.
   * @param posX The X position of the Coordinate.
   * @param posY The Y position of the Coordinate.
   */
  public getCoordinate(posX: number, posY: number): Coordinate {
    let coord = new Coordinate();
    coord.positionX = posX;
    coord.positionY = posY;
    return coord;
  }
}
