using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using RestSharp;

namespace F1PCO.Integration.F1
{
    public class F1PersonRepository : IF1PersonRepository
    {
        private const string V2XmlAPIContentType = "application/vnd.fellowshiponeapi.com.people.people.v2+xml";
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
            request.AddHeader("content-type", V2XmlAPIContentType);
            request.AddParameter("searchFor", searchTerm);
            request.AddParameter("recordsperpage", maxResults.ToString());
            request.AddParameter("include", "attributes,addresses,communications");

            var response = await _clientProvider.GetRestClient().ExecuteAsync(request);
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var people = GetF1PeopleFromXml(response.Content);
                    return people;
                }

                throw new Exception("An error occured: Status code: " + response.StatusCode, response.ErrorException);
            }
        }

        private static IEnumerable<F1Person> GetF1PeopleFromXml(string xml)
        {
            var xPeople = XDocument.Parse(xml).Element("results").Elements("person");
            return xPeople.Select(GetF1PersonFromXml);
        }

        private static F1Person GetF1PersonFromXml(XElement xPerson)
        {
            var lastUpdatedAtUtc = DateTime.Parse((string)xPerson.Element("lastUpdatedDate")).ToUniversalTime();
            return new F1Person
            {
                F1ID = (string)xPerson.Attribute("id"),
                LastUpdatedAtUtc = lastUpdatedAtUtc,
                FirstName = (string)xPerson.Element("firstName"),
                LastName = (string)xPerson.Element("lastName"),
                Email = TryGetCommunication(xPerson, "Email"),
                MobilePhone = TryGetCommunication(xPerson, "Mobile"),
                HomePhone = TryGetCommunication(xPerson, "Home Phone")
            };
        }

        private static string TryGetCommunication(XElement xPerson, string location)
        {
            return
                xPerson.Element("communications").Elements("communication")
                    .Where(x => (string)x.Element("communicationType").Element("name") == location)
                    .Select(x => (string)x.Element("communicationValue"))
                    .FirstOrDefault();
        }

        //private static IEnumerable<F1Person> GetF1PeopleFromJson(string json)
        //{
        //    dynamic people = JObject.Parse(json);
        //    if (people.results == null || people.results.person == null) yield break;

        //    foreach (dynamic p in people.results.person)
        //    {
        //        yield return GetF1PersonFromJObject(p);
        //    }
        //}

        //private static F1Person GetF1PersonFromJObject(dynamic p)
        //{
        //    var lastUpdatedAtUtc = ((DateTime) (p.lastUpdatedDate ?? p.createdDate)).ToUniversalTime();
        //    return new F1Person
        //               {
        //                   F1ID = p["@id"],
        //                   LastUpdatedAtUtc = lastUpdatedAtUtc,
        //                   FirstName = p.firstName,
        //                   LastName = p.lastName,
        //                   Email = TryGetEmail(p),
        //                   MobilePhone = TryGetPhoneNumber(p, "Mobile"),
        //                   HomePhone = TryGetPhoneNumber(p, "Home Phone"),
        //               };
        //}

        //private static string TryGetPhoneNumber(dynamic p, string type)
        //{
        //    foreach (dynamic c in p.communications.communication)
        //    {
        //        if (c.communicationType.name == type)
        //        {
        //            return c.communicationValue;
        //        }
        //    }

        //    return null;
        //}

        //private static string TryGetEmail(dynamic p)
        //{
        //    foreach (dynamic c in p.communications.communication)
        //    {
        //        if (c.communicationType.name == "Email")
        //        {
        //            return c.communicationValue;
        //        }
        //    }

        //    return null;
        //}
    }
}