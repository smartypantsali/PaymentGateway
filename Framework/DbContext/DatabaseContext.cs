using LiteDB;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace Framework.DbContext
{
    /// <summary>
    /// Database access class for CRUD operations
    /// </summary>
    public class DatabaseContext : IDatabaseContext
    {
        public readonly ILiteDbWrapper _liteDb;

        public DatabaseContext(ILiteDbWrapper liteDb)
        {
            _liteDb = liteDb;
        }

        /// <summary>
        /// Insert a type of T object into DB
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbObject"></param>
        /// <returns></returns>
        public long Insert<T>(T dbObject)
            where T : class
        {
            var type = typeof(T);

            var result = _liteDb.GetCollection<T>(type.Name)
            .Insert(dbObject);

            Log.Information($"Inserted type {type.Name}, with result of {result.AsInt64}");

            return result.AsInt64;
        }

        /// <summary>
        /// Update a type of T object in DB
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool Update<T>(T dbObject)
            where T : class
        {
            var type = typeof(T);

            var updated = _liteDb.GetCollection<T>(type.Name)
                .Update(dbObject);

            Log.Information($"Updated object {@dbObject}", dbObject);

            return updated;
        }

        /// <summary>
        /// Get all type of T object from DB
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAll<T>()
            where T : class
        {
            var type = typeof(T);

            var res = _liteDb.GetCollection<T>(type.Name)
            .FindAll();

            Log.Information($"Found {res.Count()} results of type {type.Name}");

            return res;
        }

        /// <summary>
        /// Get object of T by Uid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <returns></returns>
        public T GetByUid<T>(string uid)
            where T : class
        {
            var type = typeof(T);

            var @object = _liteDb.GetCollection<T>(type.Name)
            .Find(BsonExpression.Create($"$.Uid = '{uid}'")).FirstOrDefault();

            Log.Information($"Found object {@object} from Uid", @object);

            return @object;
        }
    }
}
