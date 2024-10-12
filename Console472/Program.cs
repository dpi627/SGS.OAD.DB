using SGS.OAD.DB.Builders;
using System;

namespace Console472
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var builder = DbInfoBuilder.Init())
            {
                var db = builder
                    .SetServer("TWDB009")
                    .SetDatabase("SGSLims_chem")
                    .Build();

                Console.WriteLine(db.ConnectionString);
            }


            //var builder = DbInfoBuilder.Init()
            //    .SetServer("TWDB009")
            //    .SetDatabase("SGSLims_chem");
            //var db = builder.Build();
            //Console.WriteLine(db.ConnectionString);
            //Console.ReadLine();
        }
    }
}
