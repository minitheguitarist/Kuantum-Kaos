import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.Scanner;

// --- 1. ÖZEL HATA SINIFI ---
class KuantumCokusuException extends Exception {
    public KuantumCokusuException(String nesneID) {
        super("KRİTİK HATA! Nesne " + nesneID + " yüzünden stabilite çöktü!");
    }
}

// --- 2. ARAYÜZ (Interface) ---
interface IKritik {
    void acilDurumSogutmasi();
}

// --- 3. SOYUT ATA SINIF ---
abstract class KuantumNesnesi {
    protected String id;
    private double stabilite; // Encapsulation için private
    protected int tehlikeSeviyesi;

    public KuantumNesnesi(String id, int tehlikeSeviyesi) {
        this.id = id;
        this.tehlikeSeviyesi = tehlikeSeviyesi;
        this.setStabilite(100); // Başlangıç full
    }

    public String getId() { return id; }

    public double getStabilite() { return stabilite; }

    // Java'da Property yerine Getter/Setter metodları kullanılır
    public void setStabilite(double value) {
        if (value > 100) this.stabilite = 100;
        else this.stabilite = value;

        if (this.stabilite <= 0) {
             throw new RuntimeException(new KuantumCokusuException(this.id));
        }
    }

    public abstract void analizEt();

    public String durumBilgisi() {
        return "ID: " + id + " | Stabilite: %" + String.format("%.2f", stabilite) + " | Tehlike: " + tehlikeSeviyesi;
    }
}

// --- 4. SOMUT SINIFLAR ---

class VeriPaketi extends KuantumNesnesi {
    public VeriPaketi(String id) { super(id, 1); }

    @Override
    public void analizEt() {
        // Getter ve Setter kullanarak işlemi yapıyoruz
        setStabilite(getStabilite() - 5);
        System.out.println("[" + id + "] Veri içeriği okundu.");
    }
}

class KaranlikMadde extends KuantumNesnesi implements IKritik {
    public KaranlikMadde(String id) { super(id, 5); }

    @Override
    public void analizEt() {
        setStabilite(getStabilite() - 15);
        System.out.println("[" + id + "] Karanlık madde analizi... Enerji dalgalanıyor!");
    }

    @Override
    public void acilDurumSogutmasi() {
        setStabilite(getStabilite() + 50);
        System.out.println("[" + id + "] Soğutma başarılı.");
    }
}

class AntiMadde extends KuantumNesnesi implements IKritik {
    public AntiMadde(String id) { super(id, 10); }

    @Override
    public void analizEt() {
        setStabilite(getStabilite() - 25);
        System.out.println("[" + id + "] UYARI: Evrenin dokusu titriyor...");
    }

    @Override
    public void acilDurumSogutmasi() {
        setStabilite(getStabilite() + 50);
        System.out.println("[" + id + "] Anti Madde nötralize edildi.");
    }
}

// --- 5. ANA SINIF (MAIN) ---
public class Main {
    public static void main(String[] args) {
        List<KuantumNesnesi> envanter = new ArrayList<>();
        Scanner scanner = new Scanner(System.in);
        Random rnd = new Random();
        boolean oyunDevam = true;

        System.out.println("--- OMEGA SEKTÖRÜ (JAVA TERMİNALİ) ---");

        while (oyunDevam) {
            System.out.println("\n=== KONTROL PANELİ ===");
            System.out.println("1. Yeni Nesne Ekle");
            System.out.println("2. Tüm Envanteri Listele");
            System.out.println("3. Nesneyi Analiz Et");
            System.out.println("4. Acil Durum Soğutması Yap");
            System.out.println("5. Çıkış");
            System.out.print("Seçiminiz: ");

            String secim = scanner.nextLine();

            try {
                switch (secim) {
                    case "1":
                        int sans = rnd.nextInt(3); // 0, 1, 2
                        String yeniId = "J-NESNE-" + rnd.nextInt(900) + 100;
                        if (sans == 0) envanter.add(new VeriPaketi(yeniId));
                        else if (sans == 1) envanter.add(new KaranlikMadde(yeniId));
                        else envanter.add(new AntiMadde(yeniId));
                        System.out.println(yeniId + " eklendi.");
                        break;
                    case "2":
                        if(envanter.isEmpty()) System.out.println("Ambar boş.");
                        for (KuantumNesnesi k : envanter) {
                            System.out.println(k.durumBilgisi());
                        }
                        break;
                    case "3":
                        System.out.print("Analiz ID: ");
                        String aId = scanner.nextLine();
                        // Java'da Find işlemi stream ile yapılır
                        KuantumNesnesi aNesne = envanter.stream().filter(x -> x.getId().equals(aId)).findFirst().orElse(null);
                        if (aNesne != null) aNesne.analizEt();
                        else System.out.println("Bulunamadı.");
                        break;
                    case "4":
                        System.out.print("Soğutma ID: ");
                        String sId = scanner.nextLine();
                        KuantumNesnesi sNesne = envanter.stream().filter(x -> x.getId().equals(sId)).findFirst().orElse(null);

                        if (sNesne != null) {
                            // Type Checking: instanceof kullanılır
                            if (sNesne instanceof IKritik) {
                                ((IKritik) sNesne).acilDurumSogutmasi();
                            } else {
                                System.out.println("HATA: Bu nesne soğutulamaz!");
                            }
                        } else System.out.println("Bulunamadı.");
                        break;
                    case "5":
                        oyunDevam = false;
                        break;
                }
            } catch (RuntimeException e) {
                // Kendi hatamızı RuntimeException içine sarmıştık, onu açıp bakıyoruz
                if (e.getCause() instanceof KuantumCokusuException) {
                    System.out.println("\n!!! SİSTEM ÇÖKTÜ! TAHLİYE BAŞLATILIYOR... !!!");
                    System.out.println("SEBEP: " + e.getCause().getMessage());
                    oyunDevam = false;
                } else {
                    System.out.println("Bilinmeyen hata: " + e.getMessage());
                }
            }
        }
    }
}