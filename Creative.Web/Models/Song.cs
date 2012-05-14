using System.ComponentModel.DataAnnotations;

namespace Creative.Web.Models
{
    public class Song
    {
        public string Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public bool IsActive { get; set; }
    }
}