
using Serilog.Events;
using Serilog;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SGS.OAD.DB.API.Models;
using Mapster;
using SGS.OAD.DB.API.DTOs;
using MapsterMapper;

namespace SGS.OAD.DB.API;

public class Program
{
    public static void Main(string[] args)
    {
        var appName = Assembly.GetExecutingAssembly().GetName().Name;

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console() //開發期間可參考，部署後考不用
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

            //string? endpoint = _config["Endpoint"]!;
            //string? apiKey = _config["OpenAI:ApiKey"]!;
            //string? model = _config["OpenAI:Model"]!;


            var builder = WebApplication.CreateBuilder(args);

            #region mapster settings
            var mapsterConfig = TypeAdapterConfig.GlobalSettings;

            // 映射配置，之後如果改為AES可另外修改來源 ID/PW
            mapsterConfig.NewConfig<SQLEncryptPassword, UserInfoEncryptResponse>()
                  .Map(dest => dest.ID, src => src.ID)
                  .Map(dest => dest.PW, src => src.PW);

            builder.Services.AddSingleton(mapsterConfig);
            builder.Services.AddScoped<IMapper, ServiceMapper>();
            #endregion

            builder.Logging.ClearProviders();
            builder.Services.AddSerilog();
            builder.Services.AddMapster();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
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
            //builder.Services.AddKernel()
            //    .AddOpenAIChatCompletion(
            //        modelId: model,
            //        apiKey: apiKey
            //        );
            //builder.Services.AddScoped<IExcelFormulaService, ExcelFormulaService>();

            // 註冊 DbContext，使用您的連接字串
            builder.Services.AddDbContext<SGSLims_chemContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

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
