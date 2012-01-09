using System;
using System.Linq.Expressions;
using Toolkit.Reflection;

namespace Toolkit.DataAccess.SimpleDatabase
{
    public class DeleteOperation<TDataType> : CrudOperation
    {
        public DeleteOperation(string tableName, SqlDatabase database)
            : base(database)
        {
           _builder.DeleteFrom(tableName);
        }


        public DeleteOperation<TDataType> WhereEquals(Expression<Func<TDataType, object>> field, object value)
        {
            var column = PropertyHelper.GetProperty(field).Name;
            _builder.WhereEquals(column, value);
            return this;
        }

        public DeleteOperation<TDataType> WhereEquals(string field, object value)
        {
            _builder.WhereEquals(field, value);
            return this;
        }


        public DeleteOperation<TDataType> AndEquals(Expression<Func<TDataType, object>> field, object value)
        {
            var column = PropertyHelper.GetProperty(field).Name;
            _builder.And(column, value);
            return this;
        }

        public DeleteOperation<TDataType> AndEquals(string field, object value)
        {
            _builder.And(field, value);
            return this;
        }

        public DeleteOperation<TDataType> OrEquals(Expression<Func<TDataType, object>> field, object value)
        {
            var column = PropertyHelper.GetProperty(field).Name;
            _builder.Or(column, value);
            return this;
        }

        public DeleteOperation<TDataType> OrEquals(string field, object value)
        {
            _builder.Or(field, value);
            return this;
        }

    }
}