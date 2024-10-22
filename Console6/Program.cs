using SGS.OAD.DB;
using System.Reflection;

namespace Console6;

internal class Program
{
    static async Task Main()
    {
        string appName = Assembly.GetEntryAssembly()?.GetName().Name ?? "SYSOP";

        // 一開始使用資料庫連線
        var builder = DbInfoBuilder.Init()
            .SetServer("TWDB009")
            .SetDatabase("SGSLims_chem")
            //.EnableLog()
            .SetAppName(appName);

        DbInfo db;

        for (int i = 1; i < 8; i++)
        {
            Console.WriteLine($"\nLoop Test #{i}");

            if (i == 3)
            {
                // 突然需要檔案存取，嘗試取得 db_filewriter 權限
                builder = DbInfoBuilder.Init()
                    .SetServer("TWDB021")
                    .SetDatabase("LIMS20_TPE")
                    //.EnableLog()
                    .SetDatabaseRole(DatabaseRole.db_filewriter);
            }
            else if (i == 5)
            {
                // 存取原本資料庫
                Console.WriteLine("(Back to Original Database)");
                builder = DbInfoBuilder.Init()
                    .SetServer("TWDB009")
                    //.EnableLog()
                    .SetDatabase("SGSLims_chem");
            }
            else if (i == 6)
            {
                // 清除快取
                builder.ClearCache();
            }

            db = builder.Build();
            Console.WriteLine($"Sync : {db.ConnectionString[..100]}...");
            //db = await builder.BuildAsync();
            //Console.WriteLine($"Async: {db.ConnectionString[..100]}...");
        }
    }
}
