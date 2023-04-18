namespace AdminTools.API.Entities
{
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using PlayerRoles;
    using UnityEngine;

    public class Jailed
    {
        public Dictionary<AmmoType, ushort> Ammo;

        public bool CurrentRound;

        public float Health;

        public List<Item> Items;

        public Vector3 Position;

        public RoleTypeId Role;

        public string UserId;
    }
}