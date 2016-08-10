using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;

namespace MTChat.Client.ViewModels
{
    public class LoginViewModel : ViewModelBase, IModalDialogViewModel, IDataErrorInfo
    {
        public LoginViewModel()
        {
            _loginCommand = new Lazy<RelayCommand>(() => new RelayCommand(Login, () => IsValid));
            _exitCommand = new Lazy<RelayCommand>(() => new RelayCommand(Close));

            LoadSerttings();
        }

        #region Properties

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { Set(ref _userName, value); }
        }

        private string _serverIpAddress;
        public string ServerIpAddress
        {
            get { return _serverIpAddress; }
            set { Set(ref _serverIpAddress, value); }
        }

        private string _serverPort;
        public string ServerPort
        {
            get { return _serverPort; }
            set { Set(ref _serverPort, value); }
        }

        #endregion

        #region Commands

        private readonly Lazy<RelayCommand> _loginCommand;
        public ICommand LoginCommand => _loginCommand.Value;

        private void Login()
        {
            SaveSettings();
            DialogResult = true;
        }

        private readonly Lazy<RelayCommand> _exitCommand;
        public ICommand CloseCommand => _exitCommand.Value;

        private void Close()
        {
            DialogResult = false;
        }

        #endregion

        #region IDataErrorInfo members

        public string this[string propertyName] => GetValidationError(propertyName);

        public string Error => null;

        #endregion

        #region Validation

        public bool IsValid => ValidatedProperties.All(property => GetValidationError(property) == null);

        private static readonly string[] ValidatedProperties =
        {
            "UserName", "ServerIpAddress", "ServerPort"
        };

        private string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "UserName": error = ValidateUserName(); break;
                case "ServerIpAddress": error = ValidateServerIpAddress(); break;
                case "ServerPort": error = ValidateServerPort(); break;
            }

            return error;
        }

        private string ValidateUserName()
        {
            return String.IsNullOrEmpty(UserName) ? "Имя пользователя не может быть пустым" : null;
        }

        private string ValidateServerIpAddress()
        {
            return String.IsNullOrEmpty(ServerIpAddress) ? "Не указан адрес сервера" : null;
        }

        private string ValidateServerPort()
        {
            int val;
            return int.TryParse(ServerPort, out val) && val > 0 && val < 65536 ? null : "Указан некорректный порт";
        }

        #endregion

        #region IModalDialogViewModel members

        // Результат модального окна
        private bool? _dialogResult;
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        #endregion

        #region Settings

        private void LoadSerttings()
        {
            UserName = Properties.Settings.Default.UserName;
            ServerIpAddress = Properties.Settings.Default.ServerIpAddress;
            ServerPort = Properties.Settings.Default.ServerPort;
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.UserName = UserName;
            Properties.Settings.Default.ServerIpAddress = ServerIpAddress;
            Properties.Settings.Default.ServerPort = ServerPort;
            Properties.Settings.Default.Save();
        }

        #endregion
    }
}
