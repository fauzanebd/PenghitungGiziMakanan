namespace PenghitungGiziMakanan
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Configuration;
    using System.Data;
    using System.Data.OleDb;

    public partial class User
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string JenisKelamin { get; set; }
        public string BirthDate { get; set; }
        public string Picture { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public int Age { get => setAge(DateTime.Parse(BirthDate)); }
        public double BodyMassIndex
        {
            get
            {
                double a = Convert.ToDouble(Weight);
                double b = Convert.ToDouble(Height);
                double result = a / (b * b);
                return result;
            }
        }
        public List<MakananDimakan> eatenFood = new List<MakananDimakan>();
        public string BMIAnalysis
        {
            get
            {
                if (BodyMassIndex < 18.5)
                {
                    return "Kurang";
                }
                else if (BodyMassIndex >= 18.5 && BodyMassIndex < 22.9)
                {
                    return "Normal";
                }
                else if (BodyMassIndex >= 22.9 && BodyMassIndex < 29.9)
                {
                    return "Berlebih";
                }
                else
                {
                    return "Obesitas";
                }
            }
        }
        public static string setNickName(string name)
        {
            string _nickname;
            int lastSpace = name.LastIndexOf(" ");
            if (lastSpace == -1)
            {
                _nickname = name;
            }
            else
            {
                _nickname = name.Substring(0, lastSpace);
            }
            return _nickname;
        }
        public int setAge(DateTime birthDate)
        {
            int result;
            DateTime sekarang = DateTime.Today;
            TimeSpan timeDiff = sekarang.Subtract(birthDate);
            int getYear(TimeSpan span)
            {
                DateTime zeroTime = new DateTime(1, 1, 1);
                int year = (zeroTime + span).Year - 1;
                return year;
            }
            result = getYear(timeDiff);
            return result;
        }
        private OleDbConnection dbConnection;
        private OleDbDataAdapter dataAdapter;
        private DataSet dataSet;
        private string connection;
        public Gizi KebutuhanGizi
        {
            get
            {
                Gizi result = null;
                connection = ConfigurationManager
    .ConnectionStrings["PenghitungGiziMakanan.Properties.Settings.connString"]
    .ConnectionString;
                string queryString = string.Format
                ("SELECT AKG.* FROM AKG WHERE Umur = '{0}' AND JenisKelamin = '{1}'", Age, JenisKelamin);
                dbConnection = new OleDbConnection(connection);
                dataSet = new DataSet();
                dbConnection.Open();
                dataAdapter = new OleDbDataAdapter(queryString, dbConnection);
                dataAdapter.Fill(dataSet, "Data AKG");
                dbConnection.Close();
                DataTable dataTable = dataSet.Tables["Data AKG"];
                int maxRow = dataTable.Rows.Count;
                for (int i = 0; i < maxRow; i++)
                {
                    DataRow specialRow = dataTable.Rows[i];
                    result = new Gizi(
                        Convert.ToDouble(specialRow.Field<string>("Energi")),
                        Convert.ToDouble(specialRow.Field<string>("Protein")),
                        Convert.ToDouble(specialRow.Field<string>("Lemak")),
                        Convert.ToDouble(specialRow.Field<string>("Karbohidrat")),
                        Convert.ToDouble(specialRow.Field<string>("Kalsium")),
                        Convert.ToDouble(specialRow.Field<string>("VitA")),
                        Convert.ToDouble(specialRow.Field<string>("VitC")),
                        Convert.ToDouble(specialRow.Field<string>("ZatBesi")));
                }
                return result;
            }
        }
    }
}
