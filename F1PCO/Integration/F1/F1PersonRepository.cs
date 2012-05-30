using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace F1PCO.Integration.F1
{
    public class F1PersonRepository : IF1PersonRepository
    {
        private const string V2JsonAPIContentType = "application/vnd.fellowshiponeapi.com.people.people.v2+json";
        private readonly IF1ClientProvider _clientProvider;

        public F1PersonRepository(IF1ClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public async Task<IEnumerable<F1Person>> SearchByNameAsync(string searchTerm, int maxResults = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("searchTerm cannot be blank", "searchTerm");
            if (maxResults > 100)
                throw new ArgumentOutOfRangeException("maxResults", maxResults, "maxResults should be less than 100");

            var request = new RestRequest("v1/People/Search");
            request.AddHeader("content-type", V2JsonAPIContentType);
            request.AddParameter("searchFor", searchTerm);
            request.AddParameter("recordsperpage", maxResults.ToString());
            request.AddParameter("include", "attributes,addresses");

            var response = await _clientProvider.GetRestClient().ExecuteAsync(request);
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var people = GetF1PeopleFromJson(response.Content);
                    return people;
                }

                throw new Exception("An error occured: Status code: " + response.StatusCode, response.ErrorException);
            }
        }

        private static IEnumerable<F1Person> GetF1PeopleFromJson(string json)
        {
            dynamic people = JObject.Parse(json);
            if (people.results == null || people.results.person == null) yield break;

            foreach (var p in people.results.person)
            {
                yield return new F1Person
                {
                    F1ID = p["@id"],
                    FirstName = p.firstName,
                    LastName = p.lastName,
                    Gender = p.gender,
                    DateOfBirth = (DateTime?)p.dateOfBirth
                };
            }
        }
    }
}