using System;
using System.Collections.Generic;
using System.Net;
using F1PCO.Web.Models;
using Hammock;
using Newtonsoft.Json.Linq;

namespace F1PCO.Web.Integration.F1
{
    public class F1PersonRepository : IF1PersonRepository
    {
        private const string V2JsonAPIContentType = "application/vnd.fellowshiponeapi.com.people.people.v2+json";
        private readonly IF1ClientProvider _clientProvider;

        public F1PersonRepository(IF1ClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public IEnumerable<F1Person> SearchByName(string searchTerm, int maxResults = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("searchTerm cannot be blank", "searchTerm");
            if (maxResults > 100)
                throw new ArgumentOutOfRangeException("maxResults", maxResults, "maxResults should be less than 100");

            var request = new RestRequest
            {
                Path = "People/Search",
            };

            request.AddParameter("searchFor", searchTerm);
            request.AddParameter("recordsperpage", maxResults.ToString());
            request.AddParameter("include", "attributes,addresses");

            using (var response = _clientProvider.GetRestClient(V2JsonAPIContentType).Request(request))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var people = GetF1PeopleFromJson(response.Content);
                    return people;
                }

                throw new Exception("An error occured: Status code: " + response.StatusCode, response.InnerException);
            }
        }

        private static IEnumerable<F1Person> GetF1PeopleFromJson(string json)
        {
            dynamic people = JObject.Parse(json);
            if (people.results == null || people.results.person == null) yield break;

            foreach (var p in people.results.person)
            {
                var person = new F1Person
                {
                    F1ID = p["@id"],
                    FirstName = p.firstName,
                    LastName = p.lastName,
                    Gender = p.gender,
                    DateOfBirth = (DateTime?)p.dateOfBirth
                };
                yield return person;
            }
        }
    }
}