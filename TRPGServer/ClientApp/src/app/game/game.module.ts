import { NgModule } from "@angular/core";
import { GameComponent } from "./game/game.component";
import { WorldEntityService } from "./services/world-entity.service";
import { MapService } from "./services/map.service";
import { RouterModule } from "@angular/router";
import { SharedModule } from "../shared/shared.module";
import { AuthGuard } from "../shared/auth/auth.guard";
import { CommonModule } from "@angular/common";
import { ChatboxComponent } from "./chatbox/chatbox.component";
import { MessageComponent } from "./chatbox/message.component";
import { FormsModule } from "@angular/forms";
import { GameStateService } from "./services/game-state.service";
import { ChatService } from "./services/chat.service";
import { MapTileComponent } from "./mapTile/map-tile.component";

@NgModule({
  declarations: [
    GameComponent,
    ChatboxComponent,
    MessageComponent,
    MapTileComponent
  ],

  imports: [
    SharedModule,
    CommonModule,
    FormsModule,
    RouterModule.forChild([
      { path: 'game', component: GameComponent, canActivate: [AuthGuard] },
    ])
  ],

  providers: [
    WorldEntityService,
    MapService,
    GameStateService,
    ChatService
  ],

  exports: [
    RouterModule
  ]
})
export class GameModule { }
