using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Bgt.Ocean.Models.PreVault
{
    public class ISAPassingInfoModel
    {
        private const string v = "http://schemas.xmlsoap.org/soap/envelope/";
        private const string tem = "http://tempuri.org/";
        private const string del = "http://schemas.datacontract.org/2004/07/DeLaRue.Isa.Core.WebServices";
        private const string del1= "http://schemas.datacontract.org/2004/07/DeLaRue.Isa.ServiceModels.Reception";


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
                staticxmlns.Add("del1", del1);
            }
            private static XmlSerializerNamespaces staticxmlns;
            [XmlNamespaceDeclarations]
            public XmlSerializerNamespaces xmlns { get { return staticxmlns; } set { } }
        }

        [XmlType(Namespace = v)]
        public class Header { }
        [XmlType(Namespace = tem)]
        public class Body
        {
            [XmlElement(ElementName = "CreateTrackAndTraceData")]
            public CreateTrackAndTraceData CreateTrackAndTraceData { get; set; }
        }

        [XmlType(Namespace = tem)]
        public class CreateTrackAndTraceData
        {
            [XmlElement(ElementName = "token")]
            public Token token { get; set; }
            [XmlElement(ElementName = "trackAndTraceData")]
            public trackAndTraceData trackAndTraceData { get; set; }           
        }

        [XmlType(Namespace = del)]
        public class Token
        {
            [XmlElement(ElementName = "SessionId")]
            public string SessionId { get; set; }
            [XmlElement(ElementName = "SignedTimeStamp")]
            public string SignedTimeStamp { get; set; }
            [XmlElement(ElementName = "TimeStamp")]
            public string TimeStamp { get; set; }
        }

        [XmlType(Namespace = del1)]
        public class trackAndTraceData
        {
            [XmlElement(ElementName = "DeliveryDate")]
            public string DeliveryDate { get; set; }
            [XmlElement(ElementName = "RouteCode")]
            public string RouteCode { get; set; }
            [XmlElement(ElementName = "TrackAndTraceContainers")]
            public TrackAndTraceContainers TrackAndTraceContainers { get; set; }
        }

        [XmlType(Namespace = del1)]
        public class TrackAndTraceContainers
        {
            [XmlElement(ElementName = "TrackAndTraceServiceContainersSM")]
            public List<TrackAndTraceServiceContainersSM> TrackAndTraceServiceContainersSM { get; set; }
          
        }

        [XmlType(Namespace = del1)]
        public class TrackAndTraceServiceContainersSM
        {

            [XmlElement(ElementName = "ClientCode")]
            public string ClientCode { get; set; }

            [XmlElement(ElementName = "ContainerId")]
            public string ContainerId { get; set; }

            [XmlElement(ElementName = "ContentHierarchyCode")]
            public int ContentHierarchyCode { get; set; }

            [XmlElement(ElementName = "HierarchyGroup")]
            public string HierarchyGroup { get; set; }

            [XmlElement(ElementName = "Value")]
            public string Value { get; set; }
        }
       
    }

    public class SOAPErrorModel
    {

        [XmlRoot(ElementName = "WebServiceError", Namespace = "http://schemas.datacontract.org/2004/07/DeLaRue.Isa.Core.WebServices")]
        public class WebServiceError
        {

            [XmlElement(ElementName = "Category", Namespace = "http://schemas.datacontract.org/2004/07/DeLaRue.Isa.Core.WebServices")]
            public string Category { get; set; }

            [XmlElement(ElementName = "Data", Namespace = "http://schemas.datacontract.org/2004/07/DeLaRue.Isa.Core.WebServices")]
            public object Data { get; set; }

            [XmlElement(ElementName = "Error", Namespace = "http://schemas.datacontract.org/2004/07/DeLaRue.Isa.Core.WebServices")]
            public string Error { get; set; }
        }

        [XmlRoot(ElementName = "Errors", Namespace = "http://schemas.datacontract.org/2004/07/DeLaRue.Isa.Core.WebServices")]
        public class Errors
        {

            [XmlElement(ElementName = "WebServiceError", Namespace = "http://schemas.datacontract.org/2004/07/DeLaRue.Isa.Core.WebServices")]
            public WebServiceError WebServiceError { get; set; }
        }

        [XmlRoot(ElementName = "WebServiceErrorCollection", Namespace = "http://schemas.datacontract.org/2004/07/DeLaRue.Isa.Core.WebServices")]
        public class WebServiceErrorCollection
        {

            [XmlElement(ElementName = "Errors", Namespace = "http://schemas.datacontract.org/2004/07/DeLaRue.Isa.Core.WebServices")]
            public Errors Errors { get; set; }

            [XmlElement(ElementName = "TimeStamp", Namespace = "http://schemas.datacontract.org/2004/07/DeLaRue.Isa.Core.WebServices")]
            public DateTime TimeStamp { get; set; }

            [XmlAttribute(AttributeName = "xmlns", Namespace = "")]
            public string Xmlns { get; set; }

            [XmlAttribute(AttributeName = "i", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string I { get; set; }

            [XmlText]
            public string Text { get; set; }
        }





    }

}
