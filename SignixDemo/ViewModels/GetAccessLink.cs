using System;
using System.Xml.Serialization;

namespace SignixDemo.ViewModels
{
    [Serializable]
    [XmlRoot("GetAccessLinkRq")]
    public class GetAccessLink
    {
        [XmlElement("CustInfo")]
        public CustInfo CustInfo { get; set; }

        [XmlElement("Data")]
        public Data Data { get; set; }
    }

    [Serializable]
    [XmlRoot("CustInfo")]
    public class CustInfo
    {
        [XmlElement("Sponsor")]
        public string Sponsor { get; set; }

        [XmlElement("Client")]
        public string Client { get; set; }

        [XmlElement("UserId")]
        public string UserId { get; set; }

        [XmlElement("Pswd")]
        public string Pswd { get; set; }
    }

    [Serializable]
    [XmlRoot("Data")]
    public class Data
    {
        [XmlElement("TransactionID")]
        public string TransactionId { get; set; }

        [XmlElement("DocumentSetID")]
        public string DocumentSetId { get; set; }

        [XmlElement("TransactionRole")]
        public string TransactionRole { get; set; }

        public bool AllowTransactionStatusViewing { get; set; }

        [XmlElement("AllowTransactionContextEditing")]
        public bool AllowTransactionContextEditing { get; set; }

        public bool AllowDocumentFieldAndTaskEditing { get; set; }

        [XmlElement("AllowDocumentEditing")]
        public bool AllowDocumentEditing { get; set; }

        [XmlElement("AllowParticipantEditing")]
        public bool AllowParticipantEditing { get; set; }

        public bool UseMyDoXWizard { get; set; }

        public string SkinID { get; set; }

        public string Embedded { get; set; }

        public string ContainerOrigin { get; set; }

        public string FinishURL { get; set; }
    }


    [XmlRoot("GetAccessLinkRs", Namespace = "urn:com:signix:schema:sdddc-1-1", IsNullable = false)]
    public class GetAccessLinkRs
    {
        [XmlElement("Status")]
        public GetAccessLinkRsStatus Status { get; set; }

        [XmlElement("Data")]
        public GetAccessLinkRsData Data { get; set; }

    }

    [Serializable]
    [XmlRoot("Status")]
    public class GetAccessLinkRsStatus
    {
        public int StatusCode { get; set; }
        public string StatusDesc { get; set; }
    }

    [Serializable]
    [XmlRoot("Data")]
    public class GetAccessLinkRsData
    {
        public string AccessLink { get; set; }
    }
}