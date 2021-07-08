using Sandbox;
using System;

[Library( "dm_pistol", Title = "Pistol" )]
[Hammer.EditorModel( "weapons/rust_pistol/rust_pistol.vmdl" )]
partial class pistol : BaseDmWeapon
{ 
	public override string ViewModelPath => "models/1911/1911.vmdl";

	private float lasttick = 0;
	private const float delaytick = 0.05f;
	// a
	public override float PrimaryRate => 20.0f;
	public override float SecondaryRate => 1.0f;
	public override int ClipSize => 14;
	public override float ReloadTime => 1.0f;
	public override int Bucket => 0;

	private const int damage = 26;

	public override AmmoType AmmoType => AmmoType.Pistol;

	private const float basespread = 0.02f;
	private const float maxspread = 0.3f;
	private const float increasespread = 1.5f;
	private const float startspreaddecrease = 1.0f;

	private const float a = 0.045f;

	private float spread = basespread;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );
		AmmoClip = 14;
	}

	public override bool CanPrimaryAttack()
	{

		return base.CanPrimaryAttack() && Input.Pressed( InputButton.Attack1 );

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
		PlaySound( "rust_pistol.shoot" );

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
		anim.SetParam( "holdtype", 1 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}

}
