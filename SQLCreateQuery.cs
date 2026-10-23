using System;
using System.Collections.Generic;
using System.Text;

namespace USS.Tool.GenCRUD
{
    public class SQLCreateQuery
    {
        private string createQry;
        public SQLCreateQuery(SQLFile sql)
        {
            createQry = sql.Query;
        }

        public string TableName
        {
            get
            {
                if (field == null)
                {
                    var idx = createQry.IndexOf("CREATE TABLE", StringComparison.OrdinalIgnoreCase);
                    if (idx >= 0)
                    {
                        idx += "CREATE TABLE".Length;

                        var after = createQry.Substring(idx).TrimStart();
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
                    var query = createQry;
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

                            if (parenStart < parenEnd)
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
                            SQLFieldInfo.SetReference(partRaw, field);
                            continue;
                        }

                        var fieldInfo = SQLFieldInfo.Parse(partRaw);
                        if(fieldInfo != null)   field.Add(fieldInfo);
                    }
                }
                return field;

            }
        }
    }
}
