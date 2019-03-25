import { NgModule } from "@angular/core";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
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
import { WorldEntityComponent } from "./worldEntity/world-entity.component";
import { CreateComponent } from "./create/create.component";
import { BrowserModule } from "@angular/platform-browser";
import { TileNodeComponent } from "./game/tile-node.component";
import { EditEntityComponent } from "./create/editEntity/edit-entity.component";
import { StatPickerComponent } from "./create/statPicker/stat-picker.component";
import { StatSliderComponent } from "./create/statPicker/statSlider/stat-slider.component";
import { StatIconComponent } from "./create/statPicker/statSlider/statIcon/stat-icon.component";

@NgModule({
  declarations: [
    GameComponent,
    CreateComponent,
    ChatboxComponent,
    MessageComponent,
    TileNodeComponent,
    MapTileComponent,
    WorldEntityComponent,
    EditEntityComponent,
    StatPickerComponent,
    StatSliderComponent,
    StatIconComponent
  ],

  imports: [
    SharedModule,
    BrowserModule,
    BrowserAnimationsModule,
    CommonModule,
    FormsModule,
    RouterModule.forChild([
      { path: 'game', component: GameComponent, canActivate: [AuthGuard] },
      { path: 'create', component: CreateComponent, canActivate: [AuthGuard] }
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
