#region Author & Verson
// Name : GLog.cs
// Author : GuanRen
// CreateTime : 2024/09/11
// Job :
#endregion

using System.Diagnostics;

namespace Game
{
    public static class GLog
    {
        [Conditional("UNITY_EDITOR")]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }
    }
}