using System.Drawing;
using System.Reflection;

namespace SyncVerseStudio.Helpers
{
    public static class IconHelper
    {
        public static Icon? GetApplicationIcon()
        {
            try
            {
 // First try to load from embedded resource
    var assembly = Assembly.GetExecutingAssembly();
     var resourceName = "syncversestudio.logo.png";
 
      using (var stream = assembly.GetManifestResourceStream(resourceName))
      {
  if (stream != null)
  {
          using (var bitmap = new Bitmap(stream))
  {
 return Icon.FromHandle(bitmap.GetHicon());
    }
    }
    }
     
    // Fallback: try to load from file system
                string iconPath = Path.Combine(Application.StartupPath, "logo.png");
             if (File.Exists(iconPath))
                {
   using (var bitmap = new Bitmap(iconPath))
             {
      return Icon.FromHandle(bitmap.GetHicon());
      }
       }
            }
         catch (Exception ex)
            {
            // Log error but don't throw - use default icon instead
         Console.WriteLine($"Could not load application icon: {ex.Message}");
            }
            
         return null;
        }
    
        public static void SetFormIcon(Form form)
      {
  var icon = GetApplicationIcon();
            if (icon != null)
       {
   form.Icon = icon;
            }
        }
    }
}