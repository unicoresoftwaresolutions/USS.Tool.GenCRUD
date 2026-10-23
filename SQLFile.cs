using System.Text.RegularExpressions;

namespace USS.Tool.GenCRUD
{
    public class SQLFile
    {
        public readonly string Query;        
        public SQLFile(string FileName)
        {
            var qry = LoadQuery(FileName);

            if (string.IsNullOrWhiteSpace(qry)) Query = string.Empty;
            else
            {
                var compact = Regex.Replace(qry, @"\r|\n|\t", " ");
                compact = Regex.Replace(compact, @" +", " ");

                var cmp = compact.Trim();
                Query = qry.Trim();
            }
        }

        public string LoadQuery(string sqlfile)
        {            
            return File.ReadAllText(sqlfile);
        }
    }
}
