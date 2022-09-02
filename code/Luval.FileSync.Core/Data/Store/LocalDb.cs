using Luval.DataStore;
using Luval.DataStore.Database;
using Luval.DataStore.Database.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FileSync.Core.Data.Store
{
    public static class LocalDb
    {

        public static bool IsInialized { get; private set; }

        /// <summary>
        /// Initializes the local database
        /// </summary>
        public static void Initialize()
        {
            if (IsInialized) return;
            ForceInitialize();
        }

        /// <summary>
        /// Initializes the local database
        /// </summary>
        public static void ForceInitialize()
        {
            var dbFile = new FileInfo(GetLocation());
            if (dbFile.Directory != null && !dbFile.Directory.Exists) dbFile.Directory.Create();
            if (!dbFile.Exists) dbFile.Create();
            ValidateDatabase();
            IsInialized = true;
        }

        /// <summary>
        /// Creates a new Unit of Work Factory
        /// </summary>
        /// <returns></returns>
        public static IUnitOfWorkFactory CreateUoWFactory()
        {
            return new SqliteUnitOfWorkFactory(GetLocation());
        }

        /// <summary>
        /// Creates a new instance of a <see cref="IDataStore"/>
        /// </summary>
        public static IDatabaseStore CreateDataStore()
        {
            return new SqliteDataStore(GetLocation());
        }

        /// <summary>
        /// Gets the location of the database
        /// </summary>
        public static string GetLocation()
        {
            return Environment.CurrentDirectory + @"\Data\local.data";
        }

        private static void ValidateDatabase()
        {
            var db = CreateDataStore();
            if (!ValidateTable(db)) CreateTable(db);
        }

        private static bool ValidateTable(IDatabaseStore databaseStore)
        {
            var sql = "SELECT Id FROM MediaFile WHERE 1 = 2";
            var result = true;
            try
            {
                databaseStore.ExecuteScalar(sql);
            }
            catch
            { result = false; }

            return result;
        }

        private static void CreateTable(IDatabaseStore databaseStore)
        {
            var sql = @"
CREATE TABLE MediaFile (
	Id TEXT PRIMARY KEY,
	LocationInDevice TEXT NULL,
	DeviceName TEXT NULL,
    Hash TEXT NULL,
    ImageHash TEXT NULL,
    Format TEXT NULL,
    UtcFileCreatedOn TEXT NULL,
    UtcFileModifiedOn TEXT NULL,
    UtcImageTakenOn TEXT NULL,
    UtcUploadedOn TEXT NULL,
    UtcDeletedOn TEXT NULL,
    UtcProcessedOn TEXT NULL,
    UtcCreatedOn TEXT NULL,
    UtcUpdatedOn TEXT NULL,
    Longitude REAL NULL,
    Latitude REAL NULL,
    Altitude REAL NULL,
    Country TEXT NULL,
    Region1 TEXT NULL,
    Region2 TEXT NULL,
    Region3 TEXT NULL,
    City TEXT NULL
);
";
            try
            { databaseStore.Execute(sql); }
            catch (Exception ex)
            {

                throw new LocalSyncException("Unable to create local database", ex);
            }
        }




    }
}
