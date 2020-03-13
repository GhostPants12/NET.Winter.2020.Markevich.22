using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecordTypeTable
{
    public class RowInfo
    {
        private readonly List<string> fields;

        public RowInfo(List<string> fields, Type type, string name)
        {
            this.fields = fields;
            this.Type = type;
            this.MaxLength = fields.Max(s => s.Length) >= name.Length ? fields.Max(s => s.Length) : name.Length;
            this.FieldsNumber = this.fields.Count;
            this.PropertyName = name;
        }

        public int MaxLength { get; private set; }

        public Type Type { get; private set; }

        public string PropertyName { get; private set; }

        public int FieldsNumber { get; private set; }

        public string this[int index]
        {
            get => this.fields[index];
        }
    }
}
