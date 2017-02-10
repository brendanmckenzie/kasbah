using System.Collections.Generic;

namespace Kasbah.Analytics.Models
{
    public interface IBiasedContent
    {
        IDictionary<string, long> Bias { get; set; }
    }
}
