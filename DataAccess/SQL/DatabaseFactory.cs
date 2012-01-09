using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;

namespace Toolkit.DataAccess
{
    public enum  DatabaseType
    {
        SqlServer,
        SqlCompact
    }
    public class DatabaseFactory
    {
        public static  IDatabaseFactory Create(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    return new SqlServerDatabaseFactory();
                case DatabaseType.SqlCompact:
                    return new SqlCompactDatabaseFactory();
                default:
                    throw new ArgumentOutOfRangeException("dbType");
            }
        }
    }

    public interface  IDatabaseFactory
    {
        IDbConnection CreateConnection(string connectionString);
        IDbDataAdapter CreateDataAdapter();
        IDbDataParameter CreateParameter(string column, object value);
    }


    public class SqlServerDatabaseFactory : IDatabaseFactory
    {
        public IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString); 
        }

        public IDbDataAdapter CreateDataAdapter()
        {
            return new SqlDataAdapter();
        }

        public IDbDataParameter CreateParameter(string column, object value)
        {
            return new SqlParameter(column, value);
        }
    }

    public class SqlCompactDatabaseFactory : IDatabaseFactory
    {
        public IDbConnection CreateConnection(string connectionString)
        {
            return new SqlCeConnection(connectionString);
        }

        public IDbDataAdapter CreateDataAdapter()
        {
            return new SqlCeDataAdapter();
        }

        public IDbDataParameter CreateParameter(string column, object value)
        {
            return new SqlCeParameter(column, value);
        }
    }
}
