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
  @Output() onClick: EventEmitter<FormationNodeEvent> = new EventEmitter();
  @Output() onMouseEnter: EventEmitter<FormationNodeEvent> = new EventEmitter();
  @Output() onMouseLeave: EventEmitter<FormationNodeEvent> = new EventEmitter();
  @Input() getNodeStateFunc: (entity: DisplayableEntity) => string;

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
    return this.getNodeStateFunc(this.entity);
  }

  /**Creates the event args for EventEmitters. */
  private createEventArgs(): FormationNodeEvent {
    let args = new FormationNodeEvent();
    args.coordinate = this.coordinate;
    args.entity = this.entity;
    return args;
  }
}

export class FormationNodeEvent {
  public entity: CombatEntity;
  public coordinate: Coordinate;
}
