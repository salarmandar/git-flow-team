using System;

namespace Bgt.Ocean.Infrastructure.CustomAttributes
{
    public class AvoidAuditLogAttribute : Attribute
    {
        public string Description { get; set; }
    }
}
