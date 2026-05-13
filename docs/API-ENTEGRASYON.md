# Refleks 360 ÜP — REST API Dokümantasyonu

> Faz 3 ile gelen dış sistem entegrasyon API'leri. Logo Tiger HR, Mikro Bordro veya
> herhangi bir ERP'den çalışan/maaş verisi senkronizasyonu için kullanılır.

---

## Kimlik Doğrulama

Tüm API uçları `X-Api-Key` header'ı gerektirir. API key'ler `appsettings.json` içinde:

```json
{
  "Api": {
    "Keys": [
      "DEV-aw3xkPp9qLq2zJk", 
      "PROD-yQv7m2BcLx8RtA"
    ]
  }
}
```

Birden fazla key tanımlanabilir (rotasyon için). Üretimde her bağlanan sistem için
ayrı key kullanılması önerilir.

---

## Endpoint'ler

Base URL: `https://<sunucu>/api/v1`

### `GET /employees`
Tüm çalışanların özet listesi.

**Yanıt** (200):
```json
[
  {
    "id": 1,
    "employeeNumber": "1001",
    "firstName": "Ahmet",
    "lastName": "Yılmaz",
    "email": "ahmet.yilmaz@firma.com",
    "department": "Bilgi Teknolojileri",
    "position": "Yazılım Geliştirme Mühendisi",
    "grade": "P2",
    "hireDate": "2020-01-01",
    "status": "Active",
    "currentGross": 64400
  }
]
```

### `GET /employees/{id}`
Tek çalışan detayı.

### `GET /employees/{id}/salaries`
Çalışanın ücret geçmişi (en yeniden eskiye).

### `POST /employees`
Yeni çalışan oluştur (sicil unique olmalı).

**İstek**:
```json
{
  "employeeNumber": "9999",
  "firstName": "Test",
  "lastName": "Kullanıcı",
  "email": "test@firma.com",
  "phone": "5551112233",
  "hireDate": "2026-06-01",
  "positionId": 2,
  "departmentId": 1,
  "locationId": 1,
  "managerId": null,
  "notes": "API üzerinden oluşturuldu"
}
```

**Yanıt** (201):
```json
{ "id": 31 }
```

### `GET /companies`, `GET /departments`, `GET /positions`
Lookup verileri (entegrasyon için Id eşleştirme).

---

## cURL Örnekleri

```bash
# Tüm çalışanlar
curl -H "X-Api-Key: PROD-yQv7m2BcLx8RtA" https://refleks360/api/v1/employees

# Yeni çalışan
curl -X POST -H "X-Api-Key: PROD-yQv7m2BcLx8RtA" -H "Content-Type: application/json" \
     -d '{"employeeNumber":"9999","firstName":"Test","lastName":"X","hireDate":"2026-06-01","positionId":2,"departmentId":1,"locationId":1}' \
     https://refleks360/api/v1/employees
```

---

## Hata Kodları

| Kod | Anlam |
|---|---|
| 200 | Başarılı |
| 201 | Oluşturuldu |
| 401 | API key eksik veya geçersiz |
| 403 | Yetkisiz işlem |
| 404 | Kayıt yok |
| 400 | Geçersiz istek (validation) |

---

## Rate Limiting (Faz 4+)

Şu an rate limit yok; üretimde Microsoft.AspNetCore.RateLimiting paketi ile
kullanıcı bazında 100 req/dak gibi limit eklenecek.

---

## Logo Tiger HR / Mikro Bordro Entegrasyonu

Bu ERP'ler genellikle SQL Server üzerinden veri alışverişi yapar.
Önerilen mimari:

1. **Tek yönlü senkron (ERP → Refleks360)**:
   - ERP'nin günlük export'ladığı CSV'yi Hangfire job ile çek
   - `POST /api/v1/employees` ile yeni kayıtları oluştur
   - Sicil eşleşen kayıtları güncelle

2. **Refleks360 → ERP**:
   - Senaryo uygulandığında yeni ücretleri `EmployeeSalary` tablosundan oku
   - ERP'nin import formatına dönüştür (CSV/XML)
   - SFTP veya direkt SQL Server staging tablosu üzerinden aktar

Logo Tiger için resmî bir HR connector yok; çoğu kurulum CSV/Excel takası yapar.
Bu API tam olarak bu ihtiyaca yöneliktir.
