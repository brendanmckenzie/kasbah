using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Kasbah.Web
{
    public class ControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        readonly KasbahWebMode _mode;

        public ControllerFeatureProvider(KasbahWebMode mode)
        {
            _mode = mode;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            if (!_mode.HasFlag(KasbahWebMode.Delivery))
            {
                foreach (var controller in feature.Controllers.Where(ent => ent.Namespace.StartsWith("Kasbah.Web.Controllers.Delivery")).ToList())
                {
                    feature.Controllers.Remove(controller);
                }
            }

            if (!_mode.HasFlag(KasbahWebMode.Management))
            {
                foreach (var controller in feature.Controllers.Where(ent => ent.Namespace.StartsWith("Kasbah.Web.Controllers.Management")).ToList())
                {
                    feature.Controllers.Remove(controller);
                }
            }
        }
    }
}
