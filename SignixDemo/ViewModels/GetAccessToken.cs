using System;
using System.Xml.Serialization;

namespace SignixDemo.ViewModels
{
    [Serializable]
    [XmlRoot("GetAccessTokenRq")]
    public class GetAccessTokenRq
    {
        public CustInfo CustInfo { get; set; }
        public GetAccessTokenRqData Data { get; set; }
    }

    public class GetAccessTokenRqData
    {
        [XmlElement("DocumentSetID")]
        public string DocumentSetId { get; set; }
    }

    [XmlRoot("GetAccessTokenRs", Namespace = "urn:com:signix:schema:sdddc-1-1", IsNullable = false)]
    public class GetAccessTokenRs
    {
        public GetAccessLinkRsStatus Status { get; set; }
        public GetAccessTokenRsData Data { get; set; }
    }

    public class GetAccessTokenRsData
    {
        public string AccessToken { get; set; }
    }
}