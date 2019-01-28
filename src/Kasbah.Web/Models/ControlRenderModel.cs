using System;
using System.Collections.Generic;
using System.Text;

namespace Kasbah.Web.Models
{
    public class ControlRenderModel
    {
        public string Component { get; set; }

        public object Model { get; set;  }

        public IDictionary<string, IEnumerable<object>> Controls { get; set; }
    }
}
