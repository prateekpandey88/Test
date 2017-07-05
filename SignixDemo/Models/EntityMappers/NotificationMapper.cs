using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SignixDemo.Models.Entities;

namespace SignixDemo.Models.EntityMappers
{
    public class NotificationMapper : EntityTypeConfiguration<Notification>
    {
        public NotificationMapper()
        {
            ToTable("tbl_Notifications");

            HasKey(x => x.Id)
                .Property(x => x.Id)
                .HasColumnName("pk_int_Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Action).HasColumnName("varchar_Action");
            Property(x => x.DocumentSetId).HasColumnName("varchar_DocumentSetId");
            Property(x => x.EventDateTime).HasColumnName("varhcar_EventDatetime");
            Property(x => x.PartyId).HasColumnName("varchar_PartyId");
            Property(x => x.CreatedOn).HasColumnName("datetime_CreatedOn");
        }
    }
}