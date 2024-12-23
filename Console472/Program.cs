﻿using SGS.OAD.DB;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Console472
{
    internal class Program
    {
        static async Task Main()
        {
            // Get the name of the current application
            string appName = Assembly.GetEntryAssembly().GetName().Name;

            // create builder
            var builder = DbInfoBuilder.Init()
                .SetServer("TWDB009")
                .SetDatabase("SGSLims_chem")
                .EnableLog()
                .SetAppName(appName); // suggest to add

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
