namespace AdminTools.Commands.Id
{
    using System;
    using System.Linq;
    using System.Text;
    using CommandSystem;
    using Exiled.API.Features;
    using NorthwoodLib.Pools;
    using RemoteAdmin;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class ID : ParentCommand
    {
        public ID()
        {
            LoadGeneratedCommands();
        }

        public override string Command => "atid";

        public override string[] Aliases => null;

        public override string Description => "Gets the player ID of a selected user";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender,
            out string response)
        {
            if (arguments.Count != 1)
            {
                response = "Usage: id ((player id / name) or (all / *))";
                return false;
            }

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    StringBuilder builder = StringBuilderPool.Shared.Rent();
                    if (Player.List.Count() == 0)
                    {
                        builder.AppendLine("There are no players currently online in the server");
                        string msg = builder.ToString();
                        StringBuilderPool.Shared.Return(builder);
                        response = msg;
                        return true;
                    }
                    else
                    {
                        builder.AppendLine("List of ID's on the server:");
                        foreach (Player ply in Player.List)
                        {
                            builder.Append(ply.Nickname);
                            builder.Append(" - ");
                            builder.Append(ply.UserId);
                            builder.Append(" - ");
                            builder.AppendLine(ply.Id.ToString());
                        }

                        string msg = builder.ToString();
                        StringBuilderPool.Shared.Return(builder);
                        response = msg;
                        return true;
                    }
                default:
                    Player pl;
                    if (string.IsNullOrWhiteSpace(arguments.At(0)))
                    {
                        if (!(sender is PlayerCommandSender plysend))
                        {
                            response = "You must be in-game to run this command if you specify yourself!";
                            return false;
                        }

                        pl = Player.Get(plysend.ReferenceHub);
                    }
                    else
                    {
                        pl = Player.Get(arguments.At(0));
                        if (pl == null)
                        {
                            response = "Player not found";
                            return false;
                        }
                    }

                    response = $"{pl.Nickname} - {pl.UserId} - {pl.Id}";
                    return true;
            }
        }
    }
}