using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using RelativePositioning;

namespace AdminTools
{
    public class Jailed
	{
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