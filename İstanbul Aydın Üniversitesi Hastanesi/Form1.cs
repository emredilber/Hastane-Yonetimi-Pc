using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace İstanbul_Aydın_Üniversitesi_Hastanesi
{
    public partial class Form1 : Form
    {        
        private FirestoreDb db;
        #region TÜRKÇE KARAKTERLERE UYGUN SIRALAMA İSME GÖRE YÖNETİCİ EKRANI
        private int CompareTurkish1((string doktortc, string ad, string soyad, string dogumTarihi, string cinsiyet, string poliklinik, string gsm, string dogumYeri) a,
                           (string doktortc, string ad, string soyad, string dogumTarihi, string cinsiyet, string poliklinik, string gsm, string dogumYeri) b)
        {
            return string.Compare(a.ad, b.ad, StringComparison.CurrentCulture);
        }
        #endregion

        #region TÜRKÇE KARAKTERLERE UYGUN SIRALAMA
        private int CompareTurkish(string a, string b)
        {
            var culture = new System.Globalization.CultureInfo("tr-TR");
            return string.Compare(a, b, true, culture);
        }
        #endregion
        
        public Form1()
        {
            InitializeComponent();
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "firebase.json");
            db = FirestoreDb.Create("hastane-3a893");
        }
        string girisDurum = "-1";

        #region VERİ TABANINA HAZIR YÜKLENEN VERİLER
        private void poliklinikler()
        {
            (string polAd, int polId)[] poliklinikData =
            {
                ("Alerji ve Göğüs Hastalıkları", 1),
                ("Beyin ve Sinir Cerrahisi", 2),
                ("Çocuk Alerji Hastalıkları", 3),
                ("Çocuk Cerrahisi", 4),
                ("Çocuk Endokrinolojisi", 5),
                ("Çocuk Enfeksiyon Hastalıkları", 6),
                ("Çocuk Gastroenteroloji", 7),
                ("Çocuk Göğüs Hastalıkları ve Alerji", 8),
                ("Çocuk İmmunolojisi", 9),
                ("Çocuk Kardiyolojisi", 10),
                ("Çocuk Metabolizma Hastalıkları", 11),
                ("Çocuk Nefrolojisi", 12),
                ("Çocuk Nörolojisi", 13),
                ("Çocuk Onkolojisi", 14),
                ("Çocuk Romatolojisi", 15),
                ("Neonatoloji", 16),
                ("Çocuk Sağlığı ve Hastalıkları", 17),
                ("Çocuk Ürolojisi", 18),
                ("Çocuk ve Ergen Ruh Sağlığı Hastalıkları", 19),
                ("Deri ve Zührevi Hastalıkları", 20),
                ("Endokrinoloji ve Metabolizma", 21),
                ("Enfeksiyon Hastalıkları ve Klinik Mikrobiyoloji", 22),
                ("Fiziksel Tıp ve Rehabilitasyon", 23),
                ("Gastroenteroloji", 24),
                ("Genel Cerrahi", 25),
                ("Göğüs Cerrahisi", 26),
                ("Göğüs Hastalıkları", 27),
                ("Göz Hastalıkları", 28),
                ("İç Hastalıkları", 29),
                ("Kadın Hastalıkları ve Doğum", 30),
                ("Kalp ve Damar Cerrahisi", 31),
                ("Kardiyoloji", 32),
                ("Plastik ve Estetik Cerrahi", 33),
                ("Romatoloji", 34),
                ("Ruh Sağlığı ve Hastalıkları", 35),
                ("Üroloji", 36)

            };

            try
            {
                CollectionReference docRef = db.Collection("poliklinikler");
                foreach (var (polAd, polId) in poliklinikData)
                {
                    var poliklinik = new
                    {
                        poliklinikId = polId,
                        poliklinikAdı = polAd,
                    };
                    docRef.Document(polId.ToString()).SetAsync(poliklinik).Wait();
                }
                MessageBox.Show("Veri başarıyla Firestore'a kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Firestore'a veri eklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void sehirleriYukle()
        {
            (string cityName, int cityId)[] citiesData =
            {
                ("Adana", 1),
                ("Adıyaman", 2),
                ("Afyonkarahisar", 3),
                ("Ağrı", 4),
                ("Amasya", 5),
                ("Ankara", 6),
                ("Antalya", 7),
                ("Artvin", 8),
                ("Aydın", 9),
                ("Balıkesir", 10),
                ("Bilecik", 11),
                ("Bingöl", 12),
                ("Bitlis", 13),
                ("Bolu", 14),
                ("Burdur", 15),
                ("Bursa", 16),
                ("Çanakkale", 17),
                ("Çankırı", 18),
                ("Çorum", 19),
                ("Denizli", 20),
                ("Diyarbakır", 21),
                ("Edirne", 22),
                ("Elazığ", 23),
                ("Erzincan", 24),
                ("Erzurum", 25),
                ("Eskişehir", 26),
                ("Gaziantep", 27),
                ("Giresun", 28),
                ("Gümüşhane", 29),
                ("Hakkari", 30),
                ("Hatay", 31),
                ("Isparta", 32),
                ("Mersin", 33),
                ("İstanbul", 34),
                ("İzmir", 35),
                ("Kars", 36),
                ("Kastamonu", 37),
                ("Kayseri", 38),
                ("Kırklareli", 39),
                ("Kırşehir", 40),
                ("Kocaeli", 41),
                ("Konya", 42),
                ("Kütahya", 43),
                ("Malatya", 44),
                ("Manisa", 45),
                ("Kahramanmaraş", 46),
                ("Mardin", 47),
                ("Muğla", 48),
                ("Muş", 49),
                ("Nevşehir", 50),
                ("Niğde", 51),
                ("Ordu", 52),
                ("Rize", 53),
                ("Sakarya", 54),
                ("Samsun", 55),
                ("Siirt", 56),
                ("Sinop", 57),
                ("Sivas", 58),
                ("Tekirdağ", 59),
                ("Tokat", 60),
                ("Trabzon", 61),
                ("Tunceli", 62),
                ("Şanlıurfa", 63),
                ("Uşak", 64),
                ("Van", 65),
                ("Yozgat", 66),
                ("Zonguldak", 67),
                ("Aksaray", 68),
                ("Bayburt", 69),
                ("Karaman", 70),
                ("Kırıkkale", 71),
                ("Batman", 72),
                ("Şırnak", 73),
                ("Bartın", 74),
                ("Ardahan", 75),
                ("Iğdır", 76),
                ("Yalova", 77),
                ("Karabük", 78),
                ("Kilis", 79),
                ("Osmaniye", 80),
                ("Düzce", 81)
            };

            try
            {
                CollectionReference docRef = db.Collection("sehirler");
                foreach (var (cityName, cityId) in citiesData)
                {
                    var sehir = new
                    {
                        SehirId = cityId,
                        SehirAdi = cityName,
                    };
                    docRef.Document(cityId.ToString()).SetAsync(sehir).Wait();
                }
                MessageBox.Show("Veri başarıyla Firestore'a kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Firestore'a veri eklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void doktorlar()
        {
            (string adi, string soyadi, string poliklinikId, string tc, string cinsiyeti, string sehiri, string dogumtarihi, string gsm, string sifre)[] doktorData =
            {
                ("Ahmet", "Yılmaz", "1", "12345678901", "Erkek", "34", "01.01.1990", "05550000001", "1"),
                ("Fatma", "Demir", "2", "23456789012", "Kadın", "34", "15.03.1988", "05550000002", "1"),
                ("Zeynep", "Kaya", "3", "34567890123", "Kadın", "34", "20.07.1992", "05550000003", "1"),
                ("Ali", "Çelik", "4", "45678901234", "Erkek", "34", "10.05.1985", "05550000004", "1"),
                ("Hüseyin", "Arslan", "5", "56789012345", "Erkek", "34", "05.11.1989", "05550000005", "1"),
                ("İsmail", "Kocaman", "6", "67890123456", "Erkek", "34", "28.09.1993", "05550000006", "1"),
                ("Osman", "Şahin", "7", "78901234567", "Erkek", "34", "02.03.1991", "05550000007", "1"),
                ("Yusuf", "Aydın", "8", "89012345678", "Erkek", "34", "12.12.1987", "05550000008", "1"),
                ("Mehmet", "Yıldız", "9", "90123456789", "Erkek", "16", "14.06.1987", "05550000009", "1"),
                ("Mustafa", "Korkmaz", "10", "01234567890", "Erkek", "16", "23.09.1992", "05550000010", "1"),
                ("Ahmet", "Taşkın", "11", "10234567891", "Erkek", "16", "30.04.1985", "05550000011", "1"),
                ("Ömer", "Serbest", "12", "21234567890", "Erkek", "16", "18.11.1990", "05550000012", "1"),
                ("Ali", "Bozkurt", "13", "32345678901", "Erkek", "16", "03.07.1988", "05550000013", "1"),
                ("Mehmet", "Kılıç", "14", "43456789012", "Erkek", "16", "11.02.1994", "05550000014", "1"),
                ("Fatih", "Aksoy", "15", "54567890123", "Erkek", "16", "27.08.1986", "05550000015", "1"),
                ("İbrahim", "Güneş", "16", "65678901234", "Erkek", "16", "09.05.1991", "05550000016", "1"),
                ("Mustafa", "Öztürk", "17", "76789012345", "Erkek", "16", "22.10.1989", "05550000017", "1"),
                ("Ahmet", "Toprak", "18", "87890123456", "Erkek", "16", "07.03.1993", "05550000018", "1"),
                ("Veli", "Yılmaz", "19", "98901234567", "Kadın", "34", "01.01.1990", "05550000019", "1"),
                ("Ayşe", "Demir", "20", "11112223344", "Kadın", "34", "15.03.1988", "05550000020", "1"),
                ("Zehra", "Kaya", "21", "22223334455", "Kadın", "34", "20.07.1992", "05550000021", "1"),
                ("Emine", "Çelik", "22", "33334445566", "Kadın", "34", "10.05.1985", "05550000022", "1"),
                ("Hatice", "Arslan", "23", "44445556677", "Kadın", "34", "05.11.1989", "05550000023", "1"),
                ("Hülya", "Kocaman", "24", "55556667788", "Kadın", "34", "28.09.1993", "05550000024", "1"),
                ("Esra", "Şahin", "25", "66667778899", "Kadın", "34", "02.03.1991", "05550000025", "1"),
                ("Sevim", "Aydın", "26", "77778889900", "Kadın", "34", "12.12.1987", "05550000026", "1"),
                ("Sevgi", "Yıldız", "27", "88889990011", "Kadın", "16", "14.06.1987", "05550000027", "1"),
                ("Melek", "Korkmaz", "28", "99990001122", "Kadın", "16", "23.09.1992", "05550000028", "1"),
                ("Pınar", "Taşkın", "29", "00011222333", "Kadın", "16", "30.04.1985", "05550000029", "1"),
                ("Gamze", "Serbest", "30", "11122333444", "Kadın", "16", "18.11.1990", "05550000030", "1"),
                ("Sultan", "Bozkurt", "31", "22233444555", "Kadın", "16", "03.07.1988", "05550000031", "1"),
                ("Derya", "Kılıç", "32", "33344555666", "Kadın", "16", "11.02.1994", "05550000032", "1"),
                ("Serap", "Aksoy", "33", "44455666777", "Kadın", "16", "27.08.1986", "05550000033", "1"),
                ("Sultan", "Güneş", "34", "55566777888", "Kadın", "16", "09.05.1991", "05550000034", "1"),
                ("Şerife", "Öztürk", "35", "66677888999", "Kadın", "16", "22.10.1989", "05550000035", "1"),
                ("Hacer", "Toprak", "36", "77788999000", "Kadın", "16", "07.03.1993", "05550000036", "1"),
            };

            try
            {
                CollectionReference docRef = db.Collection("doktorlar");
                foreach (var (adi,soyadi,poliklinikId,tc,cinsiyeti,sehiri,dogumtarihi,gsm,sifre ) in doktorData)
                {
                    var doktor = new
                    {
                        tc = tc,
                        ad = adi,
                        soyad = soyadi,
                        cinsiyet = cinsiyeti,
                        dogumyeri = sehiri,
                        dogumtarihi = "" + dogumtarihi + "",
                        poliklinik = poliklinikId,
                        gsm = gsm,
                        şifre = sifre
                    };
                    docRef.Document(tc.ToString()).SetAsync(doktor).Wait();
                }
                MessageBox.Show("Veri başarıyla Firestore'a kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Firestore'a veri eklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void Sehirler()
        {
            try
            {
                // Şehir adlarını saklamak için bir liste oluşturuluyor.
                List<string> SehirAdlari = new List<string>();

                // Veri tabanından "sehirler" koleksiyonundaki tüm belgeler alınıyor ve SehirAdi alanı adına göre sıralanıyor.
                QuerySnapshot sehirSnapshot = db.Collection("sehirler").OrderBy("SehirAdi").GetSnapshotAsync().Result;

                // Sorgu sonucunda oluşan tüm verileri döngü ile listeye ekliyoruz.
                foreach (DocumentSnapshot documentSnapshot in sehirSnapshot.Documents)
                    SehirAdlari.Add(documentSnapshot.GetValue<string>("SehirAdi"));

                SehirAdlari.Sort(CompareTurkish); // Şehir adları Türkçe karakterlere göre sıralanıyor.

                // Döngü ile şehirlerin adları combobox'lara ekleniyor.
                foreach (string cityName in SehirAdlari)
                {
                    guna2ComboBox2.Items.Add(cityName);
                    guna2ComboBox4.Items.Add(cityName);
                    guna2ComboBox10.Items.Add(cityName);
                    guna2ComboBox14.Items.Add(cityName);
                }

                // İlk şehir adı seçili olarak ayarlanıyor.
                guna2ComboBox2.StartIndex = 0;
                guna2ComboBox4.StartIndex = 0;
                guna2ComboBox10.StartIndex = 0;
                guna2ComboBox14.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                // Verileri alırken bir hata oluşursa kullanıcıya hatayı bildiriliyor.
                MessageBox.Show("Veri alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Poliklinikler()
        {
            // Poliklinik adlarını saklamak için bir liste oluşturuluyor.
            List<string> PoliklinikAdlari = new List<string>();

            try
            {
                // Veri tabanındna "poliklinikler" koleksiyonundaki tüm belgeler alınıyor ve poliklinikAdı alanı adına göre sıralanıyor.
                QuerySnapshot sorgu = db.Collection("poliklinikler").OrderBy("poliklinikAdı").GetSnapshotAsync().Result;

                // Sorgu sonucunda oluşan tüm verileri döngü ile listeye ekliyoruz.
                foreach (DocumentSnapshot documentSnapshot in sorgu.Documents)
                    PoliklinikAdlari.Add(documentSnapshot.GetValue<string>("poliklinikAdı"));

                PoliklinikAdlari.Sort(CompareTurkish); // Poliklinik adları Türkçe karakterlere göre sıralanıyor.

                // Döngü ile poliklinik adları combobox'lara ekleniyor.
                foreach (string poliklinikAdi in PoliklinikAdlari)
                {
                    guna2ComboBox6.Items.Add(poliklinikAdi);
                    guna2ComboBox7.Items.Add(poliklinikAdi);
                    guna2ComboBox11.Items.Add(poliklinikAdi);
                    guna2ComboBox12.Items.Add(poliklinikAdi);
                }

                // İlk poliklinik adı seçili olarak ayarlanıyor.
                guna2ComboBox6.StartIndex = 0;
                guna2ComboBox7.StartIndex = 0;
                guna2ComboBox11.StartIndex = 0;
                guna2ComboBox12.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                // Verileri alırken bir hata oluşursa kullanıcıya hatayı bildiriliyor.
                MessageBox.Show("Veri alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Form yüklendiğinde çalışacak kodlar.
        private void Form1_Load(object sender, EventArgs e)
        {
            // Başlangıç sekmesini ayarlanıyor.
            bunifuPages1.SelectedTab = tabPage8;

            // Şehirler ve poliklinikler metodlarını çağırarak combobox'lar dolduruluyor.
            Sehirler();
            Poliklinikler();
        }

        #region GİRİŞ EKRANI BİLEŞENLERİ

        // GİRİŞ EKRANI GİRİŞ BUTONU
        private void guna2Button37_Click(object sender, EventArgs e)
        {
            // Butona basıldıktan sonra giriş biçimi doğrulaması yaptırma.
            if (guna2TextBox1.Text.Length < 11 && !Guna2RadioButton3.Checked)
                errorProvider1.SetError(guna2TextBox1, "TC Kimlik Numarası 11 haneli olarak giriniz.");
            if (guna2TextBox2.Text.Length < 1)
            {
                errorProvider1.SetError(guna2TextBox2, "Şifre boş bırakılamaz.");
                return; // Şifre boş ise yürütmeyi durduruyor.
            }

            string tc = guna2TextBox1.Text; // TC yazılan textBox'u değişkene atama.
            string parola = guna2TextBox2.Text; // Şifre yazılan textBox'u değişkene atama.
            if (Guna2RadioButton1.Checked) // RadioButton1(Hasta) seçilimi diye kontrol ediliyor.
            {
                try // Hasta seçili ise try ile hatalı durum tespiti yapılıyor.
                {
                    // Veri tabanından kolleksiyon ve döküman referansı alınıyor.
                    DocumentReference dokuman = db.Collection("hastalar").Document(tc);
                    DocumentSnapshot sorgu = dokuman.GetSnapshotAsync().Result; // Sonucu değişkene aktarılıyor.

                    if (sorgu.Exists) // Veri bulunursa...
                    {
                        string sifresi = sorgu.GetValue<string>("şifre"); // TC numarası veri tabanında eşleşen kişinin şifresi alınıyor.

                        if (sifresi == parola) // Eğer veri tabanında ki şifre kullanıcının girdiği şifre ile eşleşiyorsa...
                        {
                            girisDurum = tc; // İlerleyen kodlarda kullanılmak üzere giriş yapan kişinin tc si değişkende tutuluyor.
                            guna2TextBox2.Text = ""; // Şifre textBoxu sıfırlanıyor.

                            try
                            {
                                // Hastanın aldığı randevuları listelemek üzere veri tabanından sorgu çekiliyor.
                                QuerySnapshot querySnapshot = db.Collection("randevular").OrderBy("id")
                                    .WhereEqualTo("hastaTC", girisDurum).GetSnapshotAsync().Result;

                                // Randevuların bulunduğu tablo sonraki girişler ile birleşmesin diye temizleniyor.
                                guna2DataGridView1.Rows.Clear(); 
                                
                                // Sorgudan çekilen veriler döngüye sokuluyor.
                                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents) 
                                {
                                    // Gerekli bilgiler değişkenlere aktarılıyor.
                                    string id = documentSnapshot.Id;
                                    string doktorAd = documentSnapshot.GetValue<string>("doktorAdi");
                                    string randevuTarihi = documentSnapshot.GetValue<string>("randevuTarihi");
                                    string randevuDurumu = documentSnapshot.GetValue<string>("randevuDurumu");
                                    string randevuSaati = documentSnapshot.GetValue<string>("randevuSaati");
                                    string poliklinikAdı = documentSnapshot.GetValue<string>("PoliklinikAdi");
                                    
                                    //DataGrid(Tablo) kişinin randevuları ve bilgileri ile dolduruluyor
                                    guna2DataGridView1.Rows.Add(id, doktorAd, poliklinikAdı, randevuTarihi, randevuSaati, randevuDurumu); 
                                }
                                bunifuPages1.SelectedTab = tabPage4; // Hasta randevular sekmesine aktarılıyor.
                            }
                            catch (Exception ex) // Verileri alırken bir hata oluşursa kullanıcıya hatayı bildiriliyor.
                            {
                                MessageBox.Show("Veri alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else // Şifre yanlış girilirse çalışacak kodlar.
                            MessageBox.Show("Geçersiz TC veya parola.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else // Veri bulunmazsa çalışacak kodlar.
                        MessageBox.Show("Geçersiz TC veya parola.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex) // Verileri alırken bir hata oluşursa kullanıcıya hatayı bildiriliyor.
                {
                    MessageBox.Show("Veri alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if(Guna2RadioButton2.Checked) // RadioButton2(Doktor) seçilimi diye kontrol ediliyor.
            {
                try
                {
                    DocumentReference dokuman = db.Collection("doktorlar").Document(tc); // Doktor seçili ise try ile hatalı durum tespiti yapılıyor.
                    DocumentSnapshot sorgu = dokuman.GetSnapshotAsync().Result; // Sonuç değişkene aktarılıyor.

                    if (sorgu.Exists) // Veri bulunursa...
                    {
                        string sifresi = sorgu.GetValue<string>("şifre"); // TC numarası veri tabanında eşleşen kişinin şifresi alınıyor.

                        if (sifresi == parola) // Eğer veri tabanında ki şifre kullanıcının girdiği şifre ile eşleşiyorsa...
                        {
                            // List<(string id, string hastaAd, string randevuTarihi, string randevuDurumu, string randevuSaati)> belgeBilgileri = new List<(string id, string hastaAd, string randevuTarihi, string randevuDurumu, string randevuSaati)>();

                            girisDurum = tc; // TC değişkende tutuluyor.
                            guna2TextBox2.Text = ""; // Textbox sıfırlanıyor.

                            try
                            {
                                // Doktorun randevuları listelenmek üzere veri tabanından sorgu çekiliyor.
                                QuerySnapshot querySnapshot = db.Collection("randevular").OrderBy("id")
                                    .WhereEqualTo("doktorTC", girisDurum).GetSnapshotAsync().Result;

                                guna2DataGridView2.Rows.Clear(); // DataGrid temizleniyor.
                                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents) // Sorgudan çekilen veriler döngüye sokuluyor.
                                {
                                    // Gerekli bilgiler değişkenlere aktarılıyor.
                                    string id = documentSnapshot.Id;
                                    string hastaTC = documentSnapshot.GetValue<string>("hastaTC");
                                    DocumentReference document = db.Collection("hastalar").Document(hastaTC);
                                    DocumentSnapshot sorgu1 = document.GetSnapshotAsync().Result;
                                    string hastaAd = sorgu1.GetValue<string>("ad") + " " + sorgu1.GetValue<string>("soyad");
                                    string randevuTarihi = documentSnapshot.GetValue<string>("randevuTarihi");
                                    string randevuDurumu = documentSnapshot.GetValue<string>("randevuDurumu");
                                    string randevuSaati = documentSnapshot.GetValue<string>("randevuSaati");
                                    // DataGrid veriler ile dolduruluyor.
                                    guna2DataGridView2.Rows.Add(id, hastaAd, randevuTarihi, randevuSaati, randevuDurumu); 
                                    
                                    //belgeBilgileri.Add((id, hastaAd, randevuTarihi, randevuDurumu, randevuSaati));
                                }
                                bunifuPages1.SelectedTab = tabPage3; // Doktorun randevuları görebildiği sayfaya aktarılıyor.
                                /* //Koleksiyonu sırala (id'ye göre) 
                                belgeBilgileri.Sort((x, y) => int.Parse(x.id).CompareTo(int.Parse(y.id)));

                                // Sıralanmış koleksiyonu kullanarak DataGridView'e ekleyin 
                                foreach (var belge in belgeBilgileri)
                                {
                                    guna2DataGridView2.Rows.Add(belge.id, belge.hastaAd, belge.randevuTarihi, belge.randevuSaati, belge.randevuDurumu);
                                }*/
                            }
                            catch (Exception ex) // Verileri alırken bir hata oluşursa kullanıcıya hatayı bildiriliyor.
                            {
                                MessageBox.Show("Veri alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else // Şifre yanlış girilirse çalışacak kodlar.
                            MessageBox.Show("Geçersiz TC veya parola.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else // Veri bulunmazsa çalışacak kodlar.
                        MessageBox.Show("Geçersiz TC veya parola.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex) // Verileri alırken bir hata oluşursa kullanıcıya hatayı bildiriliyor.
                {
                    MessageBox.Show("Veri alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if(Guna2RadioButton3.Checked) //RadioButon3(Yönetici) seçili ise çalışacak kodlar.
            {
                try
                {
                    DocumentReference dokuman = db.Collection("admins").Document(tc);
                    DocumentSnapshot sorgu = dokuman.GetSnapshotAsync().Result; // Sorgu sonucu değişkene aktarılıyor.

                    if (sorgu.Exists) // Veri bulunursa
                    {
                        string sifresi = sorgu.GetValue<string>("password"); // Şifre değişkene aktarılıyor.

                        if (sifresi == parola) // Şifreler eşleşirse...
                        {
                            girisDurum = tc; // TC değişkene aktarılıyor.
                            guna2TextBox2.Text = ""; // TextBox temizleniyor.

                            //List<(string doktortc, string ad, string soyad, string dogumTarihi, string cinsiyet, string poliklinik, string gsm, string dogumYeri, string sifre)> belgeBilgileri = new
                            //    List<(string doktortc, string ad, string soyad, string dogumTarihi, string cinsiyet, string poliklinik, string gsm, string dogumYeri, string sifre)>();

                            try
                            {
                                //belgeBilgileri.Clear();

                                // Doktorların çekilmesi için sorgu oluşturuulyor.
                                QuerySnapshot querySnapshot = db.Collection("doktorlar").OrderBy("tc").GetSnapshotAsync().Result; 
                                
                                guna2DataGridView3.Rows.Clear(); //DataGrid temizleniyor.

                                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                                {
                                    // Döngüye sokulan sorgudaki veriler değişkene aktarılıyor.
                                    string doktortc = documentSnapshot.GetValue<string>("tc");
                                    string ad = documentSnapshot.GetValue<string>("ad");
                                    string soyad = documentSnapshot.GetValue<string>("soyad");
                                    string dogumTarihi = documentSnapshot.GetValue<string>("dogumtarihi");
                                    string cinsiyet = documentSnapshot.GetValue<string>("cinsiyet");
                                    string polid = documentSnapshot.GetValue<string>("poliklinik");
                                    DocumentReference document = db.Collection("poliklinikler").Document(polid);
                                    DocumentSnapshot sorgu1 = document.GetSnapshotAsync().Result;
                                    string poliklinik = sorgu1.GetValue<string>("poliklinikAdı");
                                    string gsm = documentSnapshot.GetValue<string>("gsm");
                                    string dogumid = documentSnapshot.GetValue<string>("dogumyeri");
                                    DocumentReference document2= db.Collection("sehirler").Document(dogumid);
                                    DocumentSnapshot sorgu2 = document2.GetSnapshotAsync().Result;
                                    string dogumYeri = sorgu2.GetValue<string>("SehirAdi");
                                    string sifre = documentSnapshot.GetValue<string>("şifre");

                                    //Veriler DataGride aktarılıyor.
                                    guna2DataGridView3.Rows.Add(doktortc, ad, soyad, dogumTarihi, cinsiyet, poliklinik, gsm, dogumYeri,sifre);

                                    //belgeBilgileri.Add((doktortc, ad, soyad, dogumTarihi, cinsiyet, poliklinik, gsm, dogumYeri,sifre));
                                }
                                bunifuPages1.SelectedTab = tabPage13; // Doktorların listelendiği ekrana geçiliyor.
                                //foreach (var belge in belgeBilgileri)
                                //    guna2DataGridView3.Rows.Add(belge.doktortc, belge.ad, belge.soyad, belge.dogumTarihi, belge.cinsiyet, belge.poliklinik, belge.gsm, belge.dogumYeri, belge.sifre);
                            }
                            catch (Exception ex) // Verileri alırken bir hata oluşursa kullanıcıya hatayı bildiriliyor.
                            {
                                MessageBox.Show("Veri alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else // Şifreler eşleşmezse.
                            MessageBox.Show("Geçersiz TC veya parola.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else // Kullanıcı bulunamazsa.
                        MessageBox.Show("Geçersiz TC veya parola.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex) // Verileri alırken bir hata oluşursa kullanıcıya hatayı bildiriliyor.
                {
                    MessageBox.Show("Veri alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else // Hiçbir RadioButon seçilmezse...
            {
                // ErrorProvider ile RadioButonlara hata veriliyor.
                errorProvider1.SetError(Guna2RadioButton1, "Birini seç");
                errorProvider1.SetError(Guna2RadioButton2, "Birini seç");
                errorProvider1.SetError(Guna2RadioButton3, "Birini seç");
            }
        }

        // KAYIT OL BUTONU
        private void guna2Button38_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage9; // Kayıt olma ekranına yönlendiriliyor.
        }

        // ŞİFREMİ UNUTTUM BUTONU(LABEL)
        private void guna2HtmlLabel11_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage10; //Şifremi unuttum ekranına yönlendiriliyor.
        }
                
        #region GİRİŞ TC TEXTBOX
        private void guna2TextBox1_KeyPress(object sender, KeyPressEventArgs e) 
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) // Sadece rakam girilmesine izin veriliyor
                e.Handled = true;
        }
        private void guna2TextBox1_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox1, ""); // İçine tıklandığı zaman hatalar sildiriliyor.
        }

        private void guna2TextBox1_Leave(object sender, EventArgs e)
        {   // Başka bir yere odaklanınca yönetici seçili değilse ve 11 haneden az sayısı ise hata veriliyor
            if (guna2TextBox1.Text.Length < 11 && !Guna2RadioButton3.Checked) 
                errorProvider1.SetError(guna2TextBox1, "TC Kimlik Numarası 11 haneli olarak giriniz.");
        }
        #endregion
        
        #region GİRİŞ ŞİFRE TEXTBOX
        private void guna2TextBox2_IconRightClick(object sender, EventArgs e)
        {   // Her tıklandığında şifre gözükme durumu değiştiriliyor.
            guna2TextBox2.PasswordChar = guna2TextBox2.PasswordChar == '\0' ? '●' : '\0'; 
            if (guna2TextBox2.PasswordChar == '●') // Şifre gizli ise ona göre icon gösteriliyor.
                guna2TextBox2.IconRight= Properties.Resources.eyes;
            else
                guna2TextBox2.IconRight= Properties.Resources.eyes_off;
        }
        private void guna2TextBox2_Leave(object sender, EventArgs e)
        {   // Başka bir yere odaklanınca 1 haneden az sayısı ise hata veriliyor
            if (guna2TextBox2.Text.Length < 1) // 
                errorProvider1.SetError(guna2TextBox2, "Şifre boş bırakılamaz.");
        }

        private void guna2TextBox2_Enter(object sender, EventArgs e)
        {   // İçine tıklanınca hatalar siliniyor.
            errorProvider1.SetError(guna2TextBox2, "");
        }
        #endregion
        
        #region GİRİŞ RADİO BUTON SEÇİLİ Mİ TEYİT
        // RadioButon'lar esçilince hatalar kaldırılıyor
        private void Guna2RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(Guna2RadioButton1, "");
            errorProvider1.SetError(Guna2RadioButton2, "");
            errorProvider1.SetError(Guna2RadioButton3, "");
        }

        private void Guna2RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(Guna2RadioButton1, "");
            errorProvider1.SetError(Guna2RadioButton2, "");
            errorProvider1.SetError(Guna2RadioButton3, "");
        }

        private void Guna2RadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(Guna2RadioButton1, "");
            errorProvider1.SetError(Guna2RadioButton2, "");
            errorProvider1.SetError(Guna2RadioButton3, "");
            errorProvider1.SetError(guna2TextBox1, "");
        }
        #endregion

        #endregion

        #region ŞİFREMİ UNUTTUM EKRANI BİLEŞENLERİ
        private void guna2Button40_Click(object sender, EventArgs e)
        {
            if (guna2TextBox10.Text.Length < 11)
                errorProvider1.SetError(guna2TextBox10, "TC Kimlik Numarası 11 haneli olarak giriniz.");
            if (guna2TextBox11.Text.Length < 1)
                errorProvider1.SetError(guna2TextBox11, "Şifre boş bırakılamaz.");
            if (guna2TextBox12.Text.Length < 1)
                errorProvider1.SetError(guna2TextBox12, "Şifre boş bırakılamaz.");
            // Metin girişlerinde hata varsa yürütmeyi durduruyor.
            if (guna2TextBox10.Text.Length < 11||guna2TextBox11.Text.Length < 0||guna2TextBox12.Text.Length < 0) return;

            if (guna2TextBox11.Text == guna2TextBox12.Text) // Şifre TextBoxları birbirine eşitse...
            {
                try
                {
                    DocumentReference dokuman = db.Collection("hastalar").Document(guna2TextBox10.Text.ToString());
                    DocumentSnapshot sorgu = dokuman.GetSnapshotAsync().Result; // Veri tabanından girilen TC ye göre sorgu çekiliyor...
                    if (sorgu.Exists) // Sorguda veri varsa...
                    {
                        var kullanıcı = new { şifre = guna2TextBox12.Text };
                        // Diğer verleri değiştirmeden sadece gönderilen veriliyi değiştirmesi sağlanıyor.
                        dokuman.SetAsync(kullanıcı, SetOptions.MergeAll); 
                        MessageBox.Show("Şifre başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        guna2ComboBox11.Text = "";
                        guna2ComboBox12.Text = "";
                    }
                    else // Kullanıcı bulunamazsa...
                        MessageBox.Show("Kullanıcı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Şifre güncellenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else // Şifreler eşit değilse...
            {
                // ErrorProvider hatası veriliyor ve ekrana hata çıkartılıyor.
                errorProvider1.SetError(guna2TextBox11, "Şifreler eşit değil!");
                errorProvider1.SetError(guna2TextBox12, "Şifreler eşit değil!");
                MessageBox.Show("Şifreler eşit değil."); 
            }
        }
        
        #region ŞİFREMİ UNUTTUM TC TEXTBOX
        private void guna2TextBox10_KeyPress(object sender, KeyPressEventArgs e)
        {
        if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
        private void guna2TextBox10_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox10.Text.Length < 11)
                errorProvider1.SetError(guna2TextBox10, "TC Kimlik Numarası 11 haneli olarak giriniz.");
        }

        private void guna2TextBox10_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox10, "");
        }
        #endregion

        #region ŞİFREMİ UNUTTUM ŞİFRE TEXTBOX
        private void guna2TextBox11_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox11.PasswordChar = guna2TextBox11.PasswordChar == '\0' ? '●' : '\0';
            if (guna2TextBox11.PasswordChar == '●')
                guna2TextBox11.IconRight= Properties.Resources.eyes;
            else
                guna2TextBox11.IconRight= Properties.Resources.eyes_off;
        }

        private void guna2TextBox11_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox11.Text.Length < 0)
                errorProvider1.SetError(guna2TextBox11, "Şifre boş bırakılamaz.");
        }

        private void guna2TextBox11_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox11, "");
            errorProvider1.SetError(guna2TextBox12, "");
        }
        #endregion

        #region ŞİFREMİ UNUTTUM ŞİFRE TEKRAR TEXTBOX
        private void guna2TextBox12_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox12.PasswordChar = guna2TextBox12.PasswordChar == '\0' ? '●' : '\0';
            if (guna2TextBox12.PasswordChar == '●')
                guna2TextBox12.IconRight= Properties.Resources.eyes;
            else
                guna2TextBox12.IconRight= Properties.Resources.eyes_off;
        }

        private void guna2TextBox12_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox12.Text.Length < 0)
                errorProvider1.SetError(guna2TextBox12, "Şifre boş bırakılamaz.");
        }

        private void guna2TextBox12_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox11, "");
            errorProvider1.SetError(guna2TextBox12, "");
        }
        #endregion
        #endregion

        #region KAYIT OL EKRANI BİLEŞENLERİ

        // Kayıt olma ekranı Kayıt Ol butonnu
        private void guna2Button39_Click(object sender, EventArgs e)
        {
            if (guna2TextBox3.Text.Length < 11)
                errorProvider1.SetError(guna2TextBox3, "TC Kimlik Numarası 11 haneli olarak giriniz.");
            if (guna2TextBox4.Text.Length < 3)
                errorProvider1.SetError(guna2TextBox4, "Yanlış ad girişi.");
            if (guna2TextBox5.Text.Length < 3)
                errorProvider1.SetError(guna2TextBox5, "Yanlış soyad girişi.");
            if (!Regex.IsMatch(guna2TextBox6.Text, @"^0\d{10}$"))
                errorProvider1.SetError(guna2TextBox6, "Telefon numarası 0 ile başlamalıdır ve doğru girilmelidir.");
            if (!Regex.IsMatch(guna2TextBox7.Text, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
                errorProvider1.SetError(guna2TextBox7, "Geçerli bir e-posta adresi giriniz.");          
            if (guna2TextBox9.Text.Length < 1)
                errorProvider1.SetError(guna2TextBox9, "Şifre boş bırakılamaz.");
            // Hata kontrolü yapılıyor hata varsa program akışı durduruluyor hata yazdırılıyor.
            if (guna2TextBox3.Text.Length < 11 || guna2TextBox4.Text.Length < 3 || guna2TextBox5.Text.Length < 3 ||
                !Regex.IsMatch(guna2TextBox6.Text, @"^0\d{10}$") ||
                !Regex.IsMatch(guna2TextBox7.Text, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$") || guna2TextBox9.Text.Length < 1)
                return;

            try
            {
                DocumentReference docRef = db.Collection("hastalar").Document(guna2TextBox3.Text.ToString());
                // Seçilen şehir veri tabanında sorgu yapılıyor yapılan sorguya göre seçilen şehrin id'si alınıyor değişkene atanıyor.
                QuerySnapshot sehirid = db.Collection("sehirler").WhereEqualTo("SehirAdi", guna2ComboBox2.Text).GetSnapshotAsync().Result;

                if (!docRef.GetSnapshotAsync().Result.Exists) // Eğer kayıt olurken aynı TC ye sahip başka birisi yoksa...
                {
                    var user = new
                    {
                        tc = guna2TextBox3.Text,
                        ad = guna2TextBox4.Text,
                        soyad = guna2TextBox5.Text,
                        cinsiyet = guna2ComboBox1.Text,
                        dogumyeri = sehirid.Documents[0].Id, // Şehir adından dönüştürülen id yi yazdırılıyor.
                        dogumtarihi = bunifuDatePicker2.Value.ToString("dd.MM.yyyy"), // Gün ay yıl formatında veri kaydediliyor.
                        kangrubu = guna2ComboBox3.Text,
                        gsm = guna2TextBox6.Text,
                        email = guna2TextBox7.Text,
                        adres = guna2TextBox8.Text,
                        şifre = guna2TextBox9.Text
                    };
                    docRef.SetAsync(user).Wait(); // Kayıt işlemi gerçekleşiyor
                    MessageBox.Show("Kayıt işlemi başarıyla gerçekleşti", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Böyle bir kullanıcı var!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri eklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region KAYIT OL TC TEXTBOX
        private void guna2TextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void guna2TextBox3_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox3.Text.Length < 11)
                errorProvider1.SetError(guna2TextBox3, "TC Kimlik Numarası 11 haneli olarak giriniz.");
        }

        private void guna2TextBox3_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox3, "");
        }
        #endregion

        #region KAYIT OL AD TEXTBOX
        private void guna2TextBox4_Leave(object sender, EventArgs e)
        {
            // Ad girilen textBox'tan çıkıldığında uzunluğu 3 harften kısa ise hata veriliyor.
            if (guna2TextBox4.Text.Length < 3) 
                errorProvider1.SetError(guna2TextBox4, "Yanlış ad girişi.");
        }

        private void guna2TextBox4_Enter(object sender, EventArgs e)
        {   // Textbox'a geri odaklanıldığı zaman hata siliniliyor.
            errorProvider1.SetError(guna2TextBox4, "");
        }
        #endregion

        #region KAYIT OL SOYAD TEXTBOX
        private void guna2TextBox5_Leave(object sender, EventArgs e)
        {   
            // Soyad girilen textBox'tan çıkıldığında uzunluğu 3 harften kısa ise hata veriliyor.
            if (guna2TextBox5.Text.Length < 3)
                errorProvider1.SetError(guna2TextBox5, "Yanlış soyad girişi.");
        }

        private void guna2TextBox5_Enter(object sender, EventArgs e)
        {    // Textbox'a geri odaklanıldığı zaman hata siliniliyor.
            errorProvider1.SetError(guna2TextBox5, "");
        }
        #endregion

        #region KAYIT OL GSM TEXTBOX
        private void guna2TextBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece rakam girme izni veirliyor.
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void guna2TextBox6_Validating(object sender, CancelEventArgs e)
        {
            // Girilen rakam 05 ile başlayıp başlanmadığı kontrol ediliyor.
            if (!Regex.IsMatch(guna2TextBox6.Text, @"^0\d{10}$"))
                errorProvider1.SetError(guna2TextBox6, "Telefon numarası 0 ile başlamalıdır ve doğru girilmelidir.");
        }

        private void guna2TextBox6_Enter(object sender, EventArgs e)
        {   // TextBox içine girildiğinde hata siliniyor.
            errorProvider1.SetError(guna2TextBox6, "");
        }
        #endregion

        #region KAYIT OL EMAİL TEXTBOX
        private void guna2TextBox7_Validating(object sender, CancelEventArgs e)
        {
            // E-mail formatına uygun şekilde yazıldığı kontrol ediliyor.
            if (!Regex.IsMatch(guna2TextBox7.Text, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
                errorProvider1.SetError(guna2TextBox7, "Geçerli bir e-posta adresi giriniz.");
        }

        private void guna2TextBox7_Enter(object sender, EventArgs e)
        {   // TextBox içine girildiği zaman hata siliniyor.
            errorProvider1.SetError(guna2TextBox7, "");
        }
        #endregion

        #region KAYIT OL ŞİFRE TEXTBOX
        private void guna2TextBox9_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox9.PasswordChar = guna2TextBox9.PasswordChar == '\0' ? '●' : '\0';
            if (guna2TextBox9.PasswordChar == '●')
                guna2TextBox9.IconRight= Properties.Resources.eyes;
            else
                guna2TextBox9.IconRight= Properties.Resources.eyes_off;
        }

        private void guna2TextBox9_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox9.Text.Length < 0)
                errorProvider1.SetError(guna2TextBox9, "Şifre boş bırakılamaz.");
        }

        private void guna2TextBox9_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox9, "");
        }
        #endregion

        #endregion

        #region HASTA RANDEVULARIM EKRANI BİLEŞENLERİ

        // Randevuyu iptal etme butonu.
        private void guna2Button48_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0) // DataGrid'den satır seçildi mi kontrol ediliyor.
            {
                // Randevu duruum Randevu Alındı dışında olup olmadığı kontrol ediliyor.
                if (guna2DataGridView1.SelectedRows[0].Cells["randevuDurumu"].Value.ToString() != "Randevu Alındı")
                {
                    MessageBox.Show("Bu randevu iptal edilemez.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Randevu seçiniz");
                return;
            }
            
            DataGridViewRow selectedRow = guna2DataGridView1.SelectedRows[0]; // Seçilen satır

            // Randevu tarihi ve saati parçaralara ayrılıyor dizilere kaydediliyor.
            string[] randevuTarihParcalari = selectedRow.Cells["randevuTarihi"].Value.ToString().Split('.');
            string[] randevuSaatiParcalari = selectedRow.Cells["randevuSaati"].Value.ToString().Split('-')[0].Trim().Split(':');

            // Randevu tarihi ve saat yapısı oluşturuluyor.
            int year = int.Parse(randevuTarihParcalari[2]);
            int month = int.Parse(randevuTarihParcalari[1]);
            int day = int.Parse(randevuTarihParcalari[0]);
            int hour = int.Parse(randevuSaatiParcalari[0]);
            int minute = int.Parse(randevuSaatiParcalari[1]);

            DateTime randevuTarihi = new DateTime(year, month, day, hour, minute, 0); // Datetime değişkenine aktarılıyor.

            DateTime now = DateTime.Now;

            if (randevuTarihi < now) // Randevu tarihi geçmiş mi kontrol ediliyor.
            {
                MessageBox.Show("Geçmiş randevu ile ilgili işlem yapılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime ileriTarih = now.AddHours(4);
            
            if (randevuTarihi < ileriTarih) // Randevunun bitimine 4 saat kalıp kalmadığı kontrol ediliyor.
            {
                MessageBox.Show("Bu randevuyu en geç 4 saatten önce iptal edebilirdin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Randevunuzu iptal etmek istiyormusunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes) // Buraya kadar hata yoksa iptal için onay isteniyor onaylanırsa...
            {
                try
                {
                    // İlgili randevu veri tabanından referans alınıyor.
                    DocumentReference docRef = db.Collection("randevular").Document(selectedRow.Cells["ID"].Value.ToString());

                    var durumDeis = new { randevuDurumu = "Hasta İptal Etti" };
                    docRef.SetAsync(durumDeis, SetOptions.MergeAll); // Veri güncelleniyor.

                    // Seçili satırdaki randevu durumunu değiştiriliyor.
                    selectedRow.Cells["randevuDurumu"].Value = "Hasta İptal Etti";
                }
                catch (Exception ex)
                {
                    // Hata durumunda kullanıcıya bilgi ver
                    MessageBox.Show("Firestore güncelleme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // DataGrid'i yenileme butonu.
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            try
            {   // Giriş yapan kişinin randevuları taranıyor.
                QuerySnapshot querySnapshot = db.Collection("randevular").OrderBy("id")
                    .WhereEqualTo("hastaTC", girisDurum).GetSnapshotAsync().Result;
                guna2DataGridView1.Rows.Clear();
                // Gelen sorgudan veriler döngüye alınıyor.
                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    string id = documentSnapshot.Id;
                    string doktorAd = documentSnapshot.GetValue<string>("doktorAdi");
                    string randevuTarihi = documentSnapshot.GetValue<string>("randevuTarihi");
                    string randevuDurumu = documentSnapshot.GetValue<string>("randevuDurumu");
                    string randevuSaati = documentSnapshot.GetValue<string>("randevuSaati");
                    string poliklinikAdı = documentSnapshot.GetValue<string>("PoliklinikAdi");
                    // Veriler DataGrid'e yükleniyor
                    guna2DataGridView1.Rows.Add(id, doktorAd, poliklinikAdı, randevuTarihi, randevuSaati, randevuDurumu);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region HASTA RANDEVU ALMA EKRANI BİLEŞENLERİ

        List<string> secilenDoktorlar = new List<string>();
        string secilenDoktor;
        string secilenDoktorAd;
        private List<string> tumSaatler = new List<string> // Bütün randevu saatlerinin bulunduğu liste.
        {
            "08:20 - 08:30", "08:30 - 08:40", "08:40 - 08:50", "08:50 - 09:00",
            "09:00 - 09:10", "09:10 - 09:20", "09:30 - 09:40", "09:40 - 09:50",
            "09:50 - 10:00", "10:00 - 10:10", "10:10 - 10:20", "10:20 - 10:30",
            "10:30 - 10:40", "10:40 - 10:50", "10:50 - 11:00", "11:00 - 11:10",
            "11:10 - 11:20", "11:20 - 11:30", "11:30 - 11:40", "11:40 - 11:50",
            "11:50 - 12:00", "13:10 - 13:20", "13:20 - 13:30", "13:30 - 13:40",
            "13:40 - 13:50", "13:50 - 14:00", "14:00 - 14:10", "14:10 - 14:20",
            "14:30 - 14:40", "14:40 - 14:50", "14:50 - 15:00", "15:00 - 15:10",
            "15:10 - 15:20", "15:20 - 15:30", "15:30 - 15:40", "15:40 - 15:50",
            "15:50 - 16:00", "16:00 - 16:10", "16:10 - 16:20", "16:20 - 16:30",
            "16:30 - 16:40", "16:40 - 16:50", "16:50 - 17:00"
        };

        // Hasta randevu alma ekranındaki listelenen poliklinikler
        private void guna2ComboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                guna2ComboBox8.Items.Clear();
                List<string> doktorlar = new List<string>();

                // Polikinik adına göre veri tabanından id araması yapılıyor.
                QuerySnapshot polid = db.Collection("poliklinikler")
                                    .WhereEqualTo("poliklinikAdı", (guna2ComboBox7.SelectedItem).ToString())
                                    .GetSnapshotAsync().Result;

                // Poliklinik id'sine göre hangi doktorlar o polikliniğe aitse onların verileri alınıyor.
                QuerySnapshot sorgu = db.Collection("doktorlar")
                    .WhereEqualTo("poliklinik", polid.Documents[0].Id)
                    .GetSnapshotAsync().Result;

                secilenDoktorlar.Clear();
                // Seçilen polikliniğe ait bulunan doktorlar döngüye sokuluyor.
                foreach (DocumentSnapshot documentSnapshot in sorgu.Documents)
                {
                    secilenDoktorlar.Add(documentSnapshot.Id); // Bütün doktorların id si listeye kaydediliyor.
                    // Başka bir listeye de doktorların adı soyadı kaydediliyor.
                    doktorlar.Add(documentSnapshot.GetValue<string>("ad") +" "+ documentSnapshot.GetValue<string>("soyad")); 
                }

                // Adı soyadı kaydedilen doktorlar döngüye sokuluyor.
                foreach (string doktor in doktorlar)
                    guna2ComboBox8.Items.Add(doktor); // Döngü Sonrası ComboBox'a gönderiliyor.
                guna2ComboBox8.StartIndex = 0;

                // Tüm saatler buraya aktarılıyor.
                List<string> kullanılabilirSaatler = new List<string>(tumSaatler); 
                try
                {
                    QuerySnapshot polid2 = db.Collection("poliklinikler") // Poliklinik id'si bulunuyor.
                        .WhereEqualTo("poliklinikAdı", guna2ComboBox7.Text).GetSnapshotAsync().Result;

                    // Randevu tarihi polikliniği ve doktoru ile arama yapılıyor.
                    QuerySnapshot query = db.Collection("randevular")
                        .WhereEqualTo("randevuTarihi", bunifuDatePicker1.Value.ToString("dd.MM.yyyy"))
                        .WhereEqualTo("poliklinikID", polid2.Documents[0].Id)
                        .WhereEqualTo("doktorTC", secilenDoktor)
                        .GetSnapshotAsync().Result;

                    // Sorgu sonucu döngüye alınıyor.
                    foreach (DocumentSnapshot document in query.Documents)
                    {
                        // Aynı poliklinikte aynı günde aynı doktora ve aynı saate ait randevu varsa o saatteki randevu listeden siliniyor
                        string randevuSaati = document.GetValue<string>("randevuSaati");
                        kullanılabilirSaatler.Remove(randevuSaati);
                    }
                    guna2ComboBox9.Items.Clear(); // Saat ComboBoxu temizleniyor.
                    // Seçilen gün içerisinde randevu saati başkası tarafında alın saatler silinip boş randevu saatleri gönderiliyor.
                    guna2ComboBox9.Items.AddRange(kullanılabilirSaatler.ToArray());
                    guna2ComboBox9.StartIndex = 0; // İlk veri seçiliyor.
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                // Firestore'dan veri alırken bir hata oluştuğunda kullanıcıya bir hata mesajı gösterin
                MessageBox.Show("Veri alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // Hasta randevu alma ekranındaki doktorların listelendiği ComboBox
        private void guna2ComboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {   // Poliklikte olan doktorlar arasından hangisi seçildiyse onun id si secilenDoktora aktarılıyor.
            secilenDoktor = secilenDoktorlar[guna2ComboBox8.SelectedIndex];
            secilenDoktorAd = guna2ComboBox8.Text; // Seçilen doktorun adı değişkene gönderiliyor.
        }

        // Hasta randevu alma ekranındaki DateTimePicker
        private void bunifuDatePicker1_ValueChanged(object sender, EventArgs e)
        {
            // Firestore sorgusu için kullanılacak saat listesini tüm saatlerle başlat
            List<string> kullanılabilirSaatler = new List<string>(tumSaatler);

            try
            {   
                QuerySnapshot polid = db.Collection("poliklinikler") // Poliklinik id'si bulunuyor.
                    .WhereEqualTo("poliklinikAdı", guna2ComboBox7.Text).GetSnapshotAsync().Result;

                // Randevu tarihi polikliniği ve doktoru ile arama yapılıyor.
                QuerySnapshot query = db.Collection("randevular")
                    .WhereEqualTo("randevuTarihi", bunifuDatePicker1.Value.ToString("dd.MM.yyyy"))
                    .WhereEqualTo("poliklinikID", polid.Documents[0].Id)
                    .WhereEqualTo("doktorTC", secilenDoktor)
                    .GetSnapshotAsync().Result;
                
                // Aynı poliklinikte aynı günde aynı doktora ve aynı saate ait randevu varsa o saatteki randevu listeden siliniyor
                foreach (DocumentSnapshot document in query.Documents)
                {
                    // Aynı poliklinikte aynı günde aynı doktora ve aynı saate ait randevu varsa o saatteki randevu listeden siliniyor
                    string randevuSaati = document.GetValue<string>("randevuSaati");
                    kullanılabilirSaatler.Remove(randevuSaati);
                }
                guna2ComboBox9.Items.Clear(); // Saat ComboBoxu temizleniyor.
                // Seçilen gün içerisinde randevu saati başkası tarafında alın saatler silinip boş randevu saatleri gönderiliyor.
                guna2ComboBox9.Items.AddRange(kullanılabilirSaatler.ToArray());
                guna2ComboBox9.StartIndex = 0; // İlk veri seçiliyor.
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
            }
        }

        // Hasta randevu alma ekranındaki randevu alma butonu
        private void guna2Button49_Click(object sender, EventArgs e)
        {
            if(guna2ComboBox9.SelectedIndex<0||guna2ComboBox8.SelectedIndex<0||guna2ComboBox7.SelectedIndex<0)
            {   // ComboBox'lardan veri seçili değilse programı durduruyor.
                MessageBox.Show("Bilgileri girini!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Hastanın aynı tarihte herhangi bir randevusu varmı sorgulanıyor.
                QuerySnapshot aynıTarihSorgu = db.Collection("randevular")
                    .WhereEqualTo("hastaTC", girisDurum)
                    .WhereEqualTo("randevuTarihi", bunifuDatePicker1.Value.ToString("dd.MM.yyyy"))
                    .WhereEqualTo("randevuSaati", guna2ComboBox9.Text)
                    .GetSnapshotAsync().Result;

                if (aynıTarihSorgu.Count != 0)
                {   // Eğer varsa program akışı durduruluyor.
                    MessageBox.Show("Aynı tarihte randevu alınamaz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int sonID = 0;
                // Randevu kolleksiyonu sondan başa doğru sıralanıyor
                QuerySnapshot querySnapshot = db.Collection("randevular") 
                    .OrderByDescending("id").Limit(1).GetSnapshotAsync().Result;
                QuerySnapshot polunId = db.Collection("poliklinikler") // Poliklinik id'si alınıyor.
                    .WhereEqualTo("poliklinikAdı", guna2ComboBox7.Text).GetSnapshotAsync().Result;

                if (querySnapshot.Count > 0) // Veri varmı diye kontrol ediliyor.
                {
                    DocumentSnapshot sonBelge = querySnapshot.Documents[0];
                    sonID = sonBelge.GetValue<int>("id"); // En son id yi kaçsa o değişkene gönderiliyor
                }

                int yeniID = sonID +1; // Bir sonraki yeni randevu için id 1 arttırılıyor.
                // Yeni randevu id ile döküman olusturuluyor.
                DocumentReference dokuman = db.Collection("randevular").Document(yeniID.ToString());
                var randevu = new
                {
                    id=yeniID,
                    hastaTC = girisDurum,
                    doktorTC= secilenDoktor,
                    doktorAdi = secilenDoktorAd,
                    poliklinikID= polunId.Documents[0].Id,
                    PoliklinikAdi= guna2ComboBox7.Text,
                    randevuTarihi = bunifuDatePicker1.Value.ToString("dd.MM.yyyy"),
                    randevuSaati = guna2ComboBox9.Text,
                    randevuDurumu   = "Randevu Alındı"
                };
                dokuman.SetAsync(randevu).Wait(); // Veriler olusturuluan dökümana gönderiliyor.
                MessageBox.Show("Randevunuz başarıyla alındı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Alınan randevu hastanın randevularım sayfasındaki randevuların bulunduğu DataGride gönderiliyor.
                guna2DataGridView1.Rows.Add(yeniID, secilenDoktorAd, guna2ComboBox7.Text, 
                    bunifuDatePicker1.Value.ToString("dd.MM.yyyy"), guna2ComboBox9.Text, "Randevu Alındı");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Firestore'a veri eklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region HASTA GÜNCELLEME EKRANI BİLEŞENLERİ

        string gsmGuncelle;
        string emailGuncelle;
        string adresGuncelle;

        private void guna2Button41_Click(object sender, EventArgs e)// Hasta bilgi güncelleme ekranı güncelle butonu
        {
            if (!Regex.IsMatch(guna2TextBox13.Text, @"^0\d{10}$"))
                errorProvider1.SetError(guna2TextBox13, "Telefon numarası 0 ile başlamalıdır ve doğru girilmelidir.");
            if (!Regex.IsMatch(guna2TextBox14.Text, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
                errorProvider1.SetError(guna2TextBox14, "Geçerli bir e-posta adresi giriniz.");
            // Metin giriş kontrolü yapılıyor.
            if (!Regex.IsMatch(guna2TextBox13.Text, @"^0\d{10}$") || !Regex.IsMatch(guna2TextBox14.Text, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
                return;
            try
            {
                DocumentReference dokuman = db.Collection("hastalar").Document(girisDurum);
                // Giriş yapan hastanın veri tabanından referansı alınıyor.
                DocumentSnapshot sorgu = dokuman.GetSnapshotAsync().Result;

                if (guna2TextBox16.Text == guna2TextBox17.Text) // Şifreler eşit ise...
                {
                    if (guna2TextBox16.Text != "" && guna2TextBox17.Text != "") // Şifre TextBox'larında şifreler boş değilse...
                    {
                        var kullanıcı = new
                        {
                            gsm = guna2TextBox13.Text,
                            email = guna2TextBox14.Text,
                            adres = guna2TextBox15.Text,
                            şifre = guna2TextBox16.Text
                        };
                        // Girilen bilgiler veri tabanına gönderiliyor.
                        dokuman.SetAsync(kullanıcı, SetOptions.MergeAll).Wait();
                        MessageBox.Show("Bilgiler başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        gsmGuncelle = guna2TextBox13.Text;
                        emailGuncelle = guna2TextBox14.Text;
                        adresGuncelle = guna2TextBox15.Text; // Veriler değişkenlere kaydediliyor.
                    }
                    else // Şifreler boş ise...
                    {
                        var kullanıcı = new
                        {
                            gsm = guna2TextBox13.Text,
                            email = guna2TextBox14.Text,
                            adres = guna2TextBox15.Text,
                        };
                        // Girilen bilgiler veri tabanına gönderiliyor.
                        dokuman.SetAsync(kullanıcı, SetOptions.MergeAll).Wait();
                        MessageBox.Show("Bilgiler başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        gsmGuncelle = guna2TextBox13.Text;
                        emailGuncelle = guna2TextBox14.Text;
                        adresGuncelle = guna2TextBox15.Text; // Veriler değişkenlere kaydediliyor.
                    }
                } 
                else // Şifreler eşit değilse gerekli hatalar veriliyor.
                {
                    errorProvider1.SetError(guna2TextBox16, "Şifreler eşit değil!");
                    errorProvider1.SetError(guna2TextBox17, "Şifreler eşit değil!");
                    MessageBox.Show("Şifreler eşit değil."); 
                }               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bilgiler kaydedilirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        #region HASTA BİLGİ GÜNCELLE GSM TEXTBOX
        private void guna2TextBox13_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
        private void guna2TextBox13_Validating(object sender, CancelEventArgs e)
        {
            if (!Regex.IsMatch(guna2TextBox13.Text, @"^0\d{10}$"))
                errorProvider1.SetError(guna2TextBox6, "Telefon numarası 0 ile başlamalıdır ve doğru girilmelidir.");
        }
        private void guna2TextBox13_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox13, "");
        }
        
        private void guna2TextBox13_TextChanged(object sender, EventArgs e)
        {   // Eğer değiştirilen değer hastanın veri tabanındaki veriden farklı mı kontrolü yapılıyor.
            if(gsmGuncelle!=guna2TextBox13.Text) // Farklı ise icon resmi belirleniyor aktif ediliyor.
                guna2TextBox13.IconRight= Properties.Resources._return;
            else
                guna2TextBox13.IconRight = null; // Değilse siliniyor.
        }
        private void guna2TextBox13_IconRightClick(object sender, EventArgs e)
        {   // İcon aktif olduğu zaman basılırsa veri tabanından değişkene aktarılan değer yazdırılıyor.
            guna2TextBox13.Text = gsmGuncelle;
        }
        #endregion

        #region HASTA BİLGİ GÜNECLLE EMAİL TEXTBOX
        private void guna2TextBox14_Validating(object sender, CancelEventArgs e)
        {
            if (!Regex.IsMatch(guna2TextBox14.Text, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
                errorProvider1.SetError(guna2TextBox14, "Geçerli bir e-posta adresi giriniz.");
        }

        private void guna2TextBox14_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox14, "");
        }
        
        private void guna2TextBox14_TextChanged(object sender, EventArgs e)
        {   // Eğer değiştirilen değer hastanın veri tabanındaki veriden farklı mı kontrolü yapılıyor.
            if(emailGuncelle!=guna2TextBox14.Text) // Farklı ise icon resmi belirleniyor aktif ediliyor.
                guna2TextBox14.IconRight= Properties.Resources._return;
            else
                guna2TextBox14.IconRight = null; // Değilse siliniyor.
        }

        private void guna2TextBox14_IconRightClick(object sender, EventArgs e)
        {   // İcon aktif olduğu zaman basılırsa veri tabanından değişkene aktarılan değer yazdırılıyor.
            guna2TextBox14.Text = emailGuncelle;
        }
        #endregion

        #region HASTA BİLGİ GÜNCELLE ADRES TEXTBOX
        private void guna2TextBox15_TextChanged(object sender, EventArgs e)
        {   // Eğer değiştirilen değer hastanın veri tabanındaki veriden farklı mı kontrolü yapılıyor.
            if(adresGuncelle!=guna2TextBox15.Text) // Farklı ise icon resmi belirleniyor aktif ediliyor.
                guna2TextBox15.IconRight= Properties.Resources._return;
            else
                guna2TextBox15.IconRight = null; // Değilse siliniyor.
        }

        private void guna2TextBox15_IconRightClick(object sender, EventArgs e)
        {   // İcon aktif olduğu zaman basılırsa veri tabanından değişkene aktarılan değer yazdırılıyor.
            guna2TextBox15.Text = adresGuncelle;
        }
        #endregion

        #region HASTA BİLGİ GÜNCELLEME ŞİFRE TEXTBOX
        private void guna2TextBox16_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox16.PasswordChar = guna2TextBox16.PasswordChar == '\0' ? '●' : '\0';
            if (guna2TextBox16.PasswordChar == '●')
                guna2TextBox16.IconRight = Properties.Resources.eyes;
            else
                guna2TextBox16.IconRight = Properties.Resources.eyes_off;
        }
        private void guna2TextBox16_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox16, "");
            errorProvider1.SetError(guna2TextBox17, "");
        }
        #endregion

        #region HASTA BİLGİ GÜNCELLEME ŞİFRE TEKRAR TEXTBOX
        private void guna2TextBox17_IconRightClick(object sender, EventArgs e)
        {            
            guna2TextBox17.PasswordChar = guna2TextBox17.PasswordChar == '\0' ? '●' : '\0';
            if (guna2TextBox17.PasswordChar == '●')
                guna2TextBox17.IconRight = Properties.Resources.eyes;
            else
                guna2TextBox17.IconRight = Properties.Resources.eyes_off;
        }
        private void guna2TextBox17_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox17.Text != guna2TextBox16.Text)
            {
                errorProvider1.SetError(guna2TextBox17, "Şifreler eşleşmiyor.");
                errorProvider1.SetError(guna2TextBox17, "Şifreler eşleşmiyor.");
            }
        }
        private void guna2TextBox17_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox17, "");
            errorProvider1.SetError(guna2TextBox16, "");
        }
        #endregion

        #endregion

        #region DOKTOR HASTA RANDEVULARI GÖRDÜĞÜ EKRANKİ BİLEŞENLER

        // Doktor hasta randevularını gördüğü ekrandaki randevu tamamlandı butonu.
        private void guna2Button44_Click(object sender, EventArgs e)
        {
           
            if (guna2DataGridView2.SelectedRows.Count > 0) // DataGrid'den satır seçildi mi kontrol ediliyor.
            {   
                // Randevu duruum Randevu Alındı dışında olup olmadığı kontrol ediliyor
                if (guna2DataGridView2.SelectedRows[0].Cells["randevuDurumu2"].Value.ToString() != "Randevu Alındı")
                {
                    MessageBox.Show("Bu randevu ile ilgili işlem yapılamaz.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Randevu seçiniz");
                return ;
            }

            try
            {
                DataGridViewRow selectedRow = guna2DataGridView2.SelectedRows[0]; // Seçilen satır.

                // Randevu tarihi ve saati parçaralara ayrılıyor dizilere kaydediliyor.
                string[] randevuTarihParcalari = selectedRow.Cells["randevuTarihi2"].Value.ToString().Split('.');
                string[] randevuSaatiParcalari = selectedRow.Cells["randevuSaati2"].Value.ToString().Split('-')[0].Trim().Split(':');
                
                // Randevu tarihi ve saat yapısı oluşturuluyor.
                int year = int.Parse(randevuTarihParcalari[2]);
                int month = int.Parse(randevuTarihParcalari[1]);
                int day = int.Parse(randevuTarihParcalari[0]);
                int hour = int.Parse(randevuSaatiParcalari[0]);
                int minute = int.Parse(randevuSaatiParcalari[1]);

                DateTime randevuTarihi = new DateTime(year, month, day, hour, minute, 0); // Datetime değişkenine aktarılıyor.

                DateTime now = DateTime.Now;
                if (randevuTarihi > now) // Randevu tarihi gelmiş mi kontrol ediliyor.
                {
                    MessageBox.Show("Tarihi gelmeyen randevuda işlem yapılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // İlgili randevu veri tabanından referans alınıyor.
                DocumentReference docRef = db.Collection("randevular").Document(selectedRow.Cells["ID2"].Value.ToString());

                var durumDeis = new { randevuDurumu = "Randevu Tamamlandı" };
                docRef.SetAsync(durumDeis, SetOptions.MergeAll).Wait(); // Veri güncelleniyor.

                selectedRow.Cells["randevuDurumu2"].Value = "Randevu Tamamlandı"; // Seçili satırdaki randevu durumunu değiştiriliyor.
                MessageBox.Show("Randevu başarıyla işaretlendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri güncelleme güncelleme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // Doktor hasta randevularını gördüğü ekrandaki randevuya gelinmedi butonu.
        private void guna2Button46_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView2.SelectedRows.Count > 0) // DataGrid'den satır seçildi mi kontrol ediliyor.
            {
                // Randevu duruum Randevu Alındı dışında olup olmadığı kontrol ediliyor
                if (guna2DataGridView2.SelectedRows[0].Cells["randevuDurumu2"].Value.ToString() != "Randevu Alındı")
                {
                    MessageBox.Show("Bu randevu ile ilgili işlem yapılamaz.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Randevu seçiniz");
                return;
            }
            try
            {
                DataGridViewRow selectedRow = guna2DataGridView2.SelectedRows[0]; // Seçilen satır.

                // Randevu tarihi ve saati parçaralara ayrılıyor dizilere kaydediliyor.
                string[] randevuTarihParcalari = selectedRow.Cells["randevuTarihi2"].Value.ToString().Split('.');
                string[] randevuSaatiParcalari = selectedRow.Cells["randevuSaati2"].Value.ToString().Split('-')[0].Trim().Split(':');

                // Randevu tarihi ve saat yapısı oluşturuluyor.
                int year = int.Parse(randevuTarihParcalari[2]);
                int month = int.Parse(randevuTarihParcalari[1]);
                int day = int.Parse(randevuTarihParcalari[0]);
                int hour = int.Parse(randevuSaatiParcalari[0]);
                int minute = int.Parse(randevuSaatiParcalari[1]);

                DateTime randevuTarihi = new DateTime(year, month, day, hour, minute, 0); // Datetime değişkenine aktarılıyor.

                DateTime now = DateTime.Now;
                if (randevuTarihi > now) // Randevu tarihi gelmiş mi kontrol ediliyor.
                {
                    MessageBox.Show("Tarihi gelmeyen randevuda işlem yapılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // İlgili randevu veri tabanından referans alınıyor
                DocumentReference docRef = db.Collection("randevular").Document(selectedRow.Cells["ID2"].Value.ToString());

                var durumDeis = new { randevuDurumu = "Randevuya Gelinmedi" };
                docRef.SetAsync(durumDeis, SetOptions.MergeAll); // Veri güncelleniyor.
                selectedRow.Cells["randevuDurumu2"].Value = "Randevuya Gelinmedi"; // Seçili satırdaki randevu durumunu değiştiriliyor.
                MessageBox.Show("Randevu başarıyla işaretlendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcıya bilgi ver
                MessageBox.Show("Firestore güncelleme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // Doktor hasta randevularını gördüğü ekrandaki yenile butonu.
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            try
            {   // Veri tabanından doktorun randevuları sorgulanıyor.
                QuerySnapshot querySnapshot = db.Collection("randevular")
                    .OrderBy("id").WhereEqualTo("doktorTC", girisDurum).GetSnapshotAsync().Result;
                guna2DataGridView2.Rows.Clear(); // DataGrid temizleniyor.
                guna2CheckBox3.Checked = false; // Filtreleme sıfırlanıyor.
                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents) // Sorgudan gelen veriler döngüye sokuluyor.
                {
                    string id = documentSnapshot.Id;
                    string hastaTC = documentSnapshot.GetValue<string>("hastaTC");
                    DocumentReference document = db.Collection("hastalar").Document(hastaTC);
                    DocumentSnapshot sorgu1 = document.GetSnapshotAsync().Result;
                    string hastaAd = sorgu1.GetValue<string>("ad") + " " + sorgu1.GetValue<string>("soyad");
                    string randevuTarihi = documentSnapshot.GetValue<string>("randevuTarihi");
                    string randevuDurumu = documentSnapshot.GetValue<string>("randevuDurumu");
                    string randevuSaati = documentSnapshot.GetValue<string>("randevuSaati");
                    // Her veri DataGride yazdırılıyor.
                    guna2DataGridView2.Rows.Add(id, hastaAd, randevuTarihi, randevuSaati, randevuDurumu);
                }
            }
            catch (Exception ex)
            {
                // Firestore'dan veri alırken bir hata oluşursa kullanıcıya bildirin
                MessageBox.Show("Veri alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Doktor hasta randevularını gördüğü ekrandaki CheckBox.
        private void guna2CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox3.Checked == true) // Filtreleme CheckBox'u aktif edilirse...
            {
                string tarihi = bunifuDatePicker5.Value.ToString("dd.MM.yyyy"); // Seçilen tarih.

                foreach (DataGridViewRow row in guna2DataGridView2.Rows) // DataGrid'deki verileri döngüye sokuyor.
                {
                    string tarih = row.Cells["randevuTarihi2"].Value.ToString(); // Döngüdeki tarihi değişkene atıyor.

                    if (tarih == tarihi) // Seçilen tarih ile döngüdeki tarih aynımı...
                        row.Visible = true; // Aynı ise filtreleme işlevi gereği gözükmesini sağlar
                    else
                        row.Visible = false; // Aynı değilse göstermesin.
                }
            }
            else // Filtreleme kapalı ise...
            {
                foreach (DataGridViewRow row in guna2DataGridView2.Rows)
                {
                    row.Visible = true; // Bütün satırları görünür hale getir.
                }
            }
        }

        #endregion

        #region DOKTOR BİLGİ GÜNCELLEME EKRANI BİLEŞENLERİ

        // Doktor bilgi güncelleme ekranı güncelle butonu
        private void guna2Button50_Click(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(guna2TextBox26.Text, @"^0\d{10}$")) // Metin girişi doğrulama kodu.
            {
                errorProvider1.SetError(guna2TextBox26, "Telefon numarası 0 ile başlamalıdır ve doğru girilmelidir.");
                return;
            }

            if (guna2TextBox23.Text == guna2TextBox24.Text) // Şifreler eşit ise...
            {
                try
                {   // Giriş yapan kişinin dökümanı referans alınıyor.
                    DocumentReference dokuman = db.Collection("doktorlar").Document(girisDurum.ToString());
                    DocumentSnapshot sorgu = dokuman.GetSnapshotAsync().Result;
                    if (sorgu.Exists) // Sorgu da veri dönüyorsa...
                    {
                        if(guna2TextBox24.Text!="" && guna2TextBox23.Text!="") // Şifre TextBox'ları boş değilse...
                        {
                            var kullanıcı = new { gsm = guna2TextBox26.Text, şifre = guna2TextBox24.Text };
                            dokuman.SetAsync(kullanıcı, SetOptions.MergeAll); // Verileri güncelleme.
                            MessageBox.Show("Bilgiler başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            gsmGuncelle = guna2TextBox26.Text; // Veriyi değişkene aktar.
                        }
                        else // Şifreler boş ise...
                        {
                            var kullanıcı = new { şifre = guna2TextBox26.Text };
                            dokuman.SetAsync(kullanıcı, SetOptions.MergeAll); // Verileri güncelleme.
                            MessageBox.Show("Bilgiler başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            gsmGuncelle = guna2TextBox26.Text; // Veriyi değişkene aktar.
                        }
                    }
                    else // Kullanıcı bulunamazsa...
                        MessageBox.Show("Kullanıcı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Şifre güncellenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else // Şifreler eşit değilse gerekli hatalar veriliyor.
            { 
                errorProvider1.SetError(guna2TextBox23, "Şifreler eşit değil!");
                errorProvider1.SetError(guna2TextBox24, "Şifreler eşit değil!");
                MessageBox.Show("Şifreler eşit değil!"); 
            }
        }
        
        #region DOKTOR BİLGİ GÜNCELLEME GSM TEXTBOX
        private void guna2TextBox26_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
        private void guna2TextBox26_Validating(object sender, CancelEventArgs e)
        {   
            if (!Regex.IsMatch(guna2TextBox26.Text, @"^0\d{10}$"))
                errorProvider1.SetError(guna2TextBox26, "Telefon numarası 0 ile başlamalıdır ve doğru girilmelidir.");
        }
        private void guna2TextBox26_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox6, "");
        }
        private void guna2TextBox26_TextChanged(object sender, EventArgs e)
        {   // Eğer değiştirilen değer doktorun veri tabanındaki veriden farklı mı kontrolü yapılıyor
            if (gsmGuncelle != guna2TextBox26.Text)
                // Farklı ise icon resmi belirleniyor aktif ediliyor
                guna2TextBox26.IconRight = Properties.Resources._return; 
            else
                guna2TextBox26.IconRight = null; // Değilse siliniyor
        }

        private void guna2TextBox26_IconRightClick(object sender, EventArgs e)
        {
            // İcon aktif olduğu zaman basılırsa veri tabanından değişkene aktarılan değer yazdırılıyor
            guna2TextBox26.Text = gsmGuncelle; 
        }
        #endregion
        
        #region DOKTOR BİLGİ GÜNCELLEME ŞİFRE TEXTBOX
        private void guna2TextBox24_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox24.PasswordChar = guna2TextBox24.PasswordChar == '\0' ? '●' : '\0';
            if (guna2TextBox24.PasswordChar == '●')
                guna2TextBox24.IconRight = Properties.Resources.eyes;
            else
                guna2TextBox24.IconRight = Properties.Resources.eyes_off;
        }

        private void guna2TextBox24_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox24, "");
            errorProvider1.SetError(guna2TextBox23, "");
        }
        #endregion

        #region DOKTOR BİLGİ GÜNCELLEME ŞİFRE TEKRAR TEXTBOX
        private void guna2TextBox23_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox23.PasswordChar = guna2TextBox23.PasswordChar == '\0' ? '●' : '\0';
            if (guna2TextBox23.PasswordChar == '●')
                guna2TextBox23.IconRight = Properties.Resources.eyes;
            else
                guna2TextBox23.IconRight = Properties.Resources.eyes_off;
        }

        private void guna2TextBox23_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox23.Text != guna2TextBox24.Text)
            {
                errorProvider1.SetError(guna2TextBox23, "Şifreler eşleşmiyor.");
                errorProvider1.SetError(guna2TextBox24, "Şifreler eşleşmiyor.");
            }
        }

        private void guna2TextBox23_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox23, "");
            errorProvider1.SetError(guna2TextBox24, "");
        }
        #endregion

        #endregion

        #region YÖNETİCİ DOKTOR KAYIT EKRANI BİLEŞENLERİ

        // Yönetici doktor kayıt ekranındaki doktoru kaydet butonu.
        private void guna2Button51_Click(object sender, EventArgs e)
        {
            if (guna2TextBox18.Text.Length < 11)
                errorProvider1.SetError(guna2TextBox18, "TC Kimlik Numarası 11 haneli olarak giriniz.");
            if (guna2TextBox19.Text.Length < 3)
                errorProvider1.SetError(guna2TextBox19, "Yanlış ad girişi.");
            if (guna2TextBox20.Text.Length < 3)
                errorProvider1.SetError(guna2TextBox20, "Yanlış soyad girişi.");
            if (!Regex.IsMatch(guna2TextBox25.Text, @"^0\d{10}$"))
                errorProvider1.SetError(guna2TextBox25, "Telefon numarası 0 ile başlamalıdır ve doğru girilmelidir.");
            if (guna2TextBox21.Text.Length < 1)
                errorProvider1.SetError(guna2TextBox21, "Şifre boş bırakılamaz.");
            // Metin hata kontrolleri yapılıyor.
            if(guna2TextBox18.Text.Length < 11||guna2TextBox19.Text.Length < 3||guna2TextBox20.Text.Length < 3||
                !Regex.IsMatch(guna2TextBox25.Text, @"^0\d{10}$")||guna2TextBox21.Text.Length < 1)
                return;

            try
            {
                DocumentReference dokuman = db.Collection("doktorlar").Document(guna2TextBox18.Text.ToString());
                QuerySnapshot sehirid = db.Collection("sehirler")
                    .WhereEqualTo("SehirAdi", guna2ComboBox4.Text) // Şehir adına göre id getiriliyor.
                    .GetSnapshotAsync().Result;
                QuerySnapshot polid = db.Collection("poliklinikler")
                    .WhereEqualTo("poliklinikAdı", guna2ComboBox6.Text) 
                    .GetSnapshotAsync().Result; // Poliklinik adına göre id getiriliyor.
                var user = new
                {
                    tc = guna2TextBox18.Text,
                    ad = guna2TextBox19.Text,
                    soyad = guna2TextBox20.Text,
                    cinsiyet = guna2ComboBox5.Text,
                    dogumyeri = sehirid.Documents[0].Id,
                    dogumtarihi = bunifuDatePicker4.Value.ToString("dd.MM.yyyy"),
                    poliklinik = polid.Documents[0].Id,
                    gsm = guna2TextBox25.Text,
                    şifre = guna2TextBox21.Text
                };
                dokuman.SetAsync(user).Wait(); // Veri tabanına verileri gönder
                MessageBox.Show("Başarıyla kayıt yapıldı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri eklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region DOKTOR KAYIT TC TEXTBOX
        private void guna2TextBox18_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void guna2TextBox18_Enter(object sender, EventArgs e)
        {
            if (guna2TextBox18.Text.Length < 11)
                errorProvider1.SetError(guna2TextBox18, "TC Kimlik Numarası 11 haneli olarak giriniz.");
        }

        private void guna2TextBox18_Leave(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox18, "");
        }
        #endregion

        #region DOKTOR KAYIT AD TEXTBOX
        private void guna2TextBox19_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox19.Text.Length < 3)
                errorProvider1.SetError(guna2TextBox19, "Yanlış ad girişi.");
        }

        private void guna2TextBox19_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox19, "");
        }
        #endregion

        #region DOKTOR KAYIT SOYAD TEXTBOX
        private void guna2TextBox20_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox20.Text.Length < 3)
                errorProvider1.SetError(guna2TextBox20, "Yanlış soyad girişi.");
        }

        private void guna2TextBox20_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox20, "");
        }
        #endregion

        #region DOKTOR KAYIT GSM TEXTBOX
        private void guna2TextBox25_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void guna2TextBox25_Validating(object sender, CancelEventArgs e)
        {
            // Telefon numarası için basit bir kontrol yapısı
            // Bu kontrolü gereksinimlerinize göre genişletebilirsiniz
            if (!Regex.IsMatch(guna2TextBox25.Text, @"^0\d{10}$"))
                errorProvider1.SetError(guna2TextBox25, "Telefon numarası 0 ile başlamalıdır ve doğru girilmelidir.");
        }

        private void guna2TextBox25_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox25, "");
        }
        #endregion

        #region DOKTOR KAYIT ŞİFRE TEXTBOX
        private void guna2TextBox21_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox21.PasswordChar = guna2TextBox21.PasswordChar == '\0' ? '●' : '\0';
            if (guna2TextBox21.PasswordChar == '●')
                guna2TextBox21.IconRight = Properties.Resources.eyes;
            else
                guna2TextBox21.IconRight = Properties.Resources.eyes_off;
        }

        private void guna2TextBox21_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox21.Text.Length < 0)
                errorProvider1.SetError(guna2TextBox21, "Şifre boş bırakılamaz.");
        }

        private void guna2TextBox21_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox21, "");
        }
        #endregion

        #endregion

        #region YÖNETİCİ DOKTOR GÖRÜNTÜLEME VE DÜZENLEME EKRANI BİLEŞENLERİ

        string doktorDuzenTc;
        string doktorDuzenAd;
        string doktorDuzenSoyad;
        string doktorDuzenDogumTarihi;
        string doktorDuzenCinsiyet;
        string doktorDuzenDogumYeri;
        string doktorDuzenPol;
        string doktorDuzenGsm;
        string doktorDuzenSifre;

        // Yönetici doktor görüntüleme ekranı doktorların listelendiği DataGrid veri seçimi olunca.
        private void guna2DataGridView3_SelectionChanged(object sender, EventArgs e)
        {
            if (guna2DataGridView3.SelectedRows.Count > 0) // Seçilen satır varsa
            {
                errorProvider1.SetError(guna2TextBox30, "");
                errorProvider1.SetError(guna2TextBox29, "");
                errorProvider1.SetError(guna2TextBox28, "");
                errorProvider1.SetError(guna2TextBox22, "");
                errorProvider1.SetError(guna2TextBox27, ""); // Yeni veri seçiminden dolayı hatalar sıfırlanıyor.

                DataGridViewRow secilenSatir = guna2DataGridView3.SelectedRows[0]; // Seçilen satırın verileri.

                doktorDuzenTc = secilenSatir.Cells["TC"].Value.ToString();
                doktorDuzenAd = secilenSatir.Cells["adı"].Value.ToString();
                doktorDuzenSoyad = secilenSatir.Cells["soyadı"].Value.ToString();
                doktorDuzenDogumTarihi = secilenSatir.Cells["dogumTarihi"].Value.ToString();
                doktorDuzenCinsiyet = secilenSatir.Cells["cinsiyet"].Value.ToString();
                doktorDuzenDogumYeri = secilenSatir.Cells["dogumYeri"].Value.ToString();
                doktorDuzenPol = secilenSatir.Cells["pol"].Value.ToString();
                doktorDuzenGsm = secilenSatir.Cells["no"].Value.ToString();
                doktorDuzenSifre = secilenSatir.Cells["sifre"].Value.ToString(); 
                // Veriler değiştirilen metnin icona basınca geri yüklenebilmesi için değişkenlere gönderiliyor.

                guna2TextBox30.Text = secilenSatir.Cells["TC"].Value.ToString();
                guna2TextBox29.Text = secilenSatir.Cells["adı"].Value.ToString();
                guna2TextBox28.Text = secilenSatir.Cells["soyadı"].Value.ToString();
                guna2DateTimePicker1.Value = DateTime.ParseExact(secilenSatir.Cells["dogumTarihi"].Value.ToString(), "dd.MM.yyyy", null);
                guna2ComboBox13.SelectedItem = secilenSatir.Cells["cinsiyet"].Value.ToString();
                guna2ComboBox10.SelectedItem = secilenSatir.Cells["dogumYeri"].Value.ToString();
                guna2ComboBox12.SelectedItem = secilenSatir.Cells["pol"].Value.ToString();
                guna2TextBox22.Text = secilenSatir.Cells["no"].Value.ToString();
                guna2TextBox27.Text = secilenSatir.Cells["sifre"].Value.ToString();
                // DataGrid'deki veriler TextBox ve ComboBox'lara gönderiliyor.
            }
        }       

        // Yönetici doktor görüntülemme ekranı doktoru düzenle butonu.
        private void guna2Button42_Click(object sender, EventArgs e)
        {
            if (guna2TextBox30.Text.Length < 11)
                errorProvider1.SetError(guna2TextBox30, "TC Kimlik Numarası 11 haneli olarak giriniz.");
            if (guna2TextBox29.Text.Length < 3)
                errorProvider1.SetError(guna2TextBox29, "Yanlış ad girişi.");
            if (guna2TextBox28.Text.Length < 3)
                errorProvider1.SetError(guna2TextBox28, "Yanlış soyad girişi.");
            if (!Regex.IsMatch(guna2TextBox22.Text, @"^0\d{10}$"))
                errorProvider1.SetError(guna2TextBox22, "Telefon numarası 0 ile başlamalıdır ve doğru girilmelidir.");
            if (guna2TextBox27.Text.Length < 1)
                errorProvider1.SetError(guna2TextBox27, "Şifre boş bırakılamaz.");
            // Metin girişlerinde hata varmı kontrol ediliyor.
            if (guna2TextBox30.Text.Length < 11 || guna2TextBox29.Text.Length < 3 || guna2TextBox28.Text.Length < 3 ||
                !Regex.IsMatch(guna2TextBox22.Text, @"^0\d{10}$") || guna2TextBox27.Text.Length < 1)
                return;

            try
            {
                // İşlem yapılacak doktorun veri tabanındaki dökümanını getirme.
                DocumentReference dokuman = db.Collection("doktorlar").Document(guna2TextBox30.Text);
                DocumentSnapshot sorgu = dokuman.GetSnapshotAsync().Result; 

                QuerySnapshot sehirid = db.Collection("sehirler")
                    .WhereEqualTo("SehirAdi", guna2ComboBox10.Text) 
                    .GetSnapshotAsync().Result; // Şehir adına göre id alınııyor.
                QuerySnapshot polid = db.Collection("poliklinikler")
                    .WhereEqualTo("poliklinikAdı", guna2ComboBox12.Text)
                    .GetSnapshotAsync().Result; // Poliklinik adına göre id alınıyor.

                if(sorgu.Exists) // Yazılan doktor TC'sine göre döküman bulunursa...
                {
                    var doktor = new
                    {
                        ad = guna2TextBox29.Text,
                        soyad = guna2TextBox28.Text,
                        cinsiyet = guna2ComboBox13.Text,
                        dogumyeri = sehirid.Documents[0].Id,
                        dogumtarihi = guna2DateTimePicker1.Value.ToString("dd.MM.yyyy"),
                        poliklinik = polid.Documents[0].Id,
                        gsm = guna2TextBox22.Text,
                        şifre = guna2TextBox27.Text
                    };
                    dokuman.SetAsync(doktor, SetOptions.MergeAll); // Veriler veri tabanına yükleniyor
                    doktorDuzenAd = guna2TextBox29.Text;
                    doktorDuzenSoyad = guna2TextBox28.Text;
                    doktorDuzenDogumTarihi = guna2DateTimePicker1.Value.ToString("dd.MM.yyyy");
                    doktorDuzenCinsiyet = guna2ComboBox13.Text;
                    doktorDuzenDogumYeri = guna2ComboBox10.Text;
                    doktorDuzenPol = guna2ComboBox12.Text;
                    doktorDuzenGsm = guna2TextBox22.Text;
                    doktorDuzenSifre = guna2TextBox27.Text; // Veriler değişkenlere aktarılıyor.

                    foreach (DataGridViewRow row in guna2DataGridView3.Rows) // DataGrid satırları döngüye alınıyor.
                    {   // Veriler veri tabanına gönderildikten sonra güncelleme işlemi DataGrid'de yapılıyor.
                        if (row.Cells["TC"].Value.ToString() == guna2TextBox30.Text) // İşlem yapılan TC bulunursa...
                        {
                            row.Cells["adı"].Value = guna2TextBox29.Text;
                            row.Cells["soyadı"].Value = guna2TextBox28.Text;
                            row.Cells["dogumTarihi"].Value = guna2DateTimePicker1.Value.ToString("dd.MM.yyyy");
                            row.Cells["cinsiyet"].Value = guna2ComboBox13.SelectedItem.ToString();
                            row.Cells["dogumYeri"].Value = guna2ComboBox10.SelectedItem.ToString();
                            row.Cells["pol"].Value = guna2ComboBox12.SelectedItem.ToString();
                            row.Cells["no"].Value = guna2TextBox22.Text;
                            row.Cells["sifre"].Value = guna2TextBox27.Text;
                            break; // Döngü durduruluyor.
                        }
                    }
                    MessageBox.Show("Veri başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Kullanıcı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri eklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Yönetici doktor görüntülemme ekranı doktoru silme butonu.
        private void guna2Button43_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView3.SelectedRows.Count <= 1) // Eğer veri seçili ise...
            {
                try
                {
                    // Yazılan TC ye göre DataGrid'den o satırın verisini arama.
                    DataGridViewRow secilenSatir = guna2DataGridView3.Rows
                        .Cast<DataGridViewRow>()
                        .FirstOrDefault(row => row.Cells["TC"].Value.ToString() == guna2TextBox30.Text); 

                    if(secilenSatir != null) // Eğer veri varsa...
                    {
                        DialogResult result = MessageBox.Show($"{doktorDuzenAd} {doktorDuzenSoyad} adlı doktoru silmek istediğine emin misin?",
                            "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes) // Kullanıcı evete basarsa...
                        {
                            DocumentReference documentRef = db.Collection("doktorlar").Document(doktorDuzenTc);
                            documentRef.DeleteAsync().Wait(); // Veri tabanından doktor siliniyor.

                            guna2DataGridView3.Rows.Remove(secilenSatir); // DataGrid'den veri siliniyor.
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata oluştu", ex.Message);
                }
            }
            else
                MessageBox.Show("Birden fazla doktor seçilemez.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Yönetici doktor görüntüleme ekranı filtreleme için kullanılan ComboBox.
        private void guna2ComboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedFilter = guna2ComboBox11.SelectedItem.ToString();

            if (selectedFilter == "Bütün Poliklinikler")  
            {   // Eğer seçilen veri bütün poliklinikler ise filtreleme işlemi iptal.
                foreach (DataGridViewRow row in guna2DataGridView3.Rows)
                    row.Visible = true; // Tüm satırlar gösteriliyor.
            }
            else
            {
                foreach (DataGridViewRow row in guna2DataGridView3.Rows)
                {
                    // Filtrelenen veri DataGrid'de aranıyor.
                    if (row.Cells["pol"].Value.ToString() == selectedFilter)
                    {
                        row.Visible = true; // Filtreye uyan satırı göster
                    }
                    else
                    {
                        row.Visible = false; // Diğer satırları gizle
                    }
                }
            }
        }

        // Yönetici doktor listeleme ekranı TC no yu elle yazmak için kullanılan CheckBox.
        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox1.Checked) // Eğer seçilirse TC yazma TextBox'u aktif ediliyor.
                guna2TextBox30.Enabled = true;
            else
            {   // Eğer seçilmezse...
                errorProvider1.SetError(guna2TextBox30, ""); // Hata temizleniyor
                guna2TextBox30.Enabled = false; // TC TextBox'u kapatılıyor.
                guna2TextBox30.Text = doktorDuzenTc; // Daha önceden kaydedilen veri yazdırılıyor.
                guna2Button42.Enabled = true; 
                guna2Button43.Enabled = true; // Düzenleme ve silme butonları aktif ediliyor.
            }
        }

        // Yönetici doktor listeleme ekranı verileri yenileme butonu.
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            try
            {
                // Doktorlar tekrardan DataGrid'e yazdrılıyor.
                QuerySnapshot querySnapshot = db.Collection("doktorlar").OrderBy("tc").GetSnapshotAsync().Result;
                guna2DataGridView3.Rows.Clear();
                guna2ComboBox11.SelectedIndex = 0;
                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    string doktortc = documentSnapshot.GetValue<string>("tc");
                    string ad = documentSnapshot.GetValue<string>("ad");
                    string soyad = documentSnapshot.GetValue<string>("soyad");
                    string dogumTarihi = documentSnapshot.GetValue<string>("dogumtarihi");
                    string cinsiyet = documentSnapshot.GetValue<string>("cinsiyet");
                    string polid = documentSnapshot.GetValue<string>("poliklinik");
                    DocumentReference document = db.Collection("poliklinikler").Document(polid);
                    DocumentSnapshot sorgu1 = document.GetSnapshotAsync().Result;
                    string poliklinik = sorgu1.GetValue<string>("poliklinikAdı");
                    string gsm = documentSnapshot.GetValue<string>("gsm");
                    string dogumid = documentSnapshot.GetValue<string>("dogumyeri");
                    DocumentReference document2 = db.Collection("sehirler").Document(dogumid);
                    DocumentSnapshot sorgu2 = document2.GetSnapshotAsync().Result;
                    string dogumYeri = sorgu2.GetValue<string>("SehirAdi");
                    string sifre = documentSnapshot.GetValue<string>("şifre");
                    guna2DataGridView3.Rows.Add(doktortc, ad, soyad, dogumTarihi, cinsiyet, poliklinik, gsm, dogumYeri, sifre);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region YÖNETİCİ DOKTOR DÜZENLEME TC TEXBOX

        private void guna2TextBox30_TextChanged(object sender, EventArgs e)
        {
            if (guna2TextBox30.Text.Length >= 11 && guna2TextBox30.Enabled == true)
            {   // Eğer veri girişi doğru ve aktif ise...
                DataGridViewRow secilenSatir = guna2DataGridView3.Rows
                    .Cast<DataGridViewRow>() // DataGrid'de yazılan TC arama yapılıyor.
                    .FirstOrDefault(row => row.Cells["TC"].Value.ToString() == guna2TextBox30.Text);
                if (secilenSatir != null)// Eğer veri bulunursa...
                {
                    guna2Button42.Enabled = true;
                    guna2Button43.Enabled = true; // Düzenleme ve silme butonları aktif ediliyor.
                    errorProvider1.SetError(guna2TextBox30, "");

                    doktorDuzenTc = secilenSatir.Cells["TC"].Value.ToString();
                    doktorDuzenAd = secilenSatir.Cells["adı"].Value.ToString();
                    doktorDuzenSoyad = secilenSatir.Cells["soyadı"].Value.ToString();
                    doktorDuzenDogumTarihi = secilenSatir.Cells["dogumTarihi"].Value.ToString();
                    doktorDuzenCinsiyet = secilenSatir.Cells["cinsiyet"].Value.ToString();
                    doktorDuzenDogumYeri = secilenSatir.Cells["dogumYeri"].Value.ToString();
                    doktorDuzenPol = secilenSatir.Cells["pol"].Value.ToString();
                    doktorDuzenGsm = secilenSatir.Cells["no"].Value.ToString();
                    doktorDuzenSifre = secilenSatir.Cells["sifre"].Value.ToString();
                    // Değişkenlere veri kaydediliyor

                    guna2TextBox30.Text = secilenSatir.Cells["TC"].Value.ToString();
                    guna2TextBox29.Text = secilenSatir.Cells["adı"].Value.ToString();
                    guna2TextBox28.Text = secilenSatir.Cells["soyadı"].Value.ToString();
                    guna2DateTimePicker1.Value = DateTime.ParseExact(secilenSatir.Cells["dogumTarihi"].Value.ToString(), "dd.MM.yyyy", null);
                    guna2ComboBox13.SelectedItem = secilenSatir.Cells["cinsiyet"].Value.ToString();
                    guna2ComboBox10.SelectedItem = secilenSatir.Cells["dogumYeri"].Value.ToString();
                    guna2ComboBox12.SelectedItem = secilenSatir.Cells["pol"].Value.ToString();
                    guna2TextBox22.Text = secilenSatir.Cells["no"].Value.ToString();
                    guna2TextBox27.Text = secilenSatir.Cells["sifre"].Value.ToString();
                    // TextBox'lar ve ComboBox'lar ilgili veriler ile dolduruluyor.
                }
                else
                {   // Doktor bulunamazsa hata veriliyor ve butonlar kapatılıyor.
                    errorProvider1.SetError(guna2TextBox30, "Doktor bulunamadı!");
                    guna2Button42.Enabled = false;
                    guna2Button43.Enabled = false;
                }
            }
            else if(guna2TextBox30.Enabled==false) 
            {   // TextBox aktif deilse veriler DataGridden geleceği için butonlar aktif ediliyor.
                guna2Button42.Enabled = true;
                guna2Button43.Enabled = true;
            }
            if(guna2TextBox30.Text.Length < 11)
            {   // Eğer TC biçimi doğru değilse butonlar kapatılıyor.
                guna2Button42.Enabled = false;
                guna2Button43.Enabled = false;
            }
            
            if (doktorDuzenTc != guna2TextBox30.Text) 
                // Daha önceden gönderilen veri ile eşit deilse icon aktif ediliyor.
                guna2TextBox30.IconRight = Properties.Resources._return;
            else
                guna2TextBox30.IconRight = null;
        }
        private void guna2TextBox30_IconRightClick(object sender, EventArgs e)
        {   // Aktif edilen icona basınca önceki veri TextBox'a yazdırılıyor.
            guna2TextBox30.Text = doktorDuzenTc;
        }
        
        private void guna2TextBox30_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void guna2TextBox30_Enter(object sender, EventArgs e)
        {
            if (guna2TextBox30.Text.Length < 11)
                errorProvider1.SetError(guna2TextBox30, "TC Kimlik Numarası 11 haneli olarak giriniz.");
        }

        private void guna2TextBox30_Leave(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox30, "");
        }
        #endregion

        #region YÖNETİCİ DOKTOR DÜZENLEME AD TEXTBOX
        private void guna2TextBox29_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox29, "");
        }

        private void guna2TextBox29_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox29.Text.Length < 3)
                errorProvider1.SetError(guna2TextBox29, "Yanlış ad girişi.");
        }
        private void guna2TextBox29_TextChanged(object sender, EventArgs e)
        {
            if (doktorDuzenAd != guna2TextBox29.Text)
                guna2TextBox29.IconRight = Properties.Resources._return;
            else
                guna2TextBox29.IconRight = null;
        }

        private void guna2TextBox29_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox29.Text = doktorDuzenAd;
        }

        #endregion
        
        #region YÖNETİCİ DOKTOR DÜZENLEME SOYAD TEXTBOX
        private void guna2TextBox28_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox28.Text.Length < 3)
                errorProvider1.SetError(guna2TextBox28, "Yanlış soyad girişi.");
        }

        private void guna2TextBox28_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox28, "");
        }
        private void guna2TextBox28_TextChanged(object sender, EventArgs e)
        {
            if (doktorDuzenSoyad != guna2TextBox28.Text)
                guna2TextBox28.IconRight = Properties.Resources._return;
            else
                guna2TextBox28.IconRight = null;
        }

        private void guna2TextBox28_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox28.Text = doktorDuzenSoyad;
        }

        #endregion

        #region YÖNETİCİ DOKTOR DÜZENLEME DOGUM TARİHİ DATEPICKER
        private void guna2DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {   // Değişen veri eski veri ile aynı deilse buton aktif ediliyor.
            if (DateTime.ParseExact(doktorDuzenDogumTarihi, "dd.MM.yyyy", null) != guna2DateTimePicker1.Value)
                guna2ImageButton1.Visible = true;
            else
                guna2ImageButton1.Visible = false;
        }

        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {   // Butona basınca eski veri yükleniyor.
            guna2DateTimePicker1.Value = DateTime.ParseExact(doktorDuzenDogumTarihi, "dd.MM.yyyy", null);
        }
        #endregion

        #region YÖNETİCİ DOKTOR DÜZENLEME CİNSİYET COMBOBOX
        private void guna2ComboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {   // Değişen veri eski veri ile aynı deilse buton aktif ediliyor.
            if (doktorDuzenCinsiyet != guna2ComboBox13.Text)
                guna2ImageButton2.Visible = true;
            else
                guna2ImageButton2.Visible = false;
        }
        
        private void guna2ImageButton2_Click(object sender, EventArgs e)
        {   // Butona basınca eski veri yükleniyor.
            guna2ComboBox13.Text = doktorDuzenCinsiyet;
        }
        #endregion

        #region YÖNETİCİ DOKTOR DÜZENLEME DOĞUM YERİ COMBOBOX
        private void guna2ComboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {   // Değişen veri eski veri ile aynı deilse buton aktif ediliyor.
            if (doktorDuzenDogumYeri != guna2ComboBox10.Text)
                guna2ImageButton3.Visible = true;
            else
                guna2ImageButton3.Visible = false;
        }

        private void guna2ImageButton3_Click(object sender, EventArgs e)
        {   // Butona basınca eski veri yükleniyor.
            guna2ComboBox10.Text = doktorDuzenDogumYeri;
        }
        #endregion

        #region YÖNETİCİ DOKTOR DUZENLEME POLİKLİNİK COMBOBOX
        private void guna2ComboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {   // Değişen veri eski veri ile aynı deilse buton aktif ediliyor.
            if (doktorDuzenPol != guna2ComboBox12.Text)
                guna2ImageButton4.Visible = true;
            else
                guna2ImageButton4.Visible = false;
        }

        private void guna2ImageButton4_Click(object sender, EventArgs e)
        {   // Butona basınca eski veri yükleniyor.
            guna2ComboBox12.Text = doktorDuzenPol;
        }
        #endregion

        #region YÖNETİCİ DOKTOR DÜZENLEME GSM TEXTBOX
        private void guna2TextBox22_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void guna2TextBox22_Validating(object sender, CancelEventArgs e)
        {
            if (!Regex.IsMatch(guna2TextBox22.Text, @"^0\d{10}$"))
                errorProvider1.SetError(guna2TextBox22, "Telefon numarası 0 ile başlamalıdır ve doğru girilmelidir.");
        }

        private void guna2TextBox22_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox22, "");
        }
        
        private void guna2TextBox22_TextChanged(object sender, EventArgs e)
        {
            if (doktorDuzenGsm != guna2TextBox22.Text)
                guna2TextBox22.IconRight = Properties.Resources._return;
            else
                guna2TextBox22.IconRight = null;
        }

        private void guna2TextBox22_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox22.Text = doktorDuzenGsm;
        }

        #endregion

        #region YÖNETİCİ DOKTOR DÜZENLEME ŞİFRE TEXTBOX
        private void guna2TextBox27_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox27.PasswordChar = guna2TextBox27.PasswordChar == '\0' ? '●' : '\0';
            if (guna2TextBox27.PasswordChar == '●')
                guna2TextBox27.IconRight = Properties.Resources.eyes;
            else
                guna2TextBox27.IconRight = Properties.Resources.eyes_off;
        }

        private void guna2TextBox27_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox27.Text.Length < 0)
                errorProvider1.SetError(guna2TextBox27, "Şifre boş bırakılamaz.");
        }

        private void guna2TextBox27_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox27, "");
        }
        private void guna2TextBox27_TextChanged(object sender, EventArgs e)
        {
            if (doktorDuzenSifre != guna2TextBox27.Text)
                guna2ImageButton5.Visible = true;
            else
                guna2ImageButton5.Visible = false;
        }

        private void guna2ImageButton5_Click(object sender, EventArgs e)
        {
            guna2TextBox27.Text = doktorDuzenSifre;
        }
        #endregion

        #endregion

        #region YÖNETİCİ HASTA GÖRÜNTÜLEME VE DÜZENLEME ERKANI BİLEŞENLERİ
        
        string hastaDuzenTc;
        string hastaDuzenAd;
        string hastaDuzenSoyad;
        string hastaDuzenDogumTarihi;
        string hastaDuzenDogumYeri;
        string hastaDuzenCinsiyet;
        string hastaDuzenKanGrubu;
        string hastaDuzenEmail;
        string hastaDuzenGsm;
        string hastaDuzenAdres;
        string hastaDuzenSifre;

        // Yönetici hasta görüntüleme ekranı doktorların listelendiği DataGrid veri seçimi olunca.
        private void guna2DataGridView4_SelectionChanged(object sender, EventArgs e)
        {
            if (guna2DataGridView4.SelectedRows.Count > 0)
            {
                errorProvider1.SetError(guna2TextBox35, "");
                errorProvider1.SetError(guna2TextBox34, "");
                errorProvider1.SetError(guna2TextBox33, "");
                errorProvider1.SetError(guna2TextBox36, "");
                errorProvider1.SetError(guna2TextBox31, "");
                errorProvider1.SetError(guna2TextBox37, "");
                errorProvider1.SetError(guna2TextBox32, "");// Yeni veri seçiminden dolayı hatalar sıfırlanıyor.
                DataGridViewRow secilenSatir = guna2DataGridView4.SelectedRows[0]; // Seçilen satırın verileri.

                hastaDuzenTc = secilenSatir.Cells["yonHastaTc"].Value.ToString();
                hastaDuzenAd = secilenSatir.Cells["yonHastaAd"].Value.ToString();
                hastaDuzenSoyad= secilenSatir.Cells["yonHastaSoyad"].Value.ToString();
                hastaDuzenDogumTarihi = secilenSatir.Cells["yonHastaDogumTarihi"].Value.ToString();
                hastaDuzenDogumYeri = secilenSatir.Cells["yonHastaDogumYeri"].Value.ToString();
                hastaDuzenCinsiyet = secilenSatir.Cells["yonHastaCinsiyet"].Value.ToString();
                hastaDuzenKanGrubu = secilenSatir.Cells["yonHastaKanGrubu"].Value.ToString();
                hastaDuzenEmail = secilenSatir.Cells["yonHastaEmail"].Value.ToString();
                hastaDuzenGsm = secilenSatir.Cells["yonHastaGsm"].Value.ToString();
                hastaDuzenAdres = secilenSatir.Cells["yonHastaAdres"].Value.ToString();
                hastaDuzenSifre = secilenSatir.Cells["yonHastaSifre"].Value.ToString();
                // Veriler değiştirilen metnin icona basınca geri yüklenebilmesi için değişkenlere gönderiliyor.

                guna2TextBox35.Text = secilenSatir.Cells["yonHastaTc"].Value.ToString();
                guna2TextBox34.Text = secilenSatir.Cells["yonHastaAd"].Value.ToString();
                guna2TextBox33.Text = secilenSatir.Cells["yonHastaSoyad"].Value.ToString();
                guna2DateTimePicker2.Value = DateTime.ParseExact(secilenSatir.Cells["yonHastaDogumTarihi"].Value.ToString(), "dd.MM.yyyy", null);
                guna2ComboBox14.SelectedItem = secilenSatir.Cells["yonHastaDogumYeri"].Value.ToString();
                guna2ComboBox16.SelectedItem = secilenSatir.Cells["yonHastaCinsiyet"].Value.ToString();
                guna2ComboBox15.SelectedItem = secilenSatir.Cells["yonHastaKanGrubu"].Value.ToString();
                guna2TextBox36.Text = secilenSatir.Cells["yonHastaEmail"].Value.ToString();
                guna2TextBox31.Text = secilenSatir.Cells["yonHastaGsm"].Value.ToString();
                guna2TextBox37.Text = secilenSatir.Cells["yonHastaAdres"].Value.ToString();
                guna2TextBox32.Text = secilenSatir.Cells["yonHastaSifre"].Value.ToString();
                // DataGrid'deki veriler TextBox ve ComboBox'lara gönderiliyor.
            }
        }

        // Yönetici hasta görüntülemme ekranı doktoru düzenle butonu.
        private void guna2Button108_Click(object sender, EventArgs e)
        {
            if (guna2TextBox35.Text.Length < 11)
                errorProvider1.SetError(guna2TextBox35, "TC Kimlik Numarası 11 haneli olarak giriniz.");
            if (guna2TextBox34.Text.Length < 3)
                errorProvider1.SetError(guna2TextBox34, "Yanlış ad girişi.");
            if (guna2TextBox33.Text.Length < 3)
                errorProvider1.SetError(guna2TextBox3, "Yanlış soyad girişi.");
            if (!Regex.IsMatch(guna2TextBox31.Text, @"^0\d{10}$"))
                errorProvider1.SetError(guna2TextBox31, "Telefon numarası 0 ile başlamalıdır ve doğru girilmelidir.");
            if (!Regex.IsMatch(guna2TextBox36.Text, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
                errorProvider1.SetError(guna2TextBox36, "Geçerli bir e-posta adresi giriniz.");
            if (guna2TextBox32.Text.Length < 1)
                errorProvider1.SetError(guna2TextBox32, "Şifre boş bırakılamaz.");
            // Metin girişlerinde hata varmı kontrol ediliyor.
            if (guna2TextBox35.Text.Length < 11 || guna2TextBox34.Text.Length < 3 || guna2TextBox33.Text.Length < 3 ||
                !Regex.IsMatch(guna2TextBox31.Text, @"^0\d{10}$") ||
                !Regex.IsMatch(guna2TextBox36.Text, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$") || guna2TextBox32.Text.Length < 0)
                return;

            try
            {
                // İşlem yapılacak hastanın veri tabanındaki dökümanını getirme.
                DocumentReference dokuman = db.Collection("hastalar").Document(guna2TextBox35.Text);
                DocumentSnapshot sorgu = dokuman.GetSnapshotAsync().Result;

                QuerySnapshot sehirid = db.Collection("sehirler")
                    .WhereEqualTo("SehirAdi", guna2ComboBox14.Text) 
                    .GetSnapshotAsync().Result;

                if (sorgu.Exists) // Yazılan doktor TC'sine göre döküman bulunursa...
                {
                    var hasta = new
                    {
                        ad = guna2TextBox34.Text,
                        soyad = guna2TextBox33.Text,
                        cinsiyet = guna2ComboBox16.Text,
                        dogumyeri = sehirid.Documents[0].Id,
                        dogumtarihi = guna2DateTimePicker2.Value.ToString("dd.MM.yyy"),
                        kangrubu = guna2ComboBox15.Text,
                        gsm = guna2TextBox31.Text,
                        email = guna2TextBox36.Text,
                        adres = guna2TextBox37.Text,
                        şifre = guna2TextBox32.Text
                    };
                    dokuman.SetAsync(hasta, SetOptions.MergeAll); // Veri tabanına veirler gönderiliyor.
                    hastaDuzenAd = guna2TextBox34.Text;
                    hastaDuzenSoyad = guna2TextBox33.Text;
                    hastaDuzenCinsiyet = guna2ComboBox16.Text;
                    hastaDuzenDogumYeri = guna2ComboBox14.Text;
                    hastaDuzenDogumTarihi = guna2DateTimePicker2.Value.ToString("dd.MM.yyy");
                    hastaDuzenKanGrubu = guna2ComboBox15.Text;
                    hastaDuzenGsm = guna2TextBox31.Text;
                    hastaDuzenEmail = guna2TextBox36.Text;
                    hastaDuzenAdres = guna2TextBox37.Text;
                    hastaDuzenSifre = guna2TextBox32.Text;
                    // Veriler değişkenler kaydediliyor.

                    foreach (DataGridViewRow row in guna2DataGridView4.Rows)
                    {   // Veriler veri tabanına gönderildikten sonra güncelleme işlemi DataGrid'de yapılıyor.
                        if (row.Cells["yonHastaTc"].Value.ToString() == guna2TextBox35.Text)
                        {
                            row.Cells["yonHastaAd"].Value = guna2TextBox34.Text;
                            row.Cells["yonHastaSoyad"].Value = guna2TextBox33.Text;
                            row.Cells["yonHastaDogumTarihi"].Value = guna2DateTimePicker2.Value.ToString("dd.MM.yyyy");
                            row.Cells["yonHastaDogumYeri"].Value = guna2ComboBox14.SelectedItem.ToString();
                            row.Cells["yonHastaCinsiyet"].Value = guna2ComboBox16.SelectedItem.ToString();
                            row.Cells["yonHastaKanGrubu"].Value = guna2ComboBox15.SelectedItem.ToString();
                            row.Cells["yonHastaEmail"].Value = guna2TextBox36.Text;
                            row.Cells["yonHastaGsm"].Value = guna2TextBox31.Text;
                            row.Cells["yonHastaAdres"].Value = guna2TextBox37.Text;
                            row.Cells["yonHastaSifre"].Value = guna2TextBox32.Text;
                            break;
                        }
                    }

                    MessageBox.Show("Veri başarıyla Firestore'a kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Kullanıcı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Firestore'a veri eklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Yönetici hasta görüntülemme ekranı doktoru silme butonu.
        private void guna2Button109_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView4.SelectedRows.Count <= 1)
            {
                try
                {
                    DataGridViewRow secilenSatir = guna2DataGridView4.Rows
                        .Cast<DataGridViewRow>() // DataGrid'den veri aranıyor değişkene yazılıyor.
                        .FirstOrDefault(row => row.Cells["yonHastaTc"].Value.ToString() == guna2TextBox35.Text);

                    if (secilenSatir != null) // Veri bulunursa...
                    {
                        DialogResult result = MessageBox.Show($"{hastaDuzenAd} {hastaDuzenSoyad} adlı hastayı silmek istediğine emin misin?",
                            "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            DocumentReference documentRef = db.Collection("hastalar").Document(hastaDuzenTc);
                            documentRef.DeleteAsync().Wait(); // Veri tabanından siliniyor.

                            guna2DataGridView4.Rows.Remove(secilenSatir); // DataGrid'den siliniyor.
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata oluştu", ex.Message);
                }
            }
            else
                MessageBox.Show("Birden fazla doktor seçilemez.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Yönetici hasta listeleme ekrnaı hasta filtreleme TextBox.
        private void guna2TextBox38_TextChanged(object sender, EventArgs e)
        {
            string[] searchTerms = guna2TextBox38.Text.Split(' '); // Metin parçalara ayrılıyor.

            foreach (DataGridViewRow row in guna2DataGridView4.Rows)
            {   // Satırlar döngüye sokuluyor.
                bool match = true; // Her satır için başlangıçta eşleşme var olarak kabul ediliyor.

                // Her bir arama terimi için döngü oluşturuluyor.
                foreach (string term in searchTerms)
                {
                    // Satırdaki "yonHastaAd" ve "yonHastaSoyad" hücrelerinde arama teriminin olup olmadığı kontrol ediliyor.
                    if (!row.Cells["yonHastaAd"].Value.ToString().ToLower().Contains(term) && !row.Cells["yonHastaSoyad"].Value.ToString().ToLower().Contains(term))
                    {
                        match = false; // Eğer arama terimi hücrelerde bulunmazsa eşleşme durumu false olarak ayarlanıyor.
                        break; // Döngüden çıkılıyor.
                    }
                }

                // Eğer TextBox içeriği boşsa veya eşleşme durumu true ise satır görünür hale getiriliyor. Aksi halde satır gizleniyor.
                row.Visible = string.IsNullOrEmpty(guna2TextBox38.Text) || match;
            }
        }

        // Yönetici hasta listeleme ekranı TC no yu elle yazmak için kullanılan CheckBox.
        private void guna2CheckBox2_CheckedChanged(object sender, EventArgs e)
        {   // Eğer seçilirse TextBox aktif ediliyor.
            if (guna2CheckBox2.Checked)
            {
                guna2TextBox35.Enabled = true;
            }
            else
            {   // Seçilmezse değişkendeki veri yazdırılıyor butonlar aktif ediliyor
                errorProvider1.SetError(guna2TextBox35, "");
                guna2TextBox35.Enabled = false;
                guna2TextBox35.Text = hastaDuzenTc;
                guna2Button108.Enabled = true;
                guna2Button109.Enabled = true;
            }
        }

        // Yönetici hasta listeleme ekranı verileri yenileme butonu.
        private void guna2Button5_Click(object sender, EventArgs e)
        {
            try
            {
                QuerySnapshot querySnapshot = db.Collection("hastalar").OrderBy("tc").GetSnapshotAsync().Result;
                guna2DataGridView4.Rows.Clear();
                guna2TextBox38.Text = "";
                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    string hastatc = documentSnapshot.GetValue<string>("tc");
                    string ad = documentSnapshot.GetValue<string>("ad");
                    string soyad = documentSnapshot.GetValue<string>("soyad");
                    string dogumTarihi = documentSnapshot.GetValue<string>("dogumtarihi");
                    string dogumid = documentSnapshot.GetValue<string>("dogumyeri");
                    DocumentReference document2 = db.Collection("sehirler").Document(dogumid);
                    DocumentSnapshot sorgu2 = document2.GetSnapshotAsync().Result;
                    string dogumYeri = sorgu2.GetValue<string>("SehirAdi");
                    string cinsiyet = documentSnapshot.GetValue<string>("cinsiyet");
                    string kangrubu = documentSnapshot.GetValue<string>("kangrubu");
                    string email = documentSnapshot.GetValue<string>("email");
                    string adres = documentSnapshot.GetValue<string>("adres");
                    string gsm = documentSnapshot.GetValue<string>("gsm");
                    string sifre = documentSnapshot.GetValue<string>("şifre");
                    guna2DataGridView4.Rows.Add(hastatc, ad, soyad, dogumTarihi, dogumYeri, cinsiyet, kangrubu, email, adres, gsm, sifre);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #region YÖNETİCİ HASTA DÜZENLEME TC TEXTBOX
        private void guna2TextBox35_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void guna2TextBox35_Enter(object sender, EventArgs e)
        {
            if (guna2TextBox35.Text.Length < 11)
                errorProvider1.SetError(guna2TextBox35, "TC Kimlik Numarası 11 haneli olarak giriniz.");
        }

        private void guna2TextBox35_Leave(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox35, "");
        }

        private void guna2TextBox35_TextChanged(object sender, EventArgs e)
        {
            if (guna2TextBox35.Text.Length >= 11 && guna2TextBox35.Enabled == true)
            {
                DataGridViewRow secilenSatir = guna2DataGridView4.Rows
                    .Cast<DataGridViewRow>()
                    .FirstOrDefault(row => row.Cells["yonHastaTc"].Value.ToString() == guna2TextBox35.Text);
                if (secilenSatir != null)
                {
                    guna2Button108.Enabled = true;
                    guna2Button109.Enabled = true;
                    errorProvider1.SetError(guna2TextBox35, "");

                    hastaDuzenTc = secilenSatir.Cells["yonHastaTc"].Value.ToString();
                    hastaDuzenAd = secilenSatir.Cells["yonHastaAd"].Value.ToString();
                    hastaDuzenSoyad = secilenSatir.Cells["yonHastaSoyad"].Value.ToString();
                    hastaDuzenDogumTarihi = secilenSatir.Cells["yonHastaDogumTarihi"].Value.ToString();
                    hastaDuzenDogumYeri = secilenSatir.Cells["yonHastaDogumYeri"].Value.ToString();
                    hastaDuzenCinsiyet = secilenSatir.Cells["yonHastaCinsiyet"].Value.ToString();
                    hastaDuzenKanGrubu = secilenSatir.Cells["yonHastaKanGrubu"].Value.ToString();
                    hastaDuzenEmail = secilenSatir.Cells["yonHastaEmail"].Value.ToString();
                    hastaDuzenGsm = secilenSatir.Cells["yonHastaGsm"].Value.ToString();
                    hastaDuzenAdres = secilenSatir.Cells["yonHastaAdres"].Value.ToString();
                    hastaDuzenSifre = secilenSatir.Cells["yonHastaSifre"].Value.ToString();

                    guna2TextBox35.Text = secilenSatir.Cells["yonHastaTc"].Value.ToString();
                    guna2TextBox34.Text = secilenSatir.Cells["yonHastaAd"].Value.ToString();
                    guna2TextBox33.Text = secilenSatir.Cells["yonHastaSoyad"].Value.ToString();
                    guna2DateTimePicker2.Value = DateTime.ParseExact(secilenSatir.Cells["yonHastaDogumTarihi"].Value.ToString(), "dd.MM.yyyy", null);
                    guna2ComboBox14.SelectedItem = secilenSatir.Cells["yonHastaDogumYeri"].Value.ToString();
                    guna2ComboBox16.SelectedItem = secilenSatir.Cells["yonHastaCinsiyet"].Value.ToString();
                    guna2ComboBox15.SelectedItem = secilenSatir.Cells["yonHastaKanGrubu"].Value.ToString();
                    guna2TextBox36.Text = secilenSatir.Cells["yonHastaEmail"].Value.ToString();
                    guna2TextBox31.Text = secilenSatir.Cells["yonHastaGsm"].Value.ToString();
                    guna2TextBox37.Text = secilenSatir.Cells["yonHastaAdres"].Value.ToString();
                    guna2TextBox32.Text = secilenSatir.Cells["yonHastaSifre"].Value.ToString();

                }
                else
                {
                    errorProvider1.SetError(guna2TextBox35, "Doktor bulunamadı!");
                }
            }
            else if (guna2TextBox35.Enabled == false)
            {
                guna2Button108.Enabled = true;
                guna2Button109.Enabled = true;
            }
            if (guna2TextBox35.Text.Length < 11)
            {
                guna2Button108.Enabled = false;
                guna2Button109.Enabled = false;
            }

            if (hastaDuzenTc != guna2TextBox35.Text)
                guna2TextBox35.IconRight = Properties.Resources._return;
            else
                guna2TextBox35.IconRight = null;
        }

        private void guna2TextBox35_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox35.Text = hastaDuzenTc;
        }
        #endregion

        #region YÖNETİCİ HASTA DÜZENLEME AD TEXTBOX
        private void guna2TextBox34_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox34, "");
        }

        private void guna2TextBox34_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox34.Text.Length < 3)
                errorProvider1.SetError(guna2TextBox34, "Yanlış ad girişi.");
        }

        private void guna2TextBox34_TextChanged(object sender, EventArgs e)
        {
            if (hastaDuzenAd != guna2TextBox34.Text)
                guna2TextBox34.IconRight = Properties.Resources._return;
            else
                guna2TextBox34.IconRight = null;
        }

        private void guna2TextBox34_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox34.Text= hastaDuzenAd;
        }
        #endregion

        #region YÖNETİCİ HASTA DÜZENLEME SOYAD TEXTBOX
        private void guna2TextBox33_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox33, "");
        }

        private void guna2TextBox33_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox33.Text.Length < 3)
                errorProvider1.SetError(guna2TextBox33, "Yanlış ad girişi.");
        }

        private void guna2TextBox33_TextChanged(object sender, EventArgs e)
        {
            if (hastaDuzenSoyad!= guna2TextBox33.Text)
                guna2TextBox33.IconRight = Properties.Resources._return;
            else
                guna2TextBox33.IconRight = null;
        }

        private void guna2TextBox33_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox33.Text= hastaDuzenSoyad;
        }
        #endregion

        #region YÖNETİCİ HASTA DÜZENLEME DOGUM TARİHİ DATEPICKER
        private void guna2DateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (DateTime.ParseExact(hastaDuzenDogumTarihi, "dd.MM.yyyy", null) != guna2DateTimePicker2.Value)
                guna2ImageButton10.Visible = true;
            else
                guna2ImageButton10.Visible = false;
        }

        private void guna2ImageButton10_Click(object sender, EventArgs e)
        {
            guna2DateTimePicker2.Value = DateTime.ParseExact(hastaDuzenDogumTarihi, "dd.MM.yyyy", null);
        }
        #endregion

        #region YÖNETİCİ HASTA DÜZENLEME CİNSİYET COMBOBOX
        private void guna2ComboBox16_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hastaDuzenCinsiyet != guna2ComboBox16.Text)
                guna2ImageButton9.Visible = true;
            else
                guna2ImageButton9.Visible = false;
        }

        private void guna2ImageButton9_Click(object sender, EventArgs e)
        {
            guna2ComboBox16.Text = hastaDuzenCinsiyet;
        }
        #endregion

        #region YÖNETİCİ HASTA DÜZENLEME DOGUM YERİ COMBOBOX
        private void guna2ComboBox14_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hastaDuzenDogumYeri != guna2ComboBox14.Text)
                guna2ImageButton8.Visible = true;
            else
                guna2ImageButton8.Visible = false;

        }

        private void guna2ImageButton8_Click(object sender, EventArgs e)
        {
            guna2ComboBox14.Text = hastaDuzenDogumYeri;
        }
        #endregion

        #region YÖNETİCİ HASTA DÜZENLEME KAN GRUBU
        private void guna2ComboBox15_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hastaDuzenKanGrubu != guna2ComboBox15.Text)
                guna2ImageButton7.Visible = true;
            else
                guna2ImageButton7.Visible = false;
        }

        private void guna2ImageButton7_Click(object sender, EventArgs e)
        {
            guna2ComboBox15.Text = hastaDuzenKanGrubu;
        }
        #endregion

        #region YÖNETİCİ HASTA DÜZENLEME EMAİL TEXTBOX
        private void guna2TextBox36_Validating(object sender, CancelEventArgs e)
        {
            if (!Regex.IsMatch(guna2TextBox36.Text, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
            {
                // Eğer e-posta geçerli değilse, geçersiz e-posta hatası göster
                errorProvider1.SetError(guna2TextBox36, "Geçerli bir e-posta adresi giriniz.");
            }
        }

        private void guna2TextBox36_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox36, "");
        }

        private void guna2TextBox36_TextChanged(object sender, EventArgs e)
        {
            if (hastaDuzenEmail != guna2TextBox36.Text)
                guna2TextBox36.IconRight = Properties.Resources._return;
            else
                guna2TextBox36.IconRight = null;
        }

        private void guna2TextBox36_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox36.Text = hastaDuzenEmail;
        }
        #endregion
        
        #region YÖNETİCİ HASTA DÜZENLEME GSM TEXTBOX
        private void guna2TextBox31_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void guna2TextBox31_Validating(object sender, CancelEventArgs e)
        {
            string phoneNumber = guna2TextBox31.Text;

            // Telefon numarası için basit bir kontrol yapısı
            // Bu kontrolü gereksinimlerinize göre genişletebilirsiniz
            if (!Regex.IsMatch(phoneNumber, @"^0\d{10}$"))
                errorProvider1.SetError(guna2TextBox31, "Telefon numarası 0 ile başlamalıdır ve doğru girilmelidir.");
        }

        private void guna2TextBox31_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox31, "");
        }

        private void guna2TextBox31_TextChanged(object sender, EventArgs e)
        {
            if (hastaDuzenGsm != guna2TextBox31.Text)
                guna2TextBox31.IconRight = Properties.Resources._return;
            else
                guna2TextBox31.IconRight = null;
        }

        private void guna2TextBox31_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox31.Text = hastaDuzenGsm;
        }
        #endregion

        #region YÖNETİCİ HASTA DÜZENLEME ADRES TEXTBOX
        private void guna2TextBox37_TextChanged(object sender, EventArgs e)
        {
            if (hastaDuzenAdres != guna2TextBox37.Text)
                guna2TextBox37.IconRight = Properties.Resources._return;
            else
                guna2TextBox37.IconRight = null;
        }

        private void guna2TextBox37_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox37.Text = hastaDuzenAdres;
        }
        #endregion

        #region YÖNETİCİ HASTA DÜZENLEME ŞİFRE TEXTBOX
        private void guna2TextBox32_Enter(object sender, EventArgs e)
        {
            errorProvider1.SetError(guna2TextBox23, "");
        }

        private void guna2TextBox32_TextChanged(object sender, EventArgs e)
        {
            if (hastaDuzenSifre != guna2TextBox32.Text)
                guna2ImageButton6.Visible = true;
            else
                guna2ImageButton6.Visible = false;
        }

        private void guna2TextBox32_IconRightClick(object sender, EventArgs e)
        {
            guna2TextBox32.PasswordChar = guna2TextBox32.PasswordChar == '\0' ? '●' : '\0';
            if (guna2TextBox32.PasswordChar == '●')
                guna2TextBox32.IconRight = Properties.Resources.eyes;
            else
                guna2TextBox32.IconRight = Properties.Resources.eyes_off;
        }

        private void guna2TextBox32_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox32.Text.Length < 0)
                errorProvider1.SetError(guna2TextBox32, "Şifre boş bırakılamaz.");
        }

        private void guna2ImageButton6_Click(object sender, EventArgs e)
        {
            guna2TextBox32.Text = hastaDuzenSifre;
        }
        #endregion

        #endregion

        #region ARAYÜZ BUTONLARI
        private void guna2Button110_Click(object sender, EventArgs e)
        {   // Yönetici doktor görüntüleme ekranında menüden hasta ekranına geçince DataGrid doldurma kodları.
            try
            {
                QuerySnapshot querySnapshot = db.Collection("hastalar").OrderBy("tc").GetSnapshotAsync().Result;
                guna2DataGridView4.Rows.Clear();
                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    string hastatc = documentSnapshot.GetValue<string>("tc");
                    string ad = documentSnapshot.GetValue<string>("ad");
                    string soyad = documentSnapshot.GetValue<string>("soyad");
                    string dogumTarihi = documentSnapshot.GetValue<string>("dogumtarihi");
                    string dogumid = documentSnapshot.GetValue<string>("dogumyeri");
                    DocumentReference document2 = db.Collection("sehirler").Document(dogumid);
                    DocumentSnapshot sorgu2 = document2.GetSnapshotAsync().Result;
                    string dogumYeri = sorgu2.GetValue<string>("SehirAdi");
                    string cinsiyet = documentSnapshot.GetValue<string>("cinsiyet");
                    string kangrubu = documentSnapshot.GetValue<string>("kangrubu");
                    string email = documentSnapshot.GetValue<string>("email");
                    string adres = documentSnapshot.GetValue<string>("adres");
                    string gsm = documentSnapshot.GetValue<string>("gsm");
                    string sifre = documentSnapshot.GetValue<string>("şifre");
                    guna2DataGridView4.Rows.Add(hastatc, ad, soyad, dogumTarihi, dogumYeri, cinsiyet, kangrubu, email, adres, gsm, sifre);
                }
                bunifuPages1.SelectedTab = tabPage7;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2Button72_Click(object sender, EventArgs e)
        {
            // Doktor randevularını gördüğü ekrana giderken çalışan kodlar.
            List<(string id, string hastaAd, string randevuTarihi, string randevuSaati, string randevuDurumu)> belgeBilgileri =
                new List<(string id, string hastaAd, string randevuTarihi, string randevuSaati, string randevuDurumu)>();
            try
            {
                QuerySnapshot querySnapshot = db.Collection("randevular")
                    .WhereEqualTo("doktorTC", girisDurum)
                    .GetSnapshotAsync().Result; // Doktorun hastalarının bilgilerini sorguya aktarılıyor.

                // Her bir dökümanı listeye ekle 
                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    string id = documentSnapshot.Id;
                    string hastaTC = documentSnapshot.GetValue<string>("hastaTC");
                    DocumentReference document = db.Collection("hastalar").Document(hastaTC);
                    DocumentSnapshot sorgu1 = document.GetSnapshotAsync().Result;
                    string hastaAd = sorgu1.GetValue<string>("ad") + " " + sorgu1.GetValue<string>("soyad");
                    string randevuTarihi = documentSnapshot.GetValue<string>("randevuTarihi");
                    string randevuDurumu = documentSnapshot.GetValue<string>("randevuDurumu");
                    string randevuSaati = documentSnapshot.GetValue<string>("randevuSaati");
                    belgeBilgileri.Add((id, hastaAd, randevuTarihi, randevuSaati, randevuDurumu));
                }

                // Koleksiyonu id ye göre sırala.
                belgeBilgileri.Sort((x, y) => int.Parse(x.id).CompareTo(int.Parse(y.id)));

                // Sıralanmış koleksiyonu kullanarak DataGridView'e ekleyin 
                foreach (var belge in belgeBilgileri)
                {   // Listedeki veriler döngü ile DataGrid'e aktarılıyor.
                    guna2DataGridView2.Rows.Add(belge.id, belge.hastaAd, belge.randevuTarihi, belge.randevuSaati, belge.randevuDurumu);
                }
                bunifuPages1.SelectedTab = tabPage3;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2Button65_Click(object sender, EventArgs e)
        {
            DocumentReference dokuman = db.Collection("doktorlar").Document(girisDurum);
            DocumentSnapshot sorgu = dokuman.GetSnapshotAsync().Result;

            // Doktor bilgi güncelleme ekranına basınca veri tabanından verileri getirilip değişkenlere yazılıyor

            gsmGuncelle = sorgu.GetValue<string>("gsm");
            guna2TextBox26.Text = gsmGuncelle;
            bunifuPages1.SelectedTab = tabPage6;
        }

        private void guna2Button59_Click(object sender, EventArgs e)
        {
            DocumentReference dokuman = db.Collection("hastalar").Document(girisDurum);
            DocumentSnapshot sorgu = dokuman.GetSnapshotAsync().Result;

            // Hasta bilgi güncelleme ekranına basınca veri tabanından verileri getirilip değişkenlere yazılıyor

            gsmGuncelle = sorgu.GetValue<string>("gsm");
            emailGuncelle = sorgu.GetValue<string>("email");
            adresGuncelle = sorgu.GetValue<string>("adres");
            guna2TextBox13.Text = gsmGuncelle;
            guna2TextBox14.Text = emailGuncelle;
            guna2TextBox15.Text = adresGuncelle;
            bunifuPages1.SelectedTab = tabPage12;
        }

        private void guna2Button57_Click(object sender, EventArgs e)
        {
            DocumentReference dokuman = db.Collection("hastalar").Document(girisDurum);
            DocumentSnapshot sorgu = dokuman.GetSnapshotAsync().Result;

            // Hasta bilgi güncelleme ekranına basınca veri tabanından verileri getirilip değişkenlere yazılıyor

            gsmGuncelle = sorgu.GetValue<string>("gsm");
            emailGuncelle = sorgu.GetValue<string>("email");
            adresGuncelle = sorgu.GetValue<string>("adres");
            guna2TextBox13.Text = gsmGuncelle;
            guna2TextBox14.Text = emailGuncelle;
            guna2TextBox15.Text = adresGuncelle;
            bunifuPages1.SelectedTab = tabPage12;
        }

        private void guna2Button63_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage2;
        }

        private void guna2Button81_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage5;
        }

        private void guna2Button60_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage4;
        }

        private void guna2Button58_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage8;
        }

        private void guna2Button64_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage8;
        }

        private void guna2Button70_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage8;
        }

        private void guna2Button75_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage8;
        }

        private void guna2Button77_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage4;
        }

        private void guna2Button78_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage2;
        }

        private void guna2Button73_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage8;
        }

        private void guna2Button106_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage13;
        }

        private void guna2Button107_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage5;
        }

        private void guna2Button105_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage8;
        }

        private void guna2Button79_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage8;
        }

        private void guna2Button74_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage8;
        }

        private void guna2Button66_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage8;
        }

        private void guna2Button56_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectedTab = tabPage8;
        }

        private void guna2Button88_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button87_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button86_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button85_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button84_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button83_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button82_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button90_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button91_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button92_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button104_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion
    }
}