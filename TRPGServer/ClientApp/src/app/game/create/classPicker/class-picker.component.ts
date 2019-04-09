import { Component, Input, OnInit, Output, EventEmitter } from "@angular/core";
import { ClassTemplate } from "../../model/class-template.model";

@Component({
  selector: "game-class-picker",
  templateUrl: "./class-picker.component.html",
  styleUrls: ["./class-picker.component.css"]
})
export class ClassPickerComponent implements OnInit {
  ngOnInit(): void {

  }

  @Input() selectedClassId: number;
  public selectedClass: ClassTemplate;
  @Output() selectedClassIdChange: EventEmitter<number> = new EventEmitter<number>();
  @Input() classTemplates: ClassTemplate[];
  @Input() canChooseClass: boolean;

  public onClassChanged(newClassId: number): void {
    // Binding from template will make newClassId a string no matter what the declared typing is
    let foundClass = this.classTemplates.find(temp => temp.id == newClassId);
    this.selectedClass = foundClass;
    this.selectedClassId = this.selectedClass.id;
    this.selectedClassIdChange.emit(this.selectedClassId);
  }
}
