using System;

namespace Bgt.Ocean.Infrastructure.CustomAttributes
{
    public class ChangeJsonPropNameAttribute : Attribute
    {
        public ChangeJsonPropNameAttribute()
        {

        }

        public ChangeJsonPropNameAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}
