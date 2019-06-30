import { Component, Input } from "@angular/core";
import { trigger, state, style, transition, animate } from "@angular/animations";

/**Component representing an in-game resource bar used for things like health and mana. */
@Component({
  selector: "game-resource-bar",
  templateUrl: "./resource-bar.component.html",
  styleUrls: ["./resource-bar.component.css"],
  animations: [
    trigger("changedResource", [
      state("initial", style({
        width: "*"
      })),
      state("changed", style({
        width: "{{currentValue}}"
      }), { params: { currentValue: "0%" } }),
      transition("initial => changed",
        animate("0.3s")
      )
    ])
  ]
})
export class ResourceBarComponent {
  @Input() resourceName: string;
  @Input() current: number;
  @Input() max: number;
  @Input() resourceMaxStyles: any;
  @Input() resourceCurrentStyles: any;
  @Input() resourceTextStyles: any;
}
