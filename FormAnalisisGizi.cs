using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PenghitungGiziMakanan
{
    public partial class FormAnalisisGizi : Form
    {
        public FormAnalisisGizi()
        {
            InitializeComponent();
        }

        private void FormAnalisisGizi_Load(object sender, EventArgs e)
        {
            UpdatePanelPemenuhanGizi();
            UpdatePanelBMI();
            bunifuPictureBox1.Load(FormMain.penggunaGlobal.Picture);
        }
        private void UpdatePanelPemenuhanGizi()
        {
            string dtpString = dtpConditionDate.Value.ToString("d");
            double totalEnergi = 0;
            double totalProtein = 0;
            double totalLemak = 0;
            double totalKarbohidrat = 0;
            double totalKalsium = 0;
            double totalVitaminA = 0;
            double totalVitaminC = 0;
            double totalZatBesi = 0;
            using (var db = new UserModel())
            {
                var query =
                    from makan in db.MakananDimakans
                    where (makan.FoodUser == FormMain.penggunaGlobal.Name && makan.Date == dtpString)
                    select makan;
                if (query.Any())
                {
                    foreach (var item in query)
                    {
                        totalEnergi += item.KandunganGizi.energi * (Convert.ToDouble(item.FoodAmount) / 100);
                        totalProtein += item.KandunganGizi.protein * (Convert.ToDouble(item.FoodAmount) / 100);
                        totalLemak += item.KandunganGizi.lemak * (Convert.ToDouble(item.FoodAmount) / 100);
                        totalKarbohidrat += item.KandunganGizi.karbohidrat * (Convert.ToDouble(item.FoodAmount) / 100);
                        totalKalsium += item.KandunganGizi.kalsium * (Convert.ToDouble(item.FoodAmount) / 100);
                        totalVitaminA += item.KandunganGizi.VitA * (Convert.ToDouble(item.FoodAmount) / 100);
                        totalVitaminC += item.KandunganGizi.VitC * (Convert.ToDouble(item.FoodAmount) / 100);
                        totalZatBesi += item.KandunganGizi.ZatBesi * (Convert.ToDouble(item.FoodAmount) / 100);
                    }
                }
            }
            lblEnergiAmount.Text = string.Format("{0:0.00}/{1} kkal",
                totalEnergi, FormMain.penggunaGlobal.KebutuhanGizi.energi);
            lblProteinAmount.Text = string.Format("{0:0.00}/{1} g",
                totalProtein, FormMain.penggunaGlobal.KebutuhanGizi.protein);
            lblLemakAmount.Text = string.Format("{0:0.00}/{1} g",
                totalLemak, FormMain.penggunaGlobal.KebutuhanGizi.lemak);
            lblKarbohidratAmount.Text = string.Format("{0:0.00}/{1} g",
                totalKarbohidrat, FormMain.penggunaGlobal.KebutuhanGizi.karbohidrat);
            lblKalsiumAmount.Text = string.Format("{0:0.00}/{1} g",
                totalKalsium, FormMain.penggunaGlobal.KebutuhanGizi.kalsium);
            lblVitAAmount.Text = string.Format("{0:0.00}/{1} g",
                totalVitaminA, FormMain.penggunaGlobal.KebutuhanGizi.VitA);
            lblVitCAmount.Text = string.Format("{0:0.00}/{1} g",
                totalVitaminC, FormMain.penggunaGlobal.KebutuhanGizi.VitC);
            lblZatBesiAmount.Text = string.Format("{0:0.00}/{1} g",
                totalZatBesi, FormMain.penggunaGlobal.KebutuhanGizi.ZatBesi);
        }

        private void UpdatePanelBMI()
        {
            lblUserNameHere.Text = FormMain.penggunaGlobal.Name;
            lblUserAgeHere.Text = string.Format("{0} Tahun", FormMain.penggunaGlobal.Age);
            lblUserGenderHere.Text = FormMain.penggunaGlobal.JenisKelamin;
            lblBMI.Text = string.Format("{0:0.000} kg/m^2", FormMain.penggunaGlobal.BodyMassIndex);
            lblBMIAnalysis.Text = string.Format("" +
                "BMI Anda termasuk kategori " +
                "[{0}]\n" +
                "Selebihnya tentang BMI, klik text ini", FormMain.penggunaGlobal.BMIAnalysis);
        }

        private void label23_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
