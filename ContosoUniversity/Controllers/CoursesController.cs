using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace ContosoUniversity.Controllers
{
    public class CoursesController : Controller
    {
        private readonly SchoolContext _context;
        public CoursesController(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.Courses.Include(c => c.Department);
            return View(await schoolContext.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var course = await _context.Courses
                .Include(c => c.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Credits,DepartmentID,RowVersion")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "Name", course.DepartmentID);
            return View(course);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "Name", course.DepartmentID);
            return View(course);
        }
        public async Task<IActionResult> Edit(int? id, byte[] rowVersion)
        {
            ModelState.Remove("Department");
            ModelState.Remove("Enrollments");
            ModelState.Remove("CourseAssignments");
            ModelState.Remove("RowVersion");
            if (id == null)
            {
                return NotFound();
            }
            var courseToUpdate = await _context.Courses
                .Include(d => d.Department)
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (courseToUpdate == null)
            {
                Course deletedCourse = new Course();
                await TryUpdateModelAsync(deletedCourse);
                ModelState.AddModelError(string.Empty, "unable to save changes. The Course has been deleted by another user.");
                ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "Name", deletedCourse.DepartmentID);
                return View(deletedCourse);
            }

            _context.Entry(courseToUpdate)
                .Property("RowVersion")
                .OriginalValue = rowVersion;

            if (await TryUpdateModelAsync<Course>(
                courseToUpdate, "",
                s => s.Title,
                s => s.Credits,
                s => s.DepartmentID
                ))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Course)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, "unable to save changes. The course has been deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Course)databaseEntry.ToObject();
                        if (databaseValues.Title != clientValues.Title)
                        {
                            ModelState.AddModelError("Title", $"Current Value: {databaseValues.Title}");
                        }
                        if (databaseValues.Credits != clientValues.Credits)
                        {
                            ModelState.AddModelError("Credits", $"Current Value: {databaseValues.Credits}");
                        }
                        if (databaseValues.DepartmentID != clientValues.DepartmentID)
                        {
                            Department databaseDepartment = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentID == databaseValues.DepartmentID);
                            ModelState.AddModelError("DepartmentID", $"Current Value: {databaseValues.DepartmentID}");
                        }
                        ModelState.AddModelError(string.Empty, "The record that you have attempted to edit"
                            + "was modified by another user after you got the original value."
                            + "The editing operation was canceled and the current values in the database"
                            + "have been displayed. If you still require to edit this record. click"
                            + "the save button again. Otherwise click the Back to List hyperlink."
                            );
                        courseToUpdate.RowVersion = databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "Name", courseToUpdate.DepartmentID);
            return View(courseToUpdate);
        }
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record that you have attempted to delete"
                    + "was modified by another user after you got the original value."
                    + "The delete operation was canceled and the current values in the database"
                    + "have been displayed. If you still require to edit this record. click"
                    + "the delete button again. Otherwise clicck the Back to List hyperlink.";
            }
            return View(course);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Course course)
        {
            try
            {
                if (await _context.Courses.AnyAsync(m => m.CourseID == course.CourseID))
                {
                    _context.Courses.Remove(course);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return RedirectToAction(nameof(Delete), new { concurrencyError = true, id = course.CourseID });
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
