using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace ClinicServiceDBTools.Extensions
{
    internal class DBTypes
    {
        private readonly DbConnectionStringBuilder _dbConnBuilder = new DbConnectionStringBuilder();

        internal DBType DbTypeParse(string connstring)
        {
            _dbConnBuilder.ConnectionString = connstring;

            if (_dbConnBuilder.TryGetValue("Data Source", out object value))
            {
                return DBType.MSSQL;
            }

            if (_dbConnBuilder.TryGetValue("Host", out value))
            {
                return DBType.PGSQL;
            }

            return DBType.Unknown;
        }
    }

    public enum DBType
    {
        [DBName("Unknown")]
        Unknown,

        [DBName("Microsoft SQL Server")]
        MSSQL,

        [DBName("Postgre SQL")]
        PGSQL
    }

    public class DBNameAttribute : Attribute
    { 
        public string DisplayNameAttribute { get; private set; }

        public DBNameAttribute(string displayNameAttribute)
        { 
            DisplayNameAttribute = displayNameAttribute;
        }
    }
}
