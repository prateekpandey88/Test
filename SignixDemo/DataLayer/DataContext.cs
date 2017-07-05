using System.Data.Entity;
using SignixDemo.Models.EntityMappers;

namespace SignixDemo.DataLayer
{
    public class DataContext : DbContext
    {
        public DataContext() : base("DataConnection") { }

        public DataContext(string connectionStringName) : base(connectionStringName) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new SendRequestMapper());
            modelBuilder.Configurations.Add(new NotificationMapper());
            modelBuilder.Configurations.Add(new NotificationUrlsMapper());

            base.OnModelCreating(modelBuilder);
        }
    }
}