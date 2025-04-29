namespace LabProject.Models
{
    public class ClassInformationTable
    {
        public IEnumerable<ClassInformationModel> Items { get; set; } = Enumerable.Empty<ClassInformationModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        // Filtre alanları (gerek duyduklarını ekle)
        public string? SearchName { get; set; }
        public int? MinStudents { get; set; }
    }
}
