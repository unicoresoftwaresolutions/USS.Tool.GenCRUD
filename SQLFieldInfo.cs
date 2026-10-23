using System.Text.RegularExpressions;

namespace USS.Tool.GenCRUD
{
    public class SQLFieldInfo
    {
        public String Name { get; set; }
        public String DataType { get; set; }
        public int DataSize { get; set; }
        public SQLReferenceInfo Reference { get; set; }

        public bool HasForeignKey => Reference?.IsForeignKey == true;

        public static SQLFieldInfo Parse(String Detail)
        {
            var tokens = Regex.Split(Detail, @"\s+");
            if (tokens.Length == 0) return null;
            var colName = tokens[0].Trim().Trim('"', '[', ']', '`');
            if (string.IsNullOrEmpty(colName)) return null;

            var rest = Detail.Substring(Detail.IndexOf(tokens[0]) + tokens[0].Length).Trim();
            if (string.IsNullOrEmpty(rest))
            {
                return new SQLFieldInfo { Name = colName };
            }

            var dtypeMatch = Regex.Match(rest, @"^([A-Za-z0-9_]+)\s*(\(([^\)]+)\))?", RegexOptions.IgnoreCase);
            string dtype = null; int dsize = 0;
            if (dtypeMatch.Success)
            {
                dtype = dtypeMatch.Groups[1].Value;
                var sizeGrp = dtypeMatch.Groups[3].Value;
                if (!string.IsNullOrEmpty(sizeGrp))
                {
                    var numMatch = Regex.Match(sizeGrp, @"(\d+)");
                    if (numMatch.Success && int.TryParse(numMatch.Groups[1].Value, out var n))
                        dsize = n;
                }
            }

            var fieldInfo = new SQLFieldInfo
            {
                Name = colName,
                DataType = dtype,
                DataSize = dsize
            };

            var inlineRef = Regex.Match(rest, @"REFERENCES\s+([\[\]`""\w\.]+)\s*\(([^\)]+)\)", RegexOptions.IgnoreCase);
            if (inlineRef.Success)
            {
                var refRaw = inlineRef.Groups[1].Value.Trim();
                var refFld = inlineRef.Groups[2].Value.Trim().Trim('"', '[', ']', '`');
                string refSchema = null;
                var refTbl = refRaw;
                if (refRaw.Contains('.'))
                {
                    var p = refRaw.Split('.');
                    refSchema = p.Length > 1 ? p[p.Length - 2].Trim('"', '[', ']', '`') : null;
                    refTbl = p[p.Length - 1].Trim('"', '[', ']', '`');
                }
                var rinfo = new SQLReferenceInfo
                {
                    ReferenceSchema = refSchema,
                    ReferenceTableName = refTbl.Trim('"', '[', ']', '`'),
                    ReferenceFieldName = refFld,
                    IsForeignKey = true
                };
                fieldInfo.Reference = rinfo;
            }
            return fieldInfo;
        }

        public static void SetReference(String Detail, List<SQLFieldInfo> allFields)
        {

            var mref = Regex.Match(Detail, @"REFERENCES\s+([\[\]`""\w\.]+)\s*\(([^\)]+)\)", RegexOptions.IgnoreCase);
            if (mref.Success)
            {
                var refTableRaw = mref.Groups[1].Value.Trim();
                var refFieldList = mref.Groups[2].Value.Trim();

                string refSchema = null;
                var refTable = refTableRaw;
                if (refTableRaw.Contains('.'))
                {
                    var p = refTableRaw.Split('.');
                    refSchema = p.Length > 1 ? p[p.Length - 2].Trim('"', '[', ']', '`') : null;
                    refTable = p[p.Length - 1].Trim('"', '[', ']', '`');
                }

                var localMatch = Regex.Match(Detail, @"FOREIGN\s+KEY\s*\(([^\)]+)\)", RegexOptions.IgnoreCase);
                var localCols = new List<string>();
                if (localMatch.Success)
                {
                    var rawLocal = localMatch.Groups[1].Value;
                    var pieces = rawLocal.Split(',');
                    foreach (var p in pieces)
                    {
                        var n = p.Trim().Trim('"', '[', ']', '`');
                        if (!string.IsNullOrEmpty(n)) localCols.Add(n);
                    }
                }

                string constraintName = null;
                var cname = Regex.Match(Detail, @"CONSTRAINT\s+([^\s(]+)", RegexOptions.IgnoreCase);
                if (cname.Success)
                    constraintName = cname.Groups[1].Value.Trim().Trim('"', '[', ']', '`');

                var refFields = new List<string>();
                if (!string.IsNullOrEmpty(refFieldList))
                {
                    var rfParts = refFieldList.Split(',');
                    foreach (var rp in rfParts)
                    {
                        var rn = rp.Trim().Trim('"', '[', ']', '`');
                        if (!string.IsNullOrEmpty(rn)) refFields.Add(rn);
                    }
                }

                foreach (var localCol in localCols)
                {
                    if (string.IsNullOrEmpty(localCol)) continue;
                    var fkField = allFields.Find(f => string.Equals(f.Name, localCol, StringComparison.OrdinalIgnoreCase));
                    if (fkField != null)
                    {
                        var refInfo = new SQLReferenceInfo
                        {
                            ReferenceSchema = refSchema,
                            ReferenceTableName = refTable,
                            ReferenceFieldName = refFields.Count > 0 ? refFields[0] : null,
                            ReferenceConstraintName = constraintName,
                            IsForeignKey = true
                        };
                        fkField.Reference = refInfo;
                    }
                }
            }
        }



        public static List<SQLFieldType> Types
        {
            get
            {

                if (field == null)
                {
                    field = new List<SQLFieldType>();

                    field.Add(new SQLFieldType() { DataType = "UNIQUEIDENTIFIER", FieldType = "Guid" });

                    field.Add(new SQLFieldType() { DataType = "NVARCHAR", FieldType = "string" });
                    field.Add(new SQLFieldType() { DataType = "VARCHAR", FieldType = "string" });
                    field.Add(new SQLFieldType() { DataType = "CHAR", FieldType = "string" });
                    field.Add(new SQLFieldType() { DataType = "TEXT", FieldType = "string" });

                    field.Add(new SQLFieldType() { DataType = "DATETIME", FieldType = "DateTime" });
                    field.Add(new SQLFieldType() { DataType = "DATE", FieldType = "DateOnly" });

                    field.Add(new SQLFieldType() { DataType = "BIT", FieldType = "bool" });

                    field.Add(new SQLFieldType() { DataType = "TINYINT", FieldType = "byte" });
                    field.Add(new SQLFieldType() { DataType = "SMALLINT", FieldType = "short" });
                    field.Add(new SQLFieldType() { DataType = "INT", FieldType = "int" });
                    field.Add(new SQLFieldType() { DataType = "BIGINT", FieldType = "long" });

                    field.Add(new SQLFieldType() { DataType = "FLOAT", FieldType = "float" });
                    field.Add(new SQLFieldType() { DataType = "REAL", FieldType = "float" });
                    field.Add(new SQLFieldType() { DataType = "DECIMAL", FieldType = "decimal" });
                    field.Add(new SQLFieldType() { DataType = "SMALLMONEY", FieldType = "decimal" });
                    field.Add(new SQLFieldType() { DataType = "MONEY", FieldType = "decimal" });

                    field.Add(new SQLFieldType() { DataType = "VARBINARY", FieldType = "byte[]" });
                    field.Add(new SQLFieldType() { DataType = "BINARY", FieldType = "byte[]" });
                }
                return field;
            }
        }
    }
}
