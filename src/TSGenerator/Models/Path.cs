namespace TSGenerator.Models
{
    public class Path
    {
        private readonly string _path;

        public Path(string path)
        {
            this._path = path?.Replace("\\", $"{DirectorySeparatorChar}");
        }

        public static char DirectorySeparatorChar => System.IO.Path.DirectorySeparatorChar;

        public override string ToString() => _path;
    }
}
