
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using koth;

public class Scoreboard : Sandbox.UI.Scoreboard<ScoreboardEntry>
{

	public Scoreboard()
	{
		StyleSheet.Load( "/ui/Scoreboard.scss" );
	}

	protected override void AddHeader()
	{
		Header = Add.Panel( "header" );
		Header.Add.Label( "player", "name" );
		Header.Add.Label( "points", "points" );
		Header.Add.Label( "ping", "ping" );
	}

}

public class ScoreboardEntry : Sandbox.UI.ScoreboardEntry
{
	public Label Fps;
	public Label points;

	public ScoreboardEntry()
	{
		points = Add.Label( "points", "points" );
		Fps = Add.Label( "", "fps" );

	}

	public override void UpdateFrom( PlayerScore.Entry entry )
	{
		base.UpdateFrom( entry );
		points.Text = "fart";
		Fps.Text = entry.Get<int>( "fps", 0 ).ToString();
	}
}
