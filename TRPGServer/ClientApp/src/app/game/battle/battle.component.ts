import { Component, OnInit, OnDestroy } from "@angular/core";
import { BattleService } from "../services/battle.service";
import { Subscription } from "rxjs/Subscription";
import { Battle } from "../model/battle.model";
import { ActiveEntities } from "../model/start-of-turn-data.model";
import { CombatEntity } from "../model/combat-entity.model";
import { LocalStorageConstants } from "../../constants";
import { Formation, FormationConstants } from "../model/formation.model";
import { SuccessfulAction, BattleAction } from "../model/battle-action.model";
import { TwoDArray } from "../../shared/static/two-d-array.static";
import { FormationNodeState } from "../formation/formationGrid/formationNode/formation-node.component";
import { FormationTargeter } from "../services/formation-targeter.static";
import { SelectedAbilityData } from "../combatPanel/combat-panel.component";

/**Component responsible for displaying to the user the state of a battle as well as allowing
 * the user to perform actions in battle.*/
@Component({
  selector: "game-battle-component",
  templateUrl: "./battle.component.html",
  styleUrls: ["./battle.component.css"]
})
export class BattleComponent implements OnInit, OnDestroy {
  constructor(private battleService: BattleService) {
    this.subscriptions = [];
  }

  public battle: Battle;
  public activeEntities: ActiveEntities[];
  public secondsInTurn: number;
  public isDefendersTurn: boolean;
  public round: number;

  // Game data
  private userId: string;
  private isUserAttacker: boolean;
  private attackingFormations: Formation[];
  private defendingFormations: Formation[];

  // User data
  public activeEntity: CombatEntity;
  public hoveredEntity: CombatEntity;
  public targetPosition: number;
  public targetPositions: number[];
  public targetFormation: Formation;
  public activeEntityPosition: number;
  public activeAbility: SelectedAbilityData;

  private subscriptions: Subscription[];

  ngOnInit(): void {
    this.subscriptions = [];
    this.userId = sessionStorage.getItem(LocalStorageConstants.userId);
    this.battleService.initializeAsync();

    this.subscriptions.push(
      this.battleService.onInitialized.subscribe({
        next: (data) => {
          this.initializeFormations(data);
          this.activeEntities = data.activeEntities;
          this.secondsInTurn = data.secondsLeftInTurn;
          this.isDefendersTurn = data.isDefenderTurn;
          this.round = data.round;
        }
      })
    );

    this.subscriptions.push(
      this.battleService.onStartOfTurn.subscribe({
        next: (data) => {
          this.applyEntityChanges(data.affectedEntities);
          this.activeEntities = data.activeEntities;
          this.isDefendersTurn = data.isDefendersTurn;
          this.secondsInTurn = data.turnExpiration;
          this.startTurn();
        }
      })
    );

    this.subscriptions.push(
      this.battleService.onSuccessfulAction.subscribe({
        next: (data) => {
          this.applyAction(data);
        }
      })
    );

    this.subscriptions.push(
      this.battleService.onInvalidAction.subscribe({
        next: (action) => {
          this.showInvalidAction(action);
        }
      })
    );

    this.subscriptions.push(
      this.battleService.onEndOfTurn.subscribe({
        next: (entities) => {
          this.applyEntityChanges(entities);
        }
      })
    );

    this.subscriptions.push(
      this.battleService.onEndOfBattle.subscribe({
        next: (didAttackersWin) => {
          // Show victory or defeat message
        }
      })
    );
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
    this.battleService.endConnectionAsync();
  }

  /**
   * Animates a successful combat action and applies changes to all affected entities.
   * @param action The object containing data about the action that was successfully performed.
   */
  private applyAction(action: SuccessfulAction): void {
    this.removeActiveEntity(action);

    if (action.ability != null) {
      // Animate ability effects
    }
    this.applyEntityChanges(action.affectedEntities);
  }

  /**
   * Removes the entity who successfully performed an action from the activeEntities array.
   *
   * If the currently active entity is the actor who successfully performed the action,
   * clears the selection from the CombatPanel.
   * @param action The object containing data about the action that was successfully performed.
   */
  private removeActiveEntity(action: SuccessfulAction): void {
    // Remove actor from activeEntities array
    let activeEntities: ActiveEntities = this.activeEntities.find(ae => ae.ownerId === action.actor.ownerId);
    let removeIndex = activeEntities.entityIds.indexOf(action.actor.id);
    if (removeIndex != -1) activeEntities.entityIds.splice(removeIndex);

    // Clears selection if the active entity was the actor successfully performing an action
    if (this.activeEntity.id === action.actor.id) this.clearSelection();
  }

  /**Clears the currently active Entity and Ability. */
  private clearSelection(): void {
    this.activeEntity = null;
    this.activeAbility = null;
  }

  /**
   * Replaces the CombatEntities stored in memory with the newly updated entities.
   * @param entities An array of all newly updated CombatEntities.
   */
  private applyEntityChanges(entities: CombatEntity[]): void {
    entities.forEach((entity, index, array) => {
      let formation: Formation = this.attackingFormations.find(f => f.ownerId === entity.ownerId);
      if (formation == null) formation = this.defendingFormations.find(f => f.ownerId === entity.ownerId);
      if (formation == null) return;

      let foundEntity = TwoDArray.find(formation.positions, e => {
        return e != null && e.id === entity.id;
      });

      if (foundEntity == null) return;

      array[index] = foundEntity;
    });
  }

  /**
   * If an action is invalid, send a message to the player alerting them.
   * @param action The action that was invalid.
   */
  private showInvalidAction(action: BattleAction): void {

  }

  /**
   * Prepares formations for use in this component.
   * @param data The object containing data about this battle.
   */
  private initializeFormations(data: Battle): void {
    this.attackingFormations = data.attackers;
    this.defendingFormations = data.defenders;

    if (data.attackers.some(attacker => attacker.ownerId === this.userId)) this.isUserAttacker = true;
    else this.isUserAttacker = false;

    //this.formationEntities = {};

    //this.indexFormations(data.attackers, true);
    //this.indexFormations(data.defenders, false);
  }

  /**
   * Makes the provided entity, if any, the currently active entity.
   * 
   * If no entity is provided, finds the next available entity and makes it the currently active entity.
   *
   * Will throw errors if the entity does not belong to the player or if the entity cannot be found in the player's
   * formation.
   * @param entity The entity to make active.
   */
  private setNextActiveEntity(entity?: CombatEntity): void {
    let ownerId: string;
    let entityId: number;

    if (entity == null) {
      let activeEntities: ActiveEntities = this.activeEntities.find(ae => ae.ownerId === this.userId);
      if (activeEntities == null) throw new Error("Cannot find activeEntities for the user in setNextActiveEntity!");

      if (activeEntities.entityIds.length === 0) {
        this.activeEntity = null;
        this.activeEntityPosition = -1;
        return;
      }

      ownerId = activeEntities.ownerId;
      entityId = activeEntities.entityIds[0];
    }
    else {
      // Prevent trying to set another player's entity as the active entity
      if (entity.ownerId !== this.userId) return;
      let activeEntities = this.activeEntities.find(ae => ae.ownerId === entity.ownerId);
      if (activeEntities == null) return;

      // If the provided entity cannot act, ignore setting it as an active entity.
      if (!activeEntities.entityIds.some(id => id === entity.id)) return;

      ownerId = entity.ownerId;
      entityId = entity.id;
    }

    let formation: Formation;
    if (this.isUserAttacker) formation = this.attackingFormations.find(f => f.ownerId === ownerId);
    else formation = this.defendingFormations.find(f => f.ownerId === ownerId);
    if (formation == null) throw new Error("Couldn't find user's formation in setNextActiveEntity!");

    // Find the first instance of an entity with the same id as the new active entity id
    let isFound = false;
    let i = 0;
    while (i < formation.positions.length && !isFound) {
      let j = 0;
      while (j < formation.positions[i].length && !isFound) {
        if (formation.positions[i][j] != null && formation.positions[i][j].id === entityId) {
          this.activeEntity = formation.positions[i][j];
          this.activeEntityPosition = i * FormationConstants.maxColumns + j + 1;
          isFound = true;
        }
        j++;
      }
      i++
    }

    if (!isFound) throw new Error("Couldn't find CombatEntity in the given Formation in setNextActiveEntity!");
  }


  private startTurn(): void {
    if (this.isUserAttacker === !this.isDefendersTurn) {
      this.setNextActiveEntity();
    }
  }

  ///**
  // * Stores the given formations in dictionary objects for fast lookup.
  // * @param formations The formations to store.
  // * @param isAttacker Stores the formations as attackers if true, else stores them as defenders.
  // */
  //private indexFormations(formations: Formation[], isAttacker: boolean): void {
  //  if (isAttacker) this.attackingFormationPositions = {};
  //  else this.defendingFormationPositions = {};

  //  formations.forEach(formation => {
  //    let positions: number[][] = [];
  //    let entities: Record<number, CombatEntity> = {};

  //    formation.positions.forEach(row => {
  //      let idRow: number[] = [];
  //      row.forEach(entity => {
  //        if (entity != null) {
  //          idRow.push(entity.id);
  //          entities[entity.id] = entity;
  //        }
  //        else idRow.push(null);
  //      });

  //      positions.push(idRow);
  //    });

  //    if (isAttacker) this.attackingFormationPositions[formation.id] = positions;
  //    else this.defendingFormationPositions[formation.id] = positions;

  //    this.formationEntities[formation.ownerId] = entities;
  //  });
  //}

  /**
   * When a FormationNode is clicked, performs different functions depending on the state of the game.
   *
   * If an ability is selected, performs an action on the location of the clicked node.
   *
   * If no ability is selected and the node has a CombatEntity, try to make that the active entity.
   * @param args The data object containing the location of the node.
   */
  public onNodeClicked(args: FormationNodeState): void {
    if (this.targetFormation == null) throw new Error("Formation node clicked with no target formation!");
    if (this.activeEntity == null) return;

    if (this.activeAbility == null) {
      // Tries to set the active entity to the one inside the clicked node
      if (args.entity != null) {
        if (args.entity.ownerId !== this.userId) return;
        let activeEntities: ActiveEntities = this.activeEntities.find(ae => ae.ownerId === this.userId);

        if (activeEntities == null) throw new Error("No ActiveEntities instance belonging to the user was found onNodeClicked!");
        if (!activeEntities.entityIds.some(id => id === args.entity.id)) return;

        this.activeEntity = args.entity;
        this.activeEntityPosition = args.coordinate.positionX * FormationConstants.maxColumns + args.coordinate.positionY + 1;
      }
      return;
    }

    if (!this.isValidTarget(args)) return;

    let action: BattleAction;
    try {
      action = this.createBattleAction();
    }
    catch (e) {
      throw new Error("Failed to create BattleAction instance!");
    }

    this.battleService.performAction(action);
  }

  /**Creates a BattleAction object from class variables. */
  private createBattleAction(): BattleAction {
    let action = new BattleAction();
    action.abilityId = this.activeAbility.ability.id;
    action.actorId = this.activeEntity.id;
    action.targetPosition = this.targetPosition;
    action.targetFormationId = this.targetFormation.id;
    action.isUsingItem = this.activeAbility.isUsingItem;

    if (this.activeAbility.ability.isPointBlank) action.targetPosition = this.activeEntityPosition;

    return action;
  }

  /**Sends a request to the server to perform a Flee or Defend action using the currently active entity. */
  public performFleeOrDefend(isFleeing: boolean): void {
    if (this.activeEntity == null) return;

    let action = new BattleAction();
    action.actorId = this.activeEntity.id;
    action.isDefending = !isFleeing;
    action.isFleeing = isFleeing;
    action.isUsingItem = false;
    action.ownerId = this.userId;

    let ownerFormation = this.attackingFormations.find(formation => formation.ownerId === this.userId);
    if (ownerFormation == null) ownerFormation = this.defendingFormations.find(formation => formation.ownerId === this.userId);
    if (ownerFormation == null) throw new Error("Couldn't find the player's formation when fleeing or defending!");

    action.targetFormationId = ownerFormation.id;
    action.targetPosition = this.activeEntityPosition;

    this.battleService.performAction(action);
  }

  /**
   * Whenever the user's mouse enters a FormationNode, if there is an active ability, show all of the targets that
   * will be hit by the target ability.
   * @param args
   */
  public onNodeMouseEnter(args: FormationNodeState): void {
    if (this.activeAbility == null) return;
    if (this.targetFormation == null) return;
    if (!this.isValidTarget(args)) return;

    this.targetPosition = args.coordinate.positionX * FormationConstants.maxColumns + args.coordinate.positionY + 1;

    this.targetPositions = FormationTargeter.translate(this.activeAbility.ability.centerOfTargets,
                                                       this.activeAbility.ability.targets,
                                                       this.targetPosition);
  }

  /**
   * Returns true if a given node is a valid target position for the currently active Ability.
   * @param args
   */
  private isValidTarget(args: FormationNodeState): boolean {
    if (this.activeAbility == null) return false;
    if (this.targetFormation == null) return false;
    if (this.activeEntity == null) return false;

    // If the active ability is point blank, requires the user's formation to be targeted to be valid
    if (this.activeAbility.ability.isPointBlank && this.activeEntity.ownerId !== this.targetFormation.ownerId) return false;
    else if (this.activeAbility.ability.isPointBlank && this.activeEntity.ownerId === this.targetFormation.ownerId) return true;

    // Target position doesn't matter for static target abilities, always valid in this case
    if (this.activeAbility.ability.isPositionStatic) return true;
    // If target can't be blocked, any target position is valid
    if (!this.activeAbility.ability.canTargetBeBlocked) return true;

    let positionNumber = args.coordinate.positionX * FormationConstants.maxColumns + args.coordinate.positionY + 1;
    return !FormationTargeter.isTargetBlocked(this.activeAbility.ability, positionNumber, this.targetFormation);
  }

  /**
   * When the user's mouse enters a FormationGrid component, set the targeted formation to the formation encapsulated
   * by that FormationGrid component.
   * @param formation
   */
  public onMouseEnterFormationGrid(formation: Formation): void {
    this.targetFormation = formation;
  }

  /**
   * Called whenever an Ability is selected by the player in the CombatPanel. Changes the active ability
   * to the one selected by the player.
   * @param ability The ability chosen by the player to make active.
   */
  public changeSelectedAbility(ability: SelectedAbilityData): void {
    this.activeAbility = ability;
  }

  /**
   * Returns a string representing the css class of the current state of a node, provided the entity that
   * exists in the node.
   * @param nodeState The state of the given node.
   */
  public getNodeState(nodeState: FormationNodeState): string {
    let position = nodeState.coordinate.positionX * FormationConstants.maxColumns + nodeState.coordinate.positionY + 1;

    if (this.targetPosition === position) return "formation-node-target-center";
    if (this.targetPositions != null && this.targetPositions.indexOf(position) !== -1) return "formation-node-target";

    if (nodeState.entity == null) return "";
    if (this.hoveredEntity != null && nodeState.entity === this.hoveredEntity) return "formation-node-hovered";
    if (this.activeEntity != null && this.activeEntity === nodeState.entity) return "formation-node-active";
    // If the node contains an entity that can act but is not active
    if (this.activeEntities.some(ae => ae.entityIds.some(eid => eid === nodeState.entity.id))) return "formation-node-inactive";

    else return "";
  }
}
