# Tubes_MUsangking
Tugas Besar strategi algoritma robocode tank royale

# Greedy Algorithm Bot for Robocode Tank Royale

## Deskripsi
Robocode Tank Royale adalah permainan pemrograman di mana pemain merancang bot berbentuk tank virtual yang bertarung dalam arena hingga hanya tersisa satu pemenang. Pemain mengembangkan algoritma bot yang mengatur strategi pergerakan, deteksi lawan, dan serangan tanpa kendali langsung selama pertempuran. Bot ini diimplementasikan menggunakan C# dengan strategi greedy.

## Algoritma Greedy yang Diimplementasikan
  1. **Dragon**: bot yang berfokus pada memaksimalkan skor melalui eliminasi musuh (kill count). Bot menggunakan algoritma Greedy dengan selalu memilih aksi yang memberikan peluang terbesar untuk menyerang target yang sedang terdeteksi. Dragon bergerak mengelilingi arena sambil melakukan scanning radar secara terus-menerus. Ketika musuh ditemukan, radar dan meriam langsung diarahkan ke target, kemudian bot memilih kekuatan tembakan berdasarkan jarak musuh untuk meningkatkan peluang memperoleh kill.
  2. **BotIreng**: bot yang berfokus pada memaksimalkan Bullet Damage dengan meningkatkan peluang peluru mengenai target. Bot menggunakan algoritma Greedy dengan selalu memilih aksi yang memberikan peluang hit terbesar terhadap musuh yang sedang terdeteksi. Ireng mempertahankan radar lock pada target, memprediksi posisi musuh berdasarkan arah dan kecepatan geraknya, kemudian menembakkan peluru ke posisi prediksi tersebut. Selain itu, bot bergerak menyamping (strafing) dan mengubah arah secara berkala untuk mengurangi kemungkinan terkena serangan lawan.
  3. **Betabot**: bot tempur yang mengutamakan keselamatan diri dengan selalu menjaga jarak aman dari musuh sambil terus mengunci posisi musuh menggunakan radar secara konsisten. Dengan posisi jarak jauh yang terjaga, Betabot fokus memberikan kerusakan melalui tembakan peluru yang akurat dan berkelanjutan sepanjang pertandingan.
  4. **Banzai**: Bot ini berkerja dengan menerapan strategi seperti tentara jepang kamikaze yang maju kehadapan musuh  ke sedekat mungkin tanpa memikirkan resiko dan mengutamakan penyerangan musuh. Bot ini akan mengunci satu target dan akan menyerang dan melakuakn segala cara untuk mendekata ke arah target dan menyerang nya sampai jarak seminimal mungkin

## Requirement
Sebelum menjalankan program, pastikan Anda memiliki beberapa dependensi berikut:
  - .NET 6.0 atau lebih baru (untuk menjalankan aplikasi C#).
  - Robocode Tank Royale: Program permainan

## Cara menjalankan Program
  1. clone repository ini ke mesin lokal Anda:
      (https://github.com/MA-D-A/Tubes_MUsangking/)
     
  2. Dalam setiap folder bot yang ingin digunakan, edit file NamaBot.csproj pada bagian TargetFramework dengan mengubahnya menjadi versi .NET anda:
  3. Hapus folder bin dan obj, dan jalankan:
## Bash
jika menggunakan linux jalankan <br>
(**file .sh**) 

## Command Prompt 
jika menggunakan os windows <br>
(**file.cmd**)

## Author
| Nama | NIM |
|------|-----|
| M Alif Dasya Anwar | 124140133 |
| Rasya Dhiandra Bangsawan | 124140163 |
| Muhammad Reza Taufiqurrahman | 124140205 |
     
    
 


