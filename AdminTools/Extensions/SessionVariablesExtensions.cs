﻿using Exiled.API.Features;

namespace AdminTools.Extensions
{
    public static class SessionVariablesExtensions
    {
        public static bool HasSessionVariable(this Player player, string sessionVariable) 
            => player.SessionVariables.ContainsKey(sessionVariable);
        
        public static void AddBooleanSessionVariable(this Player player, string sessionVariable, bool value = true) 
            => player.SessionVariables.Add(sessionVariable, value);
        public static void RemoveSessionVariable(this Player player, string sessionVariable) 
            => player.SessionVariables.Remove(sessionVariable);
    }
}