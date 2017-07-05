using System;
using System.Xml.Serialization;

namespace SignixDemo.ViewModels
{

    [Serializable]
    [XmlRoot("SubmitDocumentRq")]
    public class SubmitDocument
    {
        public SubmitDocumentCustInfo CustInfo { get; set; }
        public SubmitDocumentData Data { get; set; }
    }

    public class SubmitDocumentCustInfo
    {
        [XmlElement("Sponsor")]
        public string Sponsor { get; set; }

        [XmlElement("Client")]
        public string Client { get; set; }

        [XmlElement("UserId")]
        public string UserId { get; set; }

        [XmlElement("Pswd")]
        public string Pswd { get; set; }

        public string BillingRef { get; set; }

        public string Workgroup { get; set; }

        public string Demo { get; set; }

        public int DelDocsAfter { get; set; }

        public int ExpireAfter { get; set; }

        public string EmailContent { get; set; }
    }

    public class SubmitDocumentData
    {
        public string TransactionID { get; set; }

        public string DocSetDescription { get; set; }

        public string FileName { get; set; }

        public string SubmitterEmail { get; set; }

        public string ContactInfo { get; set; }

        public string DeliveryType { get; set; }

        public string SuspendOnStart { get; set; }

        public string UseMyDoX { get; set; }

        public string DistributionEmailContent { get; set; }

        public string DistributeToSubmitter { get; set; }

        public string DistributionEmailList { get; set; }

        public string SubmitterName { get; set; }

        [XmlElement("ClientPreference")]
        public ClientPreference[] ClientPreference { get; set; }

        [XmlElement("MemberInfo")]
        public MemberInfo[] MemberInfo { get; set; }

        public Form Form { get; set; }
    }

    public class ClientPreference
    {
        [XmlAttribute("name")]
        public string Type { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class MemberInfo
    {
        public string RefID { get; set; }

        public string WorkflowOrder { get; set; }

        //public string SignixUserId { get; set; }

        public string SSN { get; set; }

        public string DOB { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        //public string HomePhone { get; set; }

        //public string Street { get; set; }

        //public string Apartment { get; set; }

        //public string City { get; set; }

        //public string State { get; set; }

        //public string Zipcode { get; set; }

        //public string Locale { get; set; }

        public string Service { get; set; }

        public string MobileNumber { get; set; }

        public string Q1 { get; set; }

        public string A1 { get; set; }

        public string Q2 { get; set; }

        public string A2 { get; set; }
    }

    public class Form
    {
        public string RefID { get; set; }

        public string Desc { get; set; }

        public string FileName { get; set; }

        public string MimeType { get; set; }

        //public string FormPassword { get; set; }

        public string Length { get; set; }

        [XmlElement("Data")]
        public string FileData { get; set; }
    }

    [XmlRoot("SubmitDocumentRs", Namespace = "urn:com:signix:schema:sdddc-1-1", IsNullable = false)]
    public class SubmitDocumentRs
    {
        public GetAccessLinkRsStatus Status { get; set; }
        public SubmitDocumentRsData Data { get; set; }
    }

    [XmlRoot("Data")]
    public class SubmitDocumentRsData
    {
        public string DocumentSetID { get; set; }
        public string PickupToken { get; set; }
        public string PickupLink { get; set; }
    }
}