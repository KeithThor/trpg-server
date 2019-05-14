import { Component, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Formation, FormationConstants, FormationTemplate } from "../model/formation.model";
import { DisplayableEntity } from "../model/display-entity.interface";
import { CombatEntity } from "../model/combat-entity.model";
import { FormationFactory } from "../services/formation.factory";
import { Coordinate } from "../model/coordinate.model";
import { TwoDArray } from "../../shared/static/two-d-array.static";
import { FormationNodeState } from "./formationGrid/formationNode/formation-node.component";

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
  public isChoosingLeader: boolean;
  public entities: CombatEntity[];
  public selectedEntity: CombatEntity;
  public hoveredEntity: CombatEntity;
  public makeActive: boolean;
  private isEditing: boolean;

  ngOnInit(): void {
    this.isLoaded = false;
    let loadedFormations = false;
    let loadedEntities = false;
    this.isEditing = false;
    this.isChoosingLeader = false;
    this.newFormation();
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
    this.isLoaded = true;
  }

  /** Creates a new formation and sets it as the active formation. */
  public newFormation(): void {
    this.activeFormation = this.formationFactory.createFormation();
    this.activeFormationLeader = null;
    this.activeFormation.name = "Formation Name";
    this.isEditing = false;
    this.isChoosingLeader = false;
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
    if (entity == null) return null;
    if (this.activeFormationLeader != null && this.activeFormationLeader.id === entity.id) {
      return ["images/misc/tutorial_cursor.png"];
    }
    else if (this.selectedEntity == null) return null;
    else if (this.selectedEntity.id === entity.id) {
      return ["images/misc/cursor.png"];
    }
    else {
      return null;
    }
  }

  /** Gets the CombatEntity that should have its abilities and details displayed in the CombatPanel. */
  public getCombatPanelEntity(): CombatEntity {
    if (this.hoveredEntity != null) return this.hoveredEntity;
    else return this.selectedEntity;
  }

  /** Returns the formation position of the CombatEntity who should have its abilities and
   * details displayed in the CombatPanel*/
  public getCombatPanelEntityPosition(): Coordinate {
    if (this.hoveredEntity != null) {
      return this.findOccupiedPosition(this.getCombatPanelEntity(), this.activeFormation);
    }
    else return this.findOccupiedPosition(this.selectedEntity, this.activeFormation);
  }

  /**
   * Toggles selection for the clicked entity.
   * @param entity The display entity to make active.
   */
  public selectEntity(entity: CombatEntity): void {
    if (this.isChoosingLeader) {
      this.activeFormationLeader = entity;
      this.isChoosingLeader = false;
    }
    else if (this.selectedEntity === entity) this.selectedEntity = null;
    else this.selectedEntity = entity;
  }

  /**
   * Makes a formation the active formation given the index of its position in the formations array.
   * @param index The index of the formation to select from the formations array.
   */
  public selectFormation(index: number): void {
    let formation = this.formations[index];
    this.selectedEntity = null;
    this.isChoosingLeader = false;
    if (formation != null) {
      this.isEditing = true;
      this.activeFormation = this.formationFactory.copyFormation(formation);
      this.activeFormationLeader = TwoDArray.find(formation.positions, (entity) => {
        return entity.id === formation.leaderId;
      });
      this.activeFormation.name = formation.name;
    }
  }

  /** Handler called whenever the choose leader button is clicked.
   *
   * Allows the user to select a leader from the Entities in its formation.*/
  public chooseLeaderClicked(): void {
    if (this.isChoosingLeader) {
      this.isChoosingLeader = false;
    }
    else {
      this.selectedEntity = null;
      this.isChoosingLeader = true;
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
   * @param args Event object containing the clicked CombatEntity and the clicked Coordinate.
   */
  public onNodeClick(args: FormationNodeState): void {
    let entity = args.entity;
    let position = args.coordinate;

    if (this.selectedEntity == null && entity == null) return;
    else if (this.selectedEntity == null && entity != null) {
      if (this.isChoosingLeader) {
        this.isChoosingLeader = false;
        this.activeFormationLeader = this.activeFormation.positions[position.positionY][position.positionX];
      }
      else this.selectEntity(entity);
    }
    else if (entity != null && this.selectedEntity.id === entity.id) this.selectedEntity = null;
    else if (this.selectedEntity != null) {
      let entityAsCombatEntity = this.activeFormation.positions[position.positionY][position.positionX];
      let previousPosition = this.findOccupiedPosition(this.selectedEntity, this.activeFormation);
      if (this.activeFormationLeader == null) this.activeFormationLeader = this.selectedEntity;
      // Place selected entity into position, removing the occupying entity from the formation
      if (previousPosition == null) {
        this.activeFormation.positions[position.positionY][position.positionX] = this.selectedEntity;
        this.selectedEntity = null;
      }
      // Swap positions with occupying entity
      else {
        this.activeFormation.positions[position.positionY][position.positionX] = this.selectedEntity;
        this.activeFormation.positions[previousPosition.positionY][previousPosition.positionX] = entityAsCombatEntity;
        this.selectedEntity = null;
      }
    }
  }

  /**
   * Called by a FormationNode component whenever the user's mouse enters the node. Sets the hoveredEntity to the
   * CombatEntity that exists in the given node.
   * @param args
   */
  public onNodeMouseEnter(args: FormationNodeState): void {
    this.setHoveredEntity(args.entity);
  }

  /**
   * Called by a FormationNode component whenever the user's mouse leaves the node. Sets the hoveredEntity to null.
   * @param args
   */
  public onNodeMouseLeave(args: FormationNodeState): void {
    this.setHoveredEntity(null);
  }

  /**
   * Performs the act of changing the currently hovered CombatEntity.
   * @param entity
   */
  public setHoveredEntity(entity: CombatEntity): void {
    this.hoveredEntity = entity;
  }

  /**
   * Returns a coordinate containing the position the entity exists in the given formation.
   *
   * Returns null if the entity does not exist in the given formation.
   * @param entity The displayable entity to get the coordinates of.
   * @param formation The formation to find the entity in.
   */
  private findOccupiedPosition(entity: DisplayableEntity, formation: Formation): Coordinate {
    if (entity == null || formation == null) return null;
    let coord = new Coordinate();
    let indeces = TwoDArray.findIndex(formation.positions, e => {
      if (e == null) return false;
      if (e.id === entity.id) return true;
      else return false;
    });
    if (indeces == null) return null;

    coord.positionX = indeces[0];
    coord.positionY = indeces[1];
    return coord;
  }

  /**
   * Returns a string representing the css class of the current state of a node, provided the entity that
   * exists in the node.
   * @param nodeState The state of the given node.
   */
  public getNodeState(nodeState: FormationNodeState): string {
    if (nodeState.entity == null) return "";
    if (this.selectedEntity != null && nodeState.entity === this.selectedEntity) return "node-active";
    if (this.hoveredEntity != null && nodeState.entity === this.hoveredEntity) return "node-hovered";
    else return "";
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
    template.name = this.activeFormation.name;
    template.makeActive = this.makeActive;

    // Todo: Add in way to select leader
    let uniques = TwoDArray.getUnique(this.activeFormation.positions);
    if (uniques == null) return;
    template.leaderId = this.activeFormationLeader.id;
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
              this.activeFormation.leaderId = this.activeFormationLeader.id;
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
