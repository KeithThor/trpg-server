import { Component, Input, HostListener } from "@angular/core";
import { MapTile } from "../model/map-data.model";
import { Coordinate } from "../model/coordinate.model";
import { GameStateService } from "../services/game-state.service";

@Component({
  selector: 'map-tile-component',
  templateUrl: './map-tile.component.html',
  styleUrls: ['./map-tile.component.css']
})
export class MapTileComponent {
  constructor(private gameStateService: GameStateService) {

  }
  @Input() location: Coordinate;
  @Input() mapTile: MapTile;

  @HostListener('contextmenu', ['$event'])
  public openContextMenu(event: Event): void {
    event.preventDefault();
    // Todo: Move entity to tile location
    console.log("Context menu opens");
  }
}
