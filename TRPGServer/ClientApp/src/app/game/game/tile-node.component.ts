import { Component, Input, ViewChild, EventEmitter, Output } from "@angular/core";
import { Coordinate } from "../model/coordinate.model";
import { MapTile } from "../model/map-data.model";
import { WorldEntity } from "../model/world-entity.model";
import { WorldEntityComponent } from "../worldEntity/world-entity.component";

/** A component representing one tile on a map grid. Contains anything that can exist on that tile. */
@Component({
  selector: 'game-tile-node',
  templateUrl: "./tile-node.component.html",
  styleUrls: ["./tile-node.component.css"]
})
export class TileNodeComponent {
  @Input() location: Coordinate;
  @Input() mapTile: MapTile;
  @Input() entity: WorldEntity;
  @Output() onContextMenu: EventEmitter<ContextData> = new EventEmitter();

  @ViewChild("worldEntity") worldEntityComponent: WorldEntityComponent;

  public showEntity(): boolean {
    if (this.entity == null) return false;
    else return true;
  }

  public invokeContextMenu(event: MouseEvent): void {
    let data = new ContextData();
    data.contextEvent = event;
    data.mapTile = this.mapTile;
    data.location = this.location;
    data.entity = this.entity;

    console.log("TileNode" + event.clientX + ", " + event.clientY);

    this.onContextMenu.emit(data);
  }
}

/**Data passed by the event invoked by opening the context menu on a TileNodeComponent. */
export class ContextData {
  public contextEvent: MouseEvent;
  public mapTile: MapTile;
  public location: Coordinate;
  public entity: WorldEntity;
}
