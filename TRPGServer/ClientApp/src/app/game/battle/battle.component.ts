import { Component, OnInit } from "@angular/core";
import { HubConnectionBuilder, HubConnection } from "@aspnet/signalr";
import { LocalStorageConstants } from "../../constants";
import { StateHandlerService } from "../services/state-handler.service";
import { BattleAction } from "../model/battle-action.model";
import { CombatEntity } from "../model/combat-entity.model";
import { Ability } from "../model/ability.model";
import { Battle } from "../model/battle.model";
import { StartOfTurnData, ActiveEntities } from "../model/start-of-turn-data.model";
import { Router } from "@angular/router";
import { TwoDArray } from "../../shared/static/two-d-array.static";
import { Formation } from "../model/formation.model";
import { DisplayableEntity } from "../model/display-entity.interface";

@Component({
  selector: "game-battle-component",
  templateUrl: "./battle.component.html",
  styleUrls: ["./battle.component.css"]
})
export class BattleComponent implements OnInit {
  private connection: HubConnection;
  public activeAbility: Ability;
  public battle: Battle;
  public activeEntities: ActiveEntities[];
  public activeEntity: CombatEntity;
  public hoveredEntity: CombatEntity;
  public myFormation: Formation;

  constructor(private stateHandler: StateHandlerService,
    private router: Router) {

  }

  ngOnInit() {

  }

  /** Initializes this service asynchronously. */
  public async initializeAsync(): Promise<void> {
    this.connection = new HubConnectionBuilder()
      .withUrl("/hubs/battle", {
        accessTokenFactory: () => localStorage.getItem(LocalStorageConstants.authToken)
      })
      .build();
    this.connection.on("initialize", (data: Battle) => this.startBattle(data));
    this.connection.on("startOfTurn", (data: StartOfTurnData) => this.onStartOfTurn(data));
    this.connection.on("endOfTurn", (affectedEntities: CombatEntity[]) => this.onEndOfTurn(affectedEntities));
    this.connection.on("endBattle", () => this.onEndOfBattle());
    this.connection.start()
      .catch((err) => console.log(err))
      .then(() => {
        this.connection.send("startConnection");
      });
  }

  public async performActionAsync(): Promise<void> {
    let action = new BattleAction();
    action.actorId = this.activeEntity.id;
    action.abilityId = this.activeAbility.id;
  }

  public getExtraIconUris(entity: CombatEntity): string[] {
    var extraIcons: string[] = [];
    if (this.activeEntity.id == entity.id) extraIcons.push("extra");
    if (entity.isActive) extraIcons.push("active");
    if (this.hoveredEntity != null && entity.id == this.hoveredEntity.id) extraIcons.push("hovered");

    return extraIcons;
  }

  public getIndexArray(): number[] {
    let largerFormations: Formation[];
    if (this.battle.attackers.length > this.battle.defenders.length) {
      largerFormations = this.battle.attackers;
    }
    else largerFormations = this.battle.defenders;

    var indexArr: number[] = [];
    largerFormations.forEach((val, index) => {
      indexArr.push(index);
    });

    return indexArr;
  }

  private startBattle(battle: Battle): void {
    this.battle = battle;
  }

  private onStartOfTurn(data: StartOfTurnData): void {
    if (data.affectedEntities != null) {
      this.updateEntities(data.affectedEntities);
    }
    this.activeEntities = data.activeEntities;
  }

  private onEndOfTurn(affectedEntities: CombatEntity[]): void {
    if (affectedEntities == null) return;
    else this.updateEntities(affectedEntities);
  }

  private updateEntities(entities: CombatEntity[]): void {
    if (entities == null) return;
    this.battle.attackers.forEach(formation => {
      formation.positions.forEach(row => {
        var i = 0;
        while (i < row.length) {
          if (row[i] != null) {
            var index = entities.findIndex(e => e.id == row[i].id);
            if (index !== -1) {
              row[i] = entities[index];
            }
          }

          i++;
        }
      });
    });
    this.battle.defenders.forEach(formation => {
      formation.positions.forEach(row => {
        var i = 0;
        while (i < row.length) {
          if (row[i] != null) {
            var index = entities.findIndex(e => e.id == row[i].id);
            if (index !== -1) {
              row[i] = entities[index];
            }
          }

          i++;
        }
      });
    });
  }

  private onEndOfBattle(): void {
    this.router.navigateByUrl("/game");
  }
}
