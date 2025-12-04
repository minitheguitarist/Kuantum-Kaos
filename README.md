# Kuantum-Kaos
##Semih Murat ARSLAN —- 16008124054

Bu projede "Kuantum Kaos Yönetimi" ödevini dört farklı programlama
dili (C#, Java, Python, JS) kullanarak hazırladım. İlk olarak, tüm
nesneler için KuantumNesnesi adında ortak bir soyut sınıf (Abstract
Class) oluşturdum. Nesnelerin özelliklerini korumak için "Kapsülleme"
(Encapsulation) yöntemini kullandım. Bu sayede stabilite değeri her
zaman 0 ile 100 arasında kalıyor. Ayrıca, sadece tehlikeli olan
nesneler için IKritik adında bir arayüz (Interface) tasarladım. Soğutma
işlemini sadece bu arayüze sahip nesneler yapabiliyor. Programda
oluşabilecek hatalar için KuantumCokusuException isminde özel bir
hata yakalama sistemi yazdım. Eğer stabilite biterse program kontrollü
bir şekilde kapanıyor. Son olarak, "Polimorfizm" sayesinde farklı
türdeki nesneleri aynı liste içinde tuttum ve AnalizEt metoduyla hepsini
tek seferde çalıştırdım.
