using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SGS.OAD.DB.API.Filter;

public class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // 檢查該 API Action 是否有 [Authorize] 屬性
        var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                            .OfType<AuthorizeAttribute>().Any() ||
                            context.MethodInfo.GetCustomAttributes(true)
                            .OfType<AuthorizeAttribute>().Any();

        // 如果有 [Authorize] 屬性，則為該端點加上 JWT 認證需求
        if (hasAuthorize)
        {
            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            };

            operation.Security = new List<OpenApiSecurityRequirement> { securityRequirement };
        }
    }
}

