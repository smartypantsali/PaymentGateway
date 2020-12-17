using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.DbContext
{
    /// <summary>
    /// Interface for DbContext CRUD operations
    /// </summary>
    public interface IDatabaseContext
    {
        /// <summary>
        /// Insert a type of T object into DB
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbObject"></param>
        /// <returns></returns>
        long Insert<T>(T dbObject) where T : class;

        /// <summary>
        /// Update a type of T object in DB
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <returns></returns>
        bool Update<T>(T dbObject) where T : class;

        /// <summary>
        /// Get all type of T object from DB
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetAll<T>() where T : class;

        /// <summary>
        /// Get object of T by Uid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <returns></returns>
        T GetByUid<T>(string uid) where T : class;
    }
}
