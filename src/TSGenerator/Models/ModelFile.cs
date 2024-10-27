using System;
using System.Collections.Generic;
using System.Linq;
using TSGenerator.Extensions;

namespace TSGenerator.Models
{
    public class ModelFile
    {
        private readonly string _namespace;

        public ModelFile(string fileName, string project, string namespaceFromFile, string[] inheritance, Path relativePath, string[] dllPaths)
        {
            this.Name = fileName.ToPascalCase();
            this.Project = project;
            this._namespace = namespaceFromFile;
            this.Inheritance = inheritance;
            this.RelativePath = relativePath;
            this.DllPaths = dllPaths;
        }

        public string Name { get; }
        public string Project { get; }
        public string[] Inheritance { get; }

        public Path RelativePath { get; }

        public string[] DllPaths { get; }

        public bool IsEnum
            => this.Type?.IsEnum ?? false;

        public bool IsGenericType
            => this.Type?.IsGenericType ?? false;

        public string Extension
            => $"{(IsEnum ? "" : ".d")}.ts";

        public ModelFileDefinition[] Dlls
            => this.DllPaths?
            .Select(e => new ModelFileDefinition
            {
                DllPath = e,
                ModelFile = this,
            })
            .ToArray();

        public string Namespace
            => this.Type?.Namespace ?? this._namespace;

        public Type Type { get; set; }

        public Type[] GetInheritance(ModelFile[] allTypes)
        {
            List<Type> inheritance = new();
            if (this.Type == null
                || allTypes == null) return inheritance.ToArray();

            var types = allTypes
                .Where(e => e.Type != this.Type
                    && e.Type != null)
                .ToArray();
            var ifaces = this.Type.GetInterfaces();

            foreach (var def in types)
            {
                var t = def.Type;
                if (this.Type.IsSubclassOf(t)
                    || (ifaces.Any() && ifaces.Contains(t))
                    || (t.IsGenericType
                        && (ifaces?.Select(e => e.ToGenericTypeName())?.Any(e => e == t.ToGenericTypeName()) ?? false)))
                {
                    inheritance.Add(t);
                }
            }

            return inheritance.ToArray();
        }
    }
}