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
        private readonly IF1ClientProvider _clientProvider;

        public F1PersonRepository(IF1ClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public IEnumerable<F1Person> GetPeople()
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
                    var people = GetF1PeopleFromJson(response.Content);
                    return people;
                }

                throw new Exception("An error occured: Status code: " + response.StatusCode, response.InnerException);
            }
        }

        private static IEnumerable<F1Person> GetF1PeopleFromJson(string json)
        {
            dynamic people = JObject.Parse(json);
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