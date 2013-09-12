using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Repositories
{
    public class MainUnitOfWork 
    {
        ISessionFactory sessionFactory;
        ISession session;
        ITransaction transaction;
        public MainUnitOfWork()
        {
            sessionFactory = CreateSessionFactory();
            session = sessionFactory.OpenSession();
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
                  .ConnectionString(c=>c.FromConnectionStringWithKey("Oracle"))
              )
              .Mappings(m =>
                m.FluentMappings.AddFromAssemblyOf<MainUnitOfWork>())
              .BuildSessionFactory();
        }


        #region IUnitOfWork Members

        public void Begin()
        {
            transaction = session.BeginTransaction();
        }

        public void Commit()
        {
            transaction.Commit();
        }

        public void RollbackChanges()
        {
            transaction.Rollback();
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