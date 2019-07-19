import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { BattleService } from "../services/battle.service";
import { Subscription } from "rxjs/Subscription";
import { Battle } from "../model/battle.model";
import { ActiveEntities, ActionPointsChanged, ActionPointData } from "../model/start-of-turn-data.model";
import { CombatEntity } from "../model/combat-entity.model";
import { LocalStorageConstants } from "../../constants";
import { Formation, FormationConstants } from "../model/formation.model";
import { SuccessfulAction, BattleAction } from "../model/battle-action.model";
import { FormationNodeState } from "../formation/formationGrid/formationNode/formation-node.component";
import { FormationTargeter } from "../services/formation-targeter.static";
import { SelectedAbilityData, CombatPanelComponent } from "../combatPanel/combat-panel.component";
import { GameplayConstants } from "../gameplay-constants.static";
import { StateHandlerService } from "../services/state-handler.service";
import { PlayerStateConstants } from "../player-state-constants.static";

/**Component responsible for displaying to the user the state of a battle as well as allowing
 * the user to perform actions in battle.*/
@Component({
  selector: "game-battle-component",
  templateUrl: "./battle.component.html",
  styleUrls: ["./battle.component.css"]
})
export class BattleComponent implements OnInit, OnDestroy {
  constructor(private battleService: BattleService,
    private stateHandler: StateHandlerService) {
    this.subscriptions = [];
  }

  @ViewChild("combatPanel") combatPanel: CombatPanelComponent;

  public battle: Battle;
  public activeEntities: ActiveEntities[];
  public secondsInTurn: number;
  public isDefendersTurn: boolean;
  public round: number;

  // Game data
  private userId: string;
  private userFormation: Formation;
  public isUserAttacker: boolean;
  public attackingFormations: Formation[];
  public defendingFormations: Formation[];

  // User data
  public activeEntity: CombatEntity;
  public hoveredEntity: CombatEntity;
  public targetPosition: number;
  public targetPositions: number[];
  public targetFormation: Formation;
  public activeEntityPosition: number;
  public activeAbility: SelectedAbilityData;
  public errorMessage: string;
  public battleMessage: string;
  public showExitPrompt: boolean = false;
  public didAttackersWin: boolean = false;

  private subscriptions: Subscription[];

  ngOnInit(): void {
    this.subscriptions = [];
    this.userId = sessionStorage.getItem(LocalStorageConstants.userId);

    this.initializeAsync();
  }

  /**Initializes and subscribes to all event emitters in the BattleService. */
  private async initializeAsync(): Promise<void> {
    await this.battleService.initializeAsync();

    this.subscriptions.push(
      this.battleService.onInitialized.subscribe({
        next: (data) => {
          this.activeEntities = data.activeEntities;
          this.secondsInTurn = data.secondsLeftInTurn;
          this.isDefendersTurn = data.isDefenderTurn;
          this.round = data.round;
          this.initializeFormations(data);
          this.setNextActiveEntity();
        }
      })
    );

    this.subscriptions.push(
      this.battleService.onStartOfTurn.subscribe({
        next: (data) => {
          this.isDefendersTurn = data.isDefendersTurn;
          this.secondsInTurn = data.turnExpiration;
          this.applyEntityChanges(data.affectedEntities);
          this.applyActionPoints(data.actionPointsChanged);
          this.activeEntities = data.activeEntities;
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
        next: (invalidAction) => {
          this.errorMessage = invalidAction.errorMessage;
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
          this.didAttackersWin = didAttackersWin;
          this.showExitPrompt = true;
        }
      })
    );

    this.subscriptions.push(
      this.battleService.onJoinedBattle.subscribe({
        next: (data) => {
          if (data.isAttacker) {
            this.attackingFormations.push(data.joinedFormation);
          }
          else this.defendingFormations.push(data.joinedFormation);

          if (data.activeEntities != null && data.activeEntities.length > 0) {
            data.activeEntities.forEach(entitySet => {
              this.activeEntities.push(entitySet);
            });
          }

          if (this.isUserAttacker && data.isAttacker) {
            this.battleMessage = data.joinedFormation.name + " has joined our side in battle!";
          }
          else {
            this.battleMessage = data.joinedFormation.name + " has joined the enemy's side in battle!";
          }
        }
      })
    )

    await this.battleService.startConnection();
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
    if (action.nextActiveEntityId !== -1) {
      this.addActiveEntity(action);
    }
    if (action.actor.ownerId === this.userId) this.setNextActiveEntity();
    
    if (action.ability != null) {
      // Animate ability effects
    }
    this.applyEntityChanges(action.affectedEntities);
  }

  /**
   * Adds a new CombatEntity to the ActiveEntities object.
   * @param action The object to extract the new active CombatEntity from.
   */
  private addActiveEntity(action: SuccessfulAction): void {
    let activeEntities: ActiveEntities;

    if (action.nextActiveEntityId === -1) return;

    if (action.actor.ownerId === GameplayConstants.aiId) {
      let aeArray = this.activeEntities.filter(ae => ae.ownerId === GameplayConstants.aiId);
      activeEntities = aeArray.find(ae => ae.entityIds.some(id => id === action.actor.id))
    }
    else activeEntities = this.activeEntities.find(ae => ae.ownerId === action.actor.ownerId);

    activeEntities.entityIds.push(action.nextActiveEntityId);
  }

  /**
   * Removes the entity who successfully performed an action from the activeEntities array.
   *
   * If the currently active entity is the actor who successfully performed the action,
   * clears the selection from the CombatPanel.
   * @param action The object containing data about the action that was successfully performed.
   */
  private removeActiveEntity(action: SuccessfulAction): void {
    let activeEntities: ActiveEntities

    if (action.actor == null) return;

    // Remove actor from activeEntities array
    if (action.actor.ownerId === GameplayConstants.aiId) {
      let aeArray = this.activeEntities.filter(ae => ae.ownerId === GameplayConstants.aiId);
      activeEntities = aeArray.find(ae => ae.entityIds.some(id => id === action.actor.id))
    }
    else activeEntities = this.activeEntities.find(ae => ae.ownerId === action.actor.ownerId);

    if (activeEntities == null) throw new Error("No ActiveEntity set was found!");

    let removeIndex = activeEntities.entityIds.indexOf(action.actor.id);
    if (removeIndex != -1) activeEntities.entityIds.splice(removeIndex, 1);

    // Clears selection if the active entity was the actor successfully performing an action
    if (action.actor.ownerId === this.userId && this.activeEntity != null) {
      if (this.activeEntity.id === action.actor.id) this.clearSelection();
    }
  }

  /**Clears the currently active Entity and Ability. */
  private clearSelection(): void {
    this.activeAbility = null;
    this.targetPosition = null;
    this.targetPositions = null;
    this.combatPanel.reset();
  }

  /**Exits the current battle. */
  public exitBattle(): void {
    this.stateHandler.handleStateAsync(PlayerStateConstants.free);
  }

  /**
   * Replaces the CombatEntities stored in memory with the newly updated entities.
   * @param entities An array of all newly updated CombatEntities.
   */
  private applyEntityChanges(entities: CombatEntity[]): void {
    let playerEntities: CombatEntity[] = [];
    let aiEntities: CombatEntity[] = [];

    entities.forEach(entity => {
      if (entity.ownerId !== GameplayConstants.aiId) playerEntities.push(entity);
      else aiEntities.push(entity);

      // If entity is the same as the hovered or active entity, refresh the references
      if (this.hoveredEntity != null && this.hoveredEntity.id === entity.id) this.hoveredEntity = entity;
      if (this.activeEntity != null && this.activeEntity.id === entity.id) this.activeEntity = entity;
    });

    // Update a player's CombatEntity
    playerEntities.forEach(entity => {
      let formation: Formation = this.attackingFormations.find(f => f.ownerId === entity.ownerId);
      if (formation == null) formation = this.defendingFormations.find(f => f.ownerId === entity.ownerId);
      if (formation == null) return;
      
      for (var i = 0; i < formation.positions.length; i++) {
        for (var j = 0; j < formation.positions[i].length; j++) {
          if (formation.positions[i][j] == null) continue;
          if (formation.positions[i][j].id === entity.id) {
            formation.positions[i][j] = entity;
            return;
          }
        }
      }
    });

    // Update the ai's CombatEntity
    if (aiEntities.length > 0) {
      let aiFormations = this.attackingFormations.concat(this.defendingFormations)
        .filter(formation => formation.ownerId === GameplayConstants.aiId);

      aiFormations.forEach(formation => {
        for (var i = 0; i < formation.positions.length; i++) {
          for (var j = 0; j < formation.positions[i].length; j++) {
            if (aiEntities.length === 0) return;
            if (formation.positions[i][j] == null) continue;

            let removeIndex: number;
            var found = aiEntities.some((entity, index) => {
              if (entity.id === formation.positions[i][j].id) {
                removeIndex = index;
                return true;
              }
              else return false;
            });

            if (found) {
              var entity = aiEntities.splice(removeIndex, 1)[0];
              formation.positions[i][j] = entity;
            }
          }
        }
      });
    }
  }

  /**
   * Applies up-to-date action point data for every CombatEntity in every active Formation.
   * @param data Object containing the amount of action points changed for every CombatEntity in a Formation.
   */
  private applyActionPoints(data: ActionPointsChanged[]): void {
    if (data == null) return;

    let formations: Formation[];
    if (this.isDefendersTurn) formations = this.defendingFormations;
    else formations = this.attackingFormations;

    data.forEach(dataSet => {
      let formation: Formation = formations.find(f => f.id === dataSet.formationId);
      if (formation == null) throw new Error("Couldn't find a formation with " + dataSet.formationId + "id in applyActionPoints!");

      formation.positions.forEach(row => {
        row.forEach(entity => {
          if (entity == null) return;
          let actionPointData: ActionPointData = dataSet.actionPointData.find(apData => apData.entityId === entity.id);
          if (actionPointData == null) return;

          entity.resources.currentActionPoints = actionPointData.currentActionPoints;
        })
      })
    });
  }

  /**
   * Prepares formations for use in this component.
   * @param data The object containing data about this battle.
   */
  private initializeFormations(data: Battle): void {
    this.attackingFormations = data.attackers;
    this.defendingFormations = data.defenders;

    let userFormation = data.attackers.find(attacker => attacker.ownerId === this.userId);
    if (userFormation != null) {
      this.isUserAttacker = true;
      this.userFormation = userFormation;
    }
    else {
      this.isUserAttacker = false;
      this.userFormation = data.defenders.find(defender => defender.ownerId === this.userId);
    }

    if (this.isUserAttacker !== data.isDefenderTurn) {
      this.setNextActiveEntity();
    }
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
    let chooseAny: boolean = false;
    let isMyTurn = this.isDefendersTurn === !this.isUserAttacker;

    if (entity == null) {
      let activeEntities: ActiveEntities = this.activeEntities.find(ae => ae.ownerId === this.userId);
      if (activeEntities == null && isMyTurn)
        throw new Error("Cannot find activeEntities for the user in setNextActiveEntity!");
      // Not my turn, just choose any of my CombatEntities
      else if (activeEntities == null && !isMyTurn) {
        chooseAny = this.activeEntity == null;
        ownerId = this.userId;
        entityId = -1;
      }

      // No more active entities available for this turn
      else if (activeEntities.entityIds.length === 0) return;
      else {
        ownerId = activeEntities.ownerId;
        entityId = activeEntities.entityIds[0];
      }
    }
    else {
      // Prevent trying to set another player's entity as the active entity
      if (entity.ownerId !== this.userId) return;

      // Prevent setting an inactive entity as the active entity if on the player's turn
      if (isMyTurn) {
        let activeEntities = this.activeEntities.find(ae => ae.ownerId === entity.ownerId);
        if (activeEntities == null) return;

        // If the provided entity cannot act, ignore setting it as an active entity.
        if (!activeEntities.entityIds.some(id => id === entity.id)) return;
      }

      ownerId = entity.ownerId;
      entityId = entity.id;
    }

    let formation: Formation;
    if (this.isUserAttacker) formation = this.attackingFormations.find(f => f.ownerId === ownerId);
    else formation = this.defendingFormations.find(f => f.ownerId === ownerId);
    if (formation == null) throw new Error("Couldn't find user's formation in setNextActiveEntity!");

    // Find the first instance of an entity with the same id as the new active entity id or any entity if chooseAny is true
    let isFound = false;
    let i = 0;
    while (i < formation.positions.length && !isFound) {
      let j = 0;
      while (j < formation.positions[i].length && !isFound) {
        if (formation.positions[i][j] != null && (
                formation.positions[i][j].id === entityId || chooseAny)) {
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

  /**On the start of a turn, sets the next active entity if it's the player's turn. Else resets all
   * the player's selections.*/
  private startTurn(): void {
    if (this.isUserAttacker === !this.isDefendersTurn) {
      this.setNextActiveEntity();
    }
    else {
      this.targetFormation = null;
      this.targetPosition = null;
    }

    this.activeAbility = null;
    this.targetPositions = null;
    this.combatPanel.reset();
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
        //if (args.entity.ownerId !== this.userId) return;
        //let activeEntities: ActiveEntities = this.activeEntities.find(ae => ae.ownerId === this.userId);

        //if (activeEntities == null) throw new Error("No ActiveEntities instance belonging to the user was found onNodeClicked!");
        //if (!activeEntities.entityIds.some(id => id === args.entity.id)) return;

        //this.activeEntity = args.entity;
        //this.activeEntityPosition = args.coordinate.positionY * FormationConstants.maxColumns + args.coordinate.positionX + 1;
        this.setNextActiveEntity(args.entity);
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

  /**
   * Called when the context menu is opened in the BattleComponent.
   * @param event
   */
  public onContextMenu(event: MouseEvent): void {
    event.preventDefault();
    this.clearSelection();
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
   * will be hit by the target ability. Else, sets the hovered entity to that node's CombatEntity.
   * @param args
   */
  public onNodeMouseEnter(args: FormationNodeState): void {
    this.hoveredEntity = args.entity;
    if (this.activeAbility == null) return;

    if (this.targetFormation == null) return;
    if (!this.isValidTarget(args)) return;

    this.targetPosition = args.coordinate.positionY * FormationConstants.maxRows + args.coordinate.positionX + 1;

    if (this.activeAbility.ability.isPointBlank) {
      this.targetPosition = this.activeEntityPosition;
      this.targetPositions = FormationTargeter.translate(this.activeAbility.ability.centerOfTargets,
                                                         this.activeAbility.ability.targets,
                                                         this.targetPosition);
      this.targetFormation = this.userFormation;
      return;
    }
    if (this.activeAbility.ability.isPositionStatic) {
      this.targetPosition = this.activeAbility.ability.centerOfTargets;
      this.targetPositions = this.activeAbility.ability.targets;
      return;
    }
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

    // Allow the user to attack its own formation even if the active ability can be blocked by the formation
    if (this.targetFormation.ownerId === this.userId) return true;
    
    let positionNumber = args.coordinate.positionY * FormationConstants.maxColumns + args.coordinate.positionX + 1;
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
    if (ability == null) {
      this.targetPosition = null;
      this.targetPositions = null;
    }
  }

  /**
   * Returns a string representing the css class of the current state of a node, provided the entity that
   * exists in the node.
   * @param nodeState The state of the given node.
   */
  public getNodeState(nodeState: FormationNodeState): string {
    let position = nodeState.coordinate.positionY * FormationConstants.maxRows + nodeState.coordinate.positionX + 1;

    // If the ability is point blank, keep targets styled even when mousing over other formations
    if (this.activeAbility != null
        && this.activeAbility.ability.isPointBlank
        && nodeState.formation.id === this.userFormation.id) {
      if (this.targetPosition === position && this.targetPositions.some(pos => position === pos)) return "formation-node-target-center";
      if (this.targetPositions != null && this.targetPositions.indexOf(position) !== -1) return "formation-node-target";
    }

    // Style static target abilities that are not point blank
    if (this.targetFormation != null
        && this.activeAbility != null
        && !this.activeAbility.ability.isPointBlank
        && nodeState.formation.id === this.targetFormation.id) {
      // Show center only if cursor is over node and the target positions contains that position
      if (this.targetPosition === position && this.targetPositions.some(pos => position === pos)) return "formation-node-target-center";
      if (this.targetPositions != null && this.targetPositions.indexOf(position) !== -1) return "formation-node-target";
    }

    if (nodeState.entity == null) return "";
    if (this.hoveredEntity != null && nodeState.entity.id === this.hoveredEntity.id) return "formation-node-hovered";

    // If it is the player's turn and there is at least 1 entity that can act in the player's formation, return active style
    if (this.isDefendersTurn !== this.isUserAttacker) {
      if (this.activeEntity != null
        && this.activeEntity.id === nodeState.entity.id) {
        if (this.activeEntities != null) {
          let myActiveEntities: ActiveEntities = this.activeEntities.find(ae => ae.ownerId === this.userId);
          if (myActiveEntities == null || myActiveEntities.entityIds.length <= 0) return "";
          else return "formation-node-active";
        }
      }
    }

    // If the node contains an entity that can act but is not active
    if (this.activeEntities != null) {
      if (this.activeEntities.some(ae => ae.entityIds.some(eid => eid === nodeState.entity.id))) return "formation-node-inactive";
    }

    else return "";
  }

  /**
   * Returns the css style for a formation name for the given Formation.
   * @param formation The Formation to return css for.
   */
  public getFormationNameStyle(formation: Formation): string {
    if (formation.ownerId === this.userId) return "formation-player";

    // If on same side as user, return ally. Else return enemy.
    if (this.isUserAttacker === this.attackingFormations.some(attacker => attacker.id === formation.id)) return "formation-ally";
    else return "formation-enemy";
  }
}
