using Exiled.API.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace AdminTools
{
	using Exiled.API.Features.Items;
    using Exiled.CustomRoles.API.Features;

    public class Jailed
	{
		public string UserId;
		public string Name;
		public List<Item> Items;
		public RoleType Role;
		public Vector3 Position;
		public float Health;
		public Dictionary<AmmoType, ushort> Ammo;
		public bool CurrentRound;
		public CustomRole CustomRole;
	}
}