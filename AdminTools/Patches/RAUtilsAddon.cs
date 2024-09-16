using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Exiled.API.Enums;
using Exiled.API.Features;
using NorthwoodLib.Pools;
using PlayerRoles;
using Utils;

namespace AdminTools.Patches
{
    public static class CustomRAUtilsAddon
    {
        public static bool Prefix(ref List<ReferenceHub> __result, ArraySegment<string> args, int startindex, out string[] newargs, bool keepEmptyEntries = false)
        {
            Log.Debug("BetterCommands::CustomRAUtilsAddon was called.");
            string text = RAUtils.FormatArguments(args, startindex);
            List<ReferenceHub> list = ListPool<ReferenceHub>.Shared.Rent();
            
            if (text.StartsWith("@", StringComparison.Ordinal))
            {
                foreach (object obj in new Regex("@\"(.*?)\".|@[^\\s.]+\\.").Matches(text))
                {
                    Match match = (Match)obj;
                    text = RAUtils.ReplaceFirst(text, match.Value, "");
                    string name = match.Value.Substring(1).Replace("\"", "").Replace(".", "");
                    List<ReferenceHub> list2 = (from ply in ReferenceHub.AllHubs
                                                where ply.nicknameSync.MyNick.Equals(name)
                                                select ply).ToList();
                    if (list2.Count == 1 && !list.Contains(list2[0]))
                    {
                        list.Add(list2[0]);
                    }
                }
                newargs = text.Split(new[]
                {
                        ' '
                }, keepEmptyEntries ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries);
                __result = list;
            }
            else
            {
                if (args.At(startindex).Length > 0)
                {
                    string[] array = args.At(startindex).Split('.');
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i].ToLower() is "all" or "*")
                        {
                            list.AddRange(Player.List.Select(x => x.ReferenceHub));
                            break;
                        }
                        if (int.TryParse(array[i], out int playerId))
                        {
                            if (playerId < 1)
                                continue;

                            if (ReferenceHub.TryGetHub(playerId, out ReferenceHub item))
                                list.Add(item);
                        }
                        else if (Enum.TryParse(array[i], true, out SimplifyTeam simplifyTeam))
                        {
                            switch (simplifyTeam)
                            {
                                case SimplifyTeam.ALIVE:
                                    list.AddRange(Player.List.Where(p => p.IsAlive).Select(x => x.ReferenceHub));
                                    break;
                                case SimplifyTeam.HUMAN:
                                    list.AddRange(Player.List.Where(p => p.IsHuman).Select(x => x.ReferenceHub));
                                    break;
                                case SimplifyTeam.CIVILIAN:
                                    list.AddRange(Player.List.Where(p => p.Role.Team is Team.Scientists or Team.ClassD).Select(x => x.ReferenceHub));
                                    break;
                                case SimplifyTeam.MILITARY:
                                    list.AddRange(Player.List.Where(p => p.Role.Team is Team.FoundationForces or Team.ChaosInsurgency).Select(p => p.ReferenceHub));
                                    break;
                                case SimplifyTeam.STAFF:
                                    list.AddRange(Player.List.Where(p => p.RemoteAdminAccess).Select(p => p.ReferenceHub));
                                    break;
                                case SimplifyTeam.NOSTAFF:
                                    list.AddRange(Player.List.Where(p => !p.RemoteAdminAccess).Select(p => p.ReferenceHub));
                                    break;
                                default:
                                    list.AddRange(Player.Get((Team)simplifyTeam).Select(x => x.ReferenceHub));
                                    break;
                            }
                        }
                        else if (Enum.TryParse(array[i], true, out RoleTypeId roleType))
                        {
                            list.AddRange(Player.Get(roleType).Select(x => x.ReferenceHub));
                        }
                        else if (Enum.TryParse(array[i], true, out Side side))
                        {
                            list.AddRange(Player.Get(side).Select(x => x.ReferenceHub));
                        }
                        else if (Enum.TryParse(array[i], true, out Team team))
                        {
                            list.AddRange(Player.Get(team).Select(x => x.ReferenceHub));
                        }
                    }
                }
                newargs = args.Count > 1 ? RAUtils.FormatArguments(args, startindex + 1).Split(new[]
                {
                        ' '
                }, keepEmptyEntries ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries) : null;
                __result = list;
            }
            return false;
        }

        public enum SimplifyTeam
        {
            // Team but simple enum
            SCP,
            MTF,
            CI,
            SCI,
            CLD,
            RIP,
            TUT,
            // Additional (Not Team)
            ALIVE,
            HUMAN,
            CIVILIAN,
            MILITARY,
            NOSTAFF,
            STAFF,
        }
    }
}