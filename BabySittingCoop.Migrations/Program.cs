using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BabySittingCoop.Migrations
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Importing transaction log...");
            XmlSerializer s = new XmlSerializer(typeof(ArrayOfBabySittingTransaction));

            ArrayOfBabySittingTransaction transactions = null;
            using (FileStream fs = File.OpenRead(@"C:\dev\sandbox\BabySittingCoop\BabySittingCoop.Migrations\transactions.xml"))
            {
                transactions = (ArrayOfBabySittingTransaction)s.Deserialize(fs);
            }
            Console.WriteLine("Imported {0} records.", transactions.Items.Length);
        }
    }
}
