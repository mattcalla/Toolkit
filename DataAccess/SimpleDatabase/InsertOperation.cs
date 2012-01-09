using System;
using System.Linq.Expressions;
using Toolkit.Reflection;

namespace Toolkit.DataAccess.SimpleDatabase
{
    public class InsertOperation<TDataType> : CrudOperation
    {
        public InsertOperation(string tableName, SqlDatabase database) : base(database)
        {
           _builder.InsertInto(tableName);
        }

        public InsertOperation<TDataType> Values(object values)
        {
            _builder.SetTableName(values.GetType().Name);
            _builder.Values(values);
            return this;
        }
    }
}