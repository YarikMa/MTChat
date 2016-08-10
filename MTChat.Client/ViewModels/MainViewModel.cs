using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MTChat.Client.Services;
using MTChat.Client.Wcf;
using MTChat.Common;
using MTChat.Common.Messages;
using MvvmDialogs;

namespace MTChat.Client.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ILocalChatService _localChatService;
        private readonly IDialogService _dialogService;
        private readonly object _personsCollectionLock = new object();
        private Person _person;

        public MainViewModel(ILocalChatService localChatService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _localChatService = localChatService;
            _localChatService.ChatCallbackEvent += OnChatCallbackEvent;

            _loginCommand = new Lazy<RelayCommand>(() => new RelayCommand(Login, () => !IsConnected));
            _logoutCommand = new Lazy<RelayCommand>(() => new RelayCommand(Logout, () => IsConnected));
            _exitCommand = new Lazy<RelayCommand>(() => new RelayCommand(Exit));
            _sendMessageCommand = new Lazy<RelayCommand>(() => new RelayCommand(SendMessage, () => IsConnected && !String.IsNullOrEmpty(Message)));

            // позволяет обращаться к ObservableCollection's из не основных (не UI) потоков
            BindingOperations.EnableCollectionSynchronization(Persons, _personsCollectionLock);
        }

        #region Callback methods

        private void Receive(Person person, string message)
        {
            WriteToChat($"{person.Name}: {message}");
        }

        private void ReceiveWisper(Person person, string message)
        {
            WriteToChat($"{person.Name} шепнул: {message}");
        }

        private void UserEnter(Person person)
        {
            Persons.Add(person);
            WriteToChat($"{person.Name} зашел в чат");
        }

        private void UserLeave(Person person)
        {
            Persons.Remove(person);
            WriteToChat($"{person.Name} покинул чат");
        }

        private void DisconnectByTimeout()
        {
            WriteToChat("Отключение по таймауту");
            Logout();
        }

        private void WriteToChat(string message)
        {
            Chat += $"({DateTime.Now.ToString("HH:mm:ss")}) {message} {Environment.NewLine}";
        }

        #endregion

        #region Properties

        private string _chat;
        public string Chat
        {
            get { return _chat; }
            set { Set(ref _chat, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

        public ObservableCollection<Person> Persons { get; } = new ObservableCollection<Person>();

        private Person _selectedPerson;
        public Person SelectedPerson
        {
            get { return _selectedPerson; }
            set { Set(ref _selectedPerson, value); }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set { Set(ref _isConnected, value); }
        }

        #endregion

        #region Commands

        private readonly Lazy<RelayCommand> _loginCommand;
        public ICommand LoginCommand => _loginCommand.Value;

        private void Login()
        {
            var loginvm = new LoginViewModel();
            var result = _dialogService.ShowDialog(this, loginvm);
            if (result != true) return;

            try
            {
                _person = new Person { Name = loginvm.UserName };
                Persons.Clear();
                var personList = _localChatService.Join(_person, loginvm.ServerIpAddress, loginvm.ServerPort);
                personList.ToList().ForEach(p => Persons.Add(p));
                IsConnected = true;
            }
            catch (Exception e)
            {
                Disconnect();
                _dialogService.ShowMessageBox(this, e.Message, "Ошибка при выполнении операции", MessageBoxButton.OK,
                    MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private readonly Lazy<RelayCommand> _logoutCommand;
        public ICommand LogoutCommand => _logoutCommand.Value;

        private void Logout()
        {
            try
            {
                _localChatService.Leave();
            }
            catch (CommunicationObjectFaultedException)
            {
            }
            finally
            {
                Disconnect();
            }
            
        }

        private void Disconnect()
        {
            Persons.Clear();
            _person = null;
            IsConnected = false;
        }

        private readonly Lazy<RelayCommand> _exitCommand;
        public ICommand ExitCommand => _exitCommand.Value;

        private void Exit()
        {
            Application.Current.Shutdown();
        }

        private readonly Lazy<RelayCommand> _sendMessageCommand;
        public ICommand SendMessageCommand => _sendMessageCommand.Value;

        private void SendMessage()
        {
            try
            {
                if (SelectedPerson == null)
                {
                    _localChatService.Say(new TextMessage(_person, Message));
                }
                else
                {
                    _localChatService.Whisper(new PersonalTextMessage(_person, SelectedPerson, Message));
                }
            }
            catch (Exception e)
            {
                Disconnect();
                _dialogService.ShowMessageBox(this, e.Message, "Ошибка при выполнении операции", MessageBoxButton.OK,
                    MessageBoxImage.Error, MessageBoxResult.OK);
            }

            Message = String.Empty;
        }

        #endregion

        private void OnChatCallbackEvent(object sender, ProxyCallbackEventArgs e)
        {
            switch (e.CallbackType)
            {
                case CallbackType.Receive:
                    Receive(e.Person, e.Message);
                    break;
                case CallbackType.ReceiveWhisper:
                    ReceiveWisper(e.Person, e.Message);
                    break;
                case CallbackType.UserEnter:
                    UserEnter(e.Person);
                    break;
                case CallbackType.UserLeave:
                    UserLeave(e.Person);
                    break;
                case CallbackType.DisconnectByTimeout:
                    DisconnectByTimeout();
                    break;
            }
        }
    }
}