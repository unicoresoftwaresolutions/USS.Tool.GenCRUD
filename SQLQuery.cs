using System;
using System.Collections.Generic;
using System.Text;

namespace USS.Tool.GenCRUD
{
    public class SQLQuery
    {
        public readonly string query;
        public readonly string compactQuery;        
        public SQLQuery(string query)
        {
            this.query = query;

            if (string.IsNullOrWhiteSpace(query)) compactQuery = string.Empty;
            

                // Normalize whitespace and remove newlines for easier parsing
                var compact = System.Text.RegularExpressions.Regex.Replace(query, @"\r|\n|\t", " ");
            compact = System.Text.RegularExpressions.Regex.Replace(compact, @" +", " ");

            var cmp = compact.Trim();
            compactQuery = query.Trim();
        }
    }
}
