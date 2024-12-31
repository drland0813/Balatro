using System.Collections;
using System.Collections.Generic;

public static class LogUtil
{
    public static bool Enabled;

    public static void SetCustomKey(string key, string value)
    {
#if ENABLE_CRASHLYTICS_LOG
        if (Enabled)
            Firebase.Crashlytics.Crashlytics.SetCustomKey(key, value);
#endif
    }

    public static void Log(string message)
    {
#if ENABLE_CRASHLYTICS_LOG
        if (Enabled)
            Firebase.Crashlytics.Crashlytics.Log(message);
#endif
    }

    public static void LogException(System.Exception exception)
    {
#if ENABLE_CRASHLYTICS_LOG
        if (Enabled)
            Firebase.Crashlytics.Crashlytics.LogException(exception);
#endif
    }
}
