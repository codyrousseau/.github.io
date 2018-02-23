using crousseau2_College_Strike.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace crousseau2_College_Strike.DAL
{
    public class StrikeEntities : DbContext
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Position> Positions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
        public override int SaveChanges()
        {
            //Get Audit Values if not supplied
            string auditUser = "Anonymous";
            try //Need to try becuase HttpContext might not exist
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                    auditUser = HttpContext.Current.User.Identity.Name;
            }
            catch (Exception)
            { }

            DateTime auditDate = DateTime.UtcNow;
            foreach (DbEntityEntry<IAuditable> entry in ChangeTracker.Entries<IAuditable>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedOn = auditDate;
                    entry.Entity.CreatedBy = auditUser;
                    entry.Entity.UpdatedOn = auditDate;
                    entry.Entity.UpdatedBy = auditUser;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedOn = auditDate;
                    entry.Entity.UpdatedBy = auditUser;
                }
            }
            return base.SaveChanges();
        }
    }
}