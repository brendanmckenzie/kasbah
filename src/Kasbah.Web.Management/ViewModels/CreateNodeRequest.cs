using System;

namespace Kasbah.Web.Management.ViewModels
{
    public class CreateNodeRequest
    {
        public Guid? Parent { get; set; }

        public string Alias { get; set; }

        public string DisplayName { get; set; }

        public string Type { get; set; }
    }
}
