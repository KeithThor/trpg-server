import { Component, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Formation, FormationConstants, FormationTemplate } from "../model/formation.model";
import { DisplayableEntity } from "../model/display-entity.interface";
import { CombatEntity } from "../model/combat-entity.model";
import { FormationFactory } from "../services/formation.factory";
import { Coordinate } from "../model/coordinate.model";
import { TwoDArray } from "../../shared/static/two-d-array.static";

/** A component that provides CRUD operations on in-game combat formations for the user. */
@Component({
  selector: 'game-formation-component',
  templateUrl: './formation.component.html',
  styleUrls: ['./formation.component.css']
})
export class FormationComponent implements OnInit {
  constructor(private http: HttpClient,
  private formationFactory: FormationFactory) {

  }

  public activeFormation: Formation;
  public activeFormationLeader: CombatEntity;
  public activeFormationName: string;
  public formations: Formation[];
  public isLoaded: boolean;
  public entities: CombatEntity[];
  public selectedEntity: CombatEntity;
  public hoveredEntity: DisplayableEntity;
  private isEditing: boolean;

  ngOnInit(): void {
    this.isLoaded = false;
    let loadedFormations = false;
    let loadedEntities = false;
    this.isEditing = false;
    this.http.get<Formation[]>("api/formation").subscribe({
      next: (formations: Formation[]) => {
        this.formations = formations;
      },
      complete: () => {
        loadedFormations = true;
        if (loadedEntities) this.initialize();
      }
    });
    this.http.get<CombatEntity[]>("api/character/created")
      .subscribe({
        next: (entities) => {
          this.entities = entities;
          console.log(this.entities);
        },
        complete: () => {
          loadedEntities = true;
          if (loadedFormations) this.initialize();
        }
      });
  }

  /** Called whenever the component is finished loading server-side data.*/
  private initialize(): void {
    this.newFormation();
    this.isLoaded = true;
  }

  /** Creates a new formation and sets it as the active formation. */
  public newFormation(): void {
    this.activeFormation = this.formationFactory.createFormation();
    this.activeFormationLeader = null;
    this.activeFormationName = "Formation Name";
    this.isEditing = false;
  }

  /** Transforms the formations array into an array of DisplayableEntities using the leaders of each formation. */
  public getFormationLeaders(): DisplayableEntity[] {
    let arr: DisplayableEntity[] = [];
    this.formations.forEach(formation => {
      let leader = TwoDArray.find(formation.positions, (entity: CombatEntity) => {
        return entity.id === formation.leaderId;
      });
      if (leader != null) arr.push(leader);
    });
    return arr;
  }

  /**
   * Gets a string array containing the uri's of icons to stack on top of a display entity.
   * @param entity The display entity to stack icons on top of.
   */
  public getExtraIcons(entity: DisplayableEntity): string[] {
    if (this.selectedEntity == null) return null;
    if (this.selectedEntity.id !== entity.id) {
      return null;
    }
    else {
      return ["images/misc/cursor.png"];
    }
  }

  /**
   * Make the selected entity the active entity if its id exists in the current list of entities.
   * @param entity The display entity to make active.
   */
  public selectEntity(entity: DisplayableEntity): void {
    let match = this.entities.find(e => e.id === entity.id);
    if (match == null) return;
    else {
      if (this.selectedEntity === match) this.selectedEntity = null;
      else this.selectedEntity = match;
    }
  }

  /**
   * Makes a formation the active formation given the index of its position in the formations array.
   * @param index The index of the formation to select from the formations array.
   */
  public selectFormation(index: number): void {
    let formation = this.formations[index];
    if (formation != null) {
      this.isEditing = true;
      this.activeFormation = this.formationFactory.copyFormation(formation);
      this.activeFormationLeader = TwoDArray.find(formation.positions, (entity) => {
        return entity.id === formation.leaderId;
      });
      this.activeFormationName = formation.name;
    }
  }

  /**
   * Called whenever a formation or entity node is selected by the user, performs differently depending on the currently selected
   * entity and the clicked node.
   * 
   * Moves the selected entity to the position of the clicked formation node if a formation node was clicked.
   * 
   * Deselects the currently selected entity if the clicked node belongs to that entity.
   *
   * Selects the entity that exists in a given node if there are currently no selected entities.
   * @param entity The entity that exists in the selected node.
   * @param position The position the clicked node occupies on the formation grid, if any.
   */
  public onNodeClick(entity: DisplayableEntity, position: Coordinate): void {
    if (this.selectedEntity == null && entity == null) return;
    else if (this.selectedEntity == null && entity != null) this.selectEntity(entity);
    else if (entity != null && this.selectedEntity.id === entity.id) this.selectedEntity = null;
    else if (this.selectedEntity != null) {
      let occupyingEntity = this.activeFormation.positions[position.positionX][position.positionY];
      let previousPosition = this.findOccupiedPosition(this.selectedEntity, this.activeFormation);
      // Place selected entity into position, removing the occupying entity from the formation
      if (previousPosition == null) {
        this.activeFormation.positions[position.positionX][position.positionY] = this.selectedEntity;
        this.selectedEntity = null;
      }
      // Swap positions with occupying entity
      else {
        this.activeFormation.positions[position.positionX][position.positionY] = this.selectedEntity;
        this.activeFormation.positions[previousPosition.positionX][previousPosition.positionY] = occupyingEntity;
        this.selectedEntity = null;
      }
    }
  }

  /**
   * Returns a coordinate containing the position the entity exists in the given formation.
   *
   * Returns null if the entity does not exist in the given formation.
   * @param entity The displayable entity to get the coordinates of.
   * @param formation The formation to find the entity in.
   */
  private findOccupiedPosition(entity: DisplayableEntity, formation: Formation): Coordinate {
    let coord = new Coordinate();
    let found = false;
    let i = 0;
    let indeces = TwoDArray.findIndex(formation.positions, e => e.id === entity.id);
    if (indeces == null) return null;

    coord.positionX = indeces[0];
    coord.positionY = indeces[1];
    return coord;
  }

  /**
   * Returns a string representing the css class of the current state of a node, provided the entity that
   * exists in the node.
   * @param entity The entity that exists in a given node.
   */
  public getNodeState(entity: DisplayableEntity): string {
    if (entity == null) return "";
    if (this.selectedEntity != null && entity.id === this.selectedEntity.id) return "node-active";
    if (this.hoveredEntity != null && entity.id === this.hoveredEntity.id) return "node-hovered";
    else return "";
  }

  /**
   * Sets the currently hovered entity to the one provided.
   * @param entity The new entity that is being hovered over.
   */
  public setHoveredEntity(entity: DisplayableEntity): void {
    this.hoveredEntity = entity;
  }

  /** Gets the name of the request button which changes depending on whether the user is editing a formation or creating a new one. */
  public getButtonName(): string {
    if (this.isEditing) return "Save Formation";
    else return "Create Formation";
  }

  /** Sends an HTTP request to the server with FormationTemplate data.
   *
   * The request differs depending on whether the user is editing or creating formations.
   */
  public sendRequestAsync(): void {
    let template = new FormationTemplate();
    template.id = this.activeFormation.id;
    template.name = this.activeFormationName;

    // Todo: Add in way to select leader
    let uniques = TwoDArray.getUnique(this.activeFormation.positions);
    if (uniques == null) return;
    template.leaderId = uniques[0].id;
    let positions: number[][] = [];

    this.activeFormation.positions.forEach(row => {
      let arr: number[] = [];
      row.forEach(entity => {
        if (entity == null) arr.push(null);
        else arr.push(entity.id);
      });
      positions.push(arr);
    })
    template.positions = positions;

    if (this.isEditing && template.id != null) {
      this.http.patch("api/formation", template)
        .subscribe({
          error: (err) => {
            // Todo: error handling
            console.log(err);
          },
          complete: () => {
            let index = this.formations.findIndex(f => f.id === this.activeFormation.id);
            if (index !== -1) {
              this.formations[index] = this.activeFormation;
              this.selectFormation(index);
            }
          }
        });
    }
    else if (template.id == null) {
      this.http.post("api/formation", template)
        .subscribe({
          next: (formation: Formation) => {
            this.formations.push(formation);
          },
          error: (err) => {
            // Todo: error handling
            console.log(err);
          },
          complete: () => {
            this.activeFormation = this.formations[this.formations.length - 1];
          }
        });
    }
  }

  /** Sends a request to the server to delete the currently active formation. */
  public deleteRequestAsync(): void {
    if (this.activeFormation.id == null) alert("Can't delete a new formation.");

    if (confirm("Are you sure you want to delete the formation " + this.activeFormation.name + "?")) {
      this.http.delete("api/formation", { params: { "id": this.activeFormation.id.toString() } })
        .subscribe({
          error: (err) => {
            console.log(err);
          },
          complete: () => {
            let index = this.formations.findIndex(f => f.id === this.activeFormation.id);
            if (index !== -1) {
              this.formations.splice(index, 1);
            }
            this.newFormation();
          }
        });
    }
  }
}
