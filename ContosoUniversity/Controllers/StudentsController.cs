using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;


namespace ContosoUniversity.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            var result = await _context.Students.ToListAsync();
            return View(result);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var student = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentDate, FirstMidName,LastName")]
            Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException) 
            {
                ModelState.AddModelError("", "Unable to save changes" +
                    "Try again, and if the problem persists" +
                    "see your system administrator");
            }
            return View(student);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }
        [HttpPost, ActionName ("Edit")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var studentToUpdate = await _context.Students.FirstOrDefaultAsync(s => s.ID == id);
            if (await TryUpdateModelAsync<Student>(studentToUpdate,"",
                s=>s.FirstMidName,
                s => s.LastName,
                s => s.EnrollmentDate))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes" +
                    "Try again, and if the problem persists" +
                    "see your system administrator");
                }
            }
            return View(studentToUpdate);
        }
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }
            Student student = await getStudentById(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if problem persists" +
                    "see your system administrator";
            }
            return RedirectToAction(nameof(Index));
            //var student = await _context.Students
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(m => m.ID == id);
            //if (student == null)
            //{
            //    return NotFound();
            //}

            //if (saveChangesError.GetValueOrDefault())
            //{
            //    ViewData["ErrorMessage"] =
            //        "Delete failed. Try again, and if problem persists" +
            //        "see your system administrator";
            //}
            //return View(student);
        }
        private async Task<Student> getStudentById(int? id)
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(equals => equals.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
    }
}
