import { Component, Input, Output, EventEmitter } from "@angular/core";
import { WorldEntity } from "../../../model/world-entity.model";

@Component({
  selector: "game-context-menu-list",
  templateUrl: "./context-menu-list.component.html",
  styleUrls: [
    "./context-menu-list.component.css",
    "../context-menu.component.css"
  ]
})
export class ContextMenuListComponent {
  @Input() entity: WorldEntity;
  @Output() onAttackSelected: EventEmitter<WorldEntity> = new EventEmitter();
  @Output() onJoinSelected: EventEmitter<WorldEntity> = new EventEmitter();

  public showCommands: boolean = false;

  /**Called whenever the Attack command is clicked; emits the onAttackSelected event and hides all available commands. */
  public attackClicked(): void {
    this.showCommands = false;
    this.onAttackSelected.emit(this.entity);
  }

  /**Called whenever the Attack command is clicked; emits the onJoinSelected event and hides all available commands. */
  public joinClicked(): void {
    this.showCommands = false;
    this.onJoinSelected.emit(this.entity);
  }
}
