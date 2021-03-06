﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TRPGGame;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;
using TRPGGame.Managers;
using TRPGGame.Repository;
using TRPGGame.Services;
using TRPGServer.Filters;

namespace TRPGServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly IRepository<CharacterBase> _characterBaseRepo;
        private readonly IRepository<CharacterHair> _characterHairRepo;
        private readonly IRepository<IReadOnlyClassTemplate> _classTemplateRepo;
        private readonly ICombatEntityManager _combatEntityManager;
        private readonly IStateManager _stateManager;

        public CharacterController(IRepository<CharacterBase> characterBaseRepo,
                                   IRepository<CharacterHair> characterHairRepo,
                                   IRepository<IReadOnlyClassTemplate> classTemplateRepo,
                                   ICombatEntityManager combatEntityManager,
                                   IStateManager stateManager)
        {
            _characterBaseRepo = characterBaseRepo;
            _characterHairRepo = characterHairRepo;
            _classTemplateRepo = classTemplateRepo;
            _combatEntityManager = combatEntityManager;
            _stateManager = stateManager;
        }

        [Route("base")]
        [HttpGet]
        public async Task<IActionResult> GetBase(int? baseId)
        {
            var cBaseData = await _characterBaseRepo.GetDataAsync();

            if (baseId == null)
            {
                return new JsonResult(cBaseData);
            }
            else
            {
                var result = cBaseData.FirstOrDefault(cBase => cBase.Id == baseId.Value);

                if (result == null) return new NotFoundResult();
                else return new JsonResult(result);
            }
        }

        [Route("hair")]
        [HttpGet]
        public async Task<IActionResult> GetHair(int? hairId)
        {
            var cHairData = await _characterHairRepo.GetDataAsync();

            if (hairId == null)
            {
                return new JsonResult(cHairData);
            }
            else
            {
                var result = cHairData.FirstOrDefault(cHair => cHair.Id == hairId.Value);

                if (result == null) return new NotFoundResult();
                else return new JsonResult(result);
            }
        }

        [Route("classtemplates")]
        [HttpGet]
        public async Task<IActionResult> GetClassTemplates(int? templateId)
        {
            var templates = await _classTemplateRepo.GetDataAsync();

            if (templateId == null)
            {
                return new JsonResult(templates);
            }
            else
            {
                var result = templates.FirstOrDefault(cHair => cHair.Id == templateId.Value);

                if (result == null) return new NotFoundResult();
                else return new JsonResult(result);
            }
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Get all data parts
            var hair = await _characterHairRepo.GetDataAsync();
            var cBase = await _characterBaseRepo.GetDataAsync();
            var templates = await _classTemplateRepo.GetDataAsync();

            return new JsonResult(new
            {
                Hairs = hair,
                Bases = cBase,
                ClassTemplates = templates
            });
        }

        [Route("created")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCharacters()
        {
            var playerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var entities = _combatEntityManager.GetEntities()
                                               .Where(entity => entity.OwnerId == playerId);

            return new JsonResult(entities);
        }

        [Route("")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody]CharacterTemplate template)
        {
            template.OwnerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            template.OwnerName = User.FindFirst(ClaimTypes.Name).Value;
            var entity = await _combatEntityManager.CreateAsync(template);

            if (entity != null)
            {
                if (_stateManager.GetPlayerState(template.OwnerId) == PlayerStateConstants.MakeCharacter)
                    _stateManager.SetPlayerMakeFormation(template.OwnerId);
                return new CreatedAtActionResult("create", "character", template, entity);
            }
            else return new BadRequestResult();
        }

        [Route("")]
        [HttpPatch]
        [Authorize]
        [GameState(PlayerStateConstants.Free, PlayerStateConstants.MakeFormation, PlayerStateConstants.MakeCharacter)]
        public async Task<IActionResult> Patch([FromBody]CharacterTemplate template)
        {
            template.OwnerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            template.OwnerName = User.FindFirst(ClaimTypes.Name).Value;

            var entity = await _combatEntityManager.UpdateAsync(template);

            if (entity != null) return new StatusCodeResult(StatusCodes.Status204NoContent);
            else return new BadRequestResult();
        }

        [Route("")]
        [HttpDelete]
        [Authorize]
        [GameState(PlayerStateConstants.Free, PlayerStateConstants.MakeFormation, PlayerStateConstants.MakeCharacter)]
        public IActionResult Delete([FromBody]int entityId)
        {
            var ownerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (_combatEntityManager.Delete(entityId, ownerId)) return new NoContentResult();
            else return new BadRequestResult();
        }
    }
}
