using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LabProject.Helpers;
using Microsoft.EntityFrameworkCore;
using LabProject.Data;


namespace LabProject.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        
        [BindProperty(SupportsGet = true)]
        public string? SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinStudents { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        private const int PageSize = 10;

        public ClassInformationTable Table { get; set; } = new();

        [BindProperty]
        public List<string> SelectedColumns { get; set; } = new();

        
        
        // In-memory list of class data
        public static List<ClassInformationModel> Classes { get; set; } = new();

        [BindProperty]
        public ClassInformationModel FormModel { get; set; } = new();

        [BindProperty]
        public int Id { get; set; }

        public bool IsEdit { get; set; }

        public async Task OnGetAsync()
        {
            if (!IsAuthenticated())
            {
                Response.Redirect("/Login");
                return;
            }

            var query = _context.Classes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchName))
                query = query.Where(c => c.Name.Contains(SearchName));

            if (MinStudents.HasValue)
                query = query.Where(c => c.PersonCount >= MinStudents.Value);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            var pageData = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            Table = new ClassInformationTable
            {
                Items = pageData.Select(c => new ClassInformationModel
                {
                    Id = c.Id,
                    ClassName = c.Name,
                    StudentCount = c.PersonCount,
                    Description = c.Description
                }),
                CurrentPage = PageNumber,
                TotalPages = totalPages,


                SearchName = SearchName,
                MinStudents = MinStudents
            };
        }



        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var newClass = new Class
            {
                Name = FormModel.ClassName,
                PersonCount = FormModel.StudentCount,
                Description = FormModel.Description,
                IsActive = true
            };

            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostEditAsync()
        {
            var existing = await _context.Classes.FindAsync(Id);
            if (existing != null)
            {
                FormModel = new ClassInformationModel
                {
                    Id = existing.Id,
                    ClassName = existing.Name,
                    StudentCount = existing.PersonCount,
                    Description = existing.Description
                };
                IsEdit = true;
            }

            return Page();
        }



        public async Task<IActionResult> OnPostUpdateAsync()
        {
            var existing = await _context.Classes.FindAsync(FormModel.Id);
            if (existing != null && ModelState.IsValid)
            {
                existing.Name = FormModel.ClassName;
                existing.PersonCount = FormModel.StudentCount;
                existing.Description = FormModel.Description;

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var toDelete = await _context.Classes.FindAsync(Id);
            if (toDelete != null)
            {
                _context.Classes.Remove(toDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostExportAllAsync()
        {
            var allData = await _context.Classes.ToListAsync();

            var json = Utils.Instance.ExportToJson(allData.Select(c => new ClassInformationModel
            {
                Id = c.Id,
                ClassName = c.Name,
                StudentCount = c.PersonCount,
                Description = c.Description
            }));

            return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", "all_data.json");
        }

        
        public async Task<IActionResult> OnPostExportFilteredAsync()
        {
            var query = _context.Classes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchName))
                query = query.Where(c => c.Name.Contains(SearchName));

            if (MinStudents.HasValue)
                query = query.Where(c => c.PersonCount >= MinStudents.Value);

            var filteredData = await query.ToListAsync();

            var mapped = filteredData.Select(c => new ClassInformationModel
            {
                Id = c.Id,
                ClassName = c.Name,
                StudentCount = c.PersonCount,
                Description = c.Description
            });

            var json = Utils.Instance.ExportToJson(mapped, SelectedColumns);

            return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", "filtered_data.json");
        }


        private bool IsAuthenticated()
        {
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionId = HttpContext.Session.GetString("session_id");

            var cookieUsername = Request.Cookies["username"];
            var cookieToken = Request.Cookies["token"];
            var cookieSessionId = Request.Cookies["session_id"];

            return sessionUsername != null &&
                sessionUsername == cookieUsername &&
                sessionToken == cookieToken &&
                sessionId == cookieSessionId;
        }


    }
}
