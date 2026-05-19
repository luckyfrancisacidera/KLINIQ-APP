using Kliniq.Application.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace Kliniq.Api.Binders
{
    public class FormWithFilesModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            var hasFileUpload = context.Metadata.ModelType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Any(p =>
                    p.PropertyType == typeof(FileUpload) ||
                    p.PropertyType == typeof(IEnumerable<FileUpload>) ||
                    p.PropertyType == typeof(List<FileUpload>));

            return hasFileUpload ? new FormWithFilesModelBinder() : null;
        }
    }
}