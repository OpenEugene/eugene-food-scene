using System.Collections.Generic;
using EugeneFoodScene.Data;
using EugeneFoodScene.Services;
using HotChocolate;
using HotChocolate.Types.Relay;
using HotChocolate.Types;

namespace EugeneFoodScene.Server.Graph
{

    [ExtendObjectType(Name = "Query")]
    public class CuisineQuery
    {
        public string Hello() => "world";

        /// <summary>
        /// Gets all character.
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>The character.</returns>
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IEnumerable<Cuisine> GetCuisines(
            [Service] AirTableService repository) => 
            repository.GetCuisinesAsync().Result;
    }
}
