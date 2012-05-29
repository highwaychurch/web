using F1PCO.OAuth;

namespace F1PCO
{
    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AccessToken F1AccessToken { get; set; }
        public AccessToken PCOAccessToken { get; set; }
    }
}