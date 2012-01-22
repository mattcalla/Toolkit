using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace Toolkit.DataAccess.UOW.NHibernate
{
    public class NHibernateUnitOfWork : IUnitOfWork
    {
        private readonly ISession _session;
        private readonly ITransaction _transaction;

        private readonly IDatabaseSession _databaseSession;

        public NHibernateUnitOfWork(ISession session)
        {
            _session = session;
            _transaction = _session.BeginTransaction();

            _databaseSession = new NHibernateDatabaseSession(_session);
        }

        public void Commit()
        {
            _session.Flush();
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public IDatabaseSession DatabaseSession
        {
            get { return _databaseSession; }
        }

        public int UserId { get; set; }

        public void Dispose()
        {
            if (_session != null)
            {
                _session.Dispose();
            }
        }
    }
}
