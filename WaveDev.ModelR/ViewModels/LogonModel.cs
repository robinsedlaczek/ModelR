using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using WaveDev.ModelR.Models;

namespace WaveDev.ModelR.ViewModels
{
    public class LogonModel : ViewModelBase
    {
        #region Private Fields

        private RelayCommand _loginCommand;
        private HubConnection _connection;
        private IHubProxy _proxy;

        #endregion

        #region Construction

        public LogonModel()
        {
            InitConnection();
            LoadSceneInformation();
        }

        #endregion

        #region Public Members

        public string ServerUri
        {
            get;
            set;
        }

        public int ServerPort
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public ICommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    // [RS] Log on takes place at startup only. So caching the login command is not needed. For the this time we cache it.
                    _loginCommand = new RelayCommand(() =>
                    {
                        // [RS] Login here!


                    }, () => true);
                }

                return _loginCommand;
            }
        }

        #endregion

        #region Private Members

        private async void InitConnection()
        {
            _connection = new HubConnection("http://localhost:8082/");
            _connection.TraceLevel = TraceLevels.All;
            _connection.TraceWriter = Console.Out;

            _proxy = _connection.CreateHubProxy("ModelRHub");

            //await _connection.Start();
            _connection.Start().Wait();
        }

        private async void LoadSceneInformation()
        {
            //var scenes = await _proxy.Invoke<IEnumerable<SceneInfoModel>>("GetAvailableScenes");
            var scenes = _proxy.Invoke<IEnumerable<SceneInfoModel>>("GetAvailableScenes").Result;
        }

        #endregion

    }
}
