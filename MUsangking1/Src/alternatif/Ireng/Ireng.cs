using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Ireng : Bot
{
    static void Main(string[] args)
    {
        new Ireng().Start();
    }

    Ireng() : base(BotInfo.FromFile("Ireng.json")) { }

    const double WallMargin = 120;

    private int moveDirection = 1;
    private int dodgeCounter = 0;

    private double lastEnemyX;
    private double lastEnemyY;
    private double lastEnemyHeading;
    private double lastEnemySpeed;

    public override void Run()
    {
        BodyColor = Color.FromArgb(0, 0, 0);
        TurretColor = Color.FromArgb(255, 247, 3);
        RadarColor = Color.SkyBlue;
        BulletColor = Color.DarkRed;

        AdjustRadarForBodyTurn = true;
        AdjustRadarForGunTurn = true;
        AdjustGunForBodyTurn = true;

        SetForward(100000);

        while (IsRunning)
        {
            SetTurnRadarRight(360);

            HandleWalls();

            dodgeCounter++;

            if (dodgeCounter > 35)
            {
                dodgeCounter = 0;

                moveDirection *= -1;

                SetTurnRight(60 * moveDirection);
                SetForward(250 * moveDirection);
            }

            if (DistanceRemaining == 0)
                SetForward(100000 * moveDirection);

            Go();
        }
    }

    private void HandleWalls()
    {
        bool nearLeft = X < WallMargin;
        bool nearRight = X > ArenaWidth - WallMargin;
        bool nearBottom = Y < WallMargin;
        bool nearTop = Y > ArenaHeight - WallMargin;

        if (nearLeft || nearRight || nearBottom || nearTop)
        {
            double centerX = ArenaWidth / 2;
            double centerY = ArenaHeight / 2;

            double angleToCenter =
                Math.Atan2(centerX - X, centerY - Y) * 180 / Math.PI;

            double turn = NormalizeRelativeAngle(angleToCenter - Direction);

            SetTurnRight(turn);
            SetForward(300);
        }
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        double distance = DistanceTo(e.X, e.Y);

        double enemyHeading = 0;

        if (lastEnemyX != 0 || lastEnemyY != 0)
        {
            enemyHeading =
                Math.Atan2(
                    e.X - lastEnemyX,
                    e.Y - lastEnemyY
                ) * 180 / Math.PI;
        }

        double enemySpeed =
            Math.Sqrt(
                Math.Pow(e.X - lastEnemyX, 2) +
                Math.Pow(e.Y - lastEnemyY, 2)
            );

        lastEnemyX = e.X;
        lastEnemyY = e.Y;
        lastEnemyHeading = enemyHeading;
        lastEnemySpeed = enemySpeed;

        double firePower;

        if (distance < 150)
            firePower = 3;
        else if (distance < 350)
            firePower = 2;
        else
            firePower = 1;

        double bulletSpeed = 20 - 3 * firePower;

        double time = distance / bulletSpeed;

        double predictedX =
            e.X +
            Math.Sin(enemyHeading * Math.PI / 180)
            * enemySpeed * time;

        double predictedY =
            e.Y +
            Math.Cos(enemyHeading * Math.PI / 180)
            * enemySpeed * time;

        predictedX = Math.Max(
            18,
            Math.Min(ArenaWidth - 18, predictedX));

        predictedY = Math.Max(
            18,
            Math.Min(ArenaHeight - 18, predictedY));

        double gunBearing =
            GunBearingTo(predictedX, predictedY);

        SetTurnGunLeft(gunBearing);

        double radarBearing =
            RadarBearingTo(e.X, e.Y);

        SetTurnRadarLeft(radarBearing * 2);

        double strafe =
            BearingTo(e.X, e.Y) + 90;

        SetTurnRight(
            NormalizeRelativeAngle(strafe));

        SetForward(200 * moveDirection);

        if (GunHeat == 0 &&
            Math.Abs(gunBearing) < 6)
        {
            Fire(firePower);
        }
    }

    public override void OnHitBot(HitBotEvent e)
    {
        moveDirection *= -1;

        SetBack(150);

        if (GunHeat == 0)
            Fire(3);
    }

    public override void OnHitWall(HitWallEvent e)
    {
        moveDirection *= -1;

        SetBack(200);
        SetTurnRight(120);
    }

    private double NormalizeRelativeAngle(double angle)
    {
        while (angle > 180)
            angle -= 360;

        while (angle < -180)
            angle += 360;

        return angle;
    }
}