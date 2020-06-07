using EugeneFoodScene.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using AirtableApiClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EugeneFoodScene.Data;
using Microsoft.Extensions.Configuration;
using EugeneFoodScene.Services;

namespace EugeneFoodScene.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TagsController : ControllerBase
    {
     
        private readonly ILogger<TagsController> logger;
        private readonly AirTableService _airTableService;

        public TagsController(ILogger<TagsController> logger, AirTableService airTableService)
        {
            this.logger = logger;
            _airTableService = airTableService;
        }

        [HttpGet]
        public async Task<IEnumerable<Tag>> Get()
        {
            var list  = await _airTableService.GetTagsAsync();
            return list;
        }
    }
}
