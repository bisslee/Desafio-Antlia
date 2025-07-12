using ManualMovements.Domain.Resources;
using System.Globalization;

namespace ManualMovements.Application.Helpers
{
    public static class ResourceHelper
    {
        public static string GetResource(string key, string inCaseBlank)
        {
            var resource = ResourceApp.ResourceManager.GetString(
                           key,
                           CultureInfo.CurrentCulture)
                           ?? inCaseBlank;

            return resource;
        }
    }
}
