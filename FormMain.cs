using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PenghitungGiziMakanan
{
    public partial class FormMain : Form
    {

        public static IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "RJChsPkf39bWtQB4ZlFrzda9opeNb7QZjkOiA5yL",
            BasePath = "https://firstfirebaseforms.firebaseio.com/"
        };
        public static IFirebaseClient client;
        public static User penggunaGlobal;
        public enum Mode { insert, edit }
        public Mode mode;
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            client = new FirebaseClient(config);
            if (client != null)
            {
                //MessageBox.Show("Connection Established");
            }
            else
            {
                MessageBox.Show("Error occured");
            }
            panelSignIn.BringToFront();
        }


        private async void btNext_Click(object sender, EventArgs e)
        {
            penggunaGlobal.Height = tbHeight.Text;
            penggunaGlobal.Weight = tbWeight.Text;
            FirebaseResponse response2 = await client.UpdateTaskAsync("UserData/" + $"User-{penggunaGlobal.ID}", penggunaGlobal);
            //MessageBox.Show("Data inserted successfully");
            panelUserMainMenu.BringToFront();
        }


        private void btEat_Click(object sender, EventArgs e)
        {
            FormFood formFood = new FormFood();
            formFood.Show();
        }

        private void btCondition_Click(object sender, EventArgs e)
        {
            FormAnalisisGizi formAnalisisGizi = new FormAnalisisGizi();
            formAnalisisGizi.Show();
        }
        private async void btnSignInWithGoogle_Click(object sender, EventArgs e)
        {
            await GoogleSignInModule.executeLoginModule();
            Dictionary<string, string> dataUser = GoogleSignInModule.Hasil;
            string announcement="";
            foreach(KeyValuePair<string, string> entry in dataUser)
            {
                announcement += $"{entry.Key} : {entry.Value}\n";
            }
            //MessageBox.Show(announcement);

            var user = new User
            {
                ID = dataUser["ID"],
                Name = dataUser["Name"],
                NickName = dataUser["NickName"],
                JenisKelamin = dataUser["JenisKelamin"],
                BirthDate = dataUser["BirthDate"],
                Picture = dataUser["Picture"]
            };
            SetResponse responseSet = await client.SetTaskAsync("UserData/" + $"User-{user.ID}", user);
            penggunaGlobal = user;
            lbMainMenuTitle.Text = "Halo, "+user.NickName;
            bunifuPictureBox1.Load(user.Picture);
            panelUserProfile.BringToFront();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
