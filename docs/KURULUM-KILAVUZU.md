# Refleks 360 ÜP — Kurulum Kılavuzu

> Üretim ortamı için adım adım kurulum talimatları. Bu doküman MVP v1.0.0 ile birlikte yayınlanan ZIP paketinin (`refleks360-vX.Y.Z-win-x64.zip`) kurulumunu kapsar.

---

## 1. Sistem Gereksinimleri

### Sunucu
- **İşletim sistemi**: Windows Server 2019 / 2022 / Windows 11
- **CPU**: 4 çekirdek (8 önerilir)
- **RAM**: 8 GB (16 GB önerilir, 5000+ çalışan için 32 GB)
- **Disk**: 20 GB (uygulama + DB + log) — SSD önerilir
- **Network**: 100 Mbps (LAN içi)

### Veritabanı
- **SQL Server 2019 / 2022** (Express, Standard veya Enterprise)
- SQL Server LocalDB **desteklenmez** (production için)
- Collation: `Turkish_CI_AS` veya uyumlu

### Çalışan Tarayıcı (kullanıcılar için)
- Edge / Chrome / Firefox güncel sürümleri
- WebSocket bağlantısı açık olmalı (Blazor Server SignalR için)

---

## 2. Veritabanı Hazırlığı

SQL Server'da boş bir veritabanı oluştur (uygulama migration'larla şemayı kuracak):

```sql
CREATE DATABASE Refleks360 COLLATE Turkish_CI_AS;
GO
```

Uygulamanın bağlanacağı bir SQL kullanıcısı oluştur (önerilen — Windows Auth kullanmıyorsan):

```sql
CREATE LOGIN refleks360_app WITH PASSWORD = 'YOUR_STRONG_PASSWORD';
USE Refleks360;
CREATE USER refleks360_app FOR LOGIN refleks360_app;
ALTER ROLE db_owner ADD MEMBER refleks360_app;
```

> **Not (Windows 11):** Yerel SQL Server kurulumunda NVMe disklerde 4 KB sektör uyumsuzluğu hatası alırsan `docs/NOTLAR-LISANS-VE-SECRETS.md §6`'daki registry fix'i uygula.

---

## 3. Uygulama Kurulumu

1. **ZIP'i kopyala**: `refleks360-vX.Y.Z-win-x64.zip` dosyasını sunucuda geçici bir klasöre çıkar.
2. **Yönetici PowerShell** aç ve şu komutu çalıştır:

```powershell
cd C:\TEMP\refleks360-v1.0.0-win-x64
.\install.ps1
```

   Bu script:
   - `C:\Refleks360`'a dosyaları kopyalar
   - Windows servisi olarak `Refleks360` kaydeder (otomatik başlatma)
   - 5000 portunu açar (firewall)
   - `appsettings.Production.template.json`'u `appsettings.Production.json` olarak kopyalar
   - Servisi başlatır

3. **Parametreleri doldur**: `C:\Refleks360\appsettings.Production.json`:

```jsonc
{
  "ConnectionStrings": {
    "Default": "Server=DB-SUNUCU\\SQLINST;Database=Refleks360;User Id=refleks360_app;Password=...;TrustServerCertificate=true;MultipleActiveResultSets=true"
  },
  "Syncfusion": {
    "LicenseKey": "Ngo9BigBOggjGy...."  // Syncfusion Community License (firmanın yıllık geliri <$1M)
  },
  "AdminSeed": {
    "Password": "BaslangicSifreniz!123"  // İlk admin şifresi (sonra UI'dan değiştirin)
  }
}
```

4. **Servisi yeniden başlat**:

```powershell
Restart-Service Refleks360
```

5. **İlk girişi yap**: <http://SUNUCU:5000/> → `admin` / `BaslangicSifreniz!123`.
   - Yeni bir admin kullanıcı oluştur, eski `admin` parolasını değiştir.

---

## 4. HTTPS (Önerilir)

### Self-signed (test):
```powershell
$cert = New-SelfSignedCertificate -DnsName "refleks360.firma.local" -CertStoreLocation cert:\LocalMachine\My
$pwd = ConvertTo-SecureString -String "CertPwd" -Force -AsPlainText
Export-PfxCertificate -Cert $cert -FilePath "C:\Refleks360\cert.pfx" -Password $pwd
```

### CA-signed (production):
Mevcut .pfx dosyasını `C:\Refleks360\cert.pfx`'e kopyala.

### appsettings.Production.json:

```jsonc
"Kestrel": {
  "Endpoints": {
    "Http":  { "Url": "http://+:5000" },
    "Https": {
      "Url": "https://+:5001",
      "Certificate": { "Path": "cert.pfx", "Password": "CertPwd" }
    }
  }
}
```

Firewall'a 5001 portunu da aç:
```powershell
New-NetFirewallRule -DisplayName "Refleks360-5001" -Direction Inbound -Action Allow -Protocol TCP -LocalPort 5001
```

Servisi yeniden başlat.

---

## 5. Yedekleme

Günlük tam yedek (SQL Server Agent veya görev zamanlayıcı):
```sql
BACKUP DATABASE Refleks360 TO DISK = 'D:\Backup\Refleks360.bak' WITH INIT;
```

Saatlik transaction log yedeği (Full recovery model):
```sql
BACKUP LOG Refleks360 TO DISK = 'D:\Backup\Refleks360-log.bak' WITH INIT;
```

> İlk pilot için sadece günlük FULL yeterli olabilir; veri kritik olduğunda PITR (Point-In-Time Restore) için log yedeği eklenir.

---

## 6. Güncelleme

1. Yeni ZIP'i geçici klasöre çıkar
2. `install.ps1`'i tekrar çalıştır — servis önce durdurulur, dosyalar üzerine yazılır, servis tekrar başlar
3. `appsettings.Production.json` korunur (yeni dosya üzerine yazılmaz)
4. DB migration'lar uygulama açılışında otomatik uygulanır

---

## 7. Kaldırma

```powershell
.\uninstall.ps1                 # Klasör + servis + firewall kaldırılır
.\uninstall.ps1 -KeepData       # Servis kaldırılır, dosyalar korunur
```

DB'yi manuel olarak `DROP DATABASE Refleks360` ile silebilirsiniz.

---

## 8. Sık Karşılaşılan Sorunlar

| Sorun | Çözüm |
|---|---|
| Servis başlamıyor | Event Viewer → Windows Logs → Application'da kaynak `Refleks360.Web`'i ara |
| DB bağlantı hatası | ConnectionStrings:Default doğru mu? SQL Server `mixed mode auth` açık mı? |
| Syncfusion lisans uyarısı çıkıyor | LicenseKey doldurulmadı veya v33 dışı bir versiyon için key kullanıldı |
| Login başarısız | Admin kilitlenmişse 15 dk bekle veya DB'de `AspNetUsers.LockoutEnd` sıfırla |
| Performans yavaş | EmployeeSalary'de index var; 5000+ çalışan için Hangfire worker sayısını arttır |

---

## 9. Lisans ve Destek

- **Yazılım lisansı**: Refleks 360 ÜP yıllık abonelik (`docs/10-KARARLAR-VE-TAKVIM.md §15`)
- **Syncfusion Community License**: yıllık gelir <$1M olan şirketler için ücretsiz; her yıl yenilenmesi gerekir
- **Destek**: pilot dönemde doğrudan geliştirici desteği
