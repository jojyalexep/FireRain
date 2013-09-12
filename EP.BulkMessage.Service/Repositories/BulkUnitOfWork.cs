using EP.BulkMessage.Service.Entity;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Repositories
{

    public class BulkUnitOfWork
    {
        ISessionFactory sessionFactory;
        ISession session;
        ITransaction transaction;
        public BulkUnitOfWork()
        {
            sessionFactory = CreateSessionFactory();
        }

        public ISession Session
        {
            get
            {
                return session;
            }

        }


        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
              .Database(
                OracleDataClientConfiguration.Oracle10
                  .ConnectionString("Persist Security Info=False;User ID=test_bulkmsg_user;Password=tbmuser;Data Source=ESVCDB2;")
              )
              .Mappings(m =>
                m.FluentMappings.AddFromAssemblyOf<MainUnitOfWork>())
              .BuildSessionFactory();
        }


        #region IUnitOfWork Members

        public void SaveBulk(List<Sms> objectList)
        {
            using (IStatelessSession statelessSession = sessionFactory.OpenStatelessSession())

            using (ITransaction transaction = statelessSession.BeginTransaction())
            {
                foreach (var item in objectList)
                {
                    statelessSession.Insert(item);
                }
                transaction.Commit();
            }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            session.Close();
        }

        #endregion
    }
}