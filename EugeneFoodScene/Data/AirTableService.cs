using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AirtableApiClient;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;


namespace EugeneFoodScene.Data
{
    public class AirTableService
    {
        private readonly string baseId;
        private readonly string appKey;

        private List<AirtableRecord<Place>> _places;

        public AirTableService(IConfiguration configuration)
        {
            baseId = configuration["AirTable:BaseId"];
            appKey = configuration["AirTable:AppKey"];
        }

        public void ResetPlaces()
        {
            _places = null;
        }

        public async Task<List<AirtableRecord<Place>>> GetPlacesAsync()
        {
            // check the cache
            if (_places != null) return _places;

            _places = new List<AirtableRecord<Place>>();
            string offset = null;

            using (AirtableBase airtableBase = new AirtableBase(appKey, baseId))
            {
                do
                {

                    Task<AirtableListRecordsResponse<Place>> task =
                        airtableBase.ListRecords<Place>(tableName: "Places", offset: offset);

                    var response = await task;

                    if (response.Success)
                    {
                        _places.AddRange(response.Records);
                        offset = response.Offset;
                    }
                    else if (response.AirtableApiError is AirtableApiException)
                    {
                        throw new Exception(response.AirtableApiError.ErrorMessage);
                        break;
                    }
                    else
                    {
                        throw new Exception("Unknown error");
                        break;
                    }

                } while (offset != null);

                   
            }
            return _places;
        }
    }
}
