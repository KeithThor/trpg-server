import { Component, Input, Output, EventEmitter } from "@angular/core";
import { Coordinate } from "../../../model/coordinate.model";
import { CombatEntity } from "../../../model/combat-entity.model";
import { Formation } from "../../../model/formation.model";

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
  @Input() formation: Formation;
  @Input() specialIconsFunc: (entity: CombatEntity) => string[];
  @Input() coordinate: Coordinate;
  @Output() onClick: EventEmitter<FormationNodeState> = new EventEmitter();
  @Output() onMouseEnter: EventEmitter<FormationNodeState> = new EventEmitter();
  @Output() onMouseLeave: EventEmitter<FormationNodeState> = new EventEmitter();
  @Output() onContextMenu: EventEmitter<FormationNodeState> = new EventEmitter();
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

  /**
   * Called by the template to emit the contextmenu event.
   * @param event
   */
  public emitOnContextMenu(event: MouseEvent): void {
    event.preventDefault();
    
    let args = this.createEventArgs();
    this.onContextMenu.emit(args);
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
    args.formation = this.formation;
    return args;
  }
}

/**Basic object containing data about a FormationNodeComponent used in events.*/
export class FormationNodeState {
  public entity: CombatEntity;
  public coordinate: Coordinate;
  public formation: Formation;
}
