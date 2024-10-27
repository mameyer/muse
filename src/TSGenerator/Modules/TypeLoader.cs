using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TSGenerator.Extensions;
using TSGenerator.Models;

namespace TSGenerator.Modules
{
    public class TypeLoader : ModuleBase
    {
        public TypeLoader(ModelFileCollection modelFiles, LogAction log, string project)
            : base(log)
        {
            this.Log = log;
            this.TypeDefinitions = new Dictionary<ModelFile, Type>();
            if (!this.LoadTypes(modelFiles, project))
            {
                throw new Exception("failed to load types..");
            }
        }

        public Dictionary<ModelFile, Type> TypeDefinitions { get; }

        private bool LoadTypes(ModelFileCollection modelFiles, string project)
        {
            var dllMapping = modelFiles.GetDLLMapping();
            using AssemblyLoader assemblyLoader = new(dllMapping, this.Log);
            foreach (var assembly in assemblyLoader.Assemblies)
            {
                this.Log($"get types for assembly {assembly.Value.FullName}");
                try
                {
                    foreach (var model in modelFiles)
                    {
                        var typeName = model.Namespace + "." + model.Name;
                        try
                        {
                            var type = assembly.Value.GetType(typeName, true, true);
                            this.TypeDefinitions[model] = type;
                            this.Log($" => found type: " + typeName);
                        }
                        catch (Exception e)
                        {
                            this.Log($"failed to get types: " + typeName, Enums.LogMessageType.Warning);
                            this.Log($"  {e.Message}", Enums.LogMessageType.Warning);
                            continue;
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    foreach (Exception inner in ex.LoaderExceptions)
                    {
                        this.Log($"  {inner.Message}", Enums.LogMessageType.Error);
                    }
                    continue;
                }
            }
            return true;
        }
    }
}
