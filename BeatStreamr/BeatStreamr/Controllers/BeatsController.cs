using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeatStreamr.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace BeatStreamr.Controllers
{
    public class BeatsController : Controller
    {
        private readonly BeatStreamrContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        public BeatsController(BeatStreamrContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Beats
        public async Task<IActionResult> Index()
        {
            var beatsList = from b in _context.Beat
                            join u in _context.User
                            on b.ArtistID equals u.ID
                            select new BeatArtistViewModel
                            {
                                Name = u.Name,
                                ProfileImagePath = u.ProfileImagePath,
                                ID = b.ID,
                                Title = b.Title,
                                BPM = b.BPM,
                                Key = b.Key,
                                ArtistID = b.ArtistID,
                                Price = b.Price,
                                AlbumID = b.AlbumID,
                                ArtFilePath = b.ArtFilePath == null ? "/images/songs/blank-art.png" : b.ArtFilePath,
                                BeatFilePath = b.BeatFilePath

                            };

            //var beatArtistViewModel = new BeatArtistViewModel

            return View(await beatsList.ToListAsync());
        }

        // GET: Beats/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beat = GetSingleBeatArtistViewModel(id);


            if (beat == null)
            {
                return NotFound();
            }

            return View(beat);
        }

        public BeatArtistViewModel GetSingleBeatArtistViewModel(int? id)
        {
            var beat = from b in _context.Beat
                       join u in _context.User
                       on b.ArtistID equals u.ID
                       where b.ID == id
                       select new BeatArtistViewModel
                       {
                           Name = u.Name,
                           ProfileImagePath = u.ProfileImagePath,
                           ID = b.ID,
                           Title = b.Title,
                           BPM = b.BPM,
                           Key = b.Key,
                           ArtistID = b.ArtistID,
                           Price = b.Price,
                           AlbumID = b.AlbumID,
                           BeatFilePath = b.BeatFilePath

                       };

            return beat.FirstOrDefault();
        }

        // GET: Beats/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Beats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,BPM,Key,Price")] Beat beat)
        {
            if (ModelState.IsValid)
            {
                var imgExtensions = new[] { ".png", ".jpg", ".gif" };
                var musicExtensions = new[] { ".mp3", ".wav", ".mp4" };
                var files = HttpContext.Request.Form.Files;
                foreach (var BeatFile in files)
                {
                    if (BeatFile != null && BeatFile.Length > 0)
                    {
                        var file = BeatFile;

                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() +
                                System.IO.Path.GetExtension(file.FileName);

                            var uploads = "";
                            if (imgExtensions.Contains(Path.GetExtension(file.FileName)))
                            {
                                uploads = Path.Combine(_hostingEnvironment.WebRootPath, "images/songs/");
                                System.Console.WriteLine(fileName);
                                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                    beat.ArtFilePath = $"/images/songs/{fileName}";
                                }
                            }
                            else if (musicExtensions.Contains(Path.GetExtension(file.FileName)))
                            {
                                uploads = Path.Combine(_hostingEnvironment.WebRootPath, "media/beats/");
                                System.Console.WriteLine(fileName);
                                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                    beat.BeatFilePath = $"/media/beats/{fileName}";
                                }
                            }
                        }
                    }
                }
                beat.ArtistID = new Random().Next(1, 5);
                _context.Add(beat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = beat.ID });
            }
            return View(beat);
        }

        //GET: Beats/BeatArt/5
        public async Task<IActionResult> BeatArt(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beat = await _context.Beat.SingleOrDefaultAsync(m => m.ID == id);

            if (beat == null)
            {
                return NotFound();
            }

            return View(new BeatArtFilePathViewModel { ID = beat.ID, ArtFilePath = beat.ArtFilePath });
        }

        //POST: Beats/BeatArt/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BeatArt(int id, BeatArtFilePathViewModel beatArt)
        {
            if (id != beatArt.ID)
            {
                return NotFound();
            }

            Beat beatUpdate = _context.Beat.Find(beatArt.ID);

            if (beatUpdate == null)
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
                            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "images/songs");

                            if (file.Length > 0)
                            {
                                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                    beatUpdate.ArtFilePath = $"/images/songs/{fileName}";
                                }
                            }
                        }
                    }
                    _context.Update(beatUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BeatExists(beatUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = beatUpdate.ID });
            }
            return View(beatArt);
        }

        // GET: Beats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beat = await _context.Beat.SingleOrDefaultAsync(m => m.ID == id);
            if (beat == null)
            {
                return NotFound();
            }
            return View(beat);
        }

        // POST: Beats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,BPM,Key,Price")] Beat beat)
        {
            if (id != beat.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(beat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BeatExists(beat.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(beat);
        }

        // GET: Beats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beat = await _context.Beat
                .SingleOrDefaultAsync(m => m.ID == id);
            if (beat == null)
            {
                return NotFound();
            }

            return View(beat);
        }

        // POST: Beats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var beat = await _context.Beat.SingleOrDefaultAsync(m => m.ID == id);
            _context.Beat.Remove(beat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BeatExists(int id)
        {
            return _context.Beat.Any(e => e.ID == id);
        }
    }
}
