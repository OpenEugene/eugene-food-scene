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
    public class PlacesController : ControllerBase
    {
     
        private readonly ILogger<PlacesController> logger;
        private readonly AirTableService _airTableService;

        public PlacesController(ILogger<PlacesController> logger, AirTableService airTableService)
        {
            this.logger = logger;
            _airTableService = airTableService;
        }

        [HttpGet]
        public async Task<IEnumerable<Place>> Get()
        {

            var places = await _airTableService.GetPlacesPopulatedAsync();

            return places.ToArray();

        }

    }
}
