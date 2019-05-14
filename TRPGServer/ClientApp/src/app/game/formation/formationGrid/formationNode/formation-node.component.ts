import { Component, Input, Output, EventEmitter } from "@angular/core";
import { DisplayableEntity } from "../../../model/display-entity.interface";
import { Coordinate } from "../../../model/coordinate.model";
import { CombatEntity } from "../../../model/combat-entity.model";

/** A component that wraps around a DisplayEntityComponent and handles coordination with a FormationComponent.*/
@Component({
  selector: "game-formation-node",
  templateUrl: "./formation-node.component.html",
  styleUrls: [
    "./formation-node.component.css",
    "../../formation.component.css"
  ]
})
export class FormationNodeComponent {
  @Input() entity: CombatEntity;
  @Input() specialIconsFunc: (entity: CombatEntity) => string[];
  @Input() coordinate: Coordinate;
  @Output() onClick: EventEmitter<FormationNodeState> = new EventEmitter();
  @Output() onMouseEnter: EventEmitter<FormationNodeState> = new EventEmitter();
  @Output() onMouseLeave: EventEmitter<FormationNodeState> = new EventEmitter();
  @Input() getNodeStateFunc: (nodeState: FormationNodeState) => string;

  /**
   * Returns an array of string containing the uris of special icons to stack on top of a DisplayableEntity.
   * @param entity The entity to stack special icons on top of.
   */
  public getSpecialIcons(): string[] {
    if (this.specialIconsFunc != null) return this.specialIconsFunc(this.entity);
    else return null;
  }

  /** Called by the template to emit the on click event. */
  public emitOnClick(): void {
    let args = this.createEventArgs();
    this.onClick.emit(args);
  }

  /** Called by the template to emit the on mouse enter event. */
  public emitOnMouseEnter(): void {
    let args = this.createEventArgs();
    this.onMouseEnter.emit(args);
  }

  /**Called by the template to emit the on mouse leave event. */
  public emitOnMouseLeave(): void {
    let args = this.createEventArgs();
    this.onMouseLeave.emit(args);
  }

  /** Gets the css class name that represents this node's state. */
  public getNodeState(): string {
    if (this.getNodeStateFunc == null) return "";

    let nodeState = this.createEventArgs();
    return this.getNodeStateFunc(nodeState);
  }

  /**Creates the event args for EventEmitters. */
  private createEventArgs(): FormationNodeState {
    let args = new FormationNodeState();
    args.coordinate = this.coordinate;
    args.entity = this.entity;
    return args;
  }
}

export class FormationNodeState {
  public entity: CombatEntity;
  public coordinate: Coordinate;
}
