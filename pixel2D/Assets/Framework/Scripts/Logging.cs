/*
** Created by fengling
** DateTime:    2021-09-22 13:26:25
** Description:  PlayerSetting下Scripting Define Symbols开启ENABLE_LOG打开Log
*/

namespace Framework.Scripts
{
    public static class Logging
    {
        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Assert(bool condition, object message)
        {
            UnityEngine.Debug.Assert(condition, message);
        }
    }
}