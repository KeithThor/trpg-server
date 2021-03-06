﻿using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;
using TRPGGame.Managers;
using TRPGGame.Managers.Combat.Interfaces;
using TRPGGame.Repository;

namespace TRPGGame.Services
{
    /// <summary>
    /// Factory responsible for creating new instances of BattleManagers.
    /// </summary>
    public class BattleManagerFactory
    {
        private readonly IAbilityManager _abilityManager;
        private readonly IEquipmentManager _equipmentManager;
        private readonly IStatusEffectManager _statusEffectManager;
        private readonly IRepository<StatusEffect> _statusEffectRepo;
        private readonly ICombatAi _combatAi;

        public BattleManagerFactory(IAbilityManager abilityManager,
                                    IEquipmentManager equipmentManager,
                                    IStatusEffectManager statusEffectManager,
                                    IRepository<StatusEffect> statusEffectRepo,
                                    ICombatAi combatAi)
        {
            _abilityManager = abilityManager;
            _equipmentManager = equipmentManager;
            _statusEffectManager = statusEffectManager;
            _statusEffectRepo = statusEffectRepo;
            _combatAi = combatAi;
        }

        /// <summary>
        /// Returns a new instance of a BattleManager.
        /// </summary>
        /// <returns></returns>
        public IBattleManager Create()
        {
            return new BattleManager(_abilityManager,
                                     _equipmentManager,
                                     _statusEffectManager,
                                     _statusEffectRepo,
                                     _combatAi);
        }
    }
}
