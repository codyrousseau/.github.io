using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using crousseau2_College_Strike.DAL;
using crousseau2_College_Strike.Models;
using crousseau2_College_Strike.ViewModels;
using System.Data.Entity.Infrastructure;
using PagedList;

namespace crousseau2_College_Strike.Controllers
{
    [Authorize]
    public class MembersController : Controller
    {
        private StrikeEntities db = new StrikeEntities();

        // GET: Members
            public ActionResult Index(string sortDirection, string sortField,
                string actionButton, string searchName, string searchPhone, int? page)
            {
                IQueryable<Member> members = db.Members.Include(a => a.Positions);
                ViewBag.Filtering = ""; //Assume not filtering

                if (!String.IsNullOrEmpty(searchName))
                {
                    members = members
                        .Where(p => p.FirstName.ToUpper().Contains(searchName.ToUpper())
                            || p.LastName.ToUpper().Contains(searchName.ToUpper()));
                    ViewBag.Filtering = " in";//Flag filtering
                    ViewBag.searchName = searchName;
                }

                if (!String.IsNullOrEmpty(searchPhone))
                {
                    members = members.Where(p => p.Phone.ToString().ToUpper().Contains(searchPhone.ToUpper()));
                    ViewBag.Filtering = " in";//Flag filtering
                    ViewBag.searchPhone = searchPhone;
                }

                //Before we sort, see if we have called for a change of filtering or sorting
                if (!String.IsNullOrEmpty(actionButton)) //Form Submitted so lets sort!
                {
                    //Reset paging if ANY button was pushed
                    page = 1;

                    if (actionButton != "Filter")//Change of sort is requested
                    {
                        if (actionButton == sortField) //Reverse order on same field
                        {
                            sortDirection = String.IsNullOrEmpty(sortDirection) ? "desc" : "";
                        }
                        sortField = actionButton;//Sort by the button clicked
                    }
                }
                //Now we know which field and direction to sort by, but a Switch is hard to use for 2 criteria
                //so we will use an if() structure instead.
                if (sortField == "Phone Number")//Sorting by Applicant Name
                {
                    if (String.IsNullOrEmpty(sortDirection))
                    {
                        members = members
                            .OrderBy(p => p.Phone);
                    }
                    else
                    {
                        members = members
                            .OrderByDescending(p => p.Phone);
                    }
                }
                else //By default sort by Applicant
                {
                    if (String.IsNullOrEmpty(sortDirection))
                    {
                        members = members
                            .OrderBy(p => p.LastName)
                            .ThenBy(p => p.FirstName);
                    }
                    else
                    {
                        members = members
                            .OrderByDescending(p => p.LastName)
                            .ThenByDescending(p => p.FirstName);
                    }

                }

                //Set sort for next time
                ViewBag.sortField = sortField;
                ViewBag.sortDirection = sortDirection;

                int pageSize = 4;
                int pageNumber = (page ?? 1);

                return View(members.ToPagedList(pageNumber, pageSize));
            }
        

        // GET: Members/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members
                .Include(m => m.Assignment)
                .Include(m => m.Positions)
                .Where(m => m.id == id).SingleOrDefault();
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // GET: Members/Create
        [Authorize(Roles = "Steward, Admin")]
        public ActionResult Create()
        {
            PopulateDropDownLists();
            var member = new Member();
            member.Positions = new List<Position>();
            PopulateAssignedPositionData(member);
            return View();
        }



        // POST: Members/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Steward, Admin")]
        public ActionResult Create([Bind(Include = "id,FirstName,LastName,Phone,eMail,AssignmentID")] Member member, string[] selectedPosition)
        {
            try
            {
                if (selectedPosition != null)
                {
                    member.Positions = new List<Position>();
                    foreach (var pos in selectedPosition)
                    {
                        var posToAdd = db.Positions.Find(int.Parse(pos));
                        member.Positions.Add(posToAdd);
                    }
                }
            if (ModelState.IsValid)
            {
                db.Members.Add(member);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DataException dex)
            {
                if (dex.InnerException.InnerException.Message.Contains("IX_Unique"))
                {
                    ModelState.AddModelError("eMail", "Unable to save changes. Remember, no two members can have the same eMail address.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, give up.");
                }
            }
            PopulateDropDownLists(member);
            return View(member);
        }

        // GET: Members/Edit/5
        [Authorize(Roles = "Steward, Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members
                .Include(a => a.Positions)
                .Where(a => a.id == id).SingleOrDefault();
            if (member == null)
            {
                return HttpNotFound();
            }
            PopulateDropDownLists(member);
            PopulateAssignedPositionData(member);
            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Steward, Admin")]
        public ActionResult Edit(int? id, Byte[] rowVersion, string[] selectedPosition, [Bind(Include = "id,FirstName,LastName,Phone,eMail,AssignmentID")] Member member)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member memberToUpdate = db.Members
                .Include(a => a.Positions)
                .Where(a => a.id == id).SingleOrDefault();
            if (TryUpdateModel(memberToUpdate, "",
                new string[] { "FirstName", "LastName", "Phone", "eMail" }))
            {
                try
                {
                    UpdateMemberPosition(selectedPosition, memberToUpdate);

                    db.Entry(memberToUpdate).OriginalValues["RowVersion"] = rowVersion;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException ex)// Added for concurrency
                {
                    var entry = ex.Entries.Single();
                    var clientValues = (Member)entry.Entity;
                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("",
                            "Unable to save changes. The member was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Member)databaseEntry.ToObject();
                        if (databaseValues.FirstName != clientValues.FirstName)
                            ModelState.AddModelError("FirstName", "Current value: "
                                + databaseValues.FirstName);
                        if (databaseValues.LastName != clientValues.LastName)
                            ModelState.AddModelError("LastName", "Current value: "
                                + databaseValues.LastName);
                        if (databaseValues.Phone != clientValues.Phone)
                            ModelState.AddModelError("Phone", "Current value: "
                                + String.Format("{0:(###) ###-####}", databaseValues.Phone));
                        if (databaseValues.eMail != clientValues.eMail)
                            ModelState.AddModelError("eMail", "Current value: "
                                + databaseValues.eMail);
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you received your values. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to save your version of this record, click "
                                + "the Save button again. Otherwise click the 'Back to List' hyperlink.");
                        memberToUpdate.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (DataException dex)
                {
                    if (dex.InnerException.InnerException.Message.Contains("IX_Unique"))
                    {
                        ModelState.AddModelError("eMail", "Unable to save changes. Remember, you cannot have duplicate eMail addresses.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists go for coffee.");
                    }
                }

            }
            PopulateDropDownLists(memberToUpdate);
            PopulateAssignedPositionData(memberToUpdate);
            return View(memberToUpdate);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Member member = db.Members.Find(id);
            try
            {
                db.Members.Remove(member);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DataException dex)
            {
                if (dex.InnerException.InnerException.Message.Contains("FK_"))
                {
                    ModelState.AddModelError("", "You cannot delete a member that has a shift in the system.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists give up.");
                }
            }
            return View(member);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void UpdateMemberPosition(string[] selectedPosition, Member memberToUpdate)
        {
            if (selectedPosition == null)
            {
                memberToUpdate.Positions = new List<Position>();
                return;
            }

            var selectedPositionHS = new HashSet<string>(selectedPosition);
            var memberPosition = new HashSet<int>
                (memberToUpdate.Positions.Select(c => c.id));
            foreach (var pos in db.Positions)
            {
                if (selectedPositionHS.Contains(pos.id.ToString()))
                {
                    if (!memberPosition.Contains(pos.id))
                    {
                        memberToUpdate.Positions.Add(pos);
                    }
                }
                else
                {
                    if (memberPosition.Contains(pos.id))
                    {
                        memberToUpdate.Positions.Remove(pos);
                    }
                }
            }
        }

        private void PopulateDropDownLists(Member posting = null)
        {
            ViewBag.AssignmentID = new SelectList(db.Assignments.OrderBy(p => p.AssignmentName), "id", "AssignmentName", posting?.AssignmentID);
        }

        private void PopulateAssignedPositionData(Member member)
        {
            var allPostions = db.Positions;
            var appPositions = new HashSet<int>(member.Positions.Select(b => b.id));
            var viewModel = new List<PositionVM>();
            foreach (var p in allPostions)
            {
                viewModel.Add(new PositionVM
                {
                    PositionID = p.id,
                    PositionName = p.PositionTitle,
                    Assigned = appPositions.Contains(p.id)
                });
            }
            ViewBag.Positions = viewModel;
        }
    }
}
