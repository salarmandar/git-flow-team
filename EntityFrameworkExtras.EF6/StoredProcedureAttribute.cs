using System;
using System.Configuration;

namespace EntityFrameworkExtras.EF6
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class StoredProcedureAttribute : Attribute
	{
        static string schema = "dbo";

        static StoredProcedureAttribute()
        {
            string staging = ConfigurationManager.AppSettings["EnvSTG"];

            if (!string.IsNullOrEmpty(staging))
            {
                schema = Convert.ToBoolean(staging) ? "stg" : "dbo";
            }
        }

        public StoredProcedureAttribute(string name)
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
