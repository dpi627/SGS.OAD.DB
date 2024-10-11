
using SGS.OAD.DB.Builders;

namespace Console6
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
        }
    }
}
