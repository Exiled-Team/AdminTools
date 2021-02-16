using CommandSystem;
using Exiled.API.Features;
using NorthwoodLib.Pools;
using System;
using System.Linq;
using System.Text;

namespace AdminTools.Commands.Id
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class ID : ParentCommand
    {
        public ID() => LoadGeneratedCommands();

        public override string Command { get; } = "id";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Gets the player ID of a selected user";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            EventHandlers.LogCommandUsed((CommandSender)sender, EventHandlers.FormatArguments(arguments, 0));
            if (arguments.Count != 1)
            {
                response = "Usage: id ((player id / name) or (all / *))";
                return false;
            }

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    StringBuilder Builder = StringBuilderPool.Shared.Rent();
                    if (Player.List.Count() == 0)
                    {
                        Builder.AppendLine("There are no players currently online in the server");
                        string msg = Builder.ToString();
                        StringBuilderPool.Shared.Return(Builder);
                        response = msg;
                        return true;
                    }
                    else
                    {
                        Builder.AppendLine("List of ID's on the server:");
                        foreach (Player Ply in Player.List)
                        {
                            Builder.Append(Ply.Nickname);
                            Builder.Append(" - ");
                            Builder.Append(Ply.UserId);
                            Builder.Append(" - ");
                            Builder.AppendLine(Ply.Id.ToString());
                        }
                        string msg = Builder.ToString();
                        StringBuilderPool.Shared.Return(Builder);
                        response = msg;
                        return true;
                    }
                default:
                    Player Pl;
                    if (String.IsNullOrWhiteSpace(arguments.At(0)))
                        Pl = Player.Get(((CommandSender)sender).ReferenceHub);
                    else
                    {
                        Pl = Player.Get(arguments.At(0));
                        if (Pl == null)
                        {
                            response = "Player not found";
                            return false;
                        }
                    }

                    response = $"{Pl.Nickname} - {Pl.UserId} - {Pl.Id}";
                    return true;
            }
        }
    }
}
