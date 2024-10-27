using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Xml.Linq;
using TSGenerator.Models;

namespace TSGenerator.Modules
{
    public class AssemblyLoader : ModuleBase, IDisposable
    {
        private bool disposedValue;

        public AssemblyLoader(DLLMapping dllMapping, LogAction log)
            : base(log)
        {
            this.Assemblies = new Dictionary<string, Assembly>();
            this.LoadAssemblies(dllMapping);
        }

        public Dictionary<string, Assembly> Assemblies { get; }
        private static bool IsCandidateLibrary(RuntimeLibrary library, AssemblyName assemblyName)
        {
            return (library.Name == (assemblyName.Name))
                    || (library.Dependencies.Any(d => d.Name.StartsWith(assemblyName.Name)));
        }
        private void LoadAssemblies(DLLMapping dllMapping)
        {
            this.Log("load assemblies:");
            foreach (var dll in dllMapping)
            {
                Assembly assembly = null;
                try
                {
                    // auto resolving
                    AssemblyLoadContext.Default.Resolving += (context, name) =>
                    {
                        if (name.Name.EndsWith("resources"))
                        {
                            return null;
                        }
                        var resolver = new AssemblyDependencyResolver(dll.Key);
                        var dependencies = DependencyContext.Default.RuntimeLibraries;
                        var inCompileLibraries = DependencyContext.Default.CompileLibraries;
                        foreach (var library in dependencies)
                        {
                            if (IsCandidateLibrary(library, name))
                            {
                                return context.LoadFromAssemblyName(new AssemblyName(library.Name));
                            }
                        }
                        string assemblyPath = resolver.ResolveAssemblyToPath(name);
                        if (assemblyPath != null)
                            return context.LoadFromAssemblyPath(assemblyPath);

                        return null;
                    };

                    assembly = GetAssembly(dll.Key);
                    this.Assemblies[dll.Key] = assembly;
                    
                    Log($" -- {dll.Key}: successfully loaded assembly {assembly.FullName}");
                }
                catch (System.IO.FileNotFoundException ex)
                {
                    this.Log($" !! {dll.Key}: failed to load assembly => {ex.Message}");
                }
            }
        }

        private static Assembly GetAssembly(string path)
            => AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Assemblies.Clear();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
