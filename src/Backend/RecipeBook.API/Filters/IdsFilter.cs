using Microsoft.OpenApi.Models;
using RecipeBook.API.Binders;
using RecipeBook.Domain.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RecipeBook.API.Filters;

public class IdsFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var encryptedIds = context
            .ApiDescription
            .ParameterDescriptions
            .Where(desc => desc.ModelMetadata.BinderType == typeof(IdBinder))
            .ToDictionary(d => d.Name, d => d);

        foreach (var parameter in operation.Parameters)
        {
            if (encryptedIds.TryGetValue(parameter.Name, out _).IsFalse()) continue;

            parameter.Schema.Format = string.Empty;
            parameter.Schema.Type = "string";
        }

        foreach (var schema in context.SchemaRepository.Schemas.Values)
        {
            foreach (var property in schema.Properties)
            {
                if (encryptedIds.TryGetValue(property.Key, out _).IsFalse()) continue;

                property.Value.Format = string.Empty;
                property.Value.Type = "string";
            }
        }
    }
}