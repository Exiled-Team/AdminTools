using Exiled.API.Features;

namespace AdminTools.Extensions
{
    public static class SessionVariablesExtensions
    {
        public static bool HasSessionVariable(this Player player, string sessionVariable) 
            => player.SessionVariables.ContainsKey(sessionVariable);
    }
}