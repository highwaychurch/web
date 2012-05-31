using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using RestSharp;

namespace F1PCO.Integration.PCO
{
    public class PCOPersonRepository : IPCOPersonRepository
    {
        private readonly IPCOClientProvider _clientProvider;

        public PCOPersonRepository(IPCOClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public async Task<IEnumerable<PCOPerson>> SearchByNameAsync(string searchTerm)
        {
            var request = new RestRequest("people.xml");
            request.AddParameter("name", searchTerm);
            var response = await _clientProvider.GetRestClient().ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var people = GetPCOPeopleFromXml(response.Content);
                return people;
            }

            throw new Exception("An error occured: Status code: " + response.StatusCode, response.ErrorException);
        }

        private static IEnumerable<PCOPerson> GetPCOPeopleFromXml(string xml)
        {
            var xPeople = XDocument.Parse(xml).Element("people");
            return xPeople.Elements().Select(GetPCOPersonFromXml);
        }

        private static PCOPerson GetPCOPersonFromXml(XElement xPerson)
        {
            var lastUpdatedAtUtc = DateTime.Parse((string) xPerson.Element("updated-at")).ToUniversalTime();
            return new PCOPerson
                       {
                           PCOID = (string) xPerson.Element("id"),
                           LastUpdatedAtUtc = lastUpdatedAtUtc,
                           FirstName = (string) xPerson.Element("first-name"),
                           LastName = (string) xPerson.Element("last-name"),
                           Email = TryGetEmailAddress(xPerson, "Home"),
                           MobilePhone = TryGetPhoneNumber(xPerson, "Mobile"),
                           HomePhone = TryGetPhoneNumber(xPerson, "Home")
                       };
        }

        private static string TryGetPhoneNumber(XElement xPerson, string location)
        {
            return
                xPerson.Element("contact-data")
                    .Element("phone-numbers").Elements("phone-number")
                    .Where(x => (string)x.Element("location") == location)
                    .Select(x => (string)x.Element("number"))
                    .FirstOrDefault();
        }

        private static string TryGetEmailAddress(XElement xPerson, string location)
        {
            return
                xPerson.Element("contact-data")
                    .Element("email-addresses").Elements("email-address")
                    .Where(x => (string) x.Element("location") == location)
                    .Select(x => (string) x.Element("address"))
                    .FirstOrDefault();
        }
    }
}