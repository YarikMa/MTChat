using System;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using MTChat.Client.Services;
using MTChat.Client.ViewModels;
using MTChat.Client.Views;
using MvvmDialogs;

namespace MTChat.Client
{
    /// <summary>
    /// Загрузчик приложения
    /// </summary>
    internal class UnityContainerBootstrapper
    {
        private readonly UnityContainer _unityContainer;

        public UnityContainerBootstrapper()
        {
            _unityContainer = new UnityContainer();
            RegisterTypes();
            Application.Current.Exit += OnApplicationExit;
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(_unityContainer));
        }

        private void RegisterTypes()
        {
            // ViewModels
            _unityContainer.RegisterType<MainViewModel>();
            _unityContainer.RegisterType<LoginViewModel>();

            // Services
            _unityContainer.RegisterType(typeof(ILocalChatService), typeof(LocalChatService), new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<IDialogService>(new InjectionFactory(c => new DialogService()));
        }

        public void Run()
        {
            new Main().ShowDialog();
        }

        private void OnApplicationExit(object sender, ExitEventArgs exitEventArgs)
        {
            // При закрытии приложения нужно попробовать выйти из чата
            // при этом игнорируем все возможные исключения
            try
            {
                _unityContainer.Resolve<ILocalChatService>().Leave();
            }
            catch (Exception)
            {
            }
        }
    }
}
