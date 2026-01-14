using System;
using System.Collections.Generic;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/bundles")]
    [ApiController]
    public class TourBundlesController : ControllerBase
    {
        private readonly IBundleService _bundleService;

        public TourBundlesController(IBundleService bundleService)
        {
            _bundleService = bundleService;
        }

        // GET: api/tourist/bundles
        [HttpGet]
        public ActionResult<List<BundleDto>> GetPublishedBundles()
        {
            try
            {
                var bundles = _bundleService.GetPublished();
                return Ok(bundles);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/tourist/bundles/{id}
        [HttpGet("{id:long}")]
        public ActionResult<BundleDto> GetPublishedById(long id)
        {
            try
            {
                var bundle = _bundleService.GetPublishedById(id);
                return Ok(bundle);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
