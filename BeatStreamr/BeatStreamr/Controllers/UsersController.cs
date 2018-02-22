using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeatStreamr.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;

namespace BeatStreamr.Controllers
{
    public class UsersController : Controller
    {
        private readonly BeatStreamrContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        public UsersController(BeatStreamrContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }


        //public UsersController(IHostingEnvironment hostingEnvironment)
        //{

        //}

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .SingleOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(user.ProfileImagePath))
            {
                user.ProfileImagePath = "/images/users/profile/anonymous.gif";
            }

            if (string.IsNullOrWhiteSpace(user.HeaderImagePath))
            {
                user.HeaderImagePath = "/images/default-header.jpg";
            }

            var beats = from b in _context.Beat
                        join u in _context.User
                        on b.ArtistID equals u.ID
                        where b.ArtistID == id
                        select new BeatArtistViewModel
                        {
                            Name = u.Name,
                            ProfileImagePath = u.ProfileImagePath == null ? "/images/users/profile/anonymous.gif" : u.ProfileImagePath,
                            ID = b.ID,
                            Title = b.Title,
                            BPM = b.BPM,
                            Key = b.Key,
                            ArtistID = b.ArtistID,
                            Price = b.Price,
                            AlbumID = b.AlbumID,
                            HeaderImagePath = u.HeaderImagePath == null ? "/images/default-header.jpg" : u.HeaderImagePath,
                            ArtFilePath = b.ArtFilePath == null ? "/images/songs/blank-art.png" : b.ArtFilePath,
                            BeatFilePath = b.BeatFilePath

                        };

            var userBeatsViewModel = new UserBeatsViewModel
            {
                User = user,
                beatsList = await beats.ToListAsync()
            };

            return View(userBeatsViewModel);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,UserName")] User user)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                foreach (var Image in files)
                {
                    if (Image != null && Image.Length > 0)
                    {
                        var file = Image;
                        var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "images/users/profile");

                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() +
                                System.IO.Path.GetExtension(file.FileName);

                            System.Console.WriteLine(fileName);
                            using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                if (Image.Name.Equals("headerPic"))
                                {
                                    user.HeaderImagePath = $"/images/users/profile/{fileName}";
                                }
                                else
                                {
                                    user.ProfileImagePath = $"/images/users/profile/{fileName}";
                                }
                            }
                        }
                    }
                }



                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = user.ID });
            }
            return View(user);
        }

        //GET: Users/HeaderImage/5
        public async Task<IActionResult> HeaderImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.SingleOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(new UserHeaderImageViewModel { ID = user.ID, HeaderImagePath = user.HeaderImagePath });
        }

        //POST: Users/HeaderImage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HeaderImage(int id, UserHeaderImageViewModel userHeader)
        {
            if (id != userHeader.ID)
            {
                return NotFound();
            }

            User updateUser = _context.User.Find(userHeader.ID);

            if (updateUser == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var files = HttpContext.Request.Form.Files;
                    foreach (var Image in files)
                    {
                        if (Image != null && Image.Length > 0)
                        {
                            var file = Image;
                            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "images/users/profile");

                            if (file.Length > 0)
                            {
                                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                    updateUser.HeaderImagePath = $"/images/users/profile/{fileName}";

                                }
                            }
                        }
                    }
                    _context.Update(updateUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(userHeader.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = updateUser.ID });
            }
            return View(userHeader);
        }

        //GET: Users/ProfileImage/5
        public async Task<IActionResult> ProfileImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.SingleOrDefaultAsync(m => m.ID == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(new UserProfileImageViewModel { ID = user.ID, ProfileImagePath = user.ProfileImagePath });
        }

        //POST: Users/ProfileImage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProfileImage(int id, UserProfileImageViewModel userProfile)
        {
            if (id != userProfile.ID)
            {
                return NotFound();
            }

            User userUpdate = _context.User.Find(userProfile.ID);

            if (userUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var files = HttpContext.Request.Form.Files;
                    foreach (var Image in files)
                    {
                        if (Image != null && Image.Length > 0)
                        {
                            var file = Image;
                            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "images/users/profile");

                            if (file.Length > 0)
                            {
                                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                    userUpdate.ProfileImagePath = $"/images/users/profile/{fileName}";
                                }
                            }
                        }
                    }
                    _context.Update(userUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(userUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = userUpdate.ID });
            }
            return View(userProfile);
        }


        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.SingleOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,UserName")] User user)
        {
            if (id != user.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var files = HttpContext.Request.Form.Files;
                    foreach (var Image in files)
                    {
                        if (Image != null && Image.Length > 0)
                        {
                            var file = Image;
                            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "images/users/profile");

                            if (file.Length > 0)
                            {
                                var fileName = Guid.NewGuid().ToString() +
                                    System.IO.Path.GetExtension(file.FileName);

                                System.Console.WriteLine(fileName);
                                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                    user.ProfileImagePath = $"/images/users/profile/{fileName}";
                                }


                            }
                        }
                    }


                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = user.ID });
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .SingleOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.SingleOrDefaultAsync(m => m.ID == id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.ID == id);
        }
    }
}
