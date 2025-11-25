namespace Lacalizer.Mobile.Entites.Helpers.Notifications;
public static class EmailTemplateHelper
{
    public static string LoadAndReplaceTemplate(string templatePath, Dictionary<string, string> replacements)
    {
        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Template not found at path: {templatePath}");

        string template = File.ReadAllText(templatePath);

        foreach (var item in replacements)
        {
            template = template.Replace(item.Key, item.Value);
        }

        return template;
    }

    public static string ReplaceTemplate(string template, Dictionary<string, string> replacements)
    {
        foreach (var item in replacements)
        {
            template = template.Replace(item.Key, item.Value);
        }

        return template;
    }
}