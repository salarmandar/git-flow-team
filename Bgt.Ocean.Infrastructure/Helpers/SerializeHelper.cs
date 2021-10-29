namespace Bgt.Ocean.Infrastructure.Helpers
{
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Helper to Serialize Objects.
    /// </summary>
    public static class SerializeHelper
    {
        /// <summary>
        /// Serialize a object to XML.
        /// </summary>
        /// <param name="obj">object to serialize.</param>
        /// <returns>a xml as string.</returns>
        public static string SerializeObjectToXML(object obj)
        {
            string retval = null;
            if (obj != null)
            {
                StringBuilder sb = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true }))
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add(string.Empty, string.Empty);
                    new XmlSerializer(obj.GetType()).Serialize(writer, obj, ns);
                }

                retval = sb.ToString().ToLower();
            }

            return retval;
        }
    }
}
