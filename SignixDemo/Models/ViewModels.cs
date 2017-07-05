using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignixDemo.Models
{
    public class RequestStatusVm
    {
        public string Signer1Name { get; set; }
        public string Signer2Name { get; set; }
        public string OriginalDocumentName { get; set; }
        public string DocumentName { get; set; }
        public string SubmitterEmail { get; set; }
        public string DocumentSetId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Status { get; set; }
    }
}