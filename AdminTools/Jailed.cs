using Exiled.API.Enums;
using System.Collections.Generic;
using UnityEngine;
using CustomPlayerEffects;

namespace AdminTools
{
	using Exiled.API.Features.Items;

	public class Jailed
	{
		public string Userid;
		public string Name;
		public List<Item> Items;
		public RoleType Role;
		public Vector3 Position;
		public float Health;
		public Dictionary<AmmoType, ushort> Ammo;
		public bool CurrentRound;
		public IEnumerable<PlayerEffect> Effects;
	}
}