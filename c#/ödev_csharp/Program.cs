using System;
using System.Collections.Generic;

namespace KuantumKaos
{
    //  1. ÖZEL HATA SINIFI (Custom Exception) 
    // Sistem çöktüğünde fırlatma
    public class KuantumCokusuException : Exception
    {
        public KuantumCokusuException(string nesneID) 
            : base($"KRİTİK HATA! Nesne {nesneID} yüzünden stabilite çöktü!") { }
    }

    // 2. ARAYÜZ (Interface)
    // Sadece tehlikeli (Karanlık Madde, Anti Madde) nesneler bu yeteneğe sahip olacak.
    public interface IKritik
    {
        void AcilDurumSogutmasi();
    }

    //3. SOYUT ATA SINIF (Abstract Class)
    public abstract class KuantumNesnesi
    {
        public string ID { get; set; }
        
        // Encapsulation (Kapsülleme) burada yapılıyor.
        private double _stabilite;
        public double Stabilite
        {
            get { return _stabilite; }
            set
            {
                // Değer 100'den büyükse 100'e sabitle.
                if (value > 100) _stabilite = 100;
                else _stabilite = value;

                // Eğer 0 veya altına düştüyse direkt patlatıyoruz
                if (_stabilite <= 0)
                {
                    throw new KuantumCokusuException(this.ID);
                }
            }
        }

        public int TehlikeSeviyesi { get; set; }

        public KuantumNesnesi(string id, int tehlike)
        {
            ID = id;
            TehlikeSeviyesi = tehlike;
            // İlk oluştuğunda stabilite full
            _stabilite = 100; 
        }

        // Alt sınıflar bunu zorunlu olarak dolduracak.
        public abstract void AnalizEt();

        public string DurumBilgisi()
        {
            return $"ID: {ID} | Stabilite: %{Stabilite:F2} | Tehlike: {TehlikeSeviyesi}";
        }
    }

    //4. SOMUT SINIFLAR (Concrete Classes)

    // 4.1 Veri Paketi (Zararsız eleman)
    public class VeriPaketi : KuantumNesnesi
    {
        public VeriPaketi(string id) : base(id, 1) { } // Tehlike seviyesi düşük

        public override void AnalizEt()
        {
            Stabilite -= 5;
            Console.WriteLine($"[{ID}] Veri içeriği okundu. Her şey yolunda.");
        }
    }

    // 4.2 Karanlık Madde (Tehlikeli - IKritik uygular)
    public class KaranlikMadde : KuantumNesnesi, IKritik
    {
        public KaranlikMadde(string id) : base(id, 5) { }

        public override void AnalizEt()
        {
            Stabilite -= 15;
            Console.WriteLine($"[{ID}] Karanlık madde analizi... Enerji dalgalanıyor!");
        }

        public void AcilDurumSogutmasi()
        {
            Stabilite += 50; // Property içindeki set bloğu 100'ü geçmesini engelleyecek.
            Console.WriteLine($"[{ID}] Soğutma başarılı. Stabilite yenilendi.");
        }
    }

    // 4.3 Anti Madde (IKritik uygular)
    public class AntiMadde : KuantumNesnesi, IKritik
    {
        public AntiMadde(string id) : base(id, 10) { }

        public override void AnalizEt()
        {
            Stabilite -= 25;
            Console.WriteLine($"[{ID}] UYARI: Evrenin dokusu titriyor...");
        }

        public void AcilDurumSogutmasi()
        {
            Stabilite += 50;
            Console.WriteLine($"[{ID}] Anti Madde nötralize edildi. Derin nefes al.");
        }
    }

    //5. OYNANIŞ DÖNGÜSÜ (Main Program)
    class Program
    {
        static void Main(string[] args)
        {
            List<KuantumNesnesi> envanter = new List<KuantumNesnesi>();
            Random rnd = new Random();
            bool oyunDevam = true;

            Console.WriteLine("--- OMEGA SEKTÖRÜ GÜVENLİK TERMİNALİ ---");

            while (oyunDevam)
            {
                Console.WriteLine("\n=== KUANTUM AMBARI KONTROL PANELİ ===");
                Console.WriteLine("1. Yeni Nesne Ekle");
                Console.WriteLine("2. Tüm Envanteri Listele");
                Console.WriteLine("3. Nesneyi Analiz Et");
                Console.WriteLine("4. Acil Durum Soğutması Yap");
                Console.WriteLine("5. Çıkış");
                Console.Write("Seçiminiz: ");
                
                string secim = Console.ReadLine();

                try
                {
                    switch (secim)
                    {
                        case "1":
                            // Rastgele bir nesne üretelim
                            int sans = rnd.Next(1, 4); // 1, 2 veya 3
                            string yeniId = "NESNE-" + rnd.Next(100, 999);
                            
                            if (sans == 1) envanter.Add(new VeriPaketi(yeniId));
                            else if (sans == 2) envanter.Add(new KaranlikMadde(yeniId));
                            else envanter.Add(new AntiMadde(yeniId));
                            
                            Console.WriteLine($"{yeniId} ambara kabul edildi.");
                            break;

                        case "2":
                            Console.WriteLine("\n--- ENVANTER DURUMU ---");
                            if (envanter.Count == 0) Console.WriteLine("Ambar boş.");
                            foreach (var nesne in envanter)
                            {
                                Console.WriteLine(nesne.DurumBilgisi());
                            }
                            break;

                        case "3":
                            Console.Write("Analiz edilecek ID: ");
                            string analizId = Console.ReadLine();
                            var analizNesnesi = envanter.Find(x => x.ID == analizId);

                            if (analizNesnesi != null)
                            {
                                //Burada Polimorfizm konuşuyor. Hangi nesneyse onun AnalizEt'i çalışır.
                                //AnalizEt() içinde stabilite düşerse exception fırlayabilir!
                                analizNesnesi.AnalizEt();
                                Console.WriteLine($"Güncel Stabilite: {analizNesnesi.Stabilite}");
                            }
                            else
                            {
                                Console.WriteLine("Hata: Böyle bir ID bulunamadı!");
                            }
                            break;

                        case "4":
                            Console.Write("Soğutulacak ID: ");
                            string sogutmaId = Console.ReadLine();
                            var sogutulacakNesne = envanter.Find(x => x.ID == sogutmaId);

                            if (sogutulacakNesne != null)
                            {
                                // Type Checking: Nesne IKritik arayüzüne sahip mi?
                                if (sogutulacakNesne is IKritik kritikNesne)
                                {
                                    kritikNesne.AcilDurumSogutmasi();
                                }
                                else
                                {
                                    Console.WriteLine("HATA: Bu nesne soğutulamaz! (Sıradan Veri Paketi)");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Nesne bulunamadı.");
                            }
                            break;

                        case "5":
                            oyunDevam = false;
                            Console.WriteLine("Vardiya sona erdi. Görüşmek üzere şef.");
                            break;

                        default:
                            Console.WriteLine("Geçersiz işlem, tekrar dene.");
                            break;
                    }
                }
                catch (KuantumCokusuException ex)
                {
                    // GAME OVER SENARYOSU
                    Console.WriteLine("\n**************************************");
                    Console.WriteLine("SİSTEM ÇÖKTÜ! TAHLİYE BAŞLATILIYOR...");
                    Console.WriteLine($"SEBEP: {ex.Message}");
                    Console.WriteLine("**************************************");
                    oyunDevam = false; // Döngüyü kırar ve programı bitirir.
                }
                catch (Exception genEx)
                {
                    // Beklenmeyen başka hatalar için (örn: boş değer girilmesi vs.)
                    Console.WriteLine($"Bilinmeyen bir hata oluştu: {genEx.Message}");
                }
            }
        }
    }
}