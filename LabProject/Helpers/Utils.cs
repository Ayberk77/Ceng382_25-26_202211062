using System.Text.Json;

namespace LabProject.Helpers
{
    public sealed class Utils
    {
        private static readonly Lazy<Utils> _instance = new(() => new Utils());
        public static Utils Instance => _instance.Value;

        private Utils() { }

        public string ExportToJson<T>(IEnumerable<T> data, IEnumerable<string>? selectedProperties = null)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            if (selectedProperties == null || !selectedProperties.Any())
                return JsonSerializer.Serialize(data, options);

            var result = data.Select(item =>
            {
                var obj = new Dictionary<string, object?>();
                foreach (var prop in item!.GetType().GetProperties())
                {
                    if (selectedProperties.Contains(prop.Name))
                    {
                        obj[prop.Name] = prop.GetValue(item);
                    }
                }
                return obj;
            });

            return JsonSerializer.Serialize(result, options);
        }
    }
}
