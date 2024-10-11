using SGS.OAD.DB.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console472
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var db = DbInfoBuilder.Init()
                .SetServer("TWDB009")
                .SetDatabase("SGSLims_chem")
                .Build();
            Console.WriteLine(db.ConnectionString);
            Console.ReadLine();
        }
    }
}
