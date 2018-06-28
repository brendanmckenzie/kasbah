using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Kasbah.Media;
using Kasbah.Media.Models;
using Npgsql;

namespace Kasbah.Provider.Npgsql
{
    public class MediaProvider : IMediaProvider
    {
        readonly NpgsqlSettings _settings;

        public MediaProvider(NpgsqlSettings settings)
        {
            _settings = settings;
        }

        public async Task CreateMediaAsync(Guid id, string fileName, string contentType)
        {
            const string Sql = "insert into media ( id, file_name, content_type ) values ( @id, @fileName, @contentType )";
            using (var connection = _settings.GetConnection())
            {
                await connection.ExecuteAsync(Sql, new { id, fileName, contentType });
            }
        }

        public async Task<IEnumerable<MediaItem>> ListMediaAsync()
        {
            const string Sql = "select id as Id, file_name as FileName, content_type as ContentType, created_at as Created from media order by created_at desc";
            using (var connection = _settings.GetConnection())
            {
                return await connection.QueryAsync<MediaItem>(Sql);
            }
        }

        public async Task<MediaItem> GetMediaAsync(Guid id)
        {
            const string Sql = "select id as Id, file_name as FileName, content_type as ContentType, created_at as Created from media where id = @id";
            using (var connection = _settings.GetConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<MediaItem>(Sql, new { id });
            }
        }

        public async Task DeleteMediaAsync(Guid id)
        {
            const string Sql = "delete from media where id = @id";
            using (var connection = _settings.GetConnection())
            {
                await connection.ExecuteAsync(Sql, new { id });
            }
        }

        public async Task<MediaItem> PutMediaAsync(MediaItem item)
        {
            const string Sql = "update media set file_name = @FileName, content_type = @ContentType, modified_at = now() where id = @Id";
            using (var connection = _settings.GetConnection())
            {
                await connection.ExecuteAsync(Sql, item);
            }

            return item;
        }
    }
}
