using System;
using Kasbah.Content.Models;

namespace Kasbah.Web.Models
{
    public class ComponentDefinition
    {
        public string Alias { get; set; }
        public Type Control { get; set; }
        public TypeDefinition Properties { get; set; }
    }
}
