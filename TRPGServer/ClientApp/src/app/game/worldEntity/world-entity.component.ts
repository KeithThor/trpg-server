import { Component, Input } from "@angular/core";
import { trigger, state, style, transition, animate } from "@angular/animations";
import { WorldEntity } from "../model/world-entity.model";
import { CharacterIconSet } from "../model/character.model";
import { WorldEntityService } from "../services/world-entity.service";
import { GameplayConstants } from "../gameplay-constants.static";

/**Component that represents an in-game WorldEntity. */
@Component({
  selector: 'world-entity-component',
  templateUrl: './world-entity.component.html',
  styleUrls: ['./world-entity.component.css'],
  animations: [
    trigger('move', [
      state('stationary', style({
        left: "0px",
        right: "0px",
        top: "0px",
        bottom: "0px"
      })),
      state('moveRight', style({
        left: "32px",
        right: "0px",
        top: "0px",
        bottom: "0px"
      })),
      state('moveLeft', style({
        left: "-32px",
        right: "0px",
        top: "0px",
        bottom: "0px"
      })),
      state('moveUp', style({
        left: "0px",
        right: "0px",
        top: "-32px",
        bottom: "0px"
      })),
      state('moveDown', style({
        left: "0px",
        right: "0px",
        top: "32px",
        bottom: "0px"
      })),
      transition("* => stationary",
        animate('0s')),
      transition("* <=> *",
        animate('0.2s'))
    ])
  ]
})
export class WorldEntityComponent {
  constructor(private worldEntityService: WorldEntityService) {
    this.animationState = WorldEntityAnimationConstants.stationary;
  }
  private _entity: WorldEntity;
  @Input() set entity(value: WorldEntity) {
    this._entity = value;
    this.animationState = WorldEntityAnimationConstants.stationary;
  }
  get entity(): WorldEntity {
    return this._entity;
  }

  public animationState: string;
  public onAnimationFinishedHandler: () => void;

  /** Invokes the onAnimationFinishedHandler when an animation is finished. */
  public onAnimationFinished(): void {
    if (this.onAnimationFinishedHandler != null) {
      this.onAnimationFinishedHandler();
    }
  }

  /**Gets the icon array from the WorldEntity to display. */
  public getIconArray(): string[] {
    return CharacterIconSet.asArray(this.entity.iconUris);
  }

  /**Gets the css class that is appropriate for the WorldEntity this component represents. */
  public getEntityCss(): string {
    if (this.entity.id === this.worldEntityService.playerEntityId) return "entity-player-name";
    if (this.entity.ownerId === GameplayConstants.aiId) return "entity-enemy-name";
    else return "entity-neutral-name";
  }

  /**
   * When the contextmenu is opened on the WorldEntity's name, stop the propagation.
   * @param event
   */
  public onNameContextMenu(event: MouseEvent): void {
    event.stopPropagation();
    event.preventDefault();
  }
}

/** Contains the names of animation states for the WorldEntityComponent. */
export class WorldEntityAnimationConstants {
  public static readonly stationary: string = "stationary";
  public static readonly moveRight: string = "moveRight";
  public static readonly moveDown: string = "moveDown";
  public static readonly moveUp: string = "moveUp";
  public static readonly moveLeft: string = "moveLeft";
}
