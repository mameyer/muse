using System.IO;
using System.Linq;

namespace TSGenerator.Models
{
    public abstract class ConfigurationBaseItem
    {
        private Path _path;

        public string Path
        {
            get => this._path?.ToString();
            set => this._path = new Path(value);
        }

        public override string ToString()
            => $"{this.Path}";

        public string[] GetWorkingDirectories()
        {
            var dir = Directory.GetCurrentDirectory();

            if (string.IsNullOrEmpty(this.Path))
            {
                return new string[] { dir };
            }

            return new string[] { this.Path };
        }

        public static string CalculateLeastCommonParentFolder(string[] dirs)
        {
            if (dirs.Length == 1)
            {
                return dirs[0];
            }

            string commonParentDir = "";

            string referenceDir = dirs[0];
            int index = 0;

            // Compute least common substring
            while (dirs.Aggregate(true, (same, next) => same && next.StartsWith(referenceDir[..index])))
            {
                index++;
            }

            if (index == 0)
            {
                return null;
            }

            commonParentDir = referenceDir[..(index - 1)];

            // Least common substring may contain partial folder names. Trim these off.
            if (!commonParentDir.EndsWith($"{Models.Path.DirectorySeparatorChar}"))
            {
                commonParentDir = commonParentDir[..(commonParentDir.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1)];
            }

            return commonParentDir;
        }
    }
}
