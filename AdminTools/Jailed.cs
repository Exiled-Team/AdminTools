using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using RelativePositioning;

namespace AdminTools
{
	/// <summary>
	/// Represents a jailed player and their associated state.
	/// </summary>
	public class Jailed
	{
		/// <summary>
		/// Gets or sets the name of the jailed player.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the list of items the player possesses.
		/// </summary>
		public List<object> Items { get; set; }

		/// <summary>
		/// Gets or sets the list of effects currently applied to the player.
		/// </summary>
		public List<Effect> Effects { get; set; }

		/// <summary>
		/// Gets or sets the player's role.
		/// </summary>
		public object Role { get; set; }

		/// <summary>
		/// Gets or sets the player's relative position.
		/// </summary>
		public RelativePosition RelativePosition { get; set; }

		/// <summary>
		/// Gets or sets the player's health.
		/// </summary>
		public float Health { get; set; }

		/// <summary>
		/// Gets or sets the player's ammo, stored as a dictionary of ammo type and count.
		/// </summary>
		public Dictionary<AmmoType, ushort> Ammo { get; set; }
		
		/// <summary>
		/// Gets or sets the player's ammo, stored as a dictionary of custom ammo type and count.
		/// </summary>
		public Dictionary<uint, ushort> CustomAmmo { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the player was jailed in the current round.
		/// </summary>
		public bool CurrentRound { get; set; }
	}

}