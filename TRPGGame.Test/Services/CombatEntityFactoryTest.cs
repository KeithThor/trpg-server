using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Entities.Combat;
using TRPGGame.Entities.Data;
using TRPGGame.Managers;
using TRPGGame.Repository;
using TRPGGame.Services;
using Xunit;

namespace TRPGGame.Test.Services
{
    public class CombatEntityFactoryTest
    {
        private readonly CombatEntityFactory _combatEntityFactory;

        public CombatEntityFactoryTest()
        {
            _combatEntityFactory = new CombatEntityFactory(new CharacterBaseRepoStub(),
                                                           new CharacterHairRepoStub(),
                                                           new ClassTemplateRepoStub(),
                                                           new EquipmentManagerStub(),
                                                           new StatusEffectManagerStub());
        }

        [Theory]
        [MemberData(nameof(GetValidTemplates))]
        public async Task CreateAsync_WithValidTemplate_ReturnsNotNull(CharacterTemplate template)
        {
            var entity = await _combatEntityFactory.CreateAsync(template);

            Assert.NotNull(entity);
        }

        [Theory]
        [MemberData(nameof(GetHighStatTemplates))]
        public async Task CreateAsync_OverMaxStatButWithinStatLimit_ReturnsNull(CharacterTemplate template)
        {
            var entity = await _combatEntityFactory.CreateAsync(template);

            Assert.Null(entity);
        }

        [Theory]
        [MemberData(nameof(GetOverStatLimitTemplates))]
        public async Task CreateAsync_OverStatLimitButWithinMinMaxStatRange_ReturnsNull(CharacterTemplate template)
        {
            var entity = await _combatEntityFactory.CreateAsync(template);

            Assert.Null(entity);
        }

        [Theory]
        [MemberData(nameof(GetUnderMinStatTemplates))]
        public async Task CreateAsync_UnderMinStatButWithinStatLimit_ReturnsNull(CharacterTemplate template)
        {
            var entity = await _combatEntityFactory.CreateAsync(template);

            Assert.Null(entity);
        }

        [Theory]
        [MemberData(nameof(GetInvalidSelectionIdTemplates))]
        public async Task CreateAsync_WithInvalidTemplates_ReturnsNull(CharacterTemplate template)
        {
            var entity = await _combatEntityFactory.CreateAsync(template);

            Assert.Null(entity);
        }

        public static TheoryData<CharacterTemplate> GetValidTemplates() =>
            new TheoryData<CharacterTemplate>
            {
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 5,
                            Dexterity = 5,
                            Agility = 5,
                            Intelligence = 5,
                            Constitution = 5
                        },
                        BaseId = 1,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "TemplateA",
                        ClassTemplateId = 1
                    }
                },
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 12,
                            Dexterity = 1,
                            Agility = 2,
                            Intelligence = 3,
                            Constitution = 7
                        },
                        BaseId = 2,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "TemplateB",
                        ClassTemplateId = 1
                    }
                },
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 12,
                            Dexterity = 4,
                            Agility = 2,
                            Intelligence = 3,
                            Constitution = 4
                        },
                        BaseId = 3,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "TemplateC",
                        ClassTemplateId = 1
                    }
                },
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 9,
                            Dexterity = 6,
                            Agility = 5,
                            Intelligence = 3,
                            Constitution = 2
                        },
                        BaseId = 4,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "TemplateD",
                        ClassTemplateId = 1
                    }
                }
            };

        public static TheoryData<CharacterTemplate> GetHighStatTemplates()
        {
            return new TheoryData<CharacterTemplate>
            {
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 15,
                            Dexterity = 2,
                            Agility = 3,
                            Intelligence = 4,
                            Constitution = 1
                        },
                        BaseId = 1,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "ExceedsMaxStatButRemainsWithinTotalStatRange",
                        ClassTemplateId = 1
                    }
                },
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 1,
                            Dexterity = 1,
                            Agility = 11,
                            Intelligence = 3,
                            Constitution = 9
                        },
                        BaseId = 2,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "ExceedsMultipleMaxStats",
                        ClassTemplateId = 1
                    }
                },
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 13,
                            Dexterity = 4,
                            Agility = 2,
                            Intelligence = 3,
                            Constitution = 4
                        },
                        BaseId = 3,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "ExceedsMaxStatWithPositiveBonuses",
                        ClassTemplateId = 1
                    }
                },
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 4,
                            Dexterity = 3,
                            Agility = 1,
                            Intelligence = 13,
                            Constitution = 4
                        },
                        BaseId = 4,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "ExceedsMaxStatWithNegativeBonuses",
                        ClassTemplateId = 1
                    }
                }
            };
        }

        public static TheoryData<CharacterTemplate> GetOverStatLimitTemplates()
        {
            return new TheoryData<CharacterTemplate>
            {
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 7,
                            Dexterity = 8,
                            Agility = 6,
                            Intelligence = 8,
                            Constitution = 7
                        },
                        BaseId = 1,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "UnderStatMaxButOverStatLimit",
                        ClassTemplateId = 1
                    }
                },
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 8,
                            Dexterity = 7,
                            Agility = 8,
                            Intelligence = 8,
                            Constitution = 9
                        },
                        BaseId = 2,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "UnderStatMaxButOverStatLimit",
                        ClassTemplateId = 1
                    }
                },
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 10,
                            Dexterity = 7,
                            Agility = 7,
                            Intelligence = 6,
                            Constitution = 6
                        },
                        BaseId = 3,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "UnderStatMaxButOverStatLimitWithPositiveBonuses",
                        ClassTemplateId = 1
                    }
                },
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 9,
                            Dexterity = 4,
                            Agility = 8,
                            Intelligence = 6,
                            Constitution = 8
                        },
                        BaseId = 4,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "UnderStatMaxButOverStatLimitWithNegativeBonuses",
                        ClassTemplateId = 1
                    }
                }
            };
        }

        public static TheoryData<CharacterTemplate> GetUnderMinStatTemplates() =>
            new TheoryData<CharacterTemplate>
            {
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 0,
                            Dexterity = 5,
                            Agility = 5,
                            Intelligence = 9,
                            Constitution = 6
                        },
                        BaseId = 1,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "UnderStatMinButSameStatTotal",
                        ClassTemplateId = 1
                    }
                },
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = -4,
                            Dexterity = 8,
                            Agility = 8,
                            Intelligence = 6,
                            Constitution = 7
                        },
                        BaseId = 2,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "WayUnderMinStat",
                        ClassTemplateId = 1
                    }
                },
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = -2,
                            Dexterity = 8,
                            Agility = 9,
                            Intelligence = 6,
                            Constitution = 4
                        },
                        BaseId = 3,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "BelowMinStatWithPositiveBonus",
                        ClassTemplateId = 1
                    }
                },
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 6,
                            Dexterity = 5,
                            Agility = 9,
                            Intelligence = 1,
                            Constitution = 4
                        },
                        BaseId = 4,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "BelowMinStatWithNegativeBonus",
                        ClassTemplateId = 1
                    }
                }
            };

        public static TheoryData<CharacterTemplate> GetInvalidSelectionIdTemplates() =>
            new TheoryData<CharacterTemplate>
            {
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 5,
                            Dexterity = 5,
                            Agility = 5,
                            Intelligence = 5,
                            Constitution = 5
                        },
                        BaseId = 941231,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "BaseDoesNotExistForThisId",
                        ClassTemplateId = 1
                    }
                },
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 12,
                            Dexterity = 1,
                            Agility = 2,
                            Intelligence = 3,
                            Constitution = 7
                        },
                        BaseId = 2,
                        HairId = -789789,
                        EntityId = 1,
                        GroupId = null,
                        Name = "HairDoesNotExistForThisId",
                        ClassTemplateId = 1
                    }
                },
                {
                    new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = null,
                        BaseId = 3,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "StatsAreNullForThisObject",
                        ClassTemplateId = 1
                    }
                },
                new CharacterTemplate{
                        OwnerId = new Guid(),
                        OwnerName = "NameHere",
                        AllocatedStats = new CharacterStats
                        {
                            Strength = 5,
                            Dexterity = 5,
                            Agility = 5,
                            Intelligence = 5,
                            Constitution = 5
                        },
                        BaseId = 1,
                        HairId = 1,
                        EntityId = 1,
                        GroupId = null,
                        Name = "ClassTemplateDoesntExist",
                        ClassTemplateId = 112312312
                    }
            };
    }

    public class CharacterBaseRepoStub : IRepository<CharacterBase>
    {
        public Task<IEnumerable<CharacterBase>> GetDataAsync()
        {
            return Task.Run(() =>
            {
                IEnumerable<CharacterBase> bases = new List<CharacterBase>
                {
                    new CharacterBase()
                    {
                        Id = 1,
                        Name = "A",
                        IconUri = "",
                        BonusStats = new CharacterStats(),
                        MaxStats = new CharacterStats
                        {
                            Strength = 10,
                            Dexterity = 10,
                            Agility = 10,
                            Intelligence = 10,
                            Constitution = 10
                        }
                    },
                    new CharacterBase()
                    {
                        Id = 2,
                        Name = "B",
                        IconUri = "",
                        BonusStats = new CharacterStats(),
                        MaxStats = new CharacterStats
                        {
                            Strength = 12,
                            Dexterity = 10,
                            Agility = 8,
                            Intelligence = 10,
                            Constitution = 8
                        }
                    },
                    new CharacterBase()
                    {
                        Id = 3,
                        Name = "C",
                        IconUri = "",
                        BonusStats = new CharacterStats()
                        {
                            Strength = 2,
                            Dexterity = 0,
                            Agility = 0,
                            Intelligence = 0,
                            Constitution = 0
                        },
                        MaxStats = new CharacterStats
                        {
                            Strength = 14,
                            Dexterity = 10,
                            Agility = 10,
                            Intelligence = 10,
                            Constitution = 10
                        }
                    },
                    new CharacterBase()
                    {
                        Id = 4,
                        Name = "D",
                        IconUri = "",
                        BonusStats = new CharacterStats()
                        {
                            Strength = 0,
                            Dexterity = 0,
                            Agility = 0,
                            Intelligence = -2,
                            Constitution = 0
                        },
                        MaxStats = new CharacterStats
                        {
                            Strength = 10,
                            Dexterity = 10,
                            Agility = 10,
                            Intelligence = 10,
                            Constitution = 10
                        }
                    }
                };

                return bases;
            });
        }
    }

    public class CharacterHairRepoStub : IRepository<CharacterHair>
    {
        public Task<IEnumerable<CharacterHair>> GetDataAsync()
        {
            return Task.Run(() =>
            {
                IEnumerable<CharacterHair> hair = new List<CharacterHair>
                {
                    new CharacterHair
                    {
                        Id = 1,
                        IconUri = "",
                        Name = "A"
                    }
                };

                return hair;
            });
        }
    }

    public class ClassTemplateRepoStub : IRepository<ClassTemplate>
    {
        public Task<IEnumerable<ClassTemplate>> GetDataAsync()
        {
            return Task.Run(() =>
            {
                return new List<ClassTemplate>
                {
                    new ClassTemplate
                    {
                        Id = 1
                    }
                } as IEnumerable<ClassTemplate>;
            });
        }
    }

    public class EquipmentManagerStub : IEquipmentManager
    {
        public bool Equip(CombatEntity entity, Item item)
        {
            return true;
        }

        public void ReduceCharges(CombatEntity entity, Item item)
        {
            return;
        }

        public void Unequip(CombatEntity entity, Item item)
        {
            return;
        }
    }

    public class StatusEffectManagerStub : IStatusEffectManager
    {
        public void Apply(CombatEntity recipient, CombatEntity applicator, StatusEffect statusEffect, bool isCrit = false)
        {
            return;
        }

        public void Apply(CombatEntity recipient, CombatEntity applicator, IEnumerable<StatusEffect> statusEffects, bool isCrit = false)
        {
            return;
        }

        public void ApplyEffects(CombatEntity entity)
        {
            return;
        }

        public bool Remove(CombatEntity entity, AppliedStatusEffect appliedStatusEffect)
        {
            return true;
        }

        public bool Remove(CombatEntity entity, int statusId)
        {
            return true;
        }

        public bool Remove(CombatEntity entity, StatusEffect statusEffect)
        {
            return true;
        }

        public void RemoveAll(CombatEntity entity)
        {
            return;
        }

        public void RemoveAll(CombatEntity entity, Func<AppliedStatusEffect, bool> predicate)
        {
            return;
        }
    }
}
