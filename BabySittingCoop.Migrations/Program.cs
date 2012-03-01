using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using BabySittingCoop.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;

namespace BabySittingCoop.Migrations
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            App_Start.NHibernateProfilerBootstrapper.PreStart();


            Console.WriteLine("Importing transaction log...");
            XmlSerializer s = new XmlSerializer(typeof (ArrayOfBabySittingTransaction));

            ArrayOfBabySittingTransaction transactions = null;
            using (
                FileStream fs =
                    File.OpenRead(@"C:\dev\sandbox\BabySittingCoop\BabySittingCoop.Migrations\transactions.xml"))
            {
                transactions = (ArrayOfBabySittingTransaction) s.Deserialize(fs);
            }
            Console.WriteLine("Imported {0} records.", transactions.Items.Length);

            var config =
                new NHibernate.Cfg.Configuration().Configure(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                                          "NHibernate.config"));
            var autoPersistenceModel =
                AutoMap.AssemblyOf<BabySitter>(new MappingConfiguration())
                    .Conventions.Setup(c =>
                                           {
                                               c.Add<PrimaryKeyConvention>();
                                               c.Add<CustomForeignKeyConvention>();
                                               c.Add(DefaultCascade.All());
                                           });

            var sessionFactory = Fluently.Configure(config)
                .Database(MsSqlConfiguration.MsSql2008)
                .Mappings(m => m.AutoMappings.Add(autoPersistenceModel))
                .BuildSessionFactory();

            
            using (var session = sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {

                var babySitterCount = session.QueryOver<BabySitter>().ToRowCountQuery().RowCount();
                if (babySitterCount == 0)
                {
                    IEnumerable<BabySitter> sitters = null;
                    sitters = transactions.Items
                        .Select(i => int.Parse(i.SittingProviderId)).Distinct()
                        .Select(di => new BabySitter {LegacyId = di, Name = String.Format("Babysitter {0}", di)});

                    foreach (var sitter in sitters)
                    {
                        session.Save(sitter);

                    }
                    transaction.Commit();
                }
            }
            using (var session = sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var sitters = session.QueryOver<BabySitter>().List();
                foreach (var transactionItem in transactions.Items)
                {
                    var bst = new BabySittingTransaction();
                    bst.SittingProvider =
                        sitters.Single(sp => sp.LegacyId == int.Parse(transactionItem.SittingProviderId));
                    bst.SittingReceiver =
                        sitters.Single(sr => sr.LegacyId == int.Parse(transactionItem.SittingReceiverId));
                    bst.ChildrenWatched = int.Parse(transactionItem.ChildrenWatched);
                    bst.Duration = System.Xml.XmlConvert.ToTimeSpan(transactionItem.Duration);
                    var dateString = transactionItem.StartedAtUtc[0].DateTime;
                    var offsetDate = transactionItem.StartedAtUtc[0].OffsetMinutes;

                    bst.StartedAtUtc =
                        new DateTimeOffset(DateTime.SpecifyKind(DateTime.Parse(dateString), DateTimeKind.Unspecified),
                                           new TimeSpan(0, 0, int.Parse(offsetDate), 0, 0));

                    session.Save(bst);
                }

                transaction.Commit();
            }
        }
    }
}

