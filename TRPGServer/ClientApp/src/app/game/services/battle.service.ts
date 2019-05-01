import { Injectable } from "@angular/core";
import { HubConnection, HubConnectionBuilder } from "@aspnet/signalr";
import { LocalStorageConstants } from "../../constants";
import { StateHandlerService } from "./state-handler.service";
import { ReplaySubject } from "rxjs";
import { BattleAction, SuccessfulAction } from "../model/battle-action.model";
import { StartOfTurnData } from "../model/start-of-turn-data.model";
import { CombatEntity } from "../model/combat-entity.model";
import { Battle } from "../model/battle.model";

@Injectable()
export class BattleService {
  constructor(private stateHandler: StateHandlerService) {

  }

  private connection: HubConnection;
  public onInitialized: ReplaySubject<Battle>;
  public onInvalidAction: ReplaySubject<BattleAction>;
  public onStartOfTurn: ReplaySubject<StartOfTurnData>;
  public onSuccessfulAction: ReplaySubject<SuccessfulAction>;
  public onEndOfTurn: ReplaySubject<CombatEntity[]>;
  public onEndOfBattle: ReplaySubject<boolean>;

  /** Initializes this service asynchronously. */
  public async initializeAsync(): Promise<void> {
    this.onInitialized = new ReplaySubject(1);
    this.onInvalidAction = new ReplaySubject(1);
    this.onStartOfTurn = new ReplaySubject(1);
    this.onSuccessfulAction = new ReplaySubject(1);
    this.onEndOfTurn = new ReplaySubject(1);
    this.onEndOfBattle = new ReplaySubject(1);

    this.connection = new HubConnectionBuilder()
      .withUrl("/hubs/battle", {
        accessTokenFactory: () => sessionStorage.getItem(LocalStorageConstants.authToken)
      })
      .build();

    this.connection.on("initialized", (data: Battle) => this.onInitialized.next(data));
    this.connection.on("invalidAction", (action: BattleAction) => this.onInvalidAction.next(action));
    this.connection.on("startOfTurn", (data: StartOfTurnData) => this.onStartOfTurn.next(data));
    this.connection.on("endOfTurn", (data: CombatEntity[]) => this.onEndOfTurn.next(data));
    this.connection.on("endBattle", (didAttackersWin: boolean) => this.onEndOfBattle.next(didAttackersWin));
    this.connection.on("notInCombat", (state: string) => this.stateHandler.handleStateAsync(state));
    this.connection.on("actionSuccess", (data: SuccessfulAction) => this.onSuccessfulAction.next(data));

    await this.connection.start()
      .catch((err) => console.log(err));
  }
}
