import { Component, Input } from "@angular/core";
import { Ability } from "../../model/ability.model";

@Component({
  selector: "game-ability-panel",
  templateUrl: "./ability-panel.component.html",
  styleUrls: ["./ability-panel.component.css"]
})
export class AbilityPanelComponent {
  @Input() abilities: Ability[];
  @Input() clickHandler: (ability: Ability) => void;
  @Input() mouseEnterHandler: (ability: Ability) => void;
  @Input() mouseLeaveHandler: (ability: Ability) => void;

  public onClick(ability: Ability): void {
    if (this.clickHandler != null) this.clickHandler(ability);
  }

  public onMouseEnter(ability: Ability): void {
    if (this.mouseEnterHandler != null) this.mouseEnterHandler(ability);
  }

  public onMouseLeave(ability: Ability): void {
    if (this.mouseLeaveHandler != null) this.mouseLeaveHandler(ability);
  }
}
