using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Banzai : Bot
{
    double[] enemy = new double[2];
    int turnsSinceShot = 0;
    double nearestDistance = double.MaxValue;
    int lockedBotId = -1;
    bool ramMode = false;
    int ramTimer = 0;

    static void Main(string[] args) => new Banzai().Start();
    Banzai() : base(BotInfo.FromFile("Banzai.json")) { }

    public override void Run()
    {
        BodyColor   = Color.Brown;
        TurretColor = Color.Brown;
        RadarColor  = Color.Orange;
        BulletColor = Color.Yellow;

        AdjustGunForBodyTurn = AdjustRadarForGunTurn = AdjustRadarForBodyTurn = true;
        enemy = new double[] { ArenaWidth / 2, ArenaHeight / 2 };
        SetTurnRadarLeft(360 * (RadarBearingTo(enemy[0], enemy[1]) > 0 ? 1 : -1));
        SetTurnGunLeft(GunBearingTo(enemy[0], enemy[1]));

        while (IsRunning)
        {
            turnsSinceShot++;
            if (ramMode && --ramTimer <= 0) ramMode = false;

            SetForward(999);
            if (turnsSinceShot > 10)
                SetTurnRadarLeft(360);
            else
                SetTurnLeft(BearingTo(enemy[0], enemy[1]));

            SetRescan();
            Go();
            nearestDistance = double.MaxValue;
        }
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        double dist = DistanceTo(e.X, e.Y);

        if (ramMode)
        {
            lockedBotId = e.ScannedBotId;
            turnsSinceShot = 0;
            SetTurnLeft(BearingTo(e.X, e.Y));
            SetForward(dist + 20); // Ditambah 20 agar melaju menembus koordinat musuh (menabrak keras)
            SetTurnGunLeft(GunBearingTo(e.X, e.Y));
            SetFire(3);
            enemy[0] = e.X; enemy[1] = e.Y;
            return;
        }

        if (lockedBotId != -1 && e.ScannedBotId != lockedBotId) return;

        if (lockedBotId == -1)
        {
            if (dist >= nearestDistance) return;
            nearestDistance = dist;
            lockedBotId = e.ScannedBotId;
        }

        turnsSinceShot = 0;
        SetTurnRadarLeft(RadarBearingTo(e.X, e.Y));

        double[] preds = { e.X, e.Y };
        double deltaTime = 0;
        double power = dist < 80 ? 4 : (dist < 350 ? 1.5 : 0.8);
        double bulletSpeed = CalcBulletSpeed(power);

        while (++deltaTime * bulletSpeed < DistanceTo(preds[0], preds[1]))
        {
            preds[0] += Math.Sin(e.Direction) * e.Speed;
            preds[1] += Math.Cos(e.Direction) * e.Speed;
            if (preds[0] < 18 || preds[1] < 18 || preds[0] > ArenaWidth - 18 || preds[1] > ArenaHeight - 18)
            {
                preds[0] = Math.Min(Math.Max(18, preds[0]), ArenaWidth - 18);
                preds[1] = Math.Min(Math.Max(18, preds[1]), ArenaHeight - 18);
                break;
            }
        }

        SetTurnGunLeft(GunBearingTo(preds[0], preds[1]));
        SetFire(power);
        Array.Copy(preds, enemy, 2);
    }

    public override void OnBotDeath(BotDeathEvent e)
    {
        if (e.VictimId == lockedBotId)
        {
            lockedBotId = -1;
            nearestDistance = double.MaxValue;
        }
    }

    public override void OnHitByBullet(HitByBulletEvent e)
    {
        double radarTurn = e.Bullet.Direction + 180 - RadarDirection;
        while (radarTurn > 180) radarTurn -= 360;
        while (radarTurn < -180) radarTurn += 360;
        SetTurnRadarLeft(radarTurn);
        ramMode = true;
        ramTimer = 60; // Durasi ramMode saat tertembak diperpanjang agar lebih mematikan
    }
}