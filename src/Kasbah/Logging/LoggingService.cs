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

        public async Task HeartbeatAsync(Heartbeat heartbeat)
        {
            _log.LogDebug(nameof(HeartbeatAsync));
            await _dataAccessProvider.PutEntryAsync(Indicies.Logging, Guid.NewGuid(), heartbeat);
        }

        public async Task RegisterInstanceAsync(Guid id, DateTime started)
        {
            _log.LogDebug(nameof(RegisterInstanceAsync));
            await _dataAccessProvider.PutEntryAsync(Indicies.Logging, Guid.NewGuid(), new Instance
            {
                Id = id,
                Started = started
            });
        }

        public async Task<IEnumerable<InstanceStatus>> ListActiveInstances()
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

            var heartbeats = (await _dataAccessProvider.QueryEntriesAsync<Heartbeat>(Indicies.Logging, query))
                .Select(ent => ent.Source);

            var instances = heartbeats.Any() ? (await _dataAccessProvider.QueryEntriesAsync<Instance>(Indicies.Logging, new
            {
                @bool = new
                {
                    should = heartbeats.Select(ent => ent.Instance).Distinct().Select(ent => new
                    {
                        match = new Dictionary<string, Guid> {
                            { "Id", ent }
                        }
                    })
                }
            })).Select(ent => ent.Source) : Enumerable.Empty<Instance>();

            return instances
                .Select(ent => new InstanceStatus
                {
                    Id = ent.Id,
                    Started = ent.Started,
                    Heartbeat = heartbeats.Where(hb => hb.Instance == ent.Id).Max(hb => hb.Created),
                    RequestsTotal = heartbeats.Where(hb => hb.Instance == ent.Id).Max(hb => hb.RequestsTotal),
                    RequestsLatest = heartbeats.Where(hb => hb.Instance == ent.Id).Max(hb => hb.RequestsLatest)
                });
        }

        public async Task InitialiseAsync()
        {
            _log.LogDebug($"Initialising {nameof(LoggingService)}");
            await _dataAccessProvider.EnsureIndexExists(Indicies.Logging);
        }
    }

    public class InstanceStatus
    {
        public Guid Id { get; set; }
        public DateTime Started { get; set; }
        public DateTime Heartbeat { get; set; }
        public ulong RequestsTotal { get; set; }
        public ulong RequestsLatest { get; set; }
    }
}