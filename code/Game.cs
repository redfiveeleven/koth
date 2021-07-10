using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;
using CapturePointEntity;

/// <summary>
/// This is the heart of the gamemode. It's responsible
/// for creating the player and stuff.
/// </summary>
/// 


namespace koth
{
	[Library( "koth", Title = "KingOfTheHill" )]
	partial class kothgame : Sandbox.Game
	{
		public kothgame()
		{
			//
			// Create the HUD entity. This is always broadcast to all clients
			// and will create the UI panels clientside. It's accessible 
			// globally via Hud.Current, so we don't need to store it.
			//
			if ( IsServer )
			{
				new DeathmatchHud();
			}


		}

		public override void PostLevelLoaded()
		{
			base.PostLevelLoaded();

			ItemRespawn.Init();
		}

		public override void ClientJoined( Client cl )
		{
			base.ClientJoined( cl );

			var player = new DeathmatchPlayer();
			player.Respawn();

			cl.Pawn = player;
		}

		public System.Collections.Generic.IReadOnlyList<CapturePoint> GetCapPoints()
		{
			List<CapturePoint> cappointlist = All.OfType<CapturePoint>().ToList();

			return cappointlist;
		}

		public System.Collections.Generic.IDictionary<Client, int> GetScoreboard()
		{
			IDictionary<Client, int> totalpoints = new Dictionary<Client, int>();

			foreach ( CapturePoint cappoints in GetCapPoints() )
			{
				foreach ( var entry in cappoints.points )
				{
					if ( !totalpoints.ContainsKey( entry.Key ) )
					{
						totalpoints.Add( entry.Key, 0 );
					}

					totalpoints[entry.Key] += entry.Value;
				}
			}

			return totalpoints;
		}

	}
}
