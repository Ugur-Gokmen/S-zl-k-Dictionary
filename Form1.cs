using System;
using System.IO;
using System.Windows.Forms;

namespace Yeni_Deneme
{
    public partial class Form1 : Form
    {
        public string selectedFile;// Seçilen dosyanın yolu
        public string secilenDeger; // Seçilen dosya türü

        public Form1()
        {
            InitializeComponent();

            // Buton olaylarını bağla
            this.btnYukle.Click += new EventHandler(this.btnYukle_Click);
            this.btnBaslat.Click += new EventHandler(this.btnBaslat_Click);
            this.btnCikis.Click += new EventHandler(this.btnCikis_Click);

            this.cmbboxDosya_Sec.SelectedIndexChanged += new EventHandler(this.cmbboxDosya_Sec_SelectedIndexChanged);
            // Form1_Load metodunu Load olayına bağla
            this.Load += new EventHandler(Form1_Load);

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            cmbboxDosya_Sec.Items.Add("YOKDIL Sosyal Bilimler");
            cmbboxDosya_Sec.Items.Add("YOKDIL Fen Bilimleri");
            btnBaslat.Enabled = false; // Başlat butonunu başlangıçta devre dışı bırak

        }

        private void cmbboxDosya_Sec_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbboxDosya_Sec.SelectedItem != null)
            {
                secilenDeger = cmbboxDosya_Sec.SelectedItem.ToString();

                try
                {
                    string dosyaYolu = @"C:\"; // Dosyaların bulunduğu klasörün yolu
                    string[] dosyalar = Directory.GetFiles(dosyaYolu, secilenDeger + "*.txt");

                    // Mevcut seçim dışındaki dosyaları ekleyin
                    foreach (var dosya in dosyalar)
                    {
                        string dosyaAdi = Path.GetFileName(dosya);
                        if (!cmbboxDosya_Sec.Items.Contains(dosyaAdi))
                        {
                            cmbboxDosya_Sec.Items.Add(dosyaAdi);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        private void btnYukle_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Metin Dosyaları (*.txt)|*.txt";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFile = openFileDialog.FileName;
                secilenDeger = Path.GetFileNameWithoutExtension(selectedFile);
                MessageBox.Show($"{secilenDeger} yüklendi.");
                btnBaslat.Enabled = true;
            }
        }
        private void btnBaslat_Click(object sender, EventArgs e) 
        { 
            if (string.IsNullOrEmpty(selectedFile))
            {
                MessageBox.Show("Önce bir dosya seçip yükleyin.");
                return;
            }

            // Form2'yi başlat ve seçilen dosyayı gönder
            Form2 form2 = new Form2(selectedFile);
            form2.Show();
            this.Hide(); // Form1'i gizle
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close(); // Uygulamayı kapat
        }
    }
}
