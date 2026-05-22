using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Dragon : Bot
{
    static void Main(string[] args)
    {
        new Dragon().Start();
    }

    Dragon() : base(BotInfo.FromFile("Dragon.json")) { }

    private const double Margin = 100;

    public override void Run()
    {
        BodyColor = Color.FromArgb(255, 201, 74);
        TurretColor = Color.FromArgb(50, 0, 0);
        RadarColor = Color.SkyBlue;
        BulletColor = Color.Black;

        AdjustRadarForBodyTurn = true;
        AdjustRadarForGunTurn = true;
        AdjustGunForBodyTurn = true;

        SetForward(50000);

        while (IsRunning)
        {
            SetTurnRadarRight(20);

            bool nearLeft = X < Margin;
            bool nearRight = X > ArenaWidth - Margin;
            bool nearBottom = Y < Margin;
            bool nearTop = Y > ArenaHeight - Margin;

            if (nearBottom && Direction >= 315 || nearBottom && Direction < 45)
            {
                SetTurnRight(90);
            }
            else if (nearRight && Direction >= 45 && Direction < 135)
            {
                SetTurnRight(90);
            }
            else if (nearTop && Direction >= 135 && Direction < 225)
            {
                SetTurnRight(90);
            }
            else if (nearLeft && Direction >= 225 && Direction < 315)
            {
                SetTurnRight(90);
            }

            if (DistanceRemaining == 0)
                SetForward(50000);

            Go();
        }
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        double distance = DistanceTo(e.X, e.Y);

        double gunBearing = GunBearingTo(e.X, e.Y);
        SetTurnGunLeft(gunBearing * 0.6);

        double radarBearing = RadarBearingTo(e.X, e.Y);
        SetTurnRadarLeft(radarBearing * 0.5);

        double firePower = 1.0;

        if (distance < 150)
            firePower = 3.0;
        else if (distance < 400)
            firePower = 2.0;

        if (GunHeat == 0 && Math.Abs(gunBearing) < 8)
            Fire(firePower);
    }

    public override void OnHitBot(HitBotEvent e)
    {
        if (GunHeat == 0)
            Fire(3);
    }

    public override void OnHitWall(HitWallEvent e)
    {
        SetTurnRight(90);
        SetForward(50000);
    }
}