/*
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Algoloop.Model;
using Algoloop.Service;
using Algoloop.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Newtonsoft.Json;
using QuantConnect.Logging;

namespace Algoloop.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly ILeanEngineService _leanEngineService;
        private bool _isBusy;
        private string _fileName;

        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand SaveAsCommand { get; private set; }
        public RelayCommand<Window> ExitCommand { get; private set; }
        public RelayCommand AboutCommand { get; private set; }
        public RelayCommand RunCommand { get; private set; }

        public LogViewModel LogViewModel { get; private set; }
        public StrategyViewModel StrategyViewModel { get; private set; }

        /// <summary>
        /// Mark ongoing operation
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged("IsBusy");
                RunCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(ILeanEngineService leanEngineService, LogViewModel logViewModel, StrategyViewModel strategyViewModel)
        {
            _leanEngineService = leanEngineService;
            LogViewModel = logViewModel;
            StrategyViewModel = strategyViewModel;

            OpenCommand = new RelayCommand(() => OpenFile(), () => !IsBusy);
            SaveCommand = new RelayCommand(() => SaveFile(), () => !IsBusy && !string.IsNullOrEmpty(_fileName) );
            SaveAsCommand = new RelayCommand(() => SaveAsFile(), () => !IsBusy);
            ExitCommand = new RelayCommand<Window>(window => Close(window), window => !IsBusy);
            AboutCommand = new RelayCommand(() => About(), () => !IsBusy);
            RunCommand = new RelayCommand(async () => await RunBacktest(), () => !IsBusy);
        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"c:\temp\";
            openFileDialog.Filter = "Strategy file (*.str)|*.str|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _fileName = openFileDialog.FileName;
                Log.Trace(_fileName);
                StrategyViewModel.Read(_fileName);
            }
        }

        private void SaveFile()
        {
            Debug.Assert(!string.IsNullOrEmpty(_fileName));
            Log.Trace(_fileName);
            StrategyViewModel.Save(_fileName);
        }

        private void SaveAsFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = @"c:\temp\";
            saveFileDialog.Filter = "Strategy file (*.str)|*.str|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                _fileName = saveFileDialog.FileName;
                Log.Trace(_fileName);
                StrategyViewModel.Save(_fileName);
            }
        }

        private void Close(Window window)
        {
            Debug.Assert(window != null);

            IsBusy = true;
            window.Close();
            IsBusy = false;
        }

        private void About()
        {
            var about = new About();
            about.ShowDialog();
        }

        private async Task RunBacktest()
        {
            IsBusy = true;
            await Task.Run(() => _leanEngineService.Run());
            IsBusy = false;
        }
    }
}