import { Component, Input } from "@angular/core";
import { DisplayableEntity } from "../../../model/display-entity.interface";
import { Coordinate } from "../../../model/coordinate.model";

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
  @Input() entity: DisplayableEntity;
  @Input() specialIconsFunc: (entity: DisplayableEntity) => string[];
  @Input() coordinate: Coordinate;
  @Input() clickHandler: (entity: DisplayableEntity, position: Coordinate) => void;
  @Input() getNodeStateFunc: (entity: DisplayableEntity) => string;
  @Input() setHoveredEntityFunc: (entity: DisplayableEntity) => void;

  /**
   * Returns an array of string containing the uris of special icons to stack on top of a DisplayableEntity.
   * @param entity The entity to stack special icons on top of.
   */
  public getSpecialIcons(entity: DisplayableEntity): string[] {
    if (this.specialIconsFunc != null) return this.specialIconsFunc(entity);
    else return null;
  }

  /** Called by the template whenever a node is clicked by the user. */
  public onClick(): void {
    if (this.clickHandler != null) this.clickHandler(this.entity, this.coordinate);
  }

  /**
   * Sets the HoveredEntity in the parent component to the entity that exists in this node whenever the user hovers
   * over this component.
   *
   * Sets the HoveredEntity to null if the user leaves this component.
   * @param entity The entity to set the HoveredEntity to.
   */
  public setHoveredEntity(entity: DisplayableEntity): void {
    if (this.setHoveredEntityFunc != null) this.setHoveredEntityFunc(entity);
  }

  /** Gets the css class name that represents this node's state. */
  public getNodeState(): string {
    if (this.getNodeStateFunc == null) return "";
    return this.getNodeStateFunc(this.entity);
  }
}
