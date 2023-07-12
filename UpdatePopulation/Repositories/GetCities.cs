using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace UpdatePopulation.Repositories
{
    public class GetCities
    {
        protected IOrganizationService _service;
        public GetCities(IOrganizationService service) 
        {
            _service = service;
        }  
        public EntityCollection GetCitiesCollection(EntityReference regionReference)
        {
            QueryExpression query = new QueryExpression("cds_city");
            query.ColumnSet = new ColumnSet("cds_population");
            query.Criteria.AddCondition("cds_region", ConditionOperator.Equal, regionReference.Id);
            EntityCollection cities = _service.RetrieveMultiple(query);

            if(cities != null)
            {
                return cities;
            }
            else
            {
                return new EntityCollection();
            }
        }
    }
}
