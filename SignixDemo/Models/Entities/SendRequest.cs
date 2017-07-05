using System;

namespace SignixDemo.Models.Entities
{
    public class SendRequest
    {
        public int Id { get; set; }
        public string Signer1Name { get; set; }
        public string Signer2Name { get; set; }
        public string OriginalDocumentName { get; set; }
        public string DocumentName { get; set; }
        public string SubmitterEmail { get; set; }
        public string DocumentSetId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}