using System.IO;
using System.Linq;

namespace TSGenerator.Modules
{
    public class DLLFinder : ModuleBase
    {
        const string dllSuffix = ".dll";

        public DLLFinder(string directory, string project = null, LogAction log = null)
            : base(log)
        {
            this.Success = false;
            this.DLLPaths = System.Array.Empty<string>();
            this.SearchDlls(directory, project);
        }

        public bool Success { get; internal set; }
        public string[] DLLPaths { get; internal set; }

        private void SearchDlls(string directory, string project)
        {
            if (string.IsNullOrEmpty(project))
            {
                this.Log($"search dlls: no project name set for working directory {directory}..");
                return;
            }

            (bool, string[]) search(string dir, int depth)
            {
                this.Log($"{new string(' ', depth)} {dir} ..");

                if (!Directory.Exists(dir))
                {
                    this.Log($"directory {dir} does not exist..");
                    return (false, System.Array.Empty<string>());
                }

                var files = Directory.GetFiles(dir);
                if (files != null
                    || files.Length > 0)
                {
                    var dlls = files
                        .Where(e => e.EndsWith(dllSuffix))
                        .ToArray();
                    if (dlls.Any())
                    {
                        dlls = dlls
                            .Where(e => e.EndsWith(project + dllSuffix))
                            .ToArray();
                    }

                    if (dlls != null
                        && dlls.Any())
                    {
                        return (true, dlls);
                    }
                }

                var subDirs = Directory.GetDirectories(dir);
                if (subDirs == null
                    || subDirs.Length == 0)
                {
                    return (false, System.Array.Empty<string>());
                }

                foreach (var subDir in subDirs)
                {
                    var result = search(subDir, depth + 1);
                    if (result.Item1) return result;
                }

                return (false, System.Array.Empty<string>());
            }

            this.Log($"search dlls in directory {directory} for project {project}..");

            var result = search(directory, 0);
            this.Success = result.Item1;
            this.DLLPaths = result.Item2;
        }
    }
}
