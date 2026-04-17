using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QuanLySinhVien.DAL
{
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["QuanLySinhVien"]?.ConnectionString;
        private static readonly IReadOnlyDictionary<string, string> StoredProcedureAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Không có alias — gọi trực tiếp bằng tên SP chính xác
        };

        private static SqlConnection CreateConnection()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new InvalidOperationException("Connection string 'QuanLySinhVien' is not configured.");
            return new SqlConnection(ConnectionString);
        }

        private static SqlCommand CreateCommand(SqlConnection connection, string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            var actualCommandText = commandType == CommandType.StoredProcedure && StoredProcedureAliases.TryGetValue(commandText, out var alias)
                ? alias
                : commandText;

            var command = new SqlCommand(actualCommandText, connection) { CommandType = commandType };
            if (parameters != null && parameters.Length > 0)
                command.Parameters.AddRange(parameters);

            return command;
        }

        public static DataTable ExecuteDataTable(string storedProcedure, params SqlParameter[] parameters)
        {
            using (var connection = CreateConnection())
            using (var command = CreateCommand(connection, storedProcedure, CommandType.StoredProcedure, parameters))
            using (var adapter = new SqlDataAdapter(command))
            {
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public static DataTable ExecuteTextDataTable(string sql, params SqlParameter[] parameters)
        {
            using (var connection = CreateConnection())
            using (var command = CreateCommand(connection, sql, CommandType.Text, parameters))
            using (var adapter = new SqlDataAdapter(command))
            {
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public static int ExecuteNonQuery(string storedProcedure, params SqlParameter[] parameters)
        {
            using (var connection = CreateConnection())
            using (var command = CreateCommand(connection, storedProcedure, CommandType.StoredProcedure, parameters))
            {
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int ExecuteTextNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (var connection = CreateConnection())
            using (var command = CreateCommand(connection, sql, CommandType.Text, parameters))
            {
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string storedProcedure, params SqlParameter[] parameters)
        {
            using (var connection = CreateConnection())
            using (var command = CreateCommand(connection, storedProcedure, CommandType.StoredProcedure, parameters))
            {
                connection.Open();
                return command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Thực thi stored procedure trả về nhiều result set.
        /// </summary>
        public static DataTable[] ExecuteMultipleResultSets(string storedProcedure, int resultSetCount, params SqlParameter[] parameters)
        {
            var results = new DataTable[resultSetCount];
            for (int i = 0; i < resultSetCount; i++) results[i] = new DataTable();

            using (var connection = CreateConnection())
            using (var command = CreateCommand(connection, storedProcedure, CommandType.StoredProcedure, parameters))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    for (int i = 0; i < resultSetCount; i++)
                    {
                        results[i].Load(reader);
                        if (i < resultSetCount - 1 && !reader.NextResult()) break;
                    }
                }
            }
            return results;
        }
    }
}
