using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TSGenerator.Models;

namespace TSGenerator.Modules
{
    public class ModelLoader : ModuleBase
    {
        public ModelLoader(BuildConfiguration configurationItem, string environment, LogAction log)
            : base(log)
        {
            this.Enviroment = environment;
            this.Models = new ModelFileCollection();
            this.Load(configurationItem);
        }

        public string Enviroment { get; }
        public ModelFileCollection Models { get; }

        private void Load(BuildConfiguration buildConfiguration)
        {
            var dirs = buildConfiguration.Source.GetWorkingDirectories();
            var commonParentDir = ConfigurationBaseItem.CalculateLeastCommonParentFolder(dirs);

            foreach (var dir in dirs)
            {
                var directory = new DirectoryInfo(dir);
                if (!directory.Exists)
                {
                    this.Log($"directory {dir} does not exist..");
                    continue;
                }

                this.Log($"loading model from directory {directory.FullName}..");
                this.Load(directory.FullName, buildConfiguration.Source.Project);
            }
        }

        void Load(string project, string currentDirectory, string rootDirectory)
        {
            if (currentDirectory.Contains(project))
            {
                BinDirFinder binDirFinder = new(currentDirectory, project, this.Log);

                string[] dllPaths = Array.Empty<string>();
                if (binDirFinder.Success)
                {
                    Log($"found bin dir {binDirFinder.Path}");

                    DLLFinder dLLFinder = new(binDirFinder.Path, project, this.Log);
                    if (dLLFinder.Success)
                    {
                        dllPaths = dLLFinder.DLLPaths;
                        Log($"found dlls: {string.Join(", ", dllPaths)}");
                    }
                }

                var files = Directory
                    .GetFiles(currentDirectory)
                    .Where(e => e.EndsWith(".cs"));
                var workingUri = new Uri(EnsureTrailingSlash(currentDirectory));
                var rootUri = new Uri(rootDirectory);
                var relativePath = rootUri.MakeRelativeUri(workingUri).OriginalString;

                foreach (var file in files)
                {
                    string namespaceFromFile = null;
                    foreach (string line in File.ReadLines(file))
                    {
                        var extractResult = ExtractNamespace(line);
                        if (extractResult.Item1)
                        {
                            namespaceFromFile = extractResult.Item2;
                            break;
                        }
                    }

                    string[] inheritanceFromFile = Array.Empty<string>();
                    foreach (string line in File.ReadLines(file))
                    {
                        var extractResult = ExtractInheritance(line);
                        if (extractResult.Item1)
                        {
                            inheritanceFromFile = extractResult.Item2;
                            break;
                        }
                    }

                    var fileName = System.IO.Path.GetFileName(file);
                    fileName = fileName.Replace(".cs", String.Empty);

                    foreach (string line in File.ReadLines(file))
                    {
                        var isGeneric = ExtractGeneric(line, fileName);
                        if (isGeneric != null)
                        {
                            fileName += isGeneric;
                            break;
                        }
                    }
                    this.Models.Add(new ModelFile(fileName, project, namespaceFromFile, inheritanceFromFile, new Models.Path(relativePath), dllPaths));
                }

                return;
            }

            var dirs = Directory.GetDirectories(currentDirectory);
            foreach (var dir in dirs)
            {
                try
                {
                    Load(project, dir, rootDirectory);
                }
                catch { }
            }
        }

        private void Load(string directory, string project)
        {
            Log($"loading model: {directory} [{ project }]");

            Load(project, directory, directory);

            TypeLoader typeLoader = new(this.Models, this.Log, project);
            foreach (var typeDefinition in typeLoader.TypeDefinitions)
            {
                typeDefinition.Key.Type = typeDefinition.Value;
            }

            foreach (var model in this.Models)
            {
                this.Log($" -- found model {model.Name}: type={model.Type?.FullName}");
            }
        }

        private static bool StrictContains(string str, string match)
        {
            string reg = "(^|\\s)" + match + "(\\s|$)";
            return Regex.IsMatch(str, reg);
        }

        private static string[] ExplodeLine(string line)
        {
            var regex = new Regex("\\s*,\\s*");

            var l = regex.Replace(line, ",");

            return l
                .Replace("public", String.Empty)
                .Trim()
                .Split(' ');
        }

        public static (bool, string) ExtractNamespace(string line)
        {
            if (StrictContains(line, "namespace"))
            {
                line = line.Replace(";", String.Empty);
                line = line.Replace("{", String.Empty);
                var modLine = new List<string>(ExplodeLine(line));




                return (true, modLine[^1]);
            }

            return (false, line);
        }

        public static string ExtractGeneric(string line, string className)
        {
            line = line.Split(':')[0];
            if (StrictContains(line, "public") && line.Contains(className))
            {
                if (StrictContains(line, "public") && line.Contains(className + "<") && line.Contains('>'))
                {
                    var split = line.Split(',');
                    return '`' + split.Length.ToString();
                }
                return string.Empty;
            }
            return null;
        }

        public static (bool, string[]) ExtractInheritance(string line)
        {
            if ((StrictContains(line, "class") && line.Contains(':')))
            {
                var split = line.Split(':');
                if (split.Length < 2) return (false, Array.Empty<string>());

                var inheritance = split[1];
                inheritance = Regex.Replace(inheritance, @"\s+", "");
                return (true, inheritance.Split(","));
            }

            return (false, Array.Empty<string>());
        }

        private static string EnsureTrailingSlash(string str)
        {
            if (!str.EndsWith(System.IO.Path.DirectorySeparatorChar))
            {
                str += System.IO.Path.DirectorySeparatorChar;
            }
            return str;
        }
    }
}