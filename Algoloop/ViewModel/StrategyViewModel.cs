using System;
using System.Collections.Generic;
using System.IO;
using Algoloop.Model;
using Newtonsoft.Json;
using QuantConnect.Configuration;
using QuantConnect.Lean.Engine.Server;
using QuantConnect.Util;

namespace Algoloop.ViewModel
{
    public class StrategyViewModel
    {
        private readonly ILeanManager _leanManager;
        public SyncObservableCollection<Strategy> Strategies { get; private set; }

        public StrategyViewModel()
        {
            Strategies = new SyncObservableCollection<Strategy>();
            _leanManager = Composer.Instance.GetExportedValueByTypeName<ILeanManager>(Config.Get("lean-manager-type", "LocalLeanManager"));
        }

        internal void Read(string fileName)
        {
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                List<Strategy> strategies = JsonConvert.DeserializeObject<List<Strategy>>(json);
            }
        }

        internal void Save(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
