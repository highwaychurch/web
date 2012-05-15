using System;
using System.Net;
using Hammock;
using Newtonsoft.Json.Linq;

namespace F1PCO.Web.Integration.F1
{
    public class F1PersonRepository : IF1PersonRepository
    {
        private readonly IF1ClientProvider _clientProvider;

        public F1PersonRepository(IF1ClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public dynamic GetPeople()
        {
            var request = new RestRequest
            {
                Path = "People/Search",
            };

            //request.AddParameter("lastUpdatedDate", lastDate);
            request.AddParameter("searchFor", "michael");
            request.AddParameter("recordsperpage", "1000");
            request.AddParameter("include", "attributes,addresses,communications");

            using (var response = _clientProvider.GetRestClient("application/vnd.fellowshiponeapi.com.people.people.v2+json").Request(request))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    dynamic people = JObject.Parse(response.Content);
                    return people;
                }

                throw new Exception("An error occured: Status code: " + response.StatusCode, response.InnerException);
            }
        }
    }
}