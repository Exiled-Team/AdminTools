namespace AdminTools.Extensions
{
    using System;
    using Exiled.API.Features;
    using UnityEngine;

    public static class PlayerExtensions
    {
        public static void SetPlayerScale(this Player target, float x, float y, float z)
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

        public static void SetPlayerScale(this Player target, float scale)
        {
            try
            {
                target.Scale = Vector3.one * scale;
            }
            catch (Exception e)
            {
                Log.Info($"Set Scale error: {e}");
            }
        }
    }
}