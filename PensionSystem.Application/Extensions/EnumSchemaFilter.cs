using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PensionSystem.Application.Extensions
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Enum != null && schema.Enum.Count > 0)
            {
                schema.Enum.Clear();
                foreach (var enumValue in Enum.GetValues(context.Type))
                {
                    schema.Enum.Add(new OpenApiString(enumValue.ToString()));
                }
            }
        }
    }

}
