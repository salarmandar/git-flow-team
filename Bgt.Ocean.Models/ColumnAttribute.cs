using System;

namespace Bgt.Ocean.Models
{

    public class ColumnAttribute
    {
        public String Name { get; set; }

        public Boolean PrimaryKey { get; set; }

        public Boolean ForeignKey { get; set; }

        public Boolean Identity { get; set; }

        public Type Type { get; set; }
    }
}
