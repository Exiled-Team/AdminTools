using System;
using System.Reflection;

namespace AdminTools.Extensions
{
	public static class ReflectionExtensions
	{
		public static void InvokeStaticMethod(this Type type, string methodName, object[] param)
		{
			var flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic |
                        BindingFlags.Static | BindingFlags.Public;
			var info = type.GetMethod(methodName, flags);
			info?.Invoke(null, param);
		}
	}
}