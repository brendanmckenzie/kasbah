using System.Collections.Generic;

namespace Kasbah.Web.Models
{
    public class Site
    {
        public string Alias { get; set; }
        public IEnumerable<string> Domains { get; set; }
        public IEnumerable<string> ContentRoot { get; set; }
    }
}