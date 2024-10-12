using System;
using System.Collections.Generic;
using BCrypt.Net;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Task.Models;
using Task;

public class SettingsController : Controller
{
    private readonly TaskManagementDBContext db;

    public SettingsController()
    {
        db = new TaskManagementDBContext();
    }

    // GET: Settings/Edit
    public ActionResult Edit()
    {
        var settings = db.SystemSettings.FirstOrDefault() ?? new SystemSettings();
        return View(settings);
    }

    // POST: Settings/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(SystemSettings settings)
    {
        if (ModelState.IsValid)
        {
            if (settings.Id == 0)
            {
                db.SystemSettings.Add(settings);
            }
            else
            {
                db.Entry(settings).State = EntityState.Modified;
            }
            db.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }
        return View(settings);
    }
}
