using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace Toolkit.DataAccess.UOW.NHibernate
{
    public class NHibernateDatabaseSession : IDatabaseSession
    {
        private readonly ISession _session;

        public NHibernateDatabaseSession(ISession session)
        {
            _session = session;
        }

        public void Add(object obj)
        {
            _session.SaveOrUpdate(obj);
        }

        public T Get<T>(object id)
        {
            return _session.Get<T>(id);
        }

        public void Delete<T>(object id)
        {
            var entity = _session.Load<T>(id);
            _session.Delete(entity);
        }
    }
}
