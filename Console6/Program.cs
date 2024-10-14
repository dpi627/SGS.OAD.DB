using SGS.OAD.DB.Builders;
using System.Reflection;

namespace Console6;

internal class Program
{
    static async Task Main()
    {
        string appName = Assembly.GetEntryAssembly()?.GetName().Name ?? "SYSOP";

        // create builder
        var builder = DbInfoBuilder.Init()
            .SetServer("TWDB009")
            .SetDatabase("SGSLims_chem")
            .SetAppName(appName); // suggest to add

        // build database object
        var db = builder.Build();
        Console.WriteLine(db.ConnectionString);
        // build database object asynchronously
        db = await builder.BuildAsync();
        Console.WriteLine(db.ConnectionString);
    }
}
