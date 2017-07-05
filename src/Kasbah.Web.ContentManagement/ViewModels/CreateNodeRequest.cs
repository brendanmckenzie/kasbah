using System;

namespace Kasbah.Web.ContentManagement.ViewModels
{
    public class CreateNodeRequest
    {
        public Guid? Parent { get; set; }

        public string Alias { get; set; }

        public string DisplayName { get; set; }

        public string Type { get; set; }
    }
}
