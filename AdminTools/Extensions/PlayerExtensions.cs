using System;
using Exiled.API.Features;
using UnityEngine;

namespace AdminTools.Extensions
{
    public static class PlayerExtensions
    {
        public static void SetPlayerScale(Player target, float x, float y, float z)
        {
            try
            {
                target.Scale = new Vector3(x, y, z);
            }
            catch (Exception e)
            {
                Log.Info($"Set Scale error: {e}");
            }
        }
    }
}