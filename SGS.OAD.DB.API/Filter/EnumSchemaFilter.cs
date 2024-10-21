using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SGS.OAD.DB.API.Filter
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Enum.Clear();
                Enum.GetNames(context.Type)
                    .ToList()
                    .ForEach(name => schema.Enum.Add(new OpenApiString(name)));

                schema.Type = "string";
                schema.Format = null;

                // 添加列舉描述
                schema.Description = string.Join("<br>", Enum.GetNames(context.Type)
                    .Select(name => $"{name}: {GetEnumDescription(context.Type, name)}"));
            }
        }

        private string GetEnumDescription(Type enumType, string enumName)
        {
            var memberInfo = enumType.GetMember(enumName).FirstOrDefault();
            var attribute = memberInfo?.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
                .FirstOrDefault() as System.ComponentModel.DescriptionAttribute;
            return attribute?.Description ?? enumName;
        }
    }
}
