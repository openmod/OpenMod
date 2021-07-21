using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using OpenMod.API;
using OpenMod.Common.Hotloading;
using Serilog;

namespace OpenMod.Core.Helpers
{
    [OpenModInternal]
    public static class AssemblyHelper
    {
        private static readonly string[] s_ExcludedResources = new[]
        {
            "packages.yaml"
        };

        public static void CopyAssemblyResources(Assembly assembly, string? baseDir, bool overwrite = false)
        {
            baseDir ??= string.Empty;

            var resourceNames = assembly.GetManifestResourceNames();

            if (resourceNames.Length > 0 && !Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }

            foreach (var resourceName in resourceNames)
            {
                if (resourceName.EndsWith("..directory"))
                {
                    continue;
                }

                var assemblyName = Hotloader.GetRealAssemblyName(assembly);

                if (!resourceName.Contains(assemblyName.Name + "."))
                {
                    Log.Warning(
                        "{ResourceName} does not contain assembly name in assembly: {AssemblyName}. <AssemblyName> and <RootNamespace> must be equal inside your plugins .csproj file",
                        resourceName, assemblyName.Name);
                }

                var regex = new Regex(Regex.Escape(assemblyName.Name + "."));
                var fileName = regex.Replace(resourceName, string.Empty, 1);

                if (s_ExcludedResources.Contains(fileName))
                {
                    continue;
                }

                var parts = fileName.Split('.');
                fileName = "";

                var pathSb = new StringBuilder(assemblyName.Name + ".");
                foreach (var part in parts)
                {
                    pathSb.Append(part + ".");
                    using var tmpStream = assembly.GetManifestResourceStream(pathSb + ".directory");

                    var isDirectory = tmpStream != null;
                    if (isDirectory)
                    {
                        fileName += part + Path.DirectorySeparatorChar;
                    }
                    else
                    {
                        fileName += part + ".";
                    }
                }

                if (fileName.EndsWith("."))
                {
                    fileName = fileName.Substring(0, fileName.Length - 1);
                }

                var directory = Path.GetDirectoryName(fileName);

                if (directory != null)
                {
                    directory = Path.Combine(baseDir, directory);
                }

                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var filePath = Path.Combine(baseDir, fileName);
                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream ??
                                                    throw new MissingManifestResourceException(
                                                        $"Couldn't find resource: {resourceName}"));
                var fileContent = reader.ReadToEnd();

                if (File.Exists(filePath) && !overwrite)
                {
                    continue;
                }

                File.WriteAllText(filePath, fileContent);
            }
        }
    }
}