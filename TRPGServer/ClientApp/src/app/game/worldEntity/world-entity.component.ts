import { Component, Input, HostListener, trigger, state, style, transition, animate } from "@angular/core";
import { WorldEntity } from "../model/world-entity.model";
import { CharacterIconSet } from "../model/character.model";

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
      transition("* <=> *",
        animate('0.2s'))
    ])
  ]
})
export class WorldEntityComponent {
  constructor() {
    this.animationState = "stationary";
  }
  @Input() entity: WorldEntity;
  public animationState: string;
  public onAnimationFinishedHandler: () => void;

  public onAnimationFinished(): void {
    console.log("Animation finished");
    if (this.onAnimationFinishedHandler != null) {
      this.onAnimationFinishedHandler();
    }
  }

  public getIconArray(): string[] {
    return CharacterIconSet.asArray(this.entity.iconUris);
  }

  @HostListener('contextmenu', ['$event'])
  public openContextMenu(event: Event): void {
    event.preventDefault();
    // Todo: Shows options for world entity
    console.log("Context menu opens");
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
