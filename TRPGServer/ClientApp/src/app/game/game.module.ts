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
import { StatPickerComponent } from "./create/statPicker/stat-picker.component";
import { StatSliderComponent } from "./create/statPicker/statSlider/stat-slider.component";
import { StatIconComponent } from "./create/statPicker/statSlider/statIcon/stat-icon.component";
import { DescriptionComponent } from "./create/description/description.component";
import { FormationComponent } from "./formation/formation.component";
import { DisplayEntityComponent } from "./displayEntity/display-entity.component";
import { FormationFactory } from "./services/formation.factory";
import { FormationGridComponent } from "./formation/formationGrid/formation-grid.component";
import { FormationNodeComponent } from "./formation/formationGrid/formationNode/formation-node.component";
import { IconDisplayComponent } from "./iconDisplay/icon-display.component";
import { CombatPanelComponent } from "./combatPanel/combat-panel.component";
import { AbilityPanelComponent } from "./combatPanel/abilityPanel/ability-panel.component";
import { CategoryPanelComponent } from "./combatPanel/categoryPanel/category-panel.component";
import { DescriptionPanelComponent } from "./combatPanel/descriptionPanel/description-panel.component";
import { TargetsGridComponent } from "./combatPanel/descriptionPanel/targetsGrid/targets-grid.component";
import { ClassPickerComponent } from "./create/classPicker/class-picker.component";
import { StateHandlerService } from "./services/state-handler.service";
import { BattleService } from "./services/battle.service";
import { BattleComponent } from "./battle/battle.component";
import { EntityResourceComponent } from "./battle/entityResource/entity-resource.component";
import { ResourceBarComponent } from "./battle/entityResource/resourceBar/resource-bar.component";
import { TimerComponent } from "./battle/timer/timer.component";
import { ContextMenuComponent } from "./game/contextMenu/context-menu.component";
import { ContextMenuListComponent } from "./game/contextMenu/contextMenuList/context-menu-list.component";
import { EntityCardComponent } from "./entityCard/entity-card.component";

@NgModule({
  declarations: [
    GameComponent,
    CreateComponent,
    FormationComponent,
    ChatboxComponent,
    MessageComponent,
    TileNodeComponent,
    MapTileComponent,
    WorldEntityComponent,
    EntityCardComponent,
    StatPickerComponent,
    StatSliderComponent,
    StatIconComponent,
    DescriptionComponent,
    DisplayEntityComponent,
    FormationGridComponent,
    FormationNodeComponent,
    IconDisplayComponent,
    CombatPanelComponent,
    AbilityPanelComponent,
    CategoryPanelComponent,
    DescriptionPanelComponent,
    TargetsGridComponent,
    ClassPickerComponent,
    BattleComponent,
    EntityResourceComponent,
    ResourceBarComponent,
    TimerComponent,
    ContextMenuComponent,
    ContextMenuListComponent
  ],

  imports: [
    SharedModule,
    BrowserModule,
    BrowserAnimationsModule,
    CommonModule,
    FormsModule,
    RouterModule.forChild([
      { path: 'game', component: GameComponent, canActivate: [AuthGuard] },
      { path: 'create', component: CreateComponent, canActivate: [AuthGuard] },
      { path: 'formation', component: FormationComponent, canActivate: [AuthGuard] },
      { path: 'battle', component: BattleComponent, canActivate: [AuthGuard] }
    ])
  ],

  providers: [
    WorldEntityService,
    MapService,
    GameStateService,
    ChatService,
    FormationFactory,
    StateHandlerService,
    BattleService
  ],

  exports: [
    RouterModule
  ]
})
export class GameModule { }
