using SGS.OAD.DB.Builders;
using System;

namespace Console47
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = DbInfoBuilder.Init()
                .SetServer("TWDB009")
                .SetDatabase("SGSLims_chem");

            var db = builder.Build();
            Console.WriteLine(db.ConnectionString);
            Console.ReadLine();
        }
    }
}
