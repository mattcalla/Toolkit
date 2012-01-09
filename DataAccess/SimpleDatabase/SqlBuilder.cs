using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Toolkit.DataAccess.SimpleDatabase
{
    public class SqlBuilder
    {
        private string _sql;
        private readonly List<IDbDataParameter> _parameters;
        private IDatabaseFactory _factory;
        private const string TABLENAMEPLACEHOLDER = "*****TABLENAME*****";
        private bool InsertIdFields = false;

        public SqlBuilder(IDatabaseFactory factory)
        {
            _factory = factory;
            _sql = String.Empty;
            _parameters = new List<IDbDataParameter>();
        }

        public string SQL
        {
            get
            {
                return _sql;
            }
        }

        public IDbDataParameter[] Parameters
        {
            get { return _parameters.ToArray(); }
        }

        public void Select(string columns)
        {
            _sql = "SELECT " + columns;
        }

        public void Select(object values)
        {
            _sql = "SELECT ";

            var properties = values.GetType().GetProperties();

            foreach (var property in properties)
            {
                _sql += "[" + property.Name + "], ";
            }

            _sql = _sql.Remove(_sql.Length - 2, 2);
        }

        

        public void Update(string tableName)
        {
            if (String.IsNullOrEmpty(tableName)) tableName = TABLENAMEPLACEHOLDER;
            _sql = "UPDATE [" + tableName + "]";
        }

        public void InsertInto(string tableName)
        {
            if (String.IsNullOrEmpty(tableName)) tableName = TABLENAMEPLACEHOLDER;
            _sql = "INSERT INTO [" + tableName + "] (";
        }

        public void DeleteFrom(string tableName)
        {
            _sql = "DELETE FROM [" + tableName + "]";
        }

        public void FromPlaceHolder()
        {
            _sql += " FROM [" + TABLENAMEPLACEHOLDER + "]";
        }

        public void From(string table)
        {
            _sql += " FROM [" + table + "]";
        }

        public void Values(object values)
        {
            var properties = values.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.Name.ToLower() == "id" && !InsertIdFields) continue;

                _sql += "[" + property.Name + "], ";
            }

            _sql = _sql.Remove(_sql.Length - 2, 2);
            _sql += ") VALUES (";

            foreach (var property in properties)
            {
                if (property.Name.ToLower() == "id" && !InsertIdFields) continue;

                _sql += "@" + property.Name + _parameters.Count + ", ";
                var value = property.GetValue(values, null);
                if(value == null)
                {
                    _parameters.Add(_factory.CreateParameter("@" + property.Name + _parameters.Count, DBNull.Value));
                }
                else
                {
                    _parameters.Add(_factory.CreateParameter("@" + property.Name + _parameters.Count, value));
                }

            }

            _sql = _sql.Remove(_sql.Length - 2, 2);
            _sql += ")";
        }

        public void Values(string column, object value)
        {
            _sql += "[" + column + "]) VALUES (@" + column + _parameters.Count + ")";
            _parameters.Add(_factory.CreateParameter("@" + column + _parameters.Count, value));
        }


        public void Set(object values)
        {
            var properties = values.GetType().GetProperties();

            _sql += " SET ";

            foreach (var property in properties)
            {
                _sql += "[" + property.Name + "] = @" + property.Name + _parameters.Count + ", ";
                _parameters.Add(_factory.CreateParameter("@" + property.Name + _parameters.Count, property.GetValue(values, null)));
            }

            _sql = _sql.Remove(_sql.Length - 2, 2);
        }

        public void Set(string column, object value)
        {
            _sql += " SET ";
            _sql += "[" + column + "] = @" + column + _parameters.Count + " ";
            _parameters.Add(_factory.CreateParameter("@" + column + _parameters.Count, value));
        }

        public void Where(object values)
        {
            _sql += " WHERE ";
            var properties = values.GetType().GetProperties();

            foreach (var property in properties)
            {
                _sql += "[" + property.Name + "] = @" + property.Name + _parameters.Count + " AND ";
                _parameters.Add(_factory.CreateParameter("@" + property.Name + _parameters.Count, property.GetValue(values, null)));
            }

            _sql = _sql.Remove(_sql.Length - 4, 4);
        }

        public void WhereEquals(string column, object value)
        {
            _sql += " WHERE ";
            _sql += "[" + column + "] = @" + column + _parameters.Count + " ";
            _parameters.Add(_factory.CreateParameter("@" + column + _parameters.Count, value));
        }

        public void And(object values)
        {
            _sql += " AND ";
            var properties = values.GetType().GetProperties();

            foreach (var property in properties)
            {
                _sql += "[" + property.Name + "] = @" + property.Name + _parameters.Count + " AND ";
                _parameters.Add(_factory.CreateParameter("@" + property.Name + _parameters.Count, property.GetValue(values, null)));
            }

            _sql = _sql.Remove(_sql.Length - 4, 4);
        }

        public void And(string column, object value)
        {
            _sql += " AND ";
            _sql += "[" + column + "] = @" + column + _parameters.Count + " ";
            _parameters.Add(_factory.CreateParameter("@" + column + _parameters.Count, value));
        }

        public void Or(object values)
        {
            _sql += " OR ";
            var properties = values.GetType().GetProperties();

            foreach (var property in properties)
            {
                _sql += "[" + property.Name + "] = @" + property.Name + _parameters.Count + " OR ";
                _parameters.Add(_factory.CreateParameter("@" + property.Name + _parameters.Count, property.GetValue(values, null)));
            }

            _sql = _sql.Remove(_sql.Length - 3, 3);
        }

        public void Or(string column, object value)
        {
            _sql += " OR ";
            _sql += "[" + column + "] = @" + column + _parameters.Count + " ";
            _parameters.Add(_factory.CreateParameter("@" + column + _parameters.Count, value));
        }

        public void SetTableName(string tableName)
        {
            _sql = _sql.Replace(TABLENAMEPLACEHOLDER, tableName);
        }

        public void AliasInsertColumn(string column, string aliasName)
        {
            _sql = _sql.Replace("[" + column + "]", "[" + aliasName + "]");
        }

        public void AliasSelectColumn(string column, string aliasName)
        {
            _sql = _sql.Replace("[" + aliasName + "]", "[" + column + "] as [" + aliasName + "]");
        }

        internal void OrderBy(string field)
        {
            _sql += " ORDER BY [" + field + "]";
        }

        internal void OrderByDesc(string field)
        {
            _sql += " ORDER BY [" + field + "] DESC";
        }
    }
}
