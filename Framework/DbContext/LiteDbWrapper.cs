using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.DbContext
{
    /// <summary>
    /// Wrapper created to be able to unit test LiteDatabase
    /// </summary>
    public class LiteDbWrapper : ILiteDbWrapper
    {
        public readonly ILiteDatabase _liteDb;

        public LiteDbWrapper()
        {
            _liteDb = new LiteDatabase("MainDB.db");
        }

        public ILiteCollection<T> GetCollection<T>(string name) => _liteDb.GetCollection<T>(name);
    }
}
