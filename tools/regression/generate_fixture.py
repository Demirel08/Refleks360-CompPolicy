"""
Refleks360 — Hafta 2 Regresyon Fixture Üretici

Mevcut Python hesap motorunun ÇIKTILARINI altın referans olarak JSON'a yazar.
C# tarafındaki SalaryCalculator bu JSON'a karşı (test) koşulur; herhangi bir vakada
1 kuruşun üstünde fark çıkarsa testler kırılır.

Kaynak hesap motoru: ../../IsciMaliyet (yerel klasör adı: Programlar/IsciMaliyet).
Python çalıştırırken IsciMaliyet/utils paketinin import edilebilmesi için PYTHONPATH'e
ekliyoruz; ayrıca app_paths import hatasını yutabilmek için import sırasını koruyoruz.

Çalıştırma:
    cd <repo-kökü>
    python tools/regression/generate_fixture.py

Çıktı:
    tests/Refleks360.Domain.Tests/Fixtures/regression-2026.json

2026 parametre kaynakları:
    - docs/05-HESAPLAMA-MOTORU.md (asgari ücret 33.030, SGK matrah max 247.725,
      GV dilim üst sınırları, GV istisna 1-7. ay %15 / 8-12. ay %20).
    - Damga ve SGK oranları sabit; SGK işveren teşviki kapalı (saf hesap için).
"""

from __future__ import annotations

import json
import os
import sys
from decimal import Decimal
from pathlib import Path

# ----------------------------------------------------------------------------
# Mevcut Python motoru (IsciMaliyet/utils/calculations.py) için yol kurulumu.
# Repo herhangi bir konumda olabilir; bu yüzden IsciMaliyet'i ya
# REFLEKS360_REFERENCE_DIR env-var'ından alıyoruz ya da yaygın konumları
# deniyoruz. (Geliştirici makinesi: C:\Users\<user>\OneDrive\Desktop\Programlar\IsciMaliyet)
# ----------------------------------------------------------------------------
HERE = Path(__file__).resolve().parent
REPO_ROOT = HERE.parent.parent


def _locate_isci_maliyet() -> Path | None:
    env = os.environ.get("REFLEKS360_REFERENCE_DIR")
    if env:
        p = Path(env).expanduser().resolve()
        if (p / "utils" / "calculations.py").exists():
            return p

    candidates: list[Path] = []
    user_home = Path.home()
    candidates.append(user_home / "OneDrive" / "Desktop" / "Programlar" / "IsciMaliyet")
    candidates.append(user_home / "Desktop" / "Programlar" / "IsciMaliyet")
    # Repo'nun siblinglari arasinda olabilir
    candidates.append(REPO_ROOT.parent / "IsciMaliyet")
    # Tarama: USERPROFILE altinda "Programlar/IsciMaliyet" varyasyonlari
    for p in candidates:
        if (p / "utils" / "calculations.py").exists():
            return p
    return None


ISCI_MALIYET_DIR = _locate_isci_maliyet()

if ISCI_MALIYET_DIR is None:
    sys.stderr.write(
        "HATA: IsciMaliyet klasoru bulunamadi.\n"
        "REFLEKS360_REFERENCE_DIR ortam degiskenini IsciMaliyet kok dizinine isaret ettir.\n"
        f"Ornek: set REFLEKS360_REFERENCE_DIR=C:\\Users\\<user>\\...\\IsciMaliyet\n"
    )
    sys.exit(1)

# IsciMaliyet kok dizinini path'e ekle ki 'utils.calculations' import edilebilsin.
sys.path.insert(0, str(ISCI_MALIYET_DIR))

# IsciMaliyet/utils/constants.py acilirken settings.json yoksa default kullanir;
# bizim 2026 degerlerimizi parametre olarak gecmemiz gerekir, default 2025 degerlerine bel baglamayiz.
from utils.calculations import monthly_components_given_gross_and_cum  # noqa: E402

# ----------------------------------------------------------------------------
# 2026 parametreleri — docs/05-HESAPLAMA-MOTORU.md'den
# ----------------------------------------------------------------------------
MIN_WAGE_GROSS_2026 = 33_030.00
SGK_BASE_MIN_2026 = MIN_WAGE_GROSS_2026
SGK_BASE_MAX_2026 = MIN_WAGE_GROSS_2026 * 7.5  # 247_725.00

PARAMS_2026 = {
    "sgk_employee": 0.14,
    "unemp_employee": 0.01,
    "sgk_employer": 0.2075,
    "unemp_employer": 0.02,
    "sgk_employer_discount": 0.05,
    "apply_sgk_discount": False,  # Fixture: indirim kapali — saf hesap
    "stamp_tax": 0.00759,
    "income_tax_brackets": [
        [190_000.00, 0.15],
        [400_000.00, 0.20],
        [1_500_000.00, 0.27],   # ucret tarifesi
        [5_300_000.00, 0.35],
        [float("inf"), 0.40],
    ],
}

# Asgari ucret istisna matrahi: brut asgariden iscinin kesintileri sonrasi kalan matrah.
# net asgari = 28.075,50  ===  matrah olarak da bu rakam (Damga ayri tutulur,
# bu istisna tutari spec'te 28.075,50 olarak veriliyor).
GV_EXEMPTION_BASE_2026 = MIN_WAGE_GROSS_2026 - (
    MIN_WAGE_GROSS_2026 * (PARAMS_2026["sgk_employee"] + PARAMS_2026["unemp_employee"])
)  # 28.075,50

STAMP_EXEMPTION_2026 = round(MIN_WAGE_GROSS_2026 * PARAMS_2026["stamp_tax"], 2)  # 250,70


def _period_for_month(month: int) -> dict:
    """Spec'e gore: 1-7. aylar %15 dilimde, 8-12. aylar %20 dilimde istisna uygulanir."""
    if month <= 7:
        rate = 0.15
    else:
        rate = 0.20

    return {
        "start_month": month,
        "end_month": month,
        "sgk_base_min": SGK_BASE_MIN_2026,
        "sgk_base_max": SGK_BASE_MAX_2026,
        "gv_exemption_amount": GV_EXEMPTION_BASE_2026,
        "gv_exemption_rate": rate,
        "stamp_exemption_amount": STAMP_EXEMPTION_2026,
        # SGK indirimi kapali -> apply_sgk_discount=False
        # Python kodu Pm dict'i icindeki anahtarlari da kullaniyor (clip vs.)
        # bu yuzden PARAMS_2026 ile birlesik bir dict olusturuyoruz asagida.
    }


def _merge_period_with_params(period: dict) -> dict:
    """Python monthly_components fonksiyonu birlesik bir 'Pm' dict bekler."""
    merged = dict(PARAMS_2026)
    merged.update(period)
    return merged


def _build_gross_cases() -> list[float]:
    """50 farkli brut maas senaryosu — spec'teki dagilim hedeflenir."""
    cases: list[float] = []

    # 1) Asgari ucret civari (10 vaka) — clip kurali, istisna sinirlari
    cases.extend([
        25_000.00,        # asgari altinda — clip min'e cikar
        30_000.00,        # asgari altinda
        33_030.00,        # asgari ucret tam
        33_500.00,
        34_500.00,
        36_000.00,
        38_500.00,
        40_000.00,
        45_000.00,
        50_000.00,
    ])

    # 2) Orta seviye (15 vaka) — yil icinde dilim gecisleri yakalanir
    cases.extend([
        55_000.00,
        60_000.00,
        65_000.00,
        70_000.00,
        75_000.00,
        80_000.00,
        90_000.00,
        100_000.00,
        110_000.00,
        120_000.00,
        125_000.00,
        135_000.00,
        140_000.00,
        145_000.00,
        150_000.00,
    ])

    # 3) Ust seviye (10 vaka) — 27% / 35% dilim gecisleri yil icinde
    cases.extend([
        160_000.00,
        175_000.00,
        190_000.00,
        200_000.00,
        210_000.00,
        220_000.00,
        230_000.00,
        240_000.00,
        247_725.00,      # SGK matrah tavani tam
        260_000.00,      # tavan ustu — clip max
    ])

    # 4) Tavan ustu (10 vaka) — SGK matrah tavanini gectikten sonra PEK sabit kalir
    cases.extend([
        280_000.00,
        300_000.00,
        350_000.00,
        400_000.00,
        500_000.00,
        650_000.00,
        800_000.00,
        950_000.00,
        1_000_000.00,
        1_200_000.00,
    ])

    # 5) Ekstrem (5 vaka) — yil ortasinda 35% / 40% dilimini gormek
    cases.extend([
        1_500_000.00,
        2_000_000.00,
        3_000_000.00,
        5_000_000.00,
        8_000_000.00,
    ])

    assert len(cases) == 50, f"50 vaka bekleniyordu, {len(cases)} bulundu"
    return cases


def _simulate_year(gross: float) -> list[dict]:
    """12 ay ayni brut ile yillik kumulatif takibi."""
    out: list[dict] = []
    cum_taxable = 0.0
    for m in range(1, 13):
        Pm = _merge_period_with_params(_period_for_month(m))
        comp = monthly_components_given_gross_and_cum(gross, cum_taxable, Pm)
        out.append({
            "month": m,
            "cumulative_taxable_before": cum_taxable,
            "pek": round(comp["pek"], 6),
            "employee_contribution": round(comp["emp_contrib"], 6),
            "monthly_tax_base": round(comp["taxable_for_gv"], 6),
            "income_tax_raw": round(comp["gv_raw"], 6),
            "income_tax_exemption": round(comp["gv_exemption"], 6),
            "income_tax": round(comp["gv"], 6),
            "stamp_tax": round(comp["stamp"], 6),
            "net": round(comp["net"], 6),
            "employer_cost": round(comp["employer_cost"], 6),
        })
        cum_taxable += comp["taxable_for_gv"]
    return out


def main() -> None:
    grosses = _build_gross_cases()
    cases = []
    for idx, g in enumerate(grosses, start=1):
        cases.append({
            "id": idx,
            "label": f"brut_{int(g):,}".replace(",", "_"),
            "gross": g,
            "months": _simulate_year(g),
        })

    fixture = {
        "version": 1,
        "generator": "tools/regression/generate_fixture.py",
        "source_engine": "IsciMaliyet/utils/calculations.py (referans Python motoru)",
        "year": 2026,
        "parameters": {
            "sgk_employee_rate": PARAMS_2026["sgk_employee"],
            "unemp_employee_rate": PARAMS_2026["unemp_employee"],
            "sgk_employer_rate": PARAMS_2026["sgk_employer"],
            "unemp_employer_rate": PARAMS_2026["unemp_employer"],
            "sgk_employer_discount_rate": PARAMS_2026["sgk_employer_discount"],
            "apply_sgk_employer_discount": PARAMS_2026["apply_sgk_discount"],
            "stamp_tax_rate": PARAMS_2026["stamp_tax"],
            "income_tax_brackets": [
                {
                    "upper_limit": (limit if limit != float("inf") else "Infinity"),
                    "rate": rate,
                }
                for (limit, rate) in PARAMS_2026["income_tax_brackets"]
            ],
            "sgk_base_min": SGK_BASE_MIN_2026,
            "sgk_base_max": SGK_BASE_MAX_2026,
            "gv_exemption_base": GV_EXEMPTION_BASE_2026,
            "stamp_exemption_per_month": STAMP_EXEMPTION_2026,
            "gv_exemption_rate_by_month": {
                str(m): (0.15 if m <= 7 else 0.20) for m in range(1, 13)
            },
        },
        "cases": cases,
    }

    out_path = REPO_ROOT / "tests" / "Refleks360.Domain.Tests" / "Fixtures" / "regression-2026.json"
    out_path.parent.mkdir(parents=True, exist_ok=True)

    with out_path.open("w", encoding="utf-8") as f:
        json.dump(fixture, f, ensure_ascii=False, indent=2)

    print(f"OK: {len(cases)} vaka yazildi -> {out_path.relative_to(REPO_ROOT)}")


if __name__ == "__main__":
    main()
