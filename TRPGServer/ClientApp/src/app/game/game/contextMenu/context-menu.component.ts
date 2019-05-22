import { Component, Input, Output, EventEmitter } from "@angular/core";
import { WorldEntity } from "../../model/world-entity.model";

@Component({
  selector: "game-context-menu",
  templateUrl: "./context-menu.component.html",
  styleUrls: ["./context-menu.component.css"]
})
export class ContextMenuComponent {
  @Input() xPosition: number;
  @Input() yPosition: number;
  @Input() entities: WorldEntity[];

  @Output() onAttackSelected: EventEmitter<WorldEntity> = new EventEmitter();
  @Output() onJoinSelected: EventEmitter<WorldEntity> = new EventEmitter();
  @Output() onMoveSelected: EventEmitter<void> = new EventEmitter();
  @Output() onCancelSelected: EventEmitter<void> = new EventEmitter();
}
