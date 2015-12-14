using System;
using System.Threading.Tasks;

namespace EFunTech.Sms.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// FaFTaskFactory.StartNew(() => {
    ///     IMessageService messageService = ServiceLocator.Current.GetInstance<GMailService>(); 
    ///     messageService.Send(message);
    /// },
    /// task => { this.logService.Error(task.Exception); }, // exception_handler
    /// task => { }); // completion_handler
    /// </example>
    public class FaFTaskFactory
    {
        public static Task StartNew(Action action)
        {
            return StartNew(0, action);
        }

        public static Task StartNew(int delayMilliseconds, Action action)
        {
            return Task.Factory.StartNew(() => {
                System.Threading.Thread.Sleep(/*避免負值*/ Math.Max(0, delayMilliseconds));
                action(); 
            }).ContinueWith(
                c =>
                {
                    AggregateException exception = c.Exception;

                    // Your Exception Handling Code
                },
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously
            ).ContinueWith(
                c =>
                {
                    // Your task accomplishing Code
                },
                TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously
            );
        }

        public static Task StartNew(Action action, Action<Task> exception_handler, Action<Task> completion_handler)
        {
            return StartNew(0, action, exception_handler, completion_handler);
        }

        public static Task StartNew(int delayMilliseconds, Action action, Action<Task> exception_handler, Action<Task> completion_handler)
        {
            return Task.Factory.StartNew(() => {
                System.Threading.Thread.Sleep(/*避免負值*/ Math.Max(0, delayMilliseconds));
                action(); 
            }).ContinueWith(
                exception_handler,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously
            ).ContinueWith(
                completion_handler,
                TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously
            );
        }
    };

}