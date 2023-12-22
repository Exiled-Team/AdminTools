using Exiled.API.Enums;
using System.Collections.Generic;

namespace AdminTools
{
    using Exiled.API.Features.Items;
	using PlayerRoles;
    using RelativePositioning;
    using Exiled.API.Features;

    public class Jailed
	{
		public string Userid;
		public string Name;
		public List<Item> Items;
		public List<Effect> Effects;
		public RoleTypeId Role;
		public RelativePosition RelativePosition;
		public float Health;
		public Dictionary<AmmoType, ushort> Ammo;
		public bool CurrentRound;
	}
}