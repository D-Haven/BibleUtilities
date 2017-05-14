using System.Reflection;
using System.Resources;

namespace DHaven.BibleUtilities
{
    internal static class Resources
    {
        static Resources()
        {
            var assembly = typeof(Resources).GetTypeInfo().Assembly;
            Books = new ResourceManager("DHaven.BibleUtilities.Resources.Books", assembly);
            CommonMistakes = new ResourceManager("DHaven.BibleUtilities.Resources.CommonMistakes", assembly);
            StandardAbbreviations = new ResourceManager("DHaven.BibleUtilities.Resources.StandardAbbreviations",
                assembly);
            ThompsonAbbreviations = new ResourceManager("DHaven.BibleUtilities.Resources.ThompsonAbbreviations",
                assembly);
        }

        public static ResourceManager Books { get; }
        public static ResourceManager CommonMistakes { get; }
        public static ResourceManager StandardAbbreviations { get; }
        public static ResourceManager ThompsonAbbreviations { get; }
    }
}