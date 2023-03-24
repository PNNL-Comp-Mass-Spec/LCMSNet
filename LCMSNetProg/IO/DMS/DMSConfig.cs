using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using LcmsNetSDK.Logging;
using PRISMDatabaseUtils;

namespace LcmsNet.IO.DMS
{
    public class DMSConfig : IEquatable<DMSConfig>
    {
        public const string DefaultDatabaseServer = "Gigasax";
        public const string DefaultDatabaseName = "DMS5";
        public const string DefaultDatabaseSchemaPrefix = ""; // if non-empty, always needs to end with a '.'
        public const string DefaultUsername = "LCMSNetUser";
        public const string DefaultEncodedPassword = "Mprptq3v";
        public const DbServerTypes DefaultDatabaseSoftware =  DbServerTypes.MSSQLServer;

        public DMSConfig()
        {
            DatabaseServer = "";
            DatabaseName = "";
            DatabaseSchemaPrefix = "";
            Username = "";
            EncodedPassword = "";
            DatabaseSoftware = DbServerTypes.Undefined;
            databaseServerSoftware = DatabaseSoftware.ToString();
        }

        public void LoadDefaults()
        {
            DatabaseServer = DefaultDatabaseServer;
            DatabaseName = DefaultDatabaseName;
            DatabaseSchemaPrefix = DefaultDatabaseSchemaPrefix;
            Username = DefaultUsername;
            EncodedPassword = DefaultEncodedPassword;
            DatabaseSoftware = DefaultDatabaseSoftware;
            databaseServerSoftware = DatabaseSoftware.ToString();

            if (!string.IsNullOrWhiteSpace(DatabaseSchemaPrefix) && !DatabaseSchemaPrefix.EndsWith("."))
            {
                // Not considering this a config file issue, just automatically adding it.
                DatabaseSchemaPrefix += ".";
            }
        }

        private string databaseServerSoftware;

        public string DatabaseServer { get; set; }

        public string DatabaseName { get; set; }

        public string DatabaseSchemaPrefix { get; set; }

        public string Username { get; set; }

        public string EncodedPassword { get; set; }

        public string DatabaseServerSoftware
        {
            get => DatabaseSoftware.ToString();
            set
            {
                databaseServerSoftware = value;
                if (Enum.TryParse(value, true, out DbServerTypes parsed))
                {
                    DatabaseSoftware = parsed;
                }
                else
                {
                    DatabaseSoftware = DbServerTypes.Undefined;
                }
            }
        }

        [JsonIgnore]
        public DbServerTypes DatabaseSoftware { get; set; }

        public override string ToString()
        {
            var software = DatabaseSoftware == DbServerTypes.Undefined ? DatabaseSoftware + $"(read '{databaseServerSoftware}')" : DatabaseSoftware.ToString();
            var schema = string.IsNullOrWhiteSpace(DatabaseSchemaPrefix) ? "" : $", SchemaPrefix '{DatabaseSchemaPrefix}'";
            return $"Server '{DatabaseServer}', Database '{DatabaseName}'{schema}, Username '{Username}', Password (encoded) '{EncodedPassword}', Software is {software}";
        }

        public static DMSConfig FromJson(string path)
        {
            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            var jsonText = File.ReadAllText(path);
            return JsonSerializer.Deserialize<DMSConfig>(jsonText, jsonOptions);
        }

        public void ToJson(string path)
        {
            /* NOTE: This is the appropriate code, with the catch that it doesn't include comments
            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true,
            };

            var jsonText = JsonSerializer.Serialize(this, jsonOptions);
            try
            {
                File.WriteAllText(path, jsonText);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(LogLevel.Info, "Exception attempting to write database configuration file", ex);
                // Do nothing; not ideal, but it's okay.
            }
            */

            // Instead, generate the json manually
            var lines = new List<string>()
            {
                "{",
                "    // databaseServer is the server hosting DMS (defaults to Gigasax if missing)",
                $"    \"databaseServer\" : \"{DatabaseServer}\",",
                "    // database is the name of the database to connect to",
                $"    \"databaseName\" : \"{DatabaseName}\",",
                "    // databaseSchemaPrefix is the schema prefix for the database; can be an empty string for default/unspecified schema. If not empty, must end with a '.' (period)",
                $"    \"databaseSchemaPrefix\" : \"{DatabaseSchemaPrefix}\",",
                "    // username is the DMS username for SQL server user (default is LCMSNetUser)",
                $"    \"username\" : \"{Username}\",",
                "    // encodedPassword is the encoded DMS password for the SQL server user",
                $"    \"encodedPassword\" : \"{EncodedPassword}\",",
                "    // Database Server Software is a reference to the database software running on the server; currently supports \"PostgreSQL\" and \"MSSQLServer\"",
                $"    \"databaseServerSoftware\" : \"{DatabaseServerSoftware}\"",
                "}"
            };

            try
            {
                File.WriteAllLines(path, lines);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(LogLevel.Info, "Exception attempting to write database configuration file", ex);
                // Do nothing; not ideal, but it's okay.
            }
        }

        /// <summary>
        /// Checks the config values
        /// </summary>
        /// <returns>true if all values okay; false if any values were reset to a default value</returns>
        public bool ValidateConfig()
        {
            var changed = false;
            if (string.IsNullOrWhiteSpace(DatabaseServer))
            {
                DatabaseServer = DefaultDatabaseServer;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(DatabaseName))
            {
                DatabaseName = DefaultDatabaseName;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(DatabaseSchemaPrefix) && !DatabaseSchemaPrefix.Equals(DefaultDatabaseSchemaPrefix))
            {
                DatabaseSchemaPrefix = DefaultDatabaseSchemaPrefix;
                changed = true;
            }

            if (!string.IsNullOrWhiteSpace(DatabaseSchemaPrefix) && !DatabaseSchemaPrefix.EndsWith("."))
            {
                // Not considering this a config file issue, just automatically adding it.
                DatabaseSchemaPrefix += ".";
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                Username = DefaultUsername;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(EncodedPassword))
            {
                EncodedPassword = DefaultEncodedPassword;
                changed = true;
            }

            if (DatabaseSoftware == DbServerTypes.Undefined)
            {
                DatabaseSoftware = DefaultDatabaseSoftware;
                changed = true;
            }

            return !changed;
        }

        public bool Equals(DMSConfig other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DatabaseServer == other.DatabaseServer && DatabaseName == other.DatabaseName && DatabaseSchemaPrefix == other.DatabaseSchemaPrefix && Username == other.Username && EncodedPassword == other.EncodedPassword && DatabaseSoftware == other.DatabaseSoftware;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DMSConfig)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DatabaseServer, DatabaseName, DatabaseSchemaPrefix, Username, EncodedPassword, (int)DatabaseSoftware);
        }
    }
}
