from abc import ABC, abstractmethod
import random


# --- 1. ÖZEL HATA SINIFI ---
class KuantumCokusuException(Exception):
    def __init__(self, nesne_id):
        super().__init__(f"KRİTİK HATA! Nesne {nesne_id} yüzünden stabilite çöktü!")


# --- 2. ARAYÜZ (Interface) ---
# Python'da interface yoktur, Abstract Base Class (ABC) ile simüle edilir.
class IKritik(ABC):
    @abstractmethod
    def acil_durum_sogutmasi(self):
        pass


# --- 3. SOYUT ATA SINIF ---
class KuantumNesnesi(ABC):
    def __init__(self, nesne_id, tehlike):
        self.id = nesne_id
        self.tehlike_seviyesi = tehlike
        self._stabilite = 100.0  # Private değişken (_ ile başlar convention olarak)

    # Encapsulation için Property Decorator
    @property
    def stabilite(self):
        return self._stabilite

    @stabilite.setter
    def stabilite(self, value):
        if value > 100:
            self._stabilite = 100
        else:
            self._stabilite = value

        # Patlama Kontrolü
        if self._stabilite <= 0:
            raise KuantumCokusuException(self.id)

    @abstractmethod
    def analiz_et(self):
        pass

    def durum_bilgisi(self):
        return f"ID: {self.id} | Stabilite: %{self.stabilite:.2f} | Tehlike: {self.tehlike_seviyesi}"


# --- 4. SOMUT SINIFLAR ---

class VeriPaketi(KuantumNesnesi):
    def __init__(self, nesne_id):
        super().__init__(nesne_id, 1)

    def analiz_et(self):
        self.stabilite -= 5  # Setter tetiklenir
        print(f"[{self.id}] Veri içeriği okundu.")


class KaranlikMadde(KuantumNesnesi, IKritik):
    def __init__(self, nesne_id):
        super().__init__(nesne_id, 5)

    def analiz_et(self):
        self.stabilite -= 15
        print(f"[{self.id}] Karanlık madde analizi... Enerji dalgalanıyor!")

    def acil_durum_sogutmasi(self):
        self.stabilite += 50
        print(f"[{self.id}] Soğutma başarılı.")


class AntiMadde(KuantumNesnesi, IKritik):
    def __init__(self, nesne_id):
        super().__init__(nesne_id, 10)

    def analiz_et(self):
        self.stabilite -= 25
        print(f"[{self.id}] UYARI: Evrenin dokusu titriyor...")

    def acil_durum_sogutmasi(self):
        self.stabilite += 50
        print(f"[{self.id}] Anti Madde nötralize edildi.")


# --- 5. MAIN LOOP ---
def main():
    envanter = []
    oyun_devam = True

    print("--- OMEGA SEKTÖRÜ (PYTHON) ---")

    while oyun_devam:
        print("\n=== KONTROL PANELİ ===")
        print("1. Yeni Nesne Ekle")
        print("2. Tüm Envanteri Listele")
        print("3. Nesneyi Analiz Et")
        print("4. Acil Durum Soğutması Yap")
        print("5. Çıkış")

        secim = input("Seçiminiz: ")

        try:
            if secim == "1":
                sans = random.randint(1, 3)
                yeni_id = f"PY-NESNE-{random.randint(100, 999)}"
                if sans == 1:
                    envanter.append(VeriPaketi(yeni_id))
                elif sans == 2:
                    envanter.append(KaranlikMadde(yeni_id))
                else:
                    envanter.append(AntiMadde(yeni_id))
                print(f"{yeni_id} eklendi.")

            elif secim == "2":
                if not envanter: print("Ambar boş.")
                for nesne in envanter:
                    print(nesne.durum_bilgisi())

            elif secim == "3":
                a_id = input("Analiz ID: ")
                nesne = next((x for x in envanter if x.id == a_id), None)
                if nesne:
                    nesne.analiz_et()
                else:
                    print("Bulunamadı.")

            elif secim == "4":
                s_id = input("Soğutma ID: ")
                nesne = next((x for x in envanter if x.id == s_id), None)
                if nesne:
                    # Type Checking: isinstance
                    if isinstance(nesne, IKritik):
                        nesne.acil_durum_sogutmasi()
                    else:
                        print("HATA: Bu nesne soğutulamaz!")
                else:
                    print("Bulunamadı.")

            elif secim == "5":
                oyun_devam = False
                print("Çıkış yapılıyor.")
            else:
                print("Geçersiz işlem.")

        except KuantumCokusuException as ex:
            print("\n**************************************")
            print("SİSTEM ÇÖKTÜ! TAHLİYE BAŞLATILIYOR...")
            print(f"SEBEP: {ex}")
            print("**************************************")
            oyun_devam = False


if __name__ == "__main__":
    main()