using System.IO;
using System.Linq;

namespace TSGenerator.Modules
{
    public class BinDirFinder : ModuleBase
    {
        public BinDirFinder(string path, string project = null, LogAction log = null)
            : base(log)
        {
            this.Success = false;
            this.Path = path;
            this.SearchBinDir(path, project);
        }

        public bool Success { get; internal set; }
        public string Path { get; internal set; }

        private void SearchBinDir(string path, string project = null)
        {
            if (!Directory.Exists(path))
            {
                this.Log($"dir {path} does not exist..");
                return;
            }

            bool IsBinDir(string dir)
                => dir.EndsWith("bin")
                    && (string.IsNullOrEmpty(project) || dir.Contains(project));


            this.Log($"searching bin dir for project={project}");
            for (int i = 0; ; i++)
            {
                this.Log($"{new string(' ', i)} {this.Path} ..");
                var dirs = Directory.GetDirectories(this.Path);
                if (dirs != null || dirs.Length > 0)
                {
                    var binDir = dirs.Where(e => IsBinDir(e)).FirstOrDefault();
                    if (binDir != null)
                    {
                        this.Success = true;
                        return;
                    }
                }

                try
                {
                    this.Path = Directory.GetParent(this.Path)?.FullName;
                }
                catch
                {
                    break;
                }
            }
        }
    }
}
