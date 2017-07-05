using System;

namespace SignixDemo.Models.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public string DocumentSetId { get; set; }
        public string EventDateTime { get; set; }
        public string PartyId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}