using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using PlayerRoles;
using UnityEngine;

namespace AdminTools.API.Entities
{
    public class Jailed
	{
		public string UserId;
        public List<Item> Items;
		public RoleTypeId Role;
		public Vector3 Position;
		public float Health;
		public Dictionary<AmmoType, ushort> Ammo;
		public bool CurrentRound;
	}
}