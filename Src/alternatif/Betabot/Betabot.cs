using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Betabot : Bot
{
    // Variabel penanda untuk mencegah perang perintah antara Run() and OnScannedBot
    private bool hasTarget = false;
    private int scanTimeout = 0;

    // Variabel pendukung gerakan menghindar
    private int moveDirection = 1;
    private readonly Random rng = new Random();

    static void Main(string[] args) => new Betabot().Start();

    Betabot() : base(BotInfo.FromFile("Betabot.json")) { }

    public override void Run()
    {
        // Warna Khas Betabot
        BodyColor = Color.Cyan;
        TurretColor = Color.DarkBlue;
        RadarColor = Color.White;
        BulletColor = Color.LightBlue;

        // Pisahkan rotasi bodi, gun, dan radar agar bergerak independen
        AdjustRadarForBodyTurn = true;
        AdjustRadarForGunTurn = true;
        AdjustGunForBodyTurn = true;

        while (IsRunning)
        {
            if (!hasTarget)
            {
                // HANYA melakukan sweep jika radar belum mendeteksi musuh sama sekali
                SetTurnRadarLeft(180);
                
                // Jika kehilangan musuh, bergerak maju perlahan untuk mencari kembali
                SetForward(50);
            }
            else
            {
                scanTimeout++;
                // Proteksi: Jika dalam 4 tick musuh bergerak ekstrem dan hilang dari radar,
                // reset status agar bot kembali melakukan sweep 180 derajat.
                if (scanTimeout > 4)
                {
                    hasTarget = false;
                }
            }

            // === STRATEGI ANTI-WALL (Pencegahan Menabrak Dinding Saat Menghindar) ===
            if (X < 70 || X > ArenaWidth - 70 || Y < 70 || Y > ArenaHeight - 70)
            {
                moveDirection *= -1; // Balikkan arah bodi
                SetTurnRight(45);    // Buat sudut buang dari dinding
                SetForward(100 * moveDirection);
            }

            // Eksekusi semua perintah secara bersamaan dalam 1 tick
            Go();
        }
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        // Set status bahwa musuh terkunci dan reset timer kehilangan musuh
        hasTarget = true;
        scanTimeout = 0;

        // Jarak ke musuh saat ini
        double distance = DistanceTo(e.X, e.Y);

        // === 1. LOCK RADAR ===
        // Hitung selisih sudut radar saat ini dengan posisi musuh
        double radarBearing = RadarBearingTo(e.X, e.Y);
        
        // Menggunakan pengali 1.5 agar terjadi "oscillating lock" (radar bergoyang tipis
        // melewati tubuh musuh agar sapuan sensor tidak terputus)
        SetTurnRadarLeft(radarBearing * 1.5);

        // === 2. GERAKAN MENGHINDAR (Circle Orbiting & Random Zig-zag) ===
        // Hitung sudut bodi robot menuju musuh
        double bodyBearing = BearingTo(e.X, e.Y);

        // Mengorbit tegak lurus (90 derajat) terhadap musuh.
        // Ditambah sedikit penyesuaian sudut: masuk mendekat jika terlalu jauh, keluar jika terlalu dekat.
        double approachAngle = 90;
        if (distance > 300) approachAngle = 75;  // Mendekat perlahan agar senjata efektif
        if (distance < 150) approachAngle = 105; // Menjauh jika terlalu intim dengan musuh

        // Set belokan bodi mengitari musuh
        SetTurnRight(bodyBearing + (approachAngle * moveDirection));
        
        // Buat bot bergerak konstan maju/mundur
        SetForward(120 * moveDirection);

        // Peluang acak 2% di setiap tick untuk berbalik arah secara mendadak.
        // Ini akan mengacaukan robot musuh yang menggunakan sistem prediksi tembakan (Linear/Circular Aim).
        if (rng.NextDouble() < 0.02)
        {
            moveDirection *= -1;
        }

        // === 3. ARAHKAN GUN & TEMBAK ===
        double gunBearing = GunBearingTo(e.X, e.Y);
        SetTurnGunLeft(gunBearing);

        // Hanya menembak jika moncong meriam sudah relatif lurus dengan musuh (toleransi 15 derajat)
        // Ini mencegah peluru keluar sia-sia saat meriam masih dalam proses berputar
        if (GunHeat == 0 && Math.Abs(gunBearing) < 15)
        {
            SetFire(2);
        }
    }

    // === 4. RESPONS TAMBAHAN KETIKA TERKENA PELURU ===
    public override void OnHitByBullet(HitByBulletEvent e)
    {
        // Jika terkena peluru, langsung paksa ganti arah gerakan secara instan untuk mengecoh
        moveDirection *= -1;
        SetForward(100 * moveDirection);
    }
}