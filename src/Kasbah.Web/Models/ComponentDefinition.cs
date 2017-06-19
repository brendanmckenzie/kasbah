using System;

namespace Kasbah.Web.Models
{
    public class ComponentDefinition
    {
        public string Alias { get; set; }
        public Type Control { get; set; }
        public Type Properties { get; set; }
    }
}
