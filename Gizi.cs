using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenghitungGiziMakanan
{
    public class Gizi
    {
        public double energi;
        public double protein;
        public double lemak;
        public double karbohidrat;
        public double kalsium;
        public double VitA;
        public double VitC;
        public double ZatBesi;

        public Gizi(double? EN, double? PRO, double? FAT, double? CRB,
            double? CALC, double? VA, double? VC, double? ZN)
        {
            energi = EN ?? 0; protein = PRO ?? 0; lemak = FAT ?? 0; karbohidrat = CRB ?? 0;
            kalsium = CALC ?? 0; VitA = VA ?? 0; VitC = VC ?? 0; ZatBesi = ZN ?? 0;
        }
    }
}
