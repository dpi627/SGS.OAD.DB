using SGS.OAD.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console46
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConnectionStringBuilder.Empty()
                .SetSever("localhost")
                .SetDatabase("mydb")
                .Build();

            Console.WriteLine(connectionString);
            Console.ReadLine();
        }
    }
}
