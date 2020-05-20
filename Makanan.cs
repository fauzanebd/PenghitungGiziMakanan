using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenghitungGiziMakanan
{
    public class Makanan
    {
        private static OleDbConnection dbConnection;
        private static OleDbCommand dbCommand;
        private static OleDbDataReader dataReader;
        private static OleDbDataAdapter dataAdapter;
        private static DataSet dataSet;
        private static string connString;
        public static void SetConnectionString(string conn)
        {
            connString = conn;
        }
        private string id;
        public string Id { get => id; set => id = value; }

        private string kategori;
        public string Kategori { get => kategori; set => kategori = value; }

        private string nama;
        public string Nama { get => nama; set => nama = value; }

        private Gizi kandunganGizi;
        public Gizi KandunganGizi { get => kandunganGizi; set => kandunganGizi = value; }

        private double amount;
        public double Amount { get => amount; set => amount = value; }

        private DateTime when;
        public DateTime When { get => when; set => when = value; }

        public Makanan(string id, string kategori, string nama,
            double? energi, double? protein, double? lemak,
            double? karbohidrat, double? kalsium, double? vitA,
            double? vitC, double? zatBesi)
        {
            Id = id; Kategori = kategori; Nama = nama;
            KandunganGizi = new Gizi(
                energi, protein, lemak, karbohidrat,
                kalsium, vitA, vitC, zatBesi);
        }
        public static List<string> ReadKategori()
        {
            string queryString = "SELECT Kategori FROM DataGizi";
            dbConnection = new OleDbConnection(connString);
            dbCommand = new OleDbCommand(queryString, dbConnection);

            List<string> daftarKategori = new List<string>();
            try
            {
                dbConnection.Open();
                dataReader = dbCommand.ExecuteReader();
                while (dataReader.Read())
                {
                    daftarKategori.Add(dataReader[0].ToString());
                }
                dbConnection.Close();
                return daftarKategori.Distinct().ToList();
            }
            catch
            {
                return null;
            }
        }
        public static List<Makanan> ReadMakanan(string Kategori)
        {
            string queryString = string.Format("SELECT DataGizi.* FROM DataGizi" +
    " WHERE Kategori = '{0}'", Kategori);
            List<Makanan> makanans = new List<Makanan>();
            dbConnection = new OleDbConnection(connString);
            dataSet = new DataSet();
            dbConnection.Open();
            dataAdapter = new OleDbDataAdapter(queryString, connString);
            dataAdapter.Fill(dataSet, "DataGizi");
            dbConnection.Close();
            DataTable dataTable = dataSet.Tables["DataGizi"];
            int maxRow = dataTable.Rows.Count;
            for (int i = 0; i < maxRow; i++)
            {
                makanans.Add(new Makanan(
                    dataTable.Rows[i].Field<int>("ID").ToString(),
                    dataTable.Rows[i].Field<string>("Kategori"),
                    dataTable.Rows[i].Field<string>("Nama"),
                    dataTable.Rows[i].Field<Double?>("Energi"),
                    dataTable.Rows[i].Field<Double?>("Protein"),
                    dataTable.Rows[i].Field<Double?>("Lemak"),
                    dataTable.Rows[i].Field<Double?>("Karbohidrat"),
                    dataTable.Rows[i].Field<Double?>("Kalsium"),
                    dataTable.Rows[i].Field<Double?>("VitA"),
                    dataTable.Rows[i].Field<Double?>("VitC"),
                    dataTable.Rows[i].Field<Double?>("ZatBesi")));
            }
            return makanans;
        }
        public static Gizi getKandunganGizi(string namaMakanan)
        {
            Gizi result = null;
            string queryString = string.Format("SELECT DataGizi.* FROM DataGizi" +
    " WHERE Nama = '{0}'", namaMakanan);
            dbConnection = new OleDbConnection(connString);
            dataSet = new DataSet();
            dbConnection.Open();
            dataAdapter = new OleDbDataAdapter(queryString, connString);
            dataAdapter.Fill(dataSet, "Data Gizi Makanan");
            dbConnection.Close();
            DataTable dataTable = dataSet.Tables["Data Gizi Makanan"];
            int maxRow = dataTable.Rows.Count;
            for (int i = 0; i < maxRow; i++)
            {
                DataRow specialRow = dataTable.Rows[i];
                result = new Gizi(
                    Convert.ToDouble(specialRow.Field<Double?>("Energi")),
                    Convert.ToDouble(specialRow.Field<Double?>("Protein")),
                    Convert.ToDouble(specialRow.Field<Double?>("Lemak")),
                    Convert.ToDouble(specialRow.Field<Double?>("Karbohidrat")),
                    Convert.ToDouble(specialRow.Field<Double?>("Kalsium")),
                    Convert.ToDouble(specialRow.Field<Double?>("VitA")),
                    Convert.ToDouble(specialRow.Field<Double?>("VitC")),
                    Convert.ToDouble(specialRow.Field<Double?>("ZatBesi")));
            }
            return result;
        }
    }
}
