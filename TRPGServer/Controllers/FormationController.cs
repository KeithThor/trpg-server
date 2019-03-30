using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TRPGGame.Entities;
using TRPGGame.Managers;

namespace TRPGServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FormationController : ControllerBase
    {
        private readonly IFormationManager _formationManager;

        public FormationController(IFormationManager formationManager)
        {
            _formationManager = formationManager;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Get()
        {
            var ownerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var formations = _formationManager.GetFormations(ownerId);

            return new JsonResult(formations);
        }

        [HttpPost]
        [Route("")]
        public IActionResult Post(FormationTemplate template)
        {
            template.OwnerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var formation = _formationManager.CreateFormation(template);

            if (formation == null) return new BadRequestResult();
            else return new CreatedAtActionResult("post", "formation", template, formation);
        }

        [HttpPatch]
        [Route("")]
        public IActionResult Patch(FormationTemplate template)
        {
            template.OwnerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var formation = _formationManager.UpdateFormation(template);

            if (formation == null) return new BadRequestResult();
            else return new StatusCodeResult(StatusCodes.Status204NoContent);
        }

        public IActionResult Delete(int id)
        {
            var ownerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            bool success = _formationManager.DeleteFormation(id, ownerId);

            if (success) return new StatusCodeResult(StatusCodes.Status200OK);
            else return new BadRequestResult();
        }
    }
}
