using Kliniq.Application.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace Kliniq.Api.Binders
{
    public class FormWithFilesModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var request = bindingContext.HttpContext.Request;

            if (!request.HasFormContentType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            var form = request.Form;
            var modelType = bindingContext.ModelMetadata.ModelType;
            var instance = Activator.CreateInstance(modelType);

            if (instance is null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            foreach (var prop in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanWrite) continue;

                if (prop.PropertyType == typeof(FileUpload))
                {
                    var file = form.Files[ToCamelCase(prop.Name)];
                    prop.SetValue(instance, ToFileUpload(file));
                    continue;
                }

                if (prop.PropertyType == typeof(IEnumerable<FileUpload>) ||
                    prop.PropertyType == typeof(List<FileUpload>))
                {
                    var files = form.Files
                        .GetFiles(ToCamelCase(prop.Name))
                        .Select(ToFileUpload)
                        .OfType<FileUpload>()
                        .ToList();

                    prop.SetValue(instance, files);
                    continue;
                }

                var formValue = form[ToCamelCase(prop.Name)];
                if (!Microsoft.Extensions.Primitives.StringValues.IsNullOrEmpty(formValue))
                {
                    var converted = ConvertValue(formValue.ToString(), prop.PropertyType);
                    if (converted is not null)
                        prop.SetValue(instance, converted);
                }
            }

            bindingContext.Result = ModelBindingResult.Success(instance);
            return Task.CompletedTask;
        }

        private static FileUpload? ToFileUpload(IFormFile? file)
        {
            if (file is null || file.Length == 0) return null;

            return new FileUpload
            {
                Content     = file.OpenReadStream(),
                FileName    = file.FileName,   
                ContentType = file.ContentType,
                Size        = file.Length,
            };
        }

        private static object? ConvertValue(string value, Type targetType)
        {
            var type = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                return type switch
                {
                    _ when type == typeof(string)   => value,
                    _ when type == typeof(int)      => int.Parse(value),
                    _ when type == typeof(long)     => long.Parse(value),
                    _ when type == typeof(decimal)  => decimal.Parse(value),
                    _ when type == typeof(bool)     => bool.Parse(value),
                    _ when type == typeof(Guid)     => Guid.Parse(value),
                    _ when type == typeof(DateTime) => DateTime.Parse(value),
                    _ when type.IsEnum              => Enum.Parse(type, value, ignoreCase: true),
                    _                               => null
                };
            }
            catch
            {
                return null;
            }
        }

        private static string ToCamelCase(string name) =>
            string.IsNullOrEmpty(name)
                ? name
                : char.ToLowerInvariant(name[0]) + name[1..];
    }
}