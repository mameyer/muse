using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.Linq;
using TSGenerator.Models;

namespace TSGenerator
{
    public class TSCleanTask : Task
    {
        /// <summary>
        /// The file path of the compilerconfig.json file
        /// </summary>
        public string FileName { get; set; }

        protected MessageImportance LoggingImportance { get; } = MessageImportance.High;  //If its not high then there are no logs

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, $"{Environment.NewLine}TSClean: Start..");

            FileInfo configFile = new(FileName);

            if (!configFile.Exists)
            {
                Log.LogWarning(configFile.FullName + " does not exist");
                return true;
            }

            Log.LogMessage(LoggingImportance, $"TSClean: Begin processing {configFile.Name}");

            void WriteIdent(int ident, string data) => Log.LogMessage(LoggingImportance, $"{new string(' ', ident)}{data}");

            bool DeleteTS(string path, int depth)
            {
                if (string.IsNullOrEmpty(path)) return true;

                var directory = new DirectoryInfo(path);
                if (!directory.Exists) return true;

                bool emptyFolder = true;
                foreach (DirectoryInfo dir in directory.EnumerateDirectories())
                {
                    WriteIdent(depth, $"handle sub directory {dir.FullName}");
                    emptyFolder &= DeleteTS(dir.FullName, depth + 1);
                }

                var files = directory.GetFiles();
                if (files.Any())
                {
                    foreach (FileInfo file in files)
                    {
                        if (file.Extension != ".ts")
                        {
                            continue;
                        }

                        WriteIdent(depth, $"delete file {file.Name}");
                        file.Delete();
                    }
                }
                else
                {
                    WriteIdent(depth, $"no files found in directory {directory.FullName}");
                }

                if (emptyFolder && directory.GetFiles().Length == 0)
                {
                    WriteIdent(depth, $"delete directory {directory.FullName}");
                    directory.Delete();
                    return true;
                }

                return false;
            }

            var config = new Configuration(configFile);
            if (config != null
                && config.Any())
            {
                var dir = Directory.GetCurrentDirectory();

                foreach (var item in config)
                {
                    if (string.IsNullOrEmpty(item.Target?.Path)) continue;

                    var localdir = System.IO.Path.Combine(dir, item.Target.Path);

                    Log.LogMessage(LoggingImportance, $"clean directory {localdir}");
                    DeleteTS(localdir, 1);
                }
            }

            Log.LogMessage(MessageImportance.High, $"TSClean: Finished{Environment.NewLine}");
            return true;
        }
    }
}
