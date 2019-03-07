import { Component, Input } from "@angular/core";
import { MapTile } from "../model/map-data.model";

@Component({
  selector: 'map-tile-component',
  templateUrl: 'map-tile.component.html'
})
export class MapTileComponent {
  @Input() mapTile: MapTile;
}
