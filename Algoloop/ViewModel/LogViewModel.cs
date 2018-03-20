﻿/*
 * Copyright 2018 Capnode AB
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Algoloop.Model;
using Algoloop.Service;
using QuantConnect.Configuration;
using QuantConnect.Logging;
using QuantConnect.Util;

namespace Algoloop.ViewModel
{
    public class LogViewModel
    {
        public SyncObservableCollection<LogItem> Logs { get; private set; }

        public LogViewModel()
        {
            Logs = new SyncObservableCollection<LogItem>();
            Log.DebuggingEnabled = Config.GetBool("debug-mode");
            Log.LogHandler = Composer.Instance.GetExportedValueByTypeName<ILogHandler>(Config.Get("log-handler", "CompositeLogHandler"));
            ILogItemHandler logService = Log.LogHandler as ILogItemHandler;
            if (logService != null)
            {
                logService.Connect((item) => Logs.Add(item));
            }
        }
    }
}
