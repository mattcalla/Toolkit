using System;
using System.Linq.Expressions;
using System.Reflection;
using Toolkit.Reflection;

namespace Toolkit.DataAccess.SimpleDatabase
{
    public class UpdateOperation<TDataType> : CrudOperation
    {

        public UpdateOperation(string tableName, SqlDatabase database)
            : base(database)
        {
            _builder.Update(tableName);
        }

        public UpdateOperation<TDataType> Set(object values)
        {
            _builder.SetTableName(values.GetType().Name);
            _builder.Set(values);
            return this;
        }


        public UpdateOperation<TDataType> WhereEquals(Expression<Func<TDataType, object>> field, object value)
        {
            var column = PropertyHelper.GetProperty(field).Name;
            _builder.WhereEquals(column, value);
            return this;
        }

        public UpdateOperation<TDataType> WhereEquals(string field, object value)
        {
            _builder.WhereEquals(field, value);
            return this;
        }


        public UpdateOperation<TDataType> AndEquals(Expression<Func<TDataType, object>> field, object value)
        {
            var column = PropertyHelper.GetProperty(field).Name;
            _builder.And(column, value);
            return this;
        }

        public UpdateOperation<TDataType> AndEquals(string field, object value)
        {
            _builder.And(field, value);
            return this;
        }

        public UpdateOperation<TDataType> OrEquals(Expression<Func<TDataType, object>> field, object value)
        {
            var column = PropertyHelper.GetProperty(field).Name;
            _builder.Or(column, value);
            return this;
        }

        public UpdateOperation<TDataType> OrEquals(string field, object value)
        {
            _builder.Or(field, value);
            return this;
        }

        
    }
}