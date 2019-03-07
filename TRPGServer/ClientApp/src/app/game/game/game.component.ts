import { Component, HostListener, OnInit, ViewChild } from "@angular/core";
import { Coordinate } from "../model/coordinate.model";
import { Router, NavigationStart } from "@angular/router";
import { Subscription } from "rxjs/Subscription";
import { ChatboxComponent } from "../chatbox/chatbox.component";
import { GameStateService } from "../services/game-state.service";

@Component({
  selector: 'app-game-component',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css'],
})
export class GameComponent implements OnInit {
  constructor(private gameStateService: GameStateService,
    private router: Router) {

  }
  @ViewChild("chatbox") chatbox: ChatboxComponent;
  private subscriptions: Subscription[];

  ngOnInit(): void {
    this.subscriptions = [];
    this.initialize();
    this.subscriptions.push(this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        this.endConnections();
      }
    }));
  }

  @HostListener('document: keydown', ['$event'])
  public onKey(event: KeyboardEvent): void {
    if (this.gameStateService.initialized) {
      if (event.key === "ArrowLeft") {
        let movement = new Coordinate();
        movement.positionY = -1;
        this.gameStateService.moveEntity(movement);
      }
      if (event.key === "ArrowRight") {
        let movement = new Coordinate();
        movement.positionY = 1;
        this.gameStateService.moveEntity(movement);
      }
      if (event.key === "ArrowUp") {
        let movement = new Coordinate();
        movement.positionX = -1;
        this.gameStateService.moveEntity(movement);
      }
      if (event.key === "ArrowDown") {
        let movement = new Coordinate();
        movement.positionX = 1;
        this.gameStateService.moveEntity(movement);
      }
      if (event.key === "`") {
        this.gameStateService.changeMapsAsync();
      }
    }
  }

  @HostListener('window:beforeunload', ['$event'])
  public onBeginUnload(event: Event): void {
    this.endConnections();
  }

  /**
   * Initializes the services and connections for the game component.
   */
  private initialize(): void {
    this.gameStateService.initializeAsync()
      .catch(err => console.log(err));
  }

  /** Ends connections with each of the game services and this components subscriptions. */
  private endConnections(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
    this.gameStateService.endConnectionsAsync();
  }

  /** Returns true if the game map is ready to be shown to the player. */
  public showMap(): boolean {
    return this.gameStateService.initialized && this.gameStateService.getMapTileIds() != null;
  }

  public getTileIds(): number[][] {
    let tileIds = this.gameStateService.getMapTileIds();
    return tileIds;
  }

  /**
   * Gets the uri of the icon that represents the tile in the specified position on the map grid.
   * @param positionX The row the map tile resides in.
   * @param positionY The column the map tile resides in.
   */
  public getTileUri(positionX: number, positionY: number): string {
    let tileId = this.gameStateService.getMapTileIds()[positionX][positionY];
    return this.gameStateService.getUniqueTiles()
      .find(tile => tile.id === tileId)
      .iconUri;
  }

  /**
   * Returns true if an entity exists in a location on the map grid and can be displayed.
   * @param positionX The row of the map the entity resides on.
   * @param positionY The column of the map the entity resides on.
   */
  public showEntity(positionX: number, positionY: number): boolean {
    if (!this.gameStateService.initialized) return false;

    let locations = this.gameStateService.getEntityLocations();
    if (!locations) return false;
    if (!locations[positionX]) return false;
    if (locations[positionX][positionY] == null) return false;

    console.log("Is showing entity");
    return true;
  }

  /**
   * Returns the uri of the icon that represents the WorldEntity that exists in a location on the map grid.
   * @param positionX The row of the map the entity resides on.
   * @param positionY The column of the map the entity resides on.
   */
  public getEntityUri(positionX: number, positionY: number): string {
    let entityId = this.gameStateService.getEntityLocations()[positionX][positionY];
    return this.gameStateService.getEntities()
      .find(entity => entity.id === entityId)
      .iconUri;
  }

  public getEntityName(positionX: number, positionY: number): string {
    let entityId = this.gameStateService.getEntityLocations()[positionX][positionY];
    return this.gameStateService.getEntities()
      .find(entity => entity.id === entityId)
      .name;
  }
}
