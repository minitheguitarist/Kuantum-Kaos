const readline = require('readline');

// Konsoldan girdi almak için arayüz (Modern Promise yapısı)
const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

const sor = (soru) => new Promise(resolve => rl.question(soru, resolve));

// --- 1. ÖZEL HATA SINIFI ---
class KuantumCokusuException extends Error {
    constructor(nesneId) {
        super(`KRİTİK HATA! Nesne ${nesneId} yüzünden stabilite çöktü!`);
        this.name = "KuantumCokusuException";
    }
}

// --- 3. SOYUT ATA SINIF ---
class KuantumNesnesi {
    constructor(id, tehlike) {
        if (this.constructor === KuantumNesnesi) {
            throw new Error("Abstract sınıftan nesne türetilemez!");
        }
        this.id = id;
        this.tehlikeSeviyesi = tehlike;
        this._stabilite = 100.0;
    }


    get stabilite() { return this._stabilite; }

    set stabilite(value) {
        if (value > 100) this._stabilite = 100;
        else this._stabilite = value;

        if (this._stabilite <= 0) {
            throw new KuantumCokusuException(this.id);
        }
    }

    analizEt() {
        throw new Error("Abstract metod implemente edilmeli!");
    }

    durumBilgisi() {
        return `ID: ${this.id} | Stabilite: %${this.stabilite.toFixed(2)} | Tehlike: ${this.tehlikeSeviyesi}`;
    }
}

// --- 2. ARA SINIF (Interface Yerine - IKritik Simülasyonu) ---
// JS'de interface olmadığı için 'Kritik' nesneleri bu sınıftan türet.
// Böylece 'instanceof KritikNesne' diyerek kontrol edebileceğiz.
class KritikNesne extends KuantumNesnesi {
    acilDurumSogutmasi() {
        throw new Error("Implemente edilmeli");
    }
}

// --- 4. SOMUT SINIFLAR ---

class VeriPaketi extends KuantumNesnesi {
    constructor(id) { super(id, 1); }

    analizEt() {
        this.stabilite -= 5;
        console.log(`[${this.id}] Veri içeriği okundu.`);
    }
}

class KaranlikMadde extends KritikNesne {
    constructor(id) { super(id, 5); }

    analizEt() {
        this.stabilite -= 15;
        console.log(`[${this.id}] Karanlık madde analizi... Enerji dalgalanıyor!`);
    }

    acilDurumSogutmasi() {
        this.stabilite += 50;
        console.log(`[${this.id}] Soğutma başarılı.`);
    }
}

class AntiMadde extends KritikNesne {
    constructor(id) { super(id, 10); }

    analizEt() {
        this.stabilite -= 25;
        console.log(`[${this.id}] UYARI: Evrenin dokusu titriyor...`);
    }

    acilDurumSogutmasi() {
        this.stabilite += 50;
        console.log(`[${this.id}] Anti Madde nötralize edildi.`);
    }
}

// --- 5. MAIN LOOP (Async Func) ---
async function main() {
    const envanter = [];
    let oyunDevam = true;

    console.log("--- OMEGA SEKTÖRÜ (NODE.JS) ---");

    while (oyunDevam) {
        console.log("\n=== KONTROL PANELİ ===");
        console.log("1. Yeni Nesne Ekle");
        console.log("2. Listele");
        console.log("3. Analiz Et");
        console.log("4. Soğutma");
        console.log("5. Çıkış");

        const secim = await sor("Seçiminiz: ");

        try {
            switch (secim) {
                case "1":
                    const r = Math.floor(Math.random() * 3);
                    const nid = "JS-NESNE-" + Math.floor(Math.random() * 900 + 100);
                    if (r === 0) envanter.push(new VeriPaketi(nid));
                    else if (r === 1) envanter.push(new KaranlikMadde(nid));
                    else envanter.push(new AntiMadde(nid));
                    console.log(`${nid} eklendi.`);
                    break;

                case "2":
                    if(envanter.length === 0) console.log("Boş.");
                    envanter.forEach(n => console.log(n.durumBilgisi()));
                    break;

                case "3":
                    const aid = await sor("Analiz ID: ");
                    const anesne = envanter.find(n => n.id === aid);
                    if (anesne) anesne.analizEt();
                    else console.log("Bulunamadı.");
                    break;

                case "4":
                    const sid = await sor("Soğutma ID: ");
                    const snesne = envanter.find(n => n.id === sid);
                    if (snesne) {
                        // Type Checking: instanceof ile miras kontrolü
                        if (snesne instanceof KritikNesne) {
                            snesne.acilDurumSogutmasi();
                        } else {
                            console.log("HATA: Bu nesne soğutulamaz!");
                        }
                    } else console.log("Bulunamadı.");
                    break;

                case "5":
                    oyunDevam = false;
                    rl.close();
                    break;
                default:
                    console.log("Geçersiz.");
            }
        } catch (e) {
            if (e.name === "KuantumCokusuException") {
                console.log("\n!!! SİSTEM ÇÖKTÜ! TAHLİYE BAŞLATILIYOR... !!!");
                console.log(`SEBEP: ${e.message}`);
                oyunDevam = false;
                rl.close();
            } else {
                console.log("Hata:", e);
            }
        }
    }
}

main();