using System;
using System.Linq;
using MTChat.Common;

namespace MTChat.Client.Wcf
{
    /// <summary>
    /// Обработчик ответов от сервера
    /// </summary>
    public static class WcfResponseHandler
    {
        public static void ResponseHandle<TResult>(TResult result)
            where TResult : OperationResult
        {
            if (result == null)
            {
                throw new InvalidCastException("Пустой результат выполнения операции");
            }

            if (result.Success == false)
            {
                var errorMsg = result.Errors.FirstOrDefault()?.ErrorMessage ?? "Ошибка при выполнении операции";
                throw new InvalidOperationException(errorMsg);
            }
        }
    }

    /// <summary>
    /// Обработчик запросов к серверу
    /// </summary>
    public static class WcfRequestHandler
    {
        public static TResult RequestHandle<TResult, TContract>(DuplexWcfProxy<TContract> proxy,
            Func<TContract, TResult> command)
            where TContract : class
            where TResult : OperationResult
        {
            try
            {
                var result = command.Invoke(proxy.WcfChannel);
                WcfResponseHandler.ResponseHandle(result);
                return result;
            }
            catch (Exception)
            {
                proxy.Abort();
                proxy.Close();
                throw;
            }
        }
    }
}
