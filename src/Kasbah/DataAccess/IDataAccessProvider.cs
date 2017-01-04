using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kasbah.Models;

namespace Kasbah.DataAccess
{
    public interface IDataAccessProvider
    {
        Task PutEntryAsync<T>(string index, Guid id, T data, Guid? parent = null);
        Task DeleteEntryAsync<T>(string index, Guid id);
        Task<IEnumerable<EntryWrapper<T>>> QueryEntriesAsync<T>(string index, object query = null);
        Task<EntryWrapper<T>> GetEntryAsync<T>(string index, Guid id, int? version = null);
        Task EnsureIndexExists(string index);
    }
}
