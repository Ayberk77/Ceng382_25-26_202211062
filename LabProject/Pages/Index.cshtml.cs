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
        // In-memory list of class data
        public static List<ClassInformationModel> Classes { get; set; } = new();

        [BindProperty]
        public ClassInformationModel FormModel { get; set; } = new();

        [BindProperty]
        public int Id { get; set; }

        public bool IsEdit { get; set; }

        public void OnGet()
        {
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
