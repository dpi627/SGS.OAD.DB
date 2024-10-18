
using Serilog.Events;
using Serilog;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SGS.OAD.DB.API.Models;
using Mapster;
using MapsterMapper;
using Microsoft.OpenApi.Models;
using SGS.OAD.DB.API.Filter;
using SGS.OAD.DB.API.Mapper;
using SGS.OAD.DB.API.Repositories.Interfaces;
using SGS.OAD.DB.API.Repositories;

namespace SGS.OAD.DB.API;

public class Program
{
    public static void Main(string[] args)
    {
        var appName = Assembly.GetExecutingAssembly().GetName().Name;

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console() //�}�o�����i�ѦҡA���p��Ҥ���
            .WriteTo.Seq(serverUrl: "http://twtpeoad002:5341")
            .WriteTo.File(
                path: "logs/.log",
                restrictedToMinimumLevel: LogEventLevel.Warning,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();

        try
        {
            Log.Information("{appName} Starting...", appName);

            IConfigurationRoot? _config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Program>()
                .Build();

            var builder = WebApplication.CreateBuilder(args);

            #region mapster settings
            // �]�w Mapster �M�g�S��W�h (���`�|�۰ʬM�g�P�˦W���ݩ�)
            MapsterConfig.Configure();
            // ���U Mapster
            builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
            builder.Services.AddScoped<IMapper, ServiceMapper>();
            #endregion

            builder.Logging.ClearProviders();
            builder.Services.AddSerilog();
            builder.Services.AddMapster();
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // �קK json �۰��ର�p�m�p���R�W (camelCase)
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                // ���o XML ��󪺸��|
                var xmlFile = $"{appName}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                // �[�J XML ����
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }

                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "OAD API", 
                    Version = "v1",
                    Description = "API �������"
                });
                c.SchemaFilter<EnumSchemaFilter>();
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            // ���U DbContext�A�ϥαz���s���r��
            builder.Services.AddDbContext<SGSLims_chemContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ���U Repository
            builder.Services.AddScoped<ISqlEncryptPasswordRepository, SqlEncryptPasswordRepository>();

            // ���U Service
            builder.Services.AddScoped<Services.Interfaces.IUserInfoService, Services.UserInfoService>();

            var app = builder.Build();

            // �t�m Swagger �����n��
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = string.Empty; // �]�w Swagger UI �b���ε{���ڥؿ�
                });
            }

            app.UseSerilogRequestLogging();
            app.UseCors("AllowAll");
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "{appName} Crashed", appName);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
