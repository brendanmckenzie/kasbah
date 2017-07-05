using System.Collections.Generic;

namespace Kasbah.Provider.Npgsql
{
    public class TranslateResponse
    {
        public string WhereClause { get; set; } = string.Empty;

        public string OrderByClause { get; set; } = string.Empty;

        public long? Skip { get; set; } = null;

        public long? Take { get; set; } = null;

        public IDictionary<string, object> Parameters { get; set; }
    }
}
