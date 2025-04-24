using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Yeni_Deneme
{
    public partial class Form2 : Form
    {
        private string filePath; // Seçilen dosyanın yolu
        private string[] satirlar;   // Dosyadaki satırlar
        private string[] kelimeVerisi; // Şu anki kelimenin verisi (kelime ve anlamları)
        private int currentKelimeIndex = 0; // Şu anki kelimenin indexi
        private int puan = 0; // Kullanıcı puanı
        private bool cevapSecildi = false; // İlk cevap seçildi mi?
        private Timer soruTimer;
        private int kalanSure = 10;
        private bool ilkDeneme = true;

        public Form2(string dosya)
        {
            InitializeComponent();

            soruTimer = new Timer();
            soruTimer.Interval = 1000;
            soruTimer.Tick += SoruTimer_Tick;
            bttnSecenek1.Click += btnSecenek_Click;
            bttnSecenek2.Click += btnSecenek_Click;
            bttnSecenek3.Click += btnSecenek_Click;
            bttnSecenek4.Click += btnSecenek_Click;
            this.Load += new EventHandler(Form2_Load);
            filePath = dosya; // Form1'den gelen dosya yolunu alıyoruz

        }

        private Random rnd = new Random();

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                satirlar = File.ReadAllLines(filePath);
                lblPuan.Text = $"Puan: {puan}";
                currentKelimeIndex = 0;
                GetNextKelime();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void SoruTimer_Tick(object sender, EventArgs e)
        {
            kalanSure--;
            lblSure.Text = $"Süre: {kalanSure} sn";

            if (kalanSure == 0)
            {
                soruTimer.Stop();
                MessageBox.Show("Süre doldu! Yeni kelime getiriliyor.");
                currentKelimeIndex++;
                GetNextKelime();
            }
        }

        private void GetNextKelime()
        {
            if (currentKelimeIndex >= satirlar.Length)
            {
                MessageBox.Show("Tüm kelimeler bitti!");
                return;
            }

            ilkDeneme = true;
            cevapSecildi = false;

            kelimeVerisi = satirlar[currentKelimeIndex].Split('\t');

            if (kelimeVerisi.Length < 2)
            {
                currentKelimeIndex++;
                GetNextKelime();
                return;
            }

            string kelime = kelimeVerisi[0];
            string[] anlamlar = kelimeVerisi[1].Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            if (anlamlar.Length < 1)
            {
                currentKelimeIndex++;
                GetNextKelime();
                return;
            }

            
            string dogruCevap = anlamlar[rnd.Next(anlamlar.Length)];

           
            var digerAnlamlar = satirlar
                .Where((s, i) => i != currentKelimeIndex)
                .SelectMany(s =>
                {
                    var split = s.Split('\t');
                    if (split.Length > 1)
                    {
                        return split[1].Split(',').Select(x => x.Trim());
                    }
                    return Array.Empty<string>();
                })
                .Where(a => !string.IsNullOrWhiteSpace(a) && a != dogruCevap)
                .Distinct()
                .OrderBy(x => rnd.Next())
                .Take(10)
                .ToList();

            var secenekler = digerAnlamlar.Take(3).ToList();

            while (secenekler.Count < 3)
            {
                secenekler.Add("?");
            }

         
            secenekler.Add(dogruCevap);

       
            secenekler = secenekler.OrderBy(x => rnd.Next()).ToList();

            
            var butonlar = new Button[] { bttnSecenek1, bttnSecenek2, bttnSecenek3, bttnSecenek4 };
            for (int i = 0; i < butonlar.Length; i++)
            {
                butonlar[i].Text = secenekler[i];
                butonlar[i].BackColor = DefaultBackColor;
            }

            for (int i = 0; i < butonlar.Length; i++)
            {
                butonlar[i].Text = secenekler[i];
                butonlar[i].BackColor = DefaultBackColor;
                butonlar[i].Image = null; 
            }

            lblSoru.Text = kelime;
            lblKelimeSayaci.Text = $"{currentKelimeIndex + 1}/{satirlar.Length}";
            kalanSure = 10;
            lblSure.Text = $"Süre: {kalanSure} sn";
            soruTimer.Start();
        }

        private void btnSecenek_Click(object sender, EventArgs e)
        {
            if (kelimeVerisi == null || kelimeVerisi.Length < 2 || cevapSecildi)
                return;

            var buton = (Button)sender;
            string secilenCevap = buton.Text;
            string dogruCevap = kelimeVerisi[1].Split(',')[0].Trim();

            if (secilenCevap == dogruCevap)
            {

                buton.BackColor = Color.LightGreen;
                buton.Text += " ✔";


                if (ilkDeneme)
                {
                    puan += 10;
                    lblPuan.Text = $"Puan: {puan}";
                    MessageBox.Show("Doğru cevap! (İlk denemede)");
                }
                else
                {
                    MessageBox.Show("Doğru cevap ama ilk denemede değil. Puan yok!");
                }

                cevapSecildi = true;
                soruTimer.Stop();
                currentKelimeIndex++;
               
                this.BeginInvoke(new Action(() =>
                {
                    System.Threading.Thread.Sleep(500);
                    GetNextKelime();
                }));
            }
            else
            {
                buton.BackColor = Color.IndianRed;
                buton.Text += " ❌";

                MessageBox.Show("Yanlış cevap! Tekrar deneyin.");
                ilkDeneme = false; 
            }
        }
    }

    
}
