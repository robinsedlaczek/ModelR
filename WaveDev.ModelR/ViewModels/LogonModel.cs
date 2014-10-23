using System.Windows;
using GalaSoft.MvvmLight;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Controls;
using System.Windows.Input;
using WaveDev.ModelR.Communication;
using WaveDev.ModelR.Shared.Models;
using System.Linq.Expressions;
using WaveDev.ModelR.Shared;
using System.Linq;

namespace WaveDev.ModelR.ViewModels
{
    public class LogonModel : ViewModelBase
    {
        #region Private Fields

        private RelayCommand _loginCommand;
        private ModelRHubClientProxy _proxy;
        private string _serverUrl;

        #endregion

        #region Construction

        public LogonModel()
        {
            _serverUrl = Constants.ModelRServerUrl;
//            _proxy = ModelRHubClientProxy.GetInstance(_serverUrl);

            //SelectedScene = _proxy.Scenes.FirstOrDefault();
        }

        #endregion

        #region Public Members

        public string ServerUrl
        {
            get
            {
                return _serverUrl;
            }

            set
            {
                if (_serverUrl != value)
                {
                    Set(ref _serverUrl, value);

                    // [RS] Get new proxy instance pointing to new server url.
                    _proxy = ModelRHubClientProxy.GetInstance(_serverUrl);

                }
            }
        }

        public string UserName
        {
            get;
            set;
        }

        public ICommand InitializeCommunicationCommand
        {
            get
            {
                return new RelayCommand(parameter =>
                {
                    try
                    {
                        _proxy = ModelRHubClientProxy.GetInstance();

                        SelectedScene = _proxy.Scenes.FirstOrDefault();
                    }
                    catch (InvalidOperationException exception)
                    {
                        // TODO: [RS] Use MVVM Light here! Open message box from window, not here in the model!
                        //MessageBox.Show(exception.InnerException.InnerException.Message, "ModelR - Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        Application.Current.Shutdown();
                    }
                }, () => true);
            }
        }

        public ICommand CancelLoginCommand
        {
            get
            {
                return new RelayCommand(parameter => Application.Current.Shutdown(), () => true);
            }
        }

        public ICommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    // [RS] Log on takes place at startup only. So caching the login command is not needed. For the this time we cache it.
                    _loginCommand = new RelayCommand(parameter =>
                    {
                        // [RS] Break the MVVM pattern here, because the Password-property in the WPF PasswordBox cannot be data bound
                        //      due to security reasons. So we pass the control to the model for query the password from the control directly.

                        var passwordBox = parameter as PasswordBox;
                        var password = string.Empty;

                        if (passwordBox != null)
                            password = passwordBox.Password;

                        _proxy.Login(UserName, password, SelectedScene.Id);

                    }, () => true);
                }

                return _loginCommand;
            }
        }

        public IEnumerable<SceneInfoModel> Scenes
        {
            get
            {
                return _proxy.Scenes;
            }
        }

        public SceneInfoModel SelectedScene
        {
            get;
            set;
        }

        #endregion

        #region Private Members


        #endregion

    }
}
