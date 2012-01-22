using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Toolkit.DataAccess.UOW.NHibernate
{
    public class NHibernateUnitOfWorkFactory<TMappingAssembly> : IUnitOfWorkFactory
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly string _connectionString;
        private readonly FluentConfiguration _configuration;

        public NHibernateUnitOfWorkFactory(string connectionString)
        {
            _connectionString = connectionString;

            _configuration = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connectionString).ShowSql().AdoNetBatchSize(500))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<TMappingAssembly>());

            _sessionFactory = _configuration.BuildSessionFactory();
        }

        public void CreateDatabase()
        {
            new SchemaExport(_configuration.BuildConfiguration()).Create(false, true);
        }

        public IUnitOfWork Create()
        {
            if (_sessionFactory == null) throw new ApplicationException("NHibernateUnitOfWorkFactory has not been initalised.");
            return new NHibernateUnitOfWork(_sessionFactory.OpenSession());
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }
    }
}
