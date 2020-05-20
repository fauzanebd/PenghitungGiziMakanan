namespace PenghitungGiziMakanan
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MakananDimakan")]
    public partial class MakananDimakan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Date { get; set; }

        [Required]
        [StringLength(50)]
        public string FoodName { get; set; }

        [Required]
        [StringLength(10)]
        public string FoodAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string FoodUser { get; set; }

        public Gizi KandunganGizi
        {
            get
            {
                Gizi result = Makanan.getKandunganGizi(FoodName);
                return result;
            }
        }
    }
}
