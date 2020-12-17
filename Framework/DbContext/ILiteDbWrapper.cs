using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.DbContext
{
    /// <summary>
    /// Wrapper created to be able to unit test LiteDatabase
    /// </summary>
    public interface ILiteDbWrapper
    {
        ILiteCollection<T> GetCollection<T>(string name);
    }
}
