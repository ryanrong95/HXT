using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Needs.Ccs.Services.Models
{
    [XmlRoot(ElementName = "responses")]
    public class EmsResponseModel
    {

        [XmlElement(ElementName = "responseItems")]
        public ResponseItems ResponseItems;

    }


    public class ResponseItems
    {
        [XmlElement(ElementName = "response")]
        public Response Response;
    }

    public class Response
    {
        [XmlElement(ElementName = "success")]
        public bool Success { get; set; }

        [XmlElement(ElementName = "waybill_no")]
        public string WaybillNo { get; set; }

        [XmlElement(ElementName = "routeCode")]
        public string RouteCode { get; set; }

        [XmlElement(ElementName = "packageCode")]
        public string PackageCode { get; set; }

        [XmlElement(ElementName = "packageCodeName")]
        public string PackageCodeName { get; set; }

        [XmlElement(ElementName = "markDestinationCode")]
        public string MarkDestinationCode { get; set; }

        [XmlElement(ElementName = "markDestinationName")]
        public string MarkDestinationName { get; set; }

        [XmlElement(ElementName = "reason")]
        public string Reason { get; set; }

    }
}
