namespace PenghitungGiziMakanan
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class UserModel : DbContext
    {
        public UserModel()
            : base("name=UserModel")
        {
        }

        public virtual DbSet<MakananDimakan> MakananDimakans { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MakananDimakan>()
                .Property(e => e.FoodAmount)
                .IsFixedLength();
        }
    }
}
