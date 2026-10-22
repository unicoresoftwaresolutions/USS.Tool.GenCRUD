namespace USS.Tool.GenCRUD
{
    public class SQLTable
    {
        private readonly string sqlfile;
        private string _createQuery;
        private readonly SQLQuery _sqlQuery;

        public SQLTable(string sqlfile)
        {
            this.sqlfile = sqlfile;
            _sqlQuery = new SQLQuery(LoadQuery(sqlfile));
        }

        public string CreateQuery
        {
            get
            {
                return _createQuery ??= _sqlQuery.compactQuery;
            }
        }

        public string LoadQuery(string sqlfile) 
        {
            return  File.ReadAllText(sqlfile);
        }

        public string TableName
        {
            get
            {
                if (field == null)
                {
                    var idx = CreateQuery.IndexOf("CREATE TABLE", StringComparison.OrdinalIgnoreCase);
                    if (idx >= 0)
                    {
                        idx += "CREATE TABLE".Length;

                        var after = CreateQuery.Substring(idx).TrimStart();
                        if (after.StartsWith("IF", StringComparison.OrdinalIgnoreCase))
                        {
                            var ine = System.Text.RegularExpressions.Regex.Match(after, @"^IF\s+NOT\s+EXISTS\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            if (ine.Success)
                            {
                                after = after.Substring(ine.Length).TrimStart();
                            }
                        }

                        var paren = after.IndexOf('(');
                        var token = paren >= 0 ? after.Substring(0, paren) : after.Split(' ')[0];
                        token = token.Trim();

                        token = token.Trim(',');
                        token = token.Trim();
                        if ((token.StartsWith("[") && token.EndsWith("]")) || (token.StartsWith("\"") && token.EndsWith("\"")) || (token.StartsWith("`") && token.EndsWith("`")))
                        {
                            token = token.Substring(1, token.Length - 2);
                        }

                        if (token.Contains('.'))
                        {
                            var parts = token.Split('.');
                            token = parts[parts.Length - 1].Trim();
                            token = token.Trim('[', ']', '"', '`');
                        }

                        field = string.IsNullOrWhiteSpace(token) ? null : token;
                    }
                }
                return field;
            }
        }
        public String FieldDetail
        {
            get
            {
                if (field == null)
                {
                    var query = CreateQuery;
                    if (!string.IsNullOrWhiteSpace(query))
                    {
                        var createIdx = query.IndexOf("CREATE TABLE", StringComparison.OrdinalIgnoreCase);
                        var parenStart = query.IndexOf('(', createIdx);

                        if (createIdx >= 0 && parenStart >= 0)
                        {
                            int depth = 0; int i = parenStart; int parenEnd = -1;
                            for (; i < query.Length; i++)
                            {
                                if (query[i] == '(') depth++;
                                else if (query[i] == ')')
                                {
                                    depth--;
                                    if (depth == 0)
                                    {
                                        parenEnd = i;
                                        break;
                                    }
                                }
                            }

                            if (parenStart<parenEnd)
                            field = query.Substring(parenStart + 1, parenEnd - parenStart - 1).Trim();
                        }
                    }
                }
                return field;
            }
        }
        public List<String> FieldDetails
        {
            get
            {
                if (field == null && !string.IsNullOrWhiteSpace(FieldDetail))
                {
                    field = new List<String>();
                    
                    var sb = new System.Text.StringBuilder();
                    var depth = 0;
                    for (int j = 0; j < FieldDetail.Length; j++)
                    {
                        var ch = FieldDetail[j];
                        if (ch == '(') { depth++; sb.Append(ch); }
                        else if (ch == ')') { depth--; sb.Append(ch); }
                        else if (ch == ',' && depth == 0)
                        {
                            var part = sb.ToString().Trim();
                            if (!string.IsNullOrEmpty(part)) field.Add(part);
                            sb.Clear();
                        }
                        else sb.Append(ch);
                    }
                    var last = sb.ToString().Trim();
                    if (!string.IsNullOrEmpty(last)) field.Add(last);
                }
                return field;
            }
        }
        public List<SQLFieldInfo> Fields 
        {
            get 
            {
                if (field == null && !string.IsNullOrWhiteSpace(FieldDetail))
                {
                    field = new List<SQLFieldInfo>();
                    
                    foreach (var partRaw in FieldDetails)
                    {
                        var lower = partRaw.TrimStart().ToUpperInvariant();
                        if (lower.StartsWith("CONSTRAINT") || lower.StartsWith("PRIMARY KEY") || lower.StartsWith("UNIQUE") || lower.StartsWith("FOREIGN KEY") || lower.StartsWith("CHECK"))
                        {
                            // Attempt to extract foreign key references from constraint
                            var mref = System.Text.RegularExpressions.Regex.Match(partRaw, @"REFERENCES\s+([\[\]`""\w\.]+)\s*\(([^\)]+)\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            if (mref.Success)
                            {
                                var refTableRaw = mref.Groups[1].Value.Trim();
                                var refFieldList = mref.Groups[2].Value.Trim();

                                // split schema.table if present
                                string refSchema = null;
                                var refTable = refTableRaw;
                                if (refTableRaw.Contains('.'))
                                {
                                    var p = refTableRaw.Split('.');
                                    refSchema = p.Length > 1 ? p[p.Length - 2].Trim('"', '[', ']', '`') : null;
                                    refTable = p[p.Length - 1].Trim('"', '[', ']', '`');
                                }

                                // try to find local columns in the constraint: FOREIGN KEY (col1, col2)
                                var localMatch = System.Text.RegularExpressions.Regex.Match(partRaw, @"FOREIGN\s+KEY\s*\(([^\)]+)\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
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

                                // Try to get constraint name if provided: CONSTRAINT <name>
                                string constraintName = null;
                                var cname = System.Text.RegularExpressions.Regex.Match(partRaw, @"CONSTRAINT\s+([^\s(]+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                                if (cname.Success)
                                    constraintName = cname.Groups[1].Value.Trim().Trim('"', '[', ']', '`');

                                // referenced columns (may be multiple) — pick first for ReferenceFieldName if needed
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

                                // Attach reference info to each local column found
                                foreach (var localCol in localCols)
                                {
                                    if (string.IsNullOrEmpty(localCol)) continue;
                                    var fkField = field.Find(f => string.Equals(f.Name, localCol, StringComparison.OrdinalIgnoreCase));
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
                            continue;
                        }

                        // Column definition: name [type] [constraints...]
                        // First token is column name
                        var tokens = System.Text.RegularExpressions.Regex.Split(partRaw, @"\s+");
                        if (tokens.Length == 0) continue;
                        var colName = tokens[0].Trim().Trim('"', '[', ']', '`');
                        if (string.IsNullOrEmpty(colName)) continue;

                        // The rest of the part after the column name is the type and constraints
                        var rest = partRaw.Substring(partRaw.IndexOf(tokens[0]) + tokens[0].Length).Trim();
                        if (string.IsNullOrEmpty(rest))
                        {
                            field.Add(new SQLFieldInfo { Name = colName });
                            continue;
                        }

                        // Data type may include parentheses, capture leading type(...) or single token
                        var dtypeMatch = System.Text.RegularExpressions.Regex.Match(rest, @"^([A-Za-z0-9_]+)\s*(\(([^\)]+)\))?", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        string dtype = null; int dsize = 0;
                        if (dtypeMatch.Success)
                        {
                            dtype = dtypeMatch.Groups[1].Value;
                            var sizeGrp = dtypeMatch.Groups[3].Value;
                            if (!string.IsNullOrEmpty(sizeGrp))
                            {
                                // take first number if multiple
                                var numMatch = System.Text.RegularExpressions.Regex.Match(sizeGrp, @"(\d+)");
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

                        // Look for inline REFERENCES in column definition
                        var inlineRef = System.Text.RegularExpressions.Regex.Match(rest, @"REFERENCES\s+([\[\]`""\w\.]+)\s*\(([^\)]+)\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
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

                        field.Add(fieldInfo);
                    }                    
                }
                return field;
                
            }
        }
    }
}
