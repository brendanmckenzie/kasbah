using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.DataAccess;
using Kasbah.Logging.Models;
using Microsoft.Extensions.Logging;

namespace Kasbah.Logging
{
    public class LoggingService
    {
        static class Indicies
        {
            public const string Logging = "logging";
        }

        readonly IDataAccessProvider _dataAccessProvider;
        readonly ILogger _log;
        public LoggingService(ILoggerFactory loggerFactory, IDataAccessProvider dataAccessProvider)
        {
            _log = loggerFactory.CreateLogger<LoggingService>();
            _dataAccessProvider = dataAccessProvider;
        }

        public async Task HeartbeatAsync(Guid instance)
        {
            await _dataAccessProvider.PutEntryAsync(Indicies.Logging, Guid.NewGuid(), new Heartbeat
            {
                Instance = instance
            });
        }

        public async Task RegisterInstanceAsync(Guid id, DateTime started)
        {
            await _dataAccessProvider.PutEntryAsync(Indicies.Logging, Guid.NewGuid(), new Instance
            {
                Id = id,
                Started = started
            });
        }

        public async Task<IEnumerable<Instance>> ListActiveInstances()
        {
            var query = new
            {
                range = new
                {
                    Created = new
                    {
                        gt = "now-10m",
                        lt = "now+10m"
                    }
                }
            };
            var entries = await _dataAccessProvider.QueryEntriesAsync<Instance>(Indicies.Logging, query);

            return entries.Select(ent => ent.Source);
        }

        public async Task InitialiseAsync()
        {
            _log.LogDebug($"Initialising {nameof(LoggingService)}");
            await _dataAccessProvider.EnsureIndexExists(Indicies.Logging);
        }
    }
}