using Shared.Core;
using Shared.Interfaces;
using SharedBinance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharedBinance.Services
{
    public sealed class AccountLoader
    {

        private readonly ILogger _logger;
        public AccountLoader(ILogger logger)
            => _logger = logger;

        private static SlaveIdentity ConvertSettings(BackEndClient slave)
            => new SlaveIdentity
            {
                ApiKey = slave.Settings.Get("APIKey") ?? "NONE",
                ApiSecret = slave.Settings.Get("APISecret") ?? "NONE",
                Name = slave.Name,
                Mode = slave.Settings.Get("Mode") ?? "NONE",
                ModeValue = slave.Settings.Get("ModeValue") ?? "1",
                CopyMasterOrders = slave.Settings.Get("CopyMasterOrders") ?? "true",
                Url = slave.Settings.Get("Url") ?? "NONE",
                Wss = slave.Settings.Get("Wss") ?? "NONE",
            };

        private static FileStream CreateNewFile(string name)
        {
            if (File.Exists(name))
            {
                File.Delete(name);
            }
            return File.Create(name);
        }
        public void SaveToFile(Dictionary<string, BackEndClient> accounts)
        {
            try
            {
                using (var fs = CreateNewFile("accounts.ini"))
                {
                    Utf8Json.JsonSerializer.Serialize(fs, accounts.Select(i => ConvertSettings(i.Value)));
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, "AccountLoader.SaveToFile", null);
                _logger.Log(LogPriority.Debug, ex, "AccountLoader.SaveToFile", null);
            }
        }
        public List<SlaveIdentity> LoadFromFile()
        {
            var slaveIdentities = new List<SlaveIdentity>();
            try
            {
                if (File.Exists("accounts.ini"))
                {
                    using (var fs = new FileStream("accounts.ini", FileMode.Open, FileAccess.Read))
                    {
                        slaveIdentities = Utf8Json.JsonSerializer.Deserialize<List<SlaveIdentity>>(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, "AccountLoader.LoadFromFile", null);
                _logger.Log(LogPriority.Debug, ex, "AccountLoader.LoadFromFile", null);
            }
            return slaveIdentities;
        }
    }
}
