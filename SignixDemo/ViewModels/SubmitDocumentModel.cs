namespace SignixDemo.ViewModels
{
    public class SubmitDocumentModel
    {
        public Signer[] Signers { get; set; }
        public string FileName { get; set; }
        public string EmailContent { get; set; }
        public string DocSetDescription { get; set; }
        public string SubmitterEmail { get; set; }
        public string ContactInfo { get; set; }
        public string DistributionEmailList { get; set; }
        public string DistributionEmailContent { get; set; }
        public string OriginalFileName { get; set; }
    }

    public class Signer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
    }
}