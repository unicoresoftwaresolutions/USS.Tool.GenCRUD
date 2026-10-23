namespace USS.Tool.GenCRUD
{
    public class SQLTable
    {
        public SQLTable(SQLCreateQuery qry)
        {
            TableName = qry.TableName;
            Fields = qry.Fields;
        }

        public string TableName { get; set; }
        public List<SQLFieldInfo> Fields { get; set; }

    }
}
