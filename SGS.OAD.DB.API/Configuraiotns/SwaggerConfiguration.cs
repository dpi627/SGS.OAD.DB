using Microsoft.OpenApi.Models;
using SGS.OAD.DB.API.Filter;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SGS.OAD.DB.API.Configurations
{
    public static class SwaggerConfig
    {
        public static void ConfigureSwagger(this IServiceCollection services, string appName)
        {
            services.AddSwaggerGen(c =>
            {
                SetApiDoc(c, appName);
                SetJwt(c);
                SetApiKey(c);
                c.OperationFilter<AuthorizeOperationFilter>();
            });
        }

        // 設置 API 文件資訊
        private static void SetApiDoc(SwaggerGenOptions c, string appName)
        {
            // 取得 XML 文件的路徑
            var xmlFile = $"{appName}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            // 加入 XML 註解
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = appName,
                Version = "v1",
                Description = "API 說明文件"
            });
        }

        // 設置 JWT 認證
        private static void SetJwt(SwaggerGenOptions c)
        {
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Please enter JWT with Bearer into field like: Bearer {token}",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

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

            c.AddSecurityDefinition("Bearer", securityScheme);
            //c.AddSecurityRequirement(securityRequirement);
            // 註冊自訂的 OperationFilter
            c.OperationFilter<AuthorizeOperationFilter>();
        }

        // 設置 API Key 認證
        private static void SetApiKey(SwaggerGenOptions c)
        {
            var apiKeyScheme = new OpenApiSecurityScheme
            {
                Description = "請輸入 API Key",
                Name = "X-API-KEY",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            };

            var apiKeyRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        }
                    },
                    new string[] { }
                }
            };

            c.AddSecurityDefinition("ApiKey", apiKeyScheme);
            //c.AddSecurityRequirement(apiKeyRequirement);
        }
    }
}
