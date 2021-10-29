using System;
using System.Configuration;

namespace EntityFrameworkExtras.EF6
{

    public class UserDefinedTableTypeAttribute : Attribute
    {
        static string schema = "dbo";

        static UserDefinedTableTypeAttribute()
        {
            string staging = ConfigurationManager.AppSettings["EnvSTG"];

            if (!string.IsNullOrEmpty(staging))
            {
                schema = Convert.ToBoolean(staging) ? "stg" : "dbo";
            }
        }

        public UserDefinedTableTypeAttribute(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Cannot be null or empty.", "name");
            }

            if (name.Contains("dbo.") || name.Contains("stg."))
            {
                Name = name;
            }
            else
            {
                Name = schema + "." + name;
            }
        }

        public string Name { get; set; }
    }
}
