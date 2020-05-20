using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace PenghitungGiziMakanan
{
    public partial class FormFood : Form
    {
        List<Makanan> daftarMakanan;
        Makanan makanan;
        DateTime now = DateTime.Now;
        public FormFood()
        {
            InitializeComponent();
        }

        private void FormFood_Load(object sender, EventArgs e)
        {
            using (var db = new UserModel())
            {
                var query =
                    from makan in db.MakananDimakans where makan.FoodUser == FormMain.penggunaGlobal.Name select makan;
                if (query.Count() > 0)
                {
                    btnFoodList.Enabled = true;
                }
            }
            Makanan.SetConnectionString(ConfigurationManager
.ConnectionStrings["PenghitungGiziMakanan.Properties.Settings.connString"]
.ConnectionString);
            List<string> daftarKategori = Makanan.ReadKategori();
            foreach (string kategori in daftarKategori)
            {
                cbFoodCategory.Items.Add(kategori);
            }
        }

        private void cbFoodCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbFoodName.Items.Clear();
            daftarMakanan = Makanan.ReadMakanan(cbFoodCategory.Text);
            foreach (Makanan makanan in daftarMakanan)
            {
                cbFoodName.Items.Add(makanan.Nama);
            }
        }

        private void cbFoodName_SelectedIndexChanged(object sender, EventArgs e)
        {
            makanan = daftarMakanan.Find(x => x.Nama.Contains(cbFoodName.Text));
            lblFoodName.Text = makanan.Nama;
            lblFoodCategory.Text = makanan.Kategori;
            lblEnergiAmount.Text = makanan.KandunganGizi.energi.ToString() + " kkal";
            lblProteinAmount.Text = makanan.KandunganGizi.protein.ToString() + " gr";
            lblLemakAmount.Text = makanan.KandunganGizi.lemak.ToString() + " gr";
            lblKarbohidratAmount.Text = makanan.KandunganGizi.lemak.ToString() + " gr";
            lblKalsiumAmount.Text = makanan.KandunganGizi.kalsium.ToString() + " gr";
            lblVitAAmount.Text = makanan.KandunganGizi.VitA.ToString() + " gr";
            lblVitCAmount.Text = makanan.KandunganGizi.VitC.ToString() + " gr";
            lblZatBesiAmount.Text = makanan.KandunganGizi.ZatBesi.ToString() + " gr";
            panelFoodInfo.Visible = true;
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            try
            {
                makanan.Amount = Convert.ToDouble(tbFoodEaten.Text);
                makanan.When = now;
            }
            catch
            {
                MessageBox.Show("Masukkan angka pada kolom amount");
            }
            using (var db = new UserModel())
            {
                try
                {
                    db.MakananDimakans.Add(new MakananDimakan
                    {
                        Date = now.ToString("d"),
                        FoodName = cbFoodName.Text,
                        FoodAmount = tbFoodEaten.Text,
                        FoodUser = FormMain.penggunaGlobal.Name
                    });
                    db.SaveChanges();
                    MessageBox.Show("Data berhasil ditambahkan");
                    Close();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }
            }
        }

        private void btnFoodList_Click(object sender, EventArgs e)
        {
            string result = string.Empty;
            using (var db = new UserModel())
            {
                var query =
                    from makan in db.MakananDimakans where makan.FoodUser == FormMain.penggunaGlobal.Name select makan;
                foreach (var item in query)
                {
                    string subResult = string.Format("{0}\t||{1}\t||{2} gr\n", item.Date, item.FoodName, item.FoodAmount);
                    result += subResult;
                }
            }
            MessageBox.Show("Yang sudah anda makan :\n" + result);
        }
        private void tbFoodEaten_TextChanged(object sender, EventArgs e)
        {
            btnAddFood.Enabled = true;
        }

        private void label23_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
