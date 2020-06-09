using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AirtableApiClient;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using EugeneFoodScene.Data;


namespace EugeneFoodScene.Services
{

  
    /// <summary>
    /// a bruit force airtable data caching service
    /// </summary>
    public abstract class AirTableBase
    {
        protected readonly string BaseId;
        protected readonly string AppKey;

        public AirTableBase(IConfiguration configuration)
        {
            BaseId = configuration["AirTable:BaseId"];
            AppKey = configuration["AirTable:AppKey"];
        }

        private void PopulateIds<T>(IEnumerable<AirtableRecord<T>> responseRecords) where T : IAirtable
        {
            foreach (var item in responseRecords)
            {
                item.Fields.Id = item.Id;
            }
        }

        protected async Task<IEnumerable<T>> GetTableAsync<T>(string tableName) where T : IAirtable
        {
            var table = new List<AirtableRecord<T>>();
            string offset = null;

            using (AirtableBase airtableBase = new AirtableBase(AppKey, BaseId))
            {
                do
                {

                    Task<AirtableListRecordsResponse<T>> task =
                        airtableBase.ListRecords<T>(tableName: tableName, offset: offset);

                    var response = await task;

                    if (response.Success)
                    {
                        PopulateIds(response.Records);
                        table.AddRange(response.Records);
                        offset = response.Offset;
                    }
                    // look for timeouts and add retry logic. 
                    else if (response.AirtableApiError is AirtableApiException)
                    {
                        throw new Exception(response.AirtableApiError.ErrorMessage);
                    }
                    else
                    {
                        throw new Exception("Unknown error");
                    }

                } while (offset != null);

            }

            List<T> list = new List<T>();
            try
            {
                list = (from c in table select c.Fields).ToList();
            }
            catch (Exception e)
            {
                var err = e.InnerException;
            }

            return list.AsEnumerable();
          
        }

    }
}
