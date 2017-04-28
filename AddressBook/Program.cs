using System.Configuration;

namespace AddressBook
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString;
            connectionString = ConfigurationManager.AppSettings["ApplicationName"];

            Rolodex rolodex = new Rolodex(connectionString);
            rolodex.DoStuff();
        }
    }
}
