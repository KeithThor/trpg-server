import { Component, OnInit, ViewChild } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { CreateCharacterData, CharacterHair, CharacterBase, CharacterTemplate } from "../model/character.model";
import { CombatEntity } from "../model/combat-entity.model";
import { NgForm } from "@angular/forms";
import { CharacterStats } from "../model/character-stats.model";
import { StatPickerComponent } from "./statPicker/stat-picker.component";
import { ClassTemplate } from "../model/class-template.model";

@Component({
  selector: 'game-create-component',
  templateUrl: './create.component.html',
  styleUrls: [
    './create.component.css',
    '../entityCard/entity-card.component.css',
    '../displayEntity/display-entity.component.css'
  ]
})
export class CreateComponent implements OnInit {
  constructor(private http: HttpClient) {

  }

  public isLoaded: boolean = false;
  public isEditing: boolean = false;
  public hairs: CharacterHair[] = [];
  public bases: CharacterBase[] = [];
  public selectedClassId: number;
  public classTemplates: ClassTemplate[] = [];
  public entities: CombatEntity[] = [];
  public growthPoints: CharacterStats;
  public selectedBase: CharacterBase;
  public entityId: number = 0;
  public name: string = "Name";
  public hairId: number = 0;
  public baseId: number = 0;
  public self = this;
  public hoveredComponentName: string;
  @ViewChild("form") form: NgForm;
  @ViewChild("statPicker") statPicker: StatPickerComponent;

  ngOnInit(): void {
    let loadedCreateData: boolean = false;
    let loadedEntityData: boolean = false;
    this.hoveredComponentName = "growth";
    this.growthPoints = new CharacterStats();

    this.http.get<CreateCharacterData>("api/character")
      .subscribe({
        next: (data) => {
          this.bases = data.bases;
          this.hairs = data.hairs;
          this.classTemplates = data.classTemplates;
        },
        complete: () => {
          if (loadedEntityData === true) this.initialize();
          else loadedCreateData = true;
        }
      });
    this.http.get<CombatEntity[]>("api/character/created")
      .subscribe({
        next: (entities) => {
          this.entities = entities;
          console.log(this.entities);
        },
        complete: () => {
          if (loadedCreateData === true) this.initialize();
          else loadedEntityData = true;
        }
      });
  }

  /** Called to initialize the component with data and let the DOM know that the component is loaded. */
  public initialize(): void {
    this.newEntity();
    this.isLoaded = true;
  }

  /** Gets the icon URI for the base of a character based on the player's selection. */
  public getBaseUri(): string {
    let base: CharacterBase = this.bases.find(b => b.id == this.baseId);
    return base.iconUri;
  }

  /** Gets the icon URI for the hair of a character based on the player's selection. */
  public getHairUri(): string {
    let hair: CharacterHair = this.hairs.find(h => h.id == this.hairId);
    return hair.iconUri;
  }

  public getButtonText(): string {
    if (this.isEditing) return "Save Changes";
    else return "Create Character";
  }

  /**
   * Called by the template whenever the user changes the baseId of a character.
   * @param baseId The id of the new base the user chose.
   */
  public onBaseChanged(baseId: number) {
    let base: CharacterBase = this.bases.find(b => b.id == baseId);
    if (base != null) {
      this.selectedBase = base;
      this.baseId = baseId;

      this.growthPoints = new CharacterStats();
      this.growthPoints.strength = 5;
      this.growthPoints.dexterity = 5;
      this.growthPoints.agility = 5;
      this.growthPoints.intelligence = 5;
      this.growthPoints.constitution = 5;
      this.statPicker.freePoints = 0;
    }
  }

  public changeHoveredComponentName(name: string): void {
    this.hoveredComponentName = name;
  }

  /** Resets the form and fills it with data for a new character.
   * Will give the user a warning if the user has prior unsaved changes.*/
  public newEntity(): void {
    if (this.areAnyFormsDirty()) {
      if (!confirm("You have unsaved changes. Leaving will discard all changes you have made so far.")) {
        return;
      }
    }

    this.entityId = -1;
    this.isEditing = false;
    this.baseId = this.bases[0].id;
    this.hairId = this.hairs[0].id;
    this.name = "Name";
    this.growthPoints = new CharacterStats();
    this.growthPoints.strength = 5;
    this.growthPoints.dexterity = 5;
    this.growthPoints.agility = 5;
    this.growthPoints.intelligence = 5;
    this.growthPoints.constitution = 5;
    this.selectedClassId = 1;

    // Deep copy array
    this.selectedBase = this.bases[0];
    if (this.statPicker != null) {
      this.statPicker.freePoints = 0;
    }
  }

  /**
   * Selects an entity from the DOM and loads its data into the form to allow the player to edit.
   * Will give the user a warning if the user has prior unsaved changes.
   * @param entity The entity the user selected from the DOM.
   */
  public selectEntity(entity: CombatEntity): void {
    if (this.areAnyFormsDirty()) {
      if (!confirm("You have unsaved changes. Leaving will discard all changes you have made so far.")) {
        return;
      }
    }

    let self = this;
    this.entityId = entity.id;
    this.name = entity.name;
    this.isEditing = true;
    this.growthPoints = JSON.parse(JSON.stringify(entity.growthPoints));
    this.selectedClassId = 1;
    this.statPicker.freePoints = 0;
    this.hairs.forEach(hair => {
      if (hair.iconUri === entity.iconUris.hairIconUri) {
        self.hairId = hair.id;
      }
    });
    this.bases.forEach(base => {
      if (base.iconUri === entity.iconUris.baseIconUri) {
        self.baseId = base.id;
        self.selectedBase = base;
      }
    });

    this.markFormsPristine();
  }

  /**
   * Gets a custom style object for an EntityCardComponent provided its encapsulated CombatEntity.
   * @param entity The CombatEntity contained within an EntityCardComponent.
   */
  public getCustomCardStyles(entity: CombatEntity): object {
    if (this.isEditing && entity != null && entity.id === this.entityId) return { "background-color": "#095900" };
    if (entity == null && !this.isEditing) return { "background-color": "#095900" };

    else return {};
  }

  /** Marks the forms in this template pristine. */
  private markFormsPristine(): void {
    let t = this;
    Object.keys(this.form.controls).forEach(control => {
      t.form.controls[control].markAsPristine();
    });
    if (this.statPicker == null) return;

    this.statPicker.isPristine = true;
  }

  /** Returns true if any forms in this component are dirty. */
  private areAnyFormsDirty(): boolean {
    if (this.statPicker == null) return false;
    return (this.form.dirty || !this.statPicker.isPristine);
  }

  /** Sends a request to the server to either POST or PATCH a character using the data from the form as a template. */
  public sendRequestAsync(): void {
    if (!this.statPicker.isValid()) {
      alert("You still have extra stat points that need to be allocated.");
      return;
    }

    let template = new CharacterTemplate();
    template.name = this.name;
    template.baseId = this.baseId;
    template.hairId = this.hairId;
    template.allocatedStats = this.growthPoints;

    if (!this.isEditing && this.selectedClassId == null) {
      alert("Please select a starting class before creating a new character.");
      return;
    }
    else if (!this.isEditing) template.classTemplateId = this.selectedClassId;

    if (this.isEditing) {
      this.http.patch("api/character", template)
        .subscribe({
          error: (err) => {
            console.log(err);
          },
          complete: () => {
            let entity = this.entities.find(e => e.id === this.entityId);
            entity.name = template.name;

            let baseIconUri = this.bases.find(b => b.id === template.baseId).iconUri;
            if (baseIconUri != null) entity.iconUris.baseIconUri = baseIconUri;
            let hairIconUri = this.hairs.find(h => h.id === template.hairId).iconUri;
            if (hairIconUri != null) entity.iconUris.hairIconUri = hairIconUri;

            this.markFormsPristine();
          }
        });
    }
    else {
      this.http.post("api/character", template).subscribe({
        next: (entity: CombatEntity) => {
          this.entities.push(entity);
        },
        error: (err) => {
          // Todo: handle error by displaying to user
          console.log(err);
        },
        complete: () => {
          this.markFormsPristine();
        }
      });
    }
  }
}
