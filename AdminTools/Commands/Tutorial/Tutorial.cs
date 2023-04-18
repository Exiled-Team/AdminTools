using CommandSystem;
using Exiled.Permissions.Extensions;
using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using RemoteAdmin;

namespace AdminTools.Commands.Tutorial
{
    using PlayerRoles;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Tutorial : ParentCommand
    {
        Player _ply;

        public Tutorial() => LoadGeneratedCommands();

        public override string Command { get; } = "tutorial";

        public override string[] Aliases { get; } = new string[] { "tut" };

        public override string Description { get; } = "Sets a player as a tutorial conveniently";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
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
                        if (!(sender is PlayerCommandSender plysend))
                        {
                            response = "You must be in-game to run this command if you specify yourself!";
                            return false;
                        }

                        _ply = Player.Get(plysend.ReferenceHub);
                    }
                    else
                    {
                        if (String.IsNullOrWhiteSpace(arguments.At(0)))
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

        private IEnumerator<float> SetClassAsTutorial(Player ply) 
        {
            var oldPos = ply.Position;
            ply.Role.Set(RoleTypeId.Tutorial);
            yield return Timing.WaitForSeconds(0.5f);
            ply.Position = oldPos;
        }

        private void DoTutorialFunction(Player ply, out string response)
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
