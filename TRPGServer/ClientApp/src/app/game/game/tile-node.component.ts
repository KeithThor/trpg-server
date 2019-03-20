import { Component, Input, ViewChild } from "@angular/core";
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
  @ViewChild("worldEntity") worldEntityComponent: WorldEntityComponent;

  public showEntity(): boolean {
    if (this.entity == null) return false;
    else return true;
  }
}
