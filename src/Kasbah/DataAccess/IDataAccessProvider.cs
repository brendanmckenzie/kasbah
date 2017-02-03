using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kasbah.Models;

namespace Kasbah.DataAccess
{
    public interface IDataAccessProvider
    {
        Task<long> PutEntryAsync<T>(string index, Guid id, T data, Guid? parent = null, bool waitForCommit = true);
        Task<long> PutEntryAsync<T>(string index, Guid id, IDictionary<string, object> data, Guid? parent = null, bool waitForCommit = true);
        Task<long> PutEntryAsync(string index, Guid id, Type type, IDictionary<string, object> data, Guid? parent = null, bool waitForCommit = true);
        Task DeleteEntryAsync<T>(string index, Guid id);
        Task<IEnumerable<EntryWrapper<T>>> QueryEntriesAsync<T>(string index, object query = null, int? skip = 0, int? take = 10);
        Task<EntryWrapper<T>> GetEntryAsync<T>(string index, Guid id, long? version = null);
        Task<EntryWrapper<T>> GetEntryAsync<T>(string index, Guid id, Type type, long? version = null);
        Task EnsureIndexExists(string index);
        Task PutTypeMapping(string index, Type type);
    }
}
