using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Toolkit.Reflection;

namespace Toolkit.DataAccess.SimpleDatabase
{
    public class SelectOperation<TDataType> : CrudOperation
    {
        public TDataType DataType { get; set; }
        private bool _isTableNameSet = false;

        public SelectOperation(string columns, string table, SqlDatabase database)
            : base(database)
        {
            _builder.Select(columns);
            if (table.Length > 0)
            {
                _builder.From(table);
                _isTableNameSet = true;
            }
            else
            {
                _builder.FromPlaceHolder();
            }
        }

        public SelectOperation(object columns, string table, SqlDatabase database)
            : base(database)
        {
            _builder.Select(columns);
            if (table.Length > 0)
            {
                _builder.From(table);
                _isTableNameSet = true;
            }
            else
            {
                _builder.FromPlaceHolder();
            }
        }

        protected SelectOperation(TDataType data, SqlDatabase database, SqlBuilder builder)
            : base(database, builder)
        {
            DataType = data;
        }

        public SelectOperation<TDataType> From(string table)
        {
            _builder.SetTableName(table);
            _isTableNameSet = true;

            return new SelectOperation<TDataType>(Activator.CreateInstance<TDataType>(), _database, _builder);
        }


        public SelectOperation<TDataType> Alias(string tableColumn, Expression<Func<TDataType, object>> alias)
        {
            var aliasName = PropertyHelper.GetProperty(alias).Name;
            _builder.AliasSelectColumn(tableColumn, aliasName);
            return this;
        }

        public SelectOperation<TDataType> WhereEquals(Expression<Func<TDataType, object>> field, object value)
        {
            var column = PropertyHelper.GetProperty(field).Name;
            _builder.WhereEquals(column, value);
            return this;
        }

        public SelectOperation<TDataType> WhereEquals(string field, object value)
        {
            _builder.WhereEquals(field, value);
            return this;
        }


        public SelectOperation<TDataType> AndEquals(Expression<Func<TDataType, object>> field, object value)
        {
            var column = PropertyHelper.GetProperty(field).Name;
            _builder.And(column, value);
            return this;
        }

        public SelectOperation<TDataType> AndEquals(string field, object value)
        {
            _builder.And(field, value);
            return this;
        }

        public SelectOperation<TDataType> OrEquals(Expression<Func<TDataType, object>> field, object value)
        {
            var column = PropertyHelper.GetProperty(field).Name;
            _builder.Or(column, value);
            return this;
        }

        public SelectOperation<TDataType> OrEquals(string field, object value)
        {
            _builder.Or(field, value);
            return this;
        }

        public SelectOperation<TDataType> OrderBy(string field)
        {
            _builder.OrderBy(field);
            return this;
        }

        public SelectOperation<TDataType> OrderByDesc(string field)
        {
            _builder.OrderByDesc(field);
            return this;
        }





        public List<TDataType> ToList()
        {
            SetTableNameIfNotSet();
            return _database.QueryList<TDataType>(_builder.SQL, _builder.Parameters);
        }

        public List<dynamic> ToDynamicList()
        {
            SetTableNameIfNotSet();
            return _database.QueryList(_builder.SQL, _builder.Parameters);
        }

        public TDataType ToSingle()
        {
            SetTableNameIfNotSet();
            return _database.QuerySingle<TDataType>(_builder.SQL, _builder.Parameters);
        }

        public dynamic ToDynamic()
        {
            SetTableNameIfNotSet();
            return _database.QuerySingle(_builder.SQL, _builder.Parameters);
        }

        public List<T> ToList<T>() where T : new()
        {
            SetTableNameIfNotSet();
            return _database.QueryList<T>(_builder.SQL, _builder.Parameters);
        }

        public T ToSingle<T>() where T : new()
        {
            SetTableNameIfNotSet();
            return _database.QuerySingle<T>(_builder.SQL, _builder.Parameters);
        }

        public T ToData<T>()
        {
            SetTableNameIfNotSet();
            return _database.ExecuteScalar<T>(_builder.SQL, _builder.Parameters);
        }

        public string ToJSON()
        {
            SetTableNameIfNotSet();
            return _database.ExecuteJsonQuery(_builder.SQL, _builder.Parameters);
        }

        private void SetTableNameIfNotSet()
        {
            if (!_isTableNameSet)
            {
                _builder.SetTableName(typeof(TDataType).Name);
            }
        }


    }
}