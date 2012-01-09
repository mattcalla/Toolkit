using System;
using System.Linq.Expressions;
using Toolkit.Reflection;

namespace Toolkit.DataAccess.SimpleDatabase
{
    public class SimpleDatabase
    {
        private readonly SqlDatabase _database;
        public SqlDatabase Advanced { get { return _database; } }

        public SimpleDatabase(DatabaseType dbType, string connectionString)
        {
            _database = new SqlDatabase(connectionString, DatabaseFactory.Create(dbType), true);
        }

        /// <summary>
        /// Insert with String Table Name
        /// </summary>
        public InsertOperation<object> Insert(string tableName)
        {
            return new InsertOperation<object>(tableName, _database);
        }

        /// <summary>
        /// Insert with Strongly Typed Table Name
        /// </summary>
        public InsertOperation<TDataType> Insert<TDataType>()
        {
            return new InsertOperation<TDataType>(typeof(TDataType).Name, _database);
        }


        /// <summary>
        /// Update with String Table Name
        /// </summary>
        /// <returns></returns>
        public UpdateOperation<object> Update(string tableName)
        {
            return new UpdateOperation<object>(tableName, _database);
        }

        /// <summary>
        /// Update with Strongly Typed Table Name
        /// </summary>
        public UpdateOperation<TDataType> Update<TDataType>()
        {
            return new UpdateOperation<TDataType>(typeof(TDataType).Name, _database);
        }

        /// <summary>
        /// Update with Strongly Typed object, but a different table name
        /// </summary>
        public UpdateOperation<TDataType> Update<TDataType>(string tableName)
        {
            return new UpdateOperation<TDataType>(tableName, _database);
        }


        /// <summary>
        /// Delete with String Table Name
        /// </summary>
        public DeleteOperation<object> Delete(string tableName)
        {
            return new DeleteOperation<object>(tableName, _database);
        }

        /// <summary>
        /// Delete with Strongly Typed Table Name
        /// </summary>
        public DeleteOperation<TDataType> Delete<TDataType>()
        {
            return new DeleteOperation<TDataType>(typeof(TDataType).Name, _database);
        }

        /// <summary>
        /// Delete with Strongly Typed object but a different table name
        /// </summary>
        public DeleteOperation<TDataType> Delete<TDataType>(string tableName)
        {
            return new DeleteOperation<TDataType>(tableName, _database);
        }


        /// <summary>
        /// Select with String Columns
        /// </summary>
        public SelectOperation<object> Select(string columns)
        {
            return new SelectOperation<object>(columns, String.Empty, _database);
        }

        /// <summary>
        /// Select *
        /// </summary>
        /// <returns></returns>
        public SelectOperation<object> Select()
        {
            return new SelectOperation<object>("*", String.Empty, _database);
        }

        /// <summary>
        /// Select With Strongly Typed Columns
        /// </summary>
        /// <typeparam name="TDataType"></typeparam>
        /// <returns></returns>
        public SelectOperation<TDataType> Select<TDataType>() 
        {
            return new SelectOperation<TDataType>(Activator.CreateInstance<TDataType>(), String.Empty, _database);
        }

        /// <summary>
        /// Select Count  with Strongly Typed Table Name
        /// </summary>
        public SelectOperation<TDataType> SelectCount<TDataType>()
        {
            return new SelectOperation<TDataType>("COUNT(1)", String.Empty, _database);
        }

        public SelectOperation<TDataType> Select<TDataType>(params Expression<Func<TDataType, object>> [] fields)
        {
            var columns = "";

            foreach(var field in fields)
            {
                var column = PropertyHelper.GetProperty(field).Name;
                columns += "[" + column + "], ";
            }
            columns = columns.Remove(columns.Length - 2, 2);

            return new SelectOperation<TDataType>(columns, String.Empty, _database);
        }


        public SelectOperation<TDataType> Select<TDataType>(Expression<Func<TDataType, object>> obj) 
        {
            var result = obj.Compile().Invoke(Activator.CreateInstance<TDataType>());
            return new SelectOperation<TDataType>(result, String.Empty, _database);
        }

     



        
    }
}
