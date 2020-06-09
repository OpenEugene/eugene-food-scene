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
    public class ConfigController : ControllerBase
    {
     
        private readonly ILogger<ConfigController> logger;
        private readonly AirTableService _airTableService;

        public ConfigController(ILogger<ConfigController> logger, AirTableService airTableService)
        {
            this.logger = logger;
            _airTableService = airTableService;
        }

        [HttpPost("Reset")]
        public async Task<ActionResult<string>> PostReset(string table)
        {
            if (table == null || table == "all")
            {
                _airTableService.ResetAll();
            }
            // add support for specific tables
          
            return new ActionResult<string>("Reset Complete!  See you in Cyberspace!");
        }
    }
}
