﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace EveScalper
{
    using SystemPair = Tuple<string, int>;

    public class StaticDataExport : IDisposable, IGameData
    {
        private readonly string path;
        private readonly SQLiteConnection connection;

        public SQLiteConnection Connection
        {
            get { return this.connection; }
        }

        public StaticDataExport(string path)
        {
            this.path = path;

            string parameters = @"Data Source=" + path + ";Version=3";
            this.connection = new SQLiteConnection(parameters);
            this.connection.Open();
        }

        public IReadOnlyList<SystemPair> systemList()
        {
            const string sql =
                @"SELECT DISTINCT
                solarSystemName, mapSolarSystems.solarSystemId
                FROM mapSolarSystems, staStations
                WHERE mapSolarSystems.solarSystemID = staStations.solarSystemID
                ORDER BY solarSystemName";

            SQLiteCommand command = new SQLiteCommand(sql, this.connection);

            SQLiteDataReader result = command.ExecuteReader();

            List<SystemPair> systems = new List<SystemPair>();

            while(result.Read())
            {
                systems.Add(
                    new SystemPair(
                        result["solarSystemName"].ToString(),
                        Convert.ToInt32(result["solarSystemId"])));
            }

            return systems.AsReadOnly();
        }

        public IReadOnlyList<int> inventoryIds()
        {
            {
                const string sql =
                    "SELECT typeID FROM invTypes WHERE invTypes.published = 1";
                SQLiteCommand command = new SQLiteCommand(sql, this.connection);

                SQLiteDataReader result = command.ExecuteReader();

                List<int> typeIds = new List<int>();

                while (result.Read())
                {
                    int current = result.GetInt32(0);
                    typeIds.Add(current);
                }
                return typeIds.AsReadOnly();
        }
    }

    #region IDisposable Support
    private bool disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                connection.Close();
                connection.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }
    #endregion
}

}
