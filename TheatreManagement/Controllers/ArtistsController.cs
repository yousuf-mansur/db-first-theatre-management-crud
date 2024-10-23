using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheatreManagement.Models;
using TheatreManagement.Models.ViewModels;

namespace TheatreManagement.Controllers
{
    public class ArtistsController : Controller
    {
        private TheatreDBEntities db = new TheatreDBEntities();
        // GET: Artists
        public ActionResult Index()
        {
            var artists = db.Artists.Include(c => c.RoleEntries.Select(b => b.Role)).OrderByDescending(x => x.ArtistId).ToList();
            return View(artists);
        }
        public ActionResult AddNewRole(int? id)
        {
            ViewBag.roles = new SelectList(db.Roles.ToList(), "RoleId", "RoleTitle", (id != null) ? id.ToString() : "");
            return PartialView("_AddNewRole");
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(ArtistViewModel artistViewModel, int[] RoleId)
        {
            if (ModelState.IsValid)
            {
                Artist artist = new Artist()
                {
                    ArtistName = artistViewModel.ArtistName,
                    DateOfBirth = artistViewModel.DateOfBirth,
                    Age = artistViewModel.Age,
                    Status = artistViewModel.Status
                };
                //Image
                HttpPostedFileBase file = artistViewModel.PictureFile;
                if (file != null)
                {
                    string fileName = DateTime.Now.Ticks.ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine("/Images", fileName);
                    file.SaveAs(Server.MapPath(filePath));
                    artist.Picture = filePath;
                }
                //All Role
                foreach (var item in RoleId)
                {
                    RoleEntry roleEntry = new RoleEntry()
                    {
                        Artist = artist,
                        ArtistId = artist.ArtistId,
                        RoleId = item
                    };
                    db.RoleEntries.Add(roleEntry);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult Edit(int? id)
        {
            Artist artist = db.Artists.First(x => x.ArtistId == id);
            var artistRoles = db.RoleEntries.Where(x => x.ArtistId == id).ToList();
            ArtistViewModel artistViewModel = new ArtistViewModel()
            {
                ArtistId = artist.ArtistId,
                ArtistName = artist.ArtistName,
                Age = artist.Age,
                DateOfBirth = artist.DateOfBirth,
                Picture = artist.Picture,
                Status = artist.Status
            };
            if (artistRoles.Count() > 0)
            {
                foreach (var item in artistRoles)
                {
                    artistViewModel.RoleList.Add(item.RoleId);
                }
            }
            return View(artistViewModel);
        }
        [HttpPost]
        public ActionResult Edit(ArtistViewModel artistViewModel, int[] RoleId)
        {
            if (ModelState.IsValid)
            {
                Artist artist = new Artist()
                {
                    ArtistId = artistViewModel.ArtistId,
                    ArtistName = artistViewModel.ArtistName,
                    DateOfBirth = artistViewModel.DateOfBirth,
                    Age = artistViewModel.Age,
                    Status = artistViewModel.Status
                };
                //Image
                HttpPostedFileBase file = artistViewModel.PictureFile;
                if (file != null)
                {
                    string fileName = DateTime.Now.Ticks.ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine("/Images", fileName);
                    file.SaveAs(Server.MapPath(filePath));
                    artist.Picture = filePath;
                }
                else
                {
                    artist.Picture = artistViewModel.Picture;
                }
                //role delete
                var existsRoleEntry = db.RoleEntries.Where(x => x.ArtistId == artist.ArtistId).ToList();
                foreach (var roleEntry in existsRoleEntry)
                {
                    db.RoleEntries.Remove(roleEntry);
                }
                //Add Role
                foreach (var item in RoleId)
                {
                    RoleEntry roleEntry = new RoleEntry()
                    {
                        Artist = artist,
                        ArtistId = artist.ArtistId,
                        RoleId = item
                    };
                    db.RoleEntries.Add(roleEntry);
                }
                db.Entry(artist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult Delete(int? id)
        {
            var artist = db.Artists.Find(id);
            var existsRoleEntry = db.RoleEntries.Where(x => x.ArtistId == artist.ArtistId).ToList();
            foreach (var roleEntries in existsRoleEntry)
            {
                db.RoleEntries.Remove(roleEntries);
            }
            db.Entry(artist).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}