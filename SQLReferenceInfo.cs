using System;
using System.Collections.Generic;
using System.Text;

namespace USS.Tool.GenCRUD
{
    public class SQLReferenceInfo
    {
        public string ReferenceSchema { get; set; }
        public string ReferenceTableName { get; set; }
        public string ReferenceFieldName { get; set; }
        public string ReferenceConstraintName { get; set; }
        public bool IsForeignKey { get; set; }

        public string TableName
        {
            get
            {
                return (ReferenceTableName.StartsWith("[") && ReferenceTableName.EndsWith("]")) ? ReferenceTableName.Substring(1, ReferenceTableName.Length - 2) : ReferenceTableName;
            }
        }

        public string FieldName
        {
            get
            {
                return (ReferenceFieldName.StartsWith("[") && ReferenceFieldName.EndsWith("]")) ? ReferenceFieldName.Substring(1, ReferenceFieldName.Length - 2) : ReferenceFieldName;
            }
        }
    }
}
