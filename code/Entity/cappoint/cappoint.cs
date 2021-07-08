using Sandbox;
using System;
using System.Collections.Generic;


namespace CapturePointEntity
{
	[Library( "ent_cappoint", Title = "Capture Point", Spawnable = true )]

	public partial class CapturePoint : Prop
	{
		public Client owner = null;
		private float lasttick = 0;
		private const float delaytick = 1.0f;
		private const int capradius = 160;
		private const int capdelay = 0;
		public IDictionary<Client, int> captime = new Dictionary<Client, int>();
		public IDictionary<Client, int> points = new Dictionary<Client, int>();

		// The person who OWNS the point is the person with the longest continual time touching it
		//List<string> players = new List<string>();




		public override void Spawn()
		{
			base.Spawn();

			SetModel( "models/tf2_cappoint/tf2_cappoint.vmdl" );
			SetupPhysicsFromModel( PhysicsMotionType.Static, false );
		}

		[Event.Tick.Server]
		public void Tick()
		{
			

			lasttick += Time.Delta;

			if ( lasttick < delaytick ) return;

			// check for players on the point
			foreach ( var ply in Client.All )
			{

				if ( !captime.ContainsKey( ply ) )
				{
					captime.Add( ply, 0 );
					//Log.Info( "Added player " + ply + " to captime dict." );
				}
				
				//Log.Info( ply.Pawn.Position.Distance( this.Position ) );

				if ( ply.Pawn.Position.Distance( this.Position ) < capradius && ply.Pawn.Health > 0)
				{
					//Log.Info(ply + " is on the point " + ply.Pawn.Position.Distance( this.Position ) );

					captime[ply] += 1;

					//Log.Info( ply +  "'s captime:"  +  captime[ply] );

					bool ownerpossibility = true;

					foreach ( var entry in captime )
					{

						if ( entry.Value > captime[ply] ) ownerpossibility = false;
						//Log.Info( ownerpossibility );
					}

					if ( captime[ply] < capdelay ) {
						ownerpossibility = false;
					}

					if (ownerpossibility == true && owner != ply )
					{
							
						

						if (this.GetMaterialGroup() == 0 ) this.SetMaterialGroup( 1 );
						else if ( (this.GetMaterialGroup() == 1)) this.SetMaterialGroup( 2 );
						else if ( (this.GetMaterialGroup() == 2) ) this.SetMaterialGroup( 1 );

						PlaySound( "kothbell" );
						// a

						if ( owner != null && owner.Pawn is ModelEntity oldperson )
						{
							oldperson.GlowActive = false;
							Log.Info( ply.Name + " took the control point " + this + " from " + owner.Name + "!" );
							//Sandbox.UI.ChatBox.Say( ply.Name + " took the control point " + this + " from " + owner.Name + "!" );
						}
						else
						{
							Log.Info( ply.Name + " took the control point!");
							//Sandbox.UI.ChatBox.Say( ply.Name + " took the control point!" );
						}


						owner = ply;

						if ( ply.Pawn is ModelEntity newperson )
						{
							newperson.GlowActive = true;
							newperson.GlowColor = Color.Red;
						}

						
;
					}
				}
				else
				{
					captime[ply] = 0;
				}


			}


			// give points to the current owner
			if ( owner != null )
			{
				if ( !points.ContainsKey( owner ) ) // make sure owner is on the points dictionary
				{
					points.Add( owner, 0 );
					Log.Info( "Added owner " + owner + " to points dict." );
				}

				points[owner] += 1;
			}
		}
	}
}
