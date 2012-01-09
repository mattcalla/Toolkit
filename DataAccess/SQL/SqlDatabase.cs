using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using AutoMapper;

namespace Toolkit.DataAccess
{
    public class SqlDatabase 
    {
        private readonly string _connectionString;
        private readonly IDatabaseFactory _factory;
        private readonly bool _isLoggingEnabled;

        public SqlDatabase(string connectionString, IDatabaseFactory factory)
        {
            _connectionString = connectionString;
            _factory = factory;
        }

        public SqlDatabase(string connectionString, IDatabaseFactory factory, bool isLoggingEnabled)
        {
            _connectionString = connectionString;
            _isLoggingEnabled = isLoggingEnabled;
            _factory = factory;
        }

        public IDatabaseFactory Factory
        {
            get { return _factory; }
        }

        public DataTable ExecuteDataTable(string sql, params IDbDataParameter[] parameters)
        {
            var start = DateTime.Now;
            try
            {
                using (var connection = Factory.CreateConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                       // command.CommandTimeout = 900; //15 minutes
                        command.CommandText = sql;
                        foreach (var param in parameters)
                            command.Parameters.Add(param);

                        var data = new DataSet();
                        var adapter = Factory.CreateDataAdapter();
                        adapter.SelectCommand = command;
                        adapter.Fill(data);
                        return data.Tables.Count > 0 ? data.Tables[0] : new DataTable();
                    }
                }
            }
            catch (DbException)
            {
                //Logger.Error("SQL: " + sql);
                throw;
            }
            finally
            {
                LogSQL("ExecuteDataTable", start, DateTime.Now, sql, parameters);
            }
        }

        public T ExecuteScalar<T>(string sql, params IDbDataParameter[] parameters)
        {
            var start = DateTime.Now;
            try
            {
                using (var connection = Factory.CreateConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandTimeout = 900; //15 minutes
                        command.CommandText = sql;
                        foreach (var param in parameters)
                            command.Parameters.Add(param);

                        var result = command.ExecuteScalar();
                        return (T)result;
                    }
                }
            }
            catch (DbException)
            {
                //Logger.Error("SQL: " + sql);
                throw;
            }
            finally
            {
                LogSQL("ExecuteDataTable", start, DateTime.Now, sql, parameters);
            }
        }

        public IDataReader ExecuteDataReader(string sql, params IDbDataParameter[] parameters)
        {
            var start = DateTime.Now;
            try
            {
                var connection = Factory.CreateConnection(_connectionString);

                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 900; //15 minutes
                    command.CommandText = sql;
                    foreach (var param in parameters)
                        command.Parameters.Add(param);

                    return command.ExecuteReader(CommandBehavior.CloseConnection);
                }

                //Connection should be closed by calling client
            }
            catch (DbException)
            {
                //Logger.Error("SQL: " + sql);
                throw;
            }
            finally
            {
                LogSQL("ExecuteReportListQuery", start, DateTime.Now, sql, parameters);
            }
        }

        public List<T> QueryList<T>(string sql, params IDbDataParameter[] parameters)
        {
            //var data = ExecuteDataTable(sql, parameters);

            var start = DateTime.Now;
            try
            {
                using (var connection =  Factory.CreateConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        foreach (var param in parameters)
                        {
                            command.Parameters.Add(param);
                        }

                        Mapper.CreateMap<IDataReader, List<T>>();

                        using (var reader = command.ExecuteReader())
                        {
                            return Mapper.Map<IDataReader, List<T>>(reader);
                        }
                        
                    }
                }
            }
            catch (DbException)
            {
                //Logger.Error("SQL: " + sql);
                throw;
            }
            finally
            {
                LogSQL("ExecuteDTOListQuery", start, DateTime.Now, sql, parameters);
            }
        }


        public List<dynamic> QueryList(string sql, params IDbDataParameter[] parameters)
        {
            //var data = ExecuteDataTable(sql, parameters);

            var start = DateTime.Now;
            try
            {
                using (var connection = Factory.CreateConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        foreach (var param in parameters)
                        {
                            command.Parameters.Add(param);
                        }

                        using (var reader = command.ExecuteReader())
                        {
                            var results = new List<dynamic>();
                            while(reader.Read())
                            {
                                results.Add(DataReaderToExpando(reader));
                            }
                            return results;
                        }

                    }
                }
            }
            catch (DbException)
            {
                //Logger.Error("SQL: " + sql);
                throw;
            }
            finally
            {
                LogSQL("ExecuteDTOListQuery", start, DateTime.Now, sql, parameters);
            }
        }

        private dynamic DataReaderToExpando(IDataReader reader)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;

            for (var i = 0; i < reader.FieldCount; i++)
                expandoObject.Add(reader.GetName(i), reader[i]);

            return expandoObject;
        }

        public dynamic QuerySingle(string sql, params IDbDataParameter[] parameters) 
        {
            var start = DateTime.Now;

            try
            {
                using (var connection = Factory.CreateConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandTimeout = 900; //15 minutes
                        command.CommandText = sql;
                        foreach (var param in parameters)
                            command.Parameters.Add(param);

                      
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return DataReaderToExpando(reader);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (DbException)
            {
                //Logger.Error("SQL: " + sql);
                throw;
            }
            finally
            {
                LogSQL("ExecuteDTOQuery", start, DateTime.Now, sql, parameters);
            }
        }


        public T QuerySingle<T>(string sql, params IDbDataParameter[] parameters) 
        {
            var start = DateTime.Now;

            try
            {
                using (var connection = Factory.CreateConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandTimeout = 900; //15 minutes
                        command.CommandText = sql;
                        foreach (var param in parameters)
                            command.Parameters.Add(param);

                        Mapper.CreateMap<IDataRecord, T>();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return Mapper.Map<IDataRecord, T>(reader);
                            }
                            else
                            {
                                return Activator.CreateInstance<T>();
                            }
                        }
                    }
                }
            }
            catch (DbException)
            {
                //Logger.Error("SQL: " + sql);
                throw;
            }
            finally
            {
                LogSQL("ExecuteDTOQuery", start, DateTime.Now, sql, parameters);
            }
        }

        public string ExecuteJsonQuery(string sql, params IDbDataParameter[] parameters)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(ExecuteDataTable(sql, parameters), new Newtonsoft.Json.Converters.DataTableConverter());
        }

        public int ExecuteNonQuery(string sql, params IDbDataParameter[] parameters)
        {
            var start = DateTime.Now;

            try
            {
                using (var connection = Factory.CreateConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        foreach (var param in parameters)
                            command.Parameters.Add(param);

                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (DbException)
            {
                //Logger.Error("SQL: " + sql);
                throw;
            }
            finally
            {
                LogSQL("ExecuteNonQuery", start, DateTime.Now, sql, parameters);
            }
        }

        private void LogSQL(string format, DateTime start, DateTime end, string sql, params IDbDataParameter[] parameters)
        {
            if (_isLoggingEnabled == false) return;

            string log = sql;

            if (parameters.Length > 0)
            {
                log += " | ";
                foreach (var param in parameters)
                {
                    log += " " + param.ParameterName + "=" + param.Value + ", ";
                }

                log = log.Remove(log.Length - 2);
            }

            Console.WriteLine(log);
            //EventStore.BuildEventStore().LogQuery(log, start, end, format);
            //Logger.Info((end - start) + " - " + format + " - " + log);
        }
  
    }
}



//typeMap = new Dictionary<Type, DbType>();
//            typeMap[typeof(byte)] = DbType.Byte;
//            typeMap[typeof(sbyte)] = DbType.SByte;
//            typeMap[typeof(short)] = DbType.Int16;
//            typeMap[typeof(ushort)] = DbType.UInt16;
//            typeMap[typeof(int)] = DbType.Int32;
//            typeMap[typeof(uint)] = DbType.UInt32;
//            typeMap[typeof(long)] = DbType.Int64;
//            typeMap[typeof(ulong)] = DbType.UInt64;
//            typeMap[typeof(float)] = DbType.Single;
//            typeMap[typeof(double)] = DbType.Double;
//            typeMap[typeof(decimal)] = DbType.Decimal;
//            typeMap[typeof(bool)] = DbType.Boolean;
//            typeMap[typeof(string)] = DbType.String;
//            typeMap[typeof(char)] = DbType.StringFixedLength;
//            typeMap[typeof(Guid)] = DbType.Guid;
//            typeMap[typeof(DateTime)] = DbType.DateTime;
//            typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
//            typeMap[typeof(byte[])] = DbType.Binary;
//            typeMap[typeof(byte?)] = DbType.Byte;
//            typeMap[typeof(sbyte?)] = DbType.SByte;
//            typeMap[typeof(short?)] = DbType.Int16;
//            typeMap[typeof(ushort?)] = DbType.UInt16;
//            typeMap[typeof(int?)] = DbType.Int32;
//            typeMap[typeof(uint?)] = DbType.UInt32;
//            typeMap[typeof(long?)] = DbType.Int64;
//            typeMap[typeof(ulong?)] = DbType.UInt64;
//            typeMap[typeof(float?)] = DbType.Single;
//            typeMap[typeof(double?)] = DbType.Double;
//            typeMap[typeof(decimal?)] = DbType.Decimal;
//            typeMap[typeof(bool?)] = DbType.Boolean;
//            typeMap[typeof(char?)] = DbType.StringFixedLength;
//            typeMap[typeof(Guid?)] = DbType.Guid;
//            typeMap[typeof(DateTime?)] = DbType.DateTime;
//            typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;