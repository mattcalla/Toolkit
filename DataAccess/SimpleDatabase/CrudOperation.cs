using System;
using System.Linq.Expressions;
using Toolkit.Reflection;

namespace Toolkit.DataAccess.SimpleDatabase
{
    public abstract class CrudOperation
    {
        protected readonly SqlDatabase _database;
        protected readonly SqlBuilder _builder;

        protected CrudOperation(SqlDatabase database)
        {
            _database = database;
            _builder = new SqlBuilder(database.Factory);
        }

        protected  CrudOperation(SqlDatabase database, SqlBuilder builder)
        {
            _database = database;
            _builder = builder;
        }

        public virtual int Go()
        {
            return _database.ExecuteNonQuery(_builder.SQL, _builder.Parameters);
        }
    }
}