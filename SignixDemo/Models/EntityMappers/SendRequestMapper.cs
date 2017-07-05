using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SignixDemo.Models.Entities;

namespace SignixDemo.Models.EntityMappers
{
    public class SendRequestMapper : EntityTypeConfiguration<SendRequest>
    {
        public SendRequestMapper()
        {
            ToTable("tbl_SendRequests");

            HasKey(x => x.Id)
                .Property(x => x.Id)
                .HasColumnName("pk_int_Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Signer1Name).HasColumnName("varchar_Signer1Name");
            Property(x => x.Signer2Name).HasColumnName("varchar_Signer2Name");
            Property(x => x.OriginalDocumentName).HasColumnName("varchar_OriginalDocumentName");
            Property(x => x.DocumentName).HasColumnName("varchar_DocumentName");
            Property(x => x.DocumentSetId).HasColumnName("varchar_DocumentSetId");
            Property(x => x.SubmitterEmail).HasColumnName("varchar_SubmitterEmail");
            Property(x => x.CreatedOn).HasColumnName("datetime_CreatedOn");

        }
    }
}