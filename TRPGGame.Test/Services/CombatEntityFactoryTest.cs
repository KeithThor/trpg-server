﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;
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
                                                           new AbilityRepoStub());
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
                        Name = "TemplateA"
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
                        Name = "TemplateB"
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
                        Name = "TemplateC"
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
                        Name = "TemplateD"
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
                        Name = "ExceedsMaxStatButRemainsWithinTotalStatRange"
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
                        Name = "ExceedsMultipleMaxStats"
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
                        Name = "ExceedsMaxStatWithPositiveBonuses"
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
                        Name = "ExceedsMaxStatWithNegativeBonuses"
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
                        Name = "UnderStatMaxButOverStatLimit"
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
                        Name = "UnderStatMaxButOverStatLimit"
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
                        Name = "UnderStatMaxButOverStatLimitWithPositiveBonuses"
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
                        Name = "UnderStatMaxButOverStatLimitWithNegativeBonuses"
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
                        Name = "UnderStatMinButSameStatTotal"
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
                        Name = "WayUnderMinStat"
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
                        Name = "BelowMinStatWithPositiveBonus"
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
                        Name = "BelowMinStatWithNegativeBonus"
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
                        Name = "BaseDoesNotExistForThisId"
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
                        Name = "HairDoesNotExistForThisId"
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
                        Name = "StatsAreNullForThisObject"
                    }
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

    public class AbilityRepoStub : IRepository<Ability>
    {
        public Task<IEnumerable<Ability>> GetDataAsync()
        {
            return Task.Run(() =>
            {
                IEnumerable<Ability> abilities = new List<Ability>();

                return abilities;
            });
        }
    }
}