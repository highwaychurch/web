using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Creative.Web.Models;
using Raven.Client;
using Highway.Shared.Mvc.FlashMessages;
using Creative.Web.Views.Resources;

namespace Creative.Web.Controllers.Admin
{
    public class ResourcesController : Controller
    {
        private readonly IDocumentSession _session;

        public ResourcesController(IDocumentSession session)
        {
            _session = session;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var songs = _session.Query<Song>().Take(20).ToArray();
            var viewModel = new IndexViewModel { Songs = songs };
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult SongDetails(string id)
        {
            var song = _session.Load<Song>(id);
            if (song == null)
                return new HttpNotFoundResult();

            return View(song);
        }

        [HttpGet]
        public ActionResult CreateSong()
        {
            return View(new Song());
        }

        [HttpPost]
        public ActionResult CreateSong(Song song)
        {
            if (ModelState.IsValid == false)
            {
                TempData.FlashError("Whoops!", "There seems to be some problems with the information you provided. Please correct the errors and try again.");
                return View(song);
            }

            _session.Store(song);
            TempData.FlashSuccess(song.Title + " created", "The new song resource was added successfully.");
            return RedirectToAction("SongDetails", new { id = song.Id });
        }

        [HttpGet]
        public ActionResult EditSong(string id)
        {
            var song = _session.Load<Song>(id);
            if (song == null)
                return new HttpNotFoundResult();

            return View(song);
        }

        [HttpPost]
        public ActionResult EditSong(Song song)
        {
            if (ModelState.IsValid == false)
            {
                TempData.FlashError("Whoops!", "There seems to be some problems with the information you provided. Please correct the errors and try again.");
                return View(song);
            }

            var songToUpdate = _session.Load<Song>(song.Id);
            if (songToUpdate == null)
                return new HttpNotFoundResult();

            songToUpdate.Title = song.Title;
            songToUpdate.Artist = song.Artist;
            songToUpdate.Album = song.Album;

            _session.SaveChanges();
            TempData.FlashSuccess("Changes saved to " + song.Title, "The song has been updated with your changes.");

            return RedirectToAction("SongDetails", new {id = song.Id});
        }

        public ActionResult DeactivateSong(string id)
        {
            var song = _session.Load<Song>(id);
            if (song == null)
                return new HttpNotFoundResult();

            song.IsActive = false;
            _session.SaveChanges();

            return RedirectToAction("SongDetails", new {id = song.Id});
        }

        public ActionResult ReactivateSong(string id)
        {
            var song = _session.Load<Song>(id);
            if (song == null)
                return new HttpNotFoundResult();

            song.IsActive = true;
            _session.SaveChanges();

            TempData.FlashSuccess(song.Title + " has been reactivated.");

            return RedirectToAction("SongDetails", new {id = song.Id});
        }
    }
}
