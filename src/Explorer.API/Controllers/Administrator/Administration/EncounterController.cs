using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos.Encounter;
using Explorer.Encounters.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Explorer.Encounters.API.Dtos.EncounterExecution;


namespace Explorer.API.Controllers.Administrator.Administration
{
    [Route("api/encounters")]
    [ApiController]
    public class EncounterController : ControllerBase
    {
        private readonly IEncounterService _encounterService;

        public EncounterController(IEncounterService encounterService)
        {
            _encounterService = encounterService;
        }

        [Authorize(Roles = "administrator, tourist")]
        [HttpGet("{id:long}")]
        public ActionResult<EncounterDto> GetById(long id)
        {
            try
            {
                return Ok(_encounterService.Get(id));
            }
            catch(NotFoundException nex)
            {
                return NotFound(nex.Message);
            }
        }

        [HttpGet("active")]
        public ActionResult<IEnumerable<EncounterDto>> GetActive()
        {
            return Ok(_encounterService.GetActive());
        }

        [Authorize(Policy = "administratorPolicy")]
        [HttpGet("all")]
        public ActionResult<PagedResult<EncounterDto>> GetPaged([FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_encounterService.GetPaged(page, pageSize));
        }

        [Authorize(Policy = "administratorPolicy")]
        [HttpPost]
        public ActionResult<EncounterDto> Create([FromBody] CreateEncounterDto createDto)
        {
            try
            {
                var created = _encounterService.Create(createDto);
                return Ok(created);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "administratorPolicy")]
        [HttpPut]
        public ActionResult<EncounterDto> Update(UpdateEncounterDto updateDto)
        {
            try
            {
                var updated = _encounterService.Update(updateDto);
                return Ok(updated);
            }
            catch(NotFoundException nex)
            {
                return NotFound(nex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "administratorPolicy")]
        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            try
            {
                _encounterService.Delete(id);
                return NoContent();
            }
            catch(NotFoundException nex)
            {
                return NotFound(nex.Message);
            }
        }

        [Authorize(Policy = "administratorPolicy")]
        [HttpGet("count")]
        public ActionResult<int> GetCount()
        {
            return Ok(_encounterService.GetCount());
        }

        [Authorize(Policy = "administratorPolicy")]
        [HttpPost("activate/{id:long}")]
        public IActionResult Activate(long id)
        {
            try
            {
                _encounterService.MakeActive(id);
                return Ok();
            }
            catch(NotFoundException nex)
            {
                return NotFound(nex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "administratorPolicy")]
        [HttpPost("archive/{id:long}")]
        public IActionResult Archive(long id)
        {
            try
            {
                _encounterService.Archive(id);
                return Ok();
            }
            catch(NotFoundException nex)
            {
                return NotFound(nex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
                // Tourist: Start/activate execution (for HiddenLocation and later Social)
        [Authorize(Policy = "touristPolicy")]
        [HttpPost("activate-execution/{id:long}")]
        public IActionResult ActivateExecution(long id)
        {
            try
            {
                long userId = long.Parse(HttpContext.User.Claims
                    .First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);

                _encounterService.ActivateEncounter(userId, id);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Tourist: Ping location every ~10s (HiddenLocation progress)
        [Authorize(Policy = "touristPolicy")]
        [HttpPost("location/{id:long}")]
        public ActionResult<EncounterExecutionStatusDto> PingLocation(long id, [FromBody] EncounterLocationPingDto dto)
        {
            try
            {
                long userId = long.Parse(HttpContext.User.Claims
                    .First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);

                var result = _encounterService.PingLocation(
                    userId,
                    id,
                    dto.Latitude,
                    dto.Longitude,
                    dto.DeltaSeconds
                );

                return Ok(new EncounterExecutionStatusDto
                {
                    IsCompleted = result.IsCompleted,
                    SecondsInsideZone = result.SecondsInsideZone,
                    RequiredSeconds = result.RequiredSeconds,
                    CompletionTime = result.CompletionTime?.ToString("O")
                });
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [Authorize(Policy = "touristPolicy")]
        [HttpPost("complete/{id:long}")]
        public ActionResult Complete(long id)
        {
            try
            {
                long userId = long.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);

                _encounterService.CompleteEncounter(userId, id);

                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
