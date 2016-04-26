using System.Windows;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using WaveDev.ModelR.Communication;
using WaveDev.ModelR.Shared.Models;
using WaveDev.ModelR.Shared;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using WaveDev.ModelR.Messages;

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

            InitializeCommunicationCommand.Execute(null);
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

                    // [RS] Reinitialize connection because of changed server adress.
                    InitializeCommunicationCommand.Execute(null);
                }
            }
        }

        public string UserName
        {
            get;
            set;
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

                        try
                        {
                            _proxy.Login(UserName, password, SelectedScene.Id);
                        }
                        catch (UserNotAuthorizedException exception)
                        {
                            Messenger.Default.Send<ExceptionCausedApplicationShutdownMessage>(
                                new ExceptionCausedApplicationShutdownMessage()
                            {
                                Exception = exception,
                                ShowMessageToUser = true
                            });
                        }

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

        private ICommand InitializeCommunicationCommand
        {
            get
            {
                return new RelayCommand(parameter =>
                {
                    try
                    {
                        _proxy = ModelRHubClientProxy.GetInstance(_serverUrl);

                        SelectedScene = _proxy.Scenes.FirstOrDefault();
                    }
                    catch (InvalidOperationException exception)
                    {
                        Messenger.Default.Send<ExceptionCausedApplicationShutdownMessage>(
                            new ExceptionCausedApplicationShutdownMessage()
                        {
                            Exception = exception,
                            ShowMessageToUser = true
                        });
                    }
                }, () => true);
            }
        }

        #endregion

    }
}
