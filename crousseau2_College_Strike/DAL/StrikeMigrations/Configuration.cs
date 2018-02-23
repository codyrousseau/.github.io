namespace crousseau2_College_Strike.DAL.StrikeMigrations
{
    using crousseau2_College_Strike.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Text;

    internal sealed class Configuration : DbMigrationsConfiguration<crousseau2_College_Strike.DAL.StrikeEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"DAL\StrikeMigrations";
        }

        private void SaveChanges(DbContext context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); 
            }
            catch (Exception e)
            {
                throw new Exception(
                     "Seed Failed - errors follow:\n" +
                     e.InnerException.InnerException.Message.ToString(), e
                 ); 
            }
        }

        protected override void Seed(crousseau2_College_Strike.DAL.StrikeEntities context)
        {
            var assignments = new List<Assignment>
            {
                new Assignment {AssignmentName = "Main Campus Gate 1"},
                new Assignment {AssignmentName = "Main Campus Gate 2"},
                new Assignment {AssignmentName = "Main Campus Gate 3"},
                new Assignment {AssignmentName = "Shuttle Driver"},
                new Assignment {AssignmentName = "Kitchen Help"},
                new Assignment {AssignmentName = "Sign Preperation"},
                new Assignment {AssignmentName = "Doctor"}
            };
            assignments.ForEach(a => context.Assignments.AddOrUpdate(n => n.AssignmentName, a));
            SaveChanges(context);

            var members = new List<Member>
            {
                new Member {FirstName = "Peter", LastName = "Bonsu", Phone = 6664206969, eMail = "drbonsu@gmail.com", AssignmentID = (context.Assignments.Where(m=>m.AssignmentName=="Doctor").SingleOrDefault().id),
                Positions = new List<Position> {new Position {PositionTitle = "Negotiator"} } },
                new Member {FirstName = "Jesse", LastName = "Mac", Phone = 1234567890, eMail = "jmac@gmail.com", AssignmentID = (context.Assignments.Where(m=>m.AssignmentName=="Kitchen Help").SingleOrDefault().id),
                Positions = new List<Position> {new Position {PositionTitle = "Captain"} } } ,
                new Member {FirstName = "Jay", LastName = "Mascis", Phone = 6664206969, eMail = "dinojr@gmail.com", AssignmentID = (context.Assignments.Where(m=>m.AssignmentName=="Sign Preperation").SingleOrDefault().id),
                Positions = new List<Position> {new Position {PositionTitle = "Spokesperson"} } },
                new Member {FirstName = "Kya", LastName = "Philip", Phone = 6664206969, eMail = "bork@gmail.com", AssignmentID = (context.Assignments.Where(m=>m.AssignmentName=="Sign Preperation").SingleOrDefault().id),
                Positions = new List<Position> {new Position {PositionTitle = "Doggo"} } }
            };
            members.ForEach(a => context.Members.AddOrUpdate(n => n.eMail, a));
            SaveChanges(context);
            var shifts = new List<Shift>
            {
                new Shift {ShiftDate=DateTime.Parse("2017-11-15"),
                AssignmentID =(context.Assignments.Where(p=>p.AssignmentName=="Doctor").SingleOrDefault().id),
                MemberID =(context.Members.Where(p=>p.eMail=="drbonsu@gmail.com").SingleOrDefault().id)}
            };
            shifts.ForEach(a => context.Shifts.AddOrUpdate(n => n.ShiftDate, a));
            SaveChanges(context);
        }
    }
}
