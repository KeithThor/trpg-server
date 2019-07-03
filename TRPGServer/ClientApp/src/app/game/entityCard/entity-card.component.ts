import { Component, Input, Output, EventEmitter } from "@angular/core";
import { CombatEntity } from "../model/combat-entity.model";

@Component({
  selector: "game-entity-card",
  templateUrl: "./entity-card.component.html",
  styleUrls: ["./entity-card.component.css"]
})
export class EntityCardComponent {
  @Input() entity: CombatEntity;
  @Input() extraIcons: string[];
  @Input() customStyles: object;
  @Output() onClick: EventEmitter<CombatEntity> = new EventEmitter();
  @Output() onMouseEnter: EventEmitter<CombatEntity> = new EventEmitter();
  @Output() onMouseLeave: EventEmitter<CombatEntity> = new EventEmitter();

  public getCustomStyles(): object {
    if (this.customStyles == null) return {};
    else return this.customStyles;
  }
}
