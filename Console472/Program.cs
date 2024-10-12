using SGS.OAD.DB.Builders;
using System;
using System.Threading.Tasks;

namespace Console472
{
    internal class Program
    {
        static async Task Main()
        {
            // create builder
            var builder = DbInfoBuilder.Init()
                .SetServer("TWDB009")
                .SetDatabase("SGSLims_chem");
            // build database object
            var db = builder.Build();
            Console.WriteLine(db.ConnectionString);
            // build database object asynchronously
            db = await builder.BuildAsync();
            Console.WriteLine(db.ConnectionString);

            Console.ReadLine();
        }
    }
}
