using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using System;
using System.Linq;

namespace AdminTools.Commands.LastTarget
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class LastTarget : ParentCommand
    {
        public override string Command { get; } = "cbc";

        public override string[] Aliases { get; } = { "Cassiebc" };

        public override string Description { get; } = "Cassie broadcast.";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("CassiePlugin.cassie"))
            {
                response = "You do not have permission to use this command";
                return false;
            }
            
            if (arguments.Count != 2)
            {
                response = "Usage: cbc (plurality 0-1) (zone 1-4)";
                return false;
            }
            if (!(arguments.At(0).Equals("0")) && !(arguments.At(0).Equals("1")))
            {
                response = "Usage: cbc (plurality 0-1) (zone 1-4)";
                return false;
            }
            if (!(arguments.At(1).Equals("1")) && !(arguments.At(1).Equals("2")) && !(arguments.At(1).Equals("3")) && !(arguments.At(1).Equals("4")))
            {
                response = "Usage: cbc (plurality 0-1) (zone 1-4)";
                return false;
            }
            Cassie.Message("ATTENTION . LAST TARGET" + getPlural(arguments.At(0)) + " " + getZone(arguments.At(1)));
            response = "Cassie broadcast sent. ";
            return true;
        }

        private string getPlural(string num)
        {
            if (num.Equals("1"))
            {
                return "S";
            }
            return "";
        }

        private string getZone(string num)
        {
            if (num.Equals("1"))
            {
                return "IN LIGHT CONTAINMENT";
            }
            if (num.Equals("2"))
            {
                return "IN HEAVY CONTAINMENT";
            }
            if (num.Equals("3"))
            {
                return "IN ENTRANCE ZONE";
            }
            if (num.Equals("4"))
            {
                return "ON SURFACE";
            }
            return "";
        }
    }
}
