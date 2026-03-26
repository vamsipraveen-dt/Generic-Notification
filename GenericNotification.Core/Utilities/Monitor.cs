
namespace GenericNotification.Core.Utilities
{
    public static class Monitor
    {
        private static bool _sendExceptions;
        public static void Initialize(bool sendException, string dsn)
        {
            _sendExceptions = sendException;

            if (_sendExceptions)
            {
                _ = NativeMethods.InitializeSentry(dsn);
            }
        }

        public static void SendException(Exception ex)
        {
            if (!_sendExceptions)
            {
                return;
            }

            NativeMethods.ReportSentryError(ex.ToString());
        }

        public static void SendMessage(string message)
        {
            if (!_sendExceptions)
            {
                return;
            }

            NativeMethods.ReportSentryError(message);
        }

        public static void SentryShutDown()
        {
            if (!_sendExceptions)
            {
                return;
            }

            NativeMethods.ShutdownSentry();
        }
    }
}
