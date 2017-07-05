using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SignixDemo.Models.Entities;

namespace SignixDemo.Models.EntityMappers
{
    public class NotificationUrlsMapper: EntityTypeConfiguration<NotificationUrls>
    {
        public NotificationUrlsMapper()
        {
            ToTable("SignixNotificationUrls");

            HasKey(x => x.Id)
                .Property(x => x.Id)
                .HasColumnName("pk_int_Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Url).HasColumnName("nvarvhar_Url");
            Property(x => x.CreatedOn).HasColumnName("datetime_CreatedOn");
        }
    }
}