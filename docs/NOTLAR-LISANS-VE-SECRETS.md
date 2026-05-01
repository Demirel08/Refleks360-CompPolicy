# Notlar — Syncfusion Lisansı ve Gizli Bilgiler (Secrets)

> Bu dosya, Syncfusion Community License anahtarı ve genel "secret" yönetimi hakkında **operasyonel hatırlatmadır**. Spec değildir. Karar değişirse `10-KARARLAR-VE-TAKVIM.md` güncellenir.

---

## Özet

Syncfusion lisans anahtarı **kaynak koda gömülü değildir**. Repo public olduğu için anahtar Git'e girerse Syncfusion onu iptal edebilir. Üç ortam var:

| Ortam | Anahtar nereden okunur | Kim koyar |
|---|---|---|
| Geliştirme (bu makine) | .NET User Secrets | Bir kez `dotnet user-secrets set` ile konuldu |
| Yeni geliştirme makinesi | .NET User Secrets | Yeni makinede aynı komut tekrar çalıştırılır |
| Müşteri sunucusu (Production) | `appsettings.Production.json` veya environment variable | WiX MSI installer (Hafta 19-20) |

Kod tarafı sabittir: `Program.cs` her ortamda `Configuration["Syncfusion:LicenseKey"]`'i okur. Sadece **anahtarın nereden geldiği** ortama göre değişir.

---

## 1. Geliştirme — User Secrets

Anahtar şu anda bu konumda:

```
%APPDATA%\Microsoft\UserSecrets\cd1827e5-0416-4759-82a3-29196189c599\secrets.json
```

(`UserSecretsId`, `src/Refleks360.Web/Refleks360.Web.csproj` içinde sabit.)

User Secrets dosyası:
- Repo'nun **dışındadır** — Git'e girmez.
- Sadece bu Windows kullanıcısı için geçerlidir.
- ASP.NET Core sadece `Development` ortamında otomatik okur (`appsettings.json` ve `appsettings.Development.json`'dan sonra, üzerine yazarak).

### Yeni bir geliştirme makinesinde çalıştırmak için

```powershell
cd src\Refleks360.Web
dotnet user-secrets set "Syncfusion:LicenseKey" "<ANAHTAR>"
```

Anahtar Syncfusion hesabından üretiliyor: https://www.syncfusion.com/account/downloads → "Get License Key" → Version: 33.x.x.

---

## 2. Production — Müşteri Sunucusu

Production'da User Secrets çalışmaz. Üç seçenek var, **birini** seçeceğiz:

### Seçenek A: appsettings.Production.json (en yaygın)

Sunucuda dosya şu şekilde olur:

```json
{
  "Syncfusion": {
    "LicenseKey": "<ANAHTAR>"
  }
}
```

- Bu dosya `.gitignore`'da **(satır 56)** — Git'e girmez.
- Kurulum sırasında installer tarafından oluşturulur.

### Seçenek B: Environment Variable

```
Syncfusion__LicenseKey=<ANAHTAR>
```

(çift alt çizgi `__` `:` yerine geçer.)

- Windows servis olarak çalışıyorsa registry'de `HKLM\SYSTEM\CurrentControlSet\Services\<ServiceName>\Environment` altında.
- Installer tarafından servis kurulumunda set edilir.

### Seçenek C: WiX installer içine gömme

Anahtar installer kaynak kodunda şifrelenmiş olarak durur, kurulum sırasında A veya B yoluyla yazılır. Kullanıcı hiçbir şey yapmaz.

---

## 3. Plan: Hafta 19-20'de WiX Installer

Hafta 19-20'de installer yazılırken bu kararlar verilecek:

- [ ] Seçenek A mı B mi? (öneri: A — appsettings.Production.json daha şeffaf)
- [ ] Anahtar installer içine nasıl gömülecek? (WiX `<Component>` + dosya enjeksiyonu)
- [ ] Müşteri kendi anahtarını mı kullanacak (ileride paid lisansa geçince)? Eğer evet, installer'a bir input dialog eklenir.

Şimdilik bu kararlar **ertelendi**. Hafta 19-20'ye gelene kadar User Secrets'ta her şey çalışır.

---

## 4. Diğer Secrets

İleride aynı yöntem (User Secrets → appsettings.Production.json/env var) şunlar için de kullanılacak:

- SQL Server connection string (`ConnectionStrings:Default`)
- Hangfire connection string (genelde aynı DB)
- E-posta SMTP şifresi (rapor/hatırlatma e-postaları için)
- Always Encrypted master key referansı

`secrets.json`, `appsettings.Production.json`, `appsettings.Local.json`, `appsettings.Staging.json`, `*.pfx`, `*.p12`, `.env*` — hepsi `.gitignore`'da.

---

## 5. Anahtar Kaybedilirse / Değiştirilirse

Syncfusion hesabına gir → License & Downloads → "Get License Key" → yeni anahtar üret.
- Aynı major version (33.x) için tekrar üretebilirsin, eski anahtar invalidate olmaz.
- Major version değişiminde (örn 34.x'e geçiş) yeni anahtar gerekir.
- `dotnet user-secrets set "Syncfusion:LicenseKey" "<YENI>"` ile güncelle.
