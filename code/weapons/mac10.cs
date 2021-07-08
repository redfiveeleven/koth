using Sandbox;
using System;

[Library( "dm_mac10", Title = "mac10" )]
[Hammer.EditorModel( "models/mac10/mac10.vmdl" )]
partial class mac10 : BaseDmWeapon
{ 
	public override string ViewModelPath => "models/1911/1911.vmdl";

	private float lasttick = 0;
	private const float delaytick = 0.05f;

	public override float PrimaryRate => 20.0f;
	public override float SecondaryRate => 1.0f;
	public override int ClipSize => 20;
	public override float ReloadTime => 1.5f;
	public override int Bucket => 2;

	private const int damage = 15;

	public override AmmoType AmmoType => AmmoType.SMG;

	private const float basespread = 0.09f;
	private const float maxspread = 0.4f;
	private const float increasespread = 1.2f;
	private const float startspreaddecrease = 0.2f;

	private const float a = 0.045f;

	private float spread = basespread;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/mac10/mac10.vmdl" );
		AmmoClip = 20;
	}

	public override void AttackPrimary()
	{
		

		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( !TakeAmmo( 1 ) )
		{
			DryFire();
			return;
		}

		(Owner as AnimEntity).SetAnimBool( "b_attack", true );

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound( "rust_smg.shoot" );

		//
		// Shoot the bullets
		//
		ShootBullet( spread, 1.5f, damage, 3.0f );

		spread = spread * increasespread;

	}

	public override void AttackSecondary()
	{
		// Grenade lob
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		if ( Owner == Local.Pawn )
		{
			new Sandbox.ScreenShake.Perlin( 0.5f, 4.0f, 1.0f, 0.5f );
		}

		ViewModelEntity?.SetAnimBool( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	public override void Simulate( Client owner )
	{


		if ( AmmoClip == 0 )
		{
			Reload();
		}

		spread = spread.Clamp( basespread, maxspread );
		//Log.Info( spread );

		if (TimeSincePrimaryAttack > startspreaddecrease) {
			spread = spread / increasespread;
		}

		if ( TimeSinceDeployed < 0.6f )
			return;

		if ( !IsReloading )
		{
			base.Simulate( owner );
		}

		if ( IsReloading && TimeSinceReload > ReloadTime )
		{
			OnReloadFinish();
		}
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 2 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}

}
