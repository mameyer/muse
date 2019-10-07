using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Muse.Helpers
{
    public class NestedModelBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var request = bindingContext.HttpContext.Request.Query.TryGetValue(modelName, out var modelValue);
            if (modelValue.Count == 0)
            {
                return Task.CompletedTask;
            }
            try
            {
                var model = JsonConvert.DeserializeObject<T>(modelValue);
                if (model == null)
                {
                    return Task.CompletedTask;
                }
                bindingContext.Result = ModelBindingResult.Success(model);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.CompletedTask;
            }
        }
    }
}