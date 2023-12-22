using System;
using System.Reflection;
using Exiled.API.Features;

namespace AdminTools
{
	public static class Extensions
	{
		public static void InvokeStaticMethod(this Type type, string methodName, object[] param)
		{
			BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic |
								 BindingFlags.Static | BindingFlags.Public;
			MethodInfo info = type.GetMethod(methodName, flags);
			info?.Invoke(null, param);
		}

		public static bool IsVanished(this Player player) => Main.VanishedPlayers.ContainsKey(player);
	}
}