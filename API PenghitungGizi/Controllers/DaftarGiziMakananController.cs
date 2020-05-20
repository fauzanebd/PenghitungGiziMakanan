using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PenghitungGiziMakanan;
using System.Configuration;
using System.Web.Script.Serialization;
using Microsoft.Ajax.Utilities;

namespace API_PenghitungGizi.Controllers
{
    public class DaftarGiziMakananController : ApiController
    { 
       
        string connection = ConfigurationManager
    .ConnectionStrings["API_PenghitungGizi.Properties.Settings.connString"]
    .ConnectionString;

        public List<string> GetKategoriMakanan()
        {
            Makanan.SetConnectionString(connection);
            return Makanan.ReadKategori();
        }
        public string getAllMakanan(int num)
        {
            if (num == 1)
            {
                Makanan.SetConnectionString(connection);
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                List<string> Kategori = Makanan.ReadKategori();
                string Hasil = "";
                foreach (string kategori in Kategori)
                {
                    List<Makanan> makanan = Makanan.ReadMakanan(kategori);
                    Hasil += javaScriptSerializer.Serialize(makanan);
                }
                return Hasil;
            }
            else
                return "";
        }
        
        public string GetMakanan(string kategori)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            
            Makanan.SetConnectionString(connection);
            List<Makanan> Hasil = Makanan.ReadMakanan(kategori);
            
            return javaScriptSerializer.Serialize(Hasil);
        }
        
        public string GetGiziMakanan(string namaMakanan, int amount)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            Makanan.SetConnectionString(connection);
            Gizi sumber = Makanan.getKandunganGizi(namaMakanan);
            Gizi Hasil = new Gizi(
                (sumber.energi * amount) / 100,
                (sumber.protein * amount) / 100,
                (sumber.lemak * amount) / 100,
                (sumber.karbohidrat * amount) / 100,
                (sumber.kalsium * amount) / 100,
                (sumber.VitA * amount) / 100,
                (sumber.VitC * amount) / 100,
                (sumber.ZatBesi * amount) / 100
                );
            return javaScriptSerializer.Serialize(Hasil);
        }   
    }
}
