namespace Empire_Rewritten.Utils
{
    public static class Logger
    {
        private const string PrefixLog = "<color=lightblue>[Empire]</color> ";
        private const string PrefixWarn = "<color=orange>[Empire]</color> ";
        private const string PrefixError = "<color=red>[Empire]</color> ";

        public static void Message(string message)
        {
            Verse.Log.Message(message);
        }

        public static void Log(string message)
        {
            Verse.Log.Message(PrefixLog + message);
        }

        public static void Warn(string message)
        {
            Verse.Log.Warning(PrefixWarn + message);
        }

        public static void Error(string message, int? maybeKey = null)
        {
            if (maybeKey is int key)
            {
                Verse.Log.ErrorOnce(PrefixError + $"{{{key}}} " + message, key);
            }
            else
            {
                Verse.Log.Error(PrefixError + message);
            }
        }

        /// <summary>
        ///     Simplifies making an error message that only errors once
        /// </summary>
        /// <param name="message"></param>
        public static void ErrorOnce(this string message) => Verse.Log.ErrorOnce(PrefixError + message, message.GetHashCode());
    }
}
