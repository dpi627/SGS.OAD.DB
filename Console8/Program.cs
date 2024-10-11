using SGS.OAD.DB.Builders;

namespace Console8
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
