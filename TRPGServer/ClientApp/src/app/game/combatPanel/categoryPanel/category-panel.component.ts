import { Component, Input } from "@angular/core";
import { Category } from "../../model/category.model";

@Component({
  selector: "game-category-panel",
  templateUrl: "./category-panel.component.html",
  styleUrls: ["./category-panel.component.css"]
})
export class CategoryPanelComponent {
  @Input() categories: Category[];
  @Input() clickHandler: (category: Category) => void;
  @Input() mouseEnterHandler: (category: Category) => void;
  @Input() mouseLeaveHandler: (category: Category) => void;

  public onClick(category: Category): void {
    if (this.clickHandler != null) this.clickHandler(category);
  }

  public onMouseEnter(category: Category): void {
    if (this.mouseEnterHandler != null) this.mouseEnterHandler(category);
  }

  public onMouseLeave(category: Category): void {
    if (this.mouseLeaveHandler != null) this.mouseLeaveHandler(category);
  }
}
