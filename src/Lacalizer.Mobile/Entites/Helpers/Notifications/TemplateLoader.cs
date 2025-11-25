using Microsoft.Extensions.Options;

namespace Lacalizer.Mobile.Entites.Helpers.Notifications;
public static class TemplateLoader
{
    public static string LoadTemplate(string fileName)
    {
        var basePath = Path.Combine(AppContext.BaseDirectory, "Templates", "html");
        var fullPath = Path.Combine(basePath, fileName);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Template file not found: {fullPath}");
        }

        return File.ReadAllText(fullPath);
    }
}

