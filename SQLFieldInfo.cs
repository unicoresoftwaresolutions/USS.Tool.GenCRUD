using System;
using System.Collections.Generic;
using System.Text;

namespace USS.Tool.GenCRUD
{
    public class SQLFieldInfo
    {
        public String Name { get; set; }
        public String DataType { get; set; }
        public int DataSize { get; set; }
        // Foreign key / REFERENCES detail (single)
        public SQLReferenceInfo Reference { get; set; }

        // Helper: indicates whether this field has a foreign key reference
        public bool HasForeignKey => Reference?.IsForeignKey == true;

        // Helper: returns a display string for the reference, or null if none
        public string GetReferenceDisplay(bool includeSchema = true)
        {
            if (Reference == null)
                return null;

            var schema = string.IsNullOrWhiteSpace(Reference.ReferenceSchema) ? null : Reference.ReferenceSchema.Trim();
            var table = (Reference.ReferenceTableName ?? string.Empty).Trim();
            var field = (Reference.ReferenceFieldName ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(table) && string.IsNullOrEmpty(field))
                return null;

            var sb = new StringBuilder();
            if (includeSchema && !string.IsNullOrEmpty(schema))
            {
                sb.Append('[').Append(schema).Append(']').Append('.');
            }

            if (!string.IsNullOrEmpty(table))
            {
                // preserve existing brackets if present
                if (table.StartsWith("[") && table.EndsWith("]")) sb.Append(table);
                else sb.Append('[').Append(table).Append(']');
            }

            if (!string.IsNullOrEmpty(field))
            {
                sb.Append('.');
                if (field.StartsWith("[") && field.EndsWith("]")) sb.Append(field);
                else sb.Append('[').Append(field).Append(']');
            }

            if (!string.IsNullOrEmpty(Reference.ReferenceConstraintName))
            {
                sb.Append(" (constraint: ").Append(Reference.ReferenceConstraintName).Append(')');
            }

            return sb.ToString();
        }

    }
}
