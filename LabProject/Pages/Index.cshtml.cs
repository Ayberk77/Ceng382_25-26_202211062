using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LabProject.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinStudents { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        private const int PageSize = 10;

        public ClassInformationTable Table { get; set; } = new();

        
        
        // In-memory list of class data
        public static List<ClassInformationModel> Classes { get; set; } = new();

        [BindProperty]
        public ClassInformationModel FormModel { get; set; } = new();

        [BindProperty]
        public int Id { get; set; }

        public bool IsEdit { get; set; }

        public void OnGet()
        {
            // 1) Test verisi üret (bir kerelik)
            if (!Classes.Any())
            {
                var rnd = new Random();
                for (int i = 1; i <= 100; i++)
                {
                    Classes.Add(new ClassInformationModel
                    {
                        Id = i,
                        ClassName = $"Class {i}",
                        StudentCount = rnd.Next(10, 45),
                        Description = $"Sample description {i}"
                    });
                }
            }

            // 2) Filtrele
            var query = Classes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchName))
                query = query.Where(c => c.ClassName.Contains(SearchName, StringComparison.OrdinalIgnoreCase));

            if (MinStudents.HasValue)
                query = query.Where(c => c.StudentCount >= MinStudents.Value);

            // 3) Sayfalama
            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            var pageData = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // 4) Görünüme gönderilecek tablo modeli
            Table = new ClassInformationTable
            {
                Items = pageData,
                CurrentPage = PageNumber,
                TotalPages = totalPages,
                SearchName = SearchName,
                MinStudents = MinStudents
            };
        }


        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
                return Page();

            Classes.Add(FormModel);
            FormModel = new(); // reset form
            return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
            var existing = Classes.FirstOrDefault(c => c.Id == Id);
            if (existing != null)
            {
                FormModel = new ClassInformationModel
                {
                    Id = existing.Id,
                    ClassName = existing.ClassName ?? "", 
                    StudentCount = existing.StudentCount,
                    Description = existing.Description ?? "" 
                };
                IsEdit = true;  // Ensure IsEdit is being set
            }

            return Page();
        }


        public IActionResult OnPostUpdate()
        {
            var existing = Classes.FirstOrDefault(c => c.Id == FormModel.Id);
            if (existing != null && ModelState.IsValid)
            {
                existing.ClassName = FormModel.ClassName;
                existing.StudentCount = FormModel.StudentCount;
                existing.Description = FormModel.Description;
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete()
        {
            var toDelete = Classes.FirstOrDefault(c => c.Id == Id);
            if (toDelete != null)
                Classes.Remove(toDelete);

            return RedirectToPage();
        }
    }
}
