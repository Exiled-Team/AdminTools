namespace AdminTools.Commands.Tutorial
{
    using System;
    using System.Collections.Generic;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using MEC;
    using PlayerRoles;
    using RemoteAdmin;
    using UnityEngine;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Tutorial : ICommand
    {
        private static Player _ply;

        public string Command => "tutorial";

        public string[] Aliases { get; } = { "tut" };

        public string Description => "Sets a player as a tutorial conveniently";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.tut"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            switch (arguments.Count)
            {
                case 0:
                case 1:
                    if (arguments.Count == 0)
                    {
                        if (sender is not PlayerCommandSender plysend)
                        {
                            response = "You must be in-game to run this command if you specify yourself!";
                            return false;
                        }

                        _ply = Player.Get(plysend.ReferenceHub);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(arguments.At(0)))
                        {
                            response = "Please do not try to put a space as tutorial";
                            return false;
                        }

                        _ply = Player.Get(arguments.At(0));
                        if (_ply == null)
                        {
                            response = $"Player not found: {arguments.At(0)}";
                            return false;
                        }
                    }

                    DoTutorialFunction(_ply, out response);
                    return true;
                default:
                    response = "Usage: tutorial (optional: id / name)";
                    return false;
            }
        }

        private static IEnumerator<float> SetClassAsTutorial(Player ply)
        {
            Vector3 oldPos = ply.Position;
            ply.Role.Set(RoleTypeId.Tutorial);

            yield return Timing.WaitForSeconds(0.5f);

            ply.Position = oldPos;
        }

        private static void DoTutorialFunction(Player ply, out string response)
        {
            if (ply.Role != RoleTypeId.Tutorial)
            {
                Timing.RunCoroutine(SetClassAsTutorial(ply));
                response = $"Player {ply.Nickname} is now set to tutorial";
            }
            else
            {
                ply.Role.Set(RoleTypeId.Spectator);
                response = $"Player {ply.Nickname} is now set to spectator";
            }
        }
    }
}