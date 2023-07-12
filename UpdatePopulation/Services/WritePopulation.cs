using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdatePopulation.Repositories;
using Microsoft.Identity.Client;
using System.Diagnostics;

namespace UpdatePopulation.Services
{
    public class WritePopulation
    {
        protected IPluginExecutionContext _context;
        protected IOrganizationService _service;
        public WritePopulation(IPluginExecutionContext context, IOrganizationService service)
        {
            _context = context;
            _service = service;
        }
        public void UpdatePopulation()
        {
            try
            {
                if (_context.MessageName == "Delete" || _context.MessageName == "Update")
                {
                    var preImage = _context.PreEntityImages["PreImage"] as Entity;
                    UpdateRegionPopulation(preImage);
                }
                else 
                {
                    var target = _context.InputParameters["Target"] as Entity;
                    UpdateRegionPopulation(target);
                }

                void UpdateRegionPopulation(Entity targetCity)
                {
                    if (targetCity.Contains("cds_population"))
                    {
                        EntityReference regionReference = targetCity.GetAttributeValue<EntityReference>("cds_region") != null ?
                            targetCity.GetAttributeValue<EntityReference>("cds_region")
                            : throw new Exception("Region is unavailable");// or return 


                        GetCities getCities = new GetCities(_service);
                        EntityCollection cityCollection = getCities.GetCitiesCollection(regionReference);

                        int totalPopulation = 0;
                        foreach (Entity city in cityCollection.Entities)
                        {
                            if (city.Contains("cds_population") && city["cds_population"] is int population)
                            {
                                totalPopulation += population;
                            }
                        }
                        var regionEntity = new Entity(regionReference.LogicalName,regionReference.Id);
                        regionEntity["cds_population"] = totalPopulation;
                        _service.Update(regionEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                var trace = new StackTrace(ex, true);
                var sb = new StringBuilder();
                foreach (var frame in trace.GetFrames())
                {


                    sb.AppendLine($"Файл: {frame.GetFileName()}");
                    sb.AppendLine($"Строка: {frame.GetFileLineNumber()}");
                    sb.AppendLine($"Столбец: {frame.GetFileColumnNumber()}");
                    sb.AppendLine($"Метод: {frame.GetMethod()}");

                }
                InvalidPluginExecutionException pluginException =
                    new InvalidPluginExecutionException($"Plugin error.{ex.Message} and {sb}", ex);
                throw pluginException;
            }

        }
    }
}
