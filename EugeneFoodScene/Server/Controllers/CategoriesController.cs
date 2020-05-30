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
    public class CategoriesController : ControllerBase
    {
     
        private readonly ILogger<CategoriesController> logger;
        private readonly AirTableService _airTableService;

        public CategoriesController(ILogger<CategoriesController> logger, AirTableService airTableService)
        {
            this.logger = logger;
            _airTableService = airTableService;
        }

        [HttpGet]
        public async Task<IEnumerable<Category>> Get()
        {
            var list  = await _airTableService.GetCatagories();
            return list.ToArray();
        }
    }
}
