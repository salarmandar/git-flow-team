using System.Collections.Generic;
using System.Xml.Serialization;

namespace Bgt.Ocean.Models.PreVault
{ 
    public class ISAAuthRequest
    {
        private const string v = "http://schemas.xmlsoap.org/soap/envelope/";
        private const string tem = "http://tempuri.org/";
        private const string del = "http://schemas.datacontract.org/2004/07/DeLaRue.Isa.Core.WebServices";

        [XmlRoot(Namespace = v)]
        public class Envelope
        {
            public Header Header { get; set; }
            public Body Body { get; set; }

            static Envelope()
            {
                staticxmlns = new XmlSerializerNamespaces();
                staticxmlns.Add("soapenv", v);
                staticxmlns.Add("tem", tem);
                staticxmlns.Add("del", del);
            }
            private static XmlSerializerNamespaces staticxmlns;
            [XmlNamespaceDeclarations]
            public XmlSerializerNamespaces xmlns { get { return staticxmlns; } set { } }
        }

        [XmlType(Namespace = v)]
        public class Header { }

        [XmlType(Namespace = del)]
        public class AuthToken
        {
            [XmlElement(ElementName = "Name")]
            public string Name { get; set; }
            [XmlElement(ElementName = "Value")]
            public string Value { get; set; }
        }

        [XmlType(Namespace = del)]
        public class Token
        {
            [XmlElement(ElementName = "AuthToken")]
            public List<AuthToken> AuthToken { get; set; }
        }

        [XmlType(Namespace = tem)]
        public class Authenticate
        {
            [XmlElement(ElementName = "type")]
            public string type { get; set; }
            [XmlElement(ElementName = "tokens")]
            public Token tokens { get; set; }
        }

        [XmlType(Namespace = tem)]
        public class Body
        {
            [XmlElement(ElementName = "Authenticate")]
            public Authenticate Authenticate { get; set; }
        }

    }    

}
