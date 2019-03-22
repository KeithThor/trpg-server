using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TRPGGame;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;
using TRPGGame.Repository;
using TRPGGame.Services;

namespace TRPGServer.Controllers
{
    [Route("api/[controller]")]
    public class CharacterController : Controller
    {
        private readonly IRepository<CharacterBase> _characterBaseRepo;
        private readonly IRepository<CharacterHair> _characterHairRepo;
        private readonly PlayerEntityFactory _playerEntityFactory;

        // Remove once db context is established
        [Obsolete]
        private readonly WorldEntityManager _worldEntityManager;

        public CharacterController(IRepository<CharacterBase> characterBaseRepo,
                                   IRepository<CharacterHair> characterHairRepo,
                                   PlayerEntityFactory playerEntityFactory,
                                   WorldEntityManager worldEntityManager)
        {
            _characterBaseRepo = characterBaseRepo;
            _characterHairRepo = characterHairRepo;
            _playerEntityFactory = playerEntityFactory;
            _worldEntityManager = worldEntityManager;
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

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Get all data parts
            var hair = await _characterHairRepo.GetDataAsync();
            var cBase = await _characterBaseRepo.GetDataAsync();

            return new JsonResult(new
            {
                Hairs = hair,
                Bases = cBase
            });
        }

        [Route("created")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCharacters()
        {
            var playerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var entities = _worldEntityManager.GetCombatEntities()
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
            var entity = await _playerEntityFactory.CreateAsync(template);

            if (entity != null) return new CreatedAtActionResult("create", "character", template, entity);
            else return new BadRequestResult();
        }

        [Route("")]
        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> Patch([FromBody]CharacterTemplate template)
        {
            template.OwnerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            template.OwnerName = User.FindFirst(ClaimTypes.Name).Value;

            var entity = await _playerEntityFactory.UpdateAsync(template);

            if (entity != null) return new StatusCodeResult(204);
            else return new BadRequestResult();
        }
    }
}
