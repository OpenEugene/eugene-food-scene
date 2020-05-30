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
    public class CuisinesController : ControllerBase
    {
     
        private readonly ILogger<CuisinesController> logger;
        private readonly AirTableService _airTableService;

        public CuisinesController(ILogger<CuisinesController> logger, AirTableService airTableService)
        {
            this.logger = logger;
            _airTableService = airTableService;
        }

        [HttpGet]
        public async Task<IEnumerable<Cuisine>> Get()
        {
            var list  = await _airTableService.GetCuisines();
            return list.ToArray();
        }
    }
}
