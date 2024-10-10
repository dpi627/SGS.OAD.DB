using SGS.OAD.DB;

namespace Console6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var resutl = ConnectionStringBuilder.Empty()
                .SetSever("localhost")
                .SetDatabase("mydb")
                .Build();
            Console.WriteLine(resutl);
        }
    }
}
