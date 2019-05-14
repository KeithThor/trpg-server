import { Component, Input, Output, EventEmitter } from "@angular/core";
import { Formation } from "../../model/formation.model";
import { Coordinate } from "../../model/coordinate.model";
import { CombatEntity } from "../../model/combat-entity.model";
import { FormationNodeState } from "./formationNode/formation-node.component";

/** A component that represents a Formation Grid. */
@Component({
  selector: "game-formation-grid",
  templateUrl: "./formation-grid.component.html",
  styleUrls: ["./formation-grid.component.css"]
})
export class FormationGridComponent {
  @Input() formation: Formation;
  @Input() specialIconsFunc: (entity: CombatEntity) => string[];
  @Input() getNodeStateFunc: (nodeState: FormationNodeState) => string;
  @Output() onNodeClicked: EventEmitter<FormationNodeState> = new EventEmitter();
  @Output() onNodeMouseEnter: EventEmitter<FormationNodeState> = new EventEmitter();
  @Output() onNodeMouseLeave: EventEmitter<FormationNodeState> = new EventEmitter();
  @Output() onMouseEnter: EventEmitter<Formation> = new EventEmitter();
  @Output() onMouseLeave: EventEmitter<Formation> = new EventEmitter();
  @Output() onClicked: EventEmitter<Formation> = new EventEmitter();

  /**
   * Emits the onNodeClicked event when the onClicked event is emitted from the child node component.
   * @param args
   */
  public nodeClicked(args: FormationNodeState): void {
    this.onNodeClicked.emit(args);
  }

  /**
   * Emits the onNodeMouseEnter event when the onMouseEnter event is emitted from the child node component.
   * @param args
   */
  public nodeMouseEnter(args: FormationNodeState): void {
    this.onNodeMouseEnter.emit(args);
  }

  /**
   * Emits the onNodeMouseLeave event when the onMouseLeave event is emitted from the child node component.
   * @param args
   */
  public nodeMouseLeave(args: FormationNodeState): void {
    this.onNodeMouseLeave.emit(args);
  }

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
