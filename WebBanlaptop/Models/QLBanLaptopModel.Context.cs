//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebBanlaptop.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class QLBANLAPTOPEntities : DbContext
    {
        public QLBANLAPTOPEntities()
            : base("name=QLBANLAPTOPEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BINHLUAN> BINHLUAN { get; set; }
        public virtual DbSet<CHITIETSP> CHITIETSP { get; set; }
        public virtual DbSet<CHITIETHD> CHITIETHD { get; set; }
        public virtual DbSet<HOADON> HOADON { get; set; }
        public virtual DbSet<KHACHHANG> KHACHHANG { get; set; }
        public virtual DbSet<LOAISANPHAM> LOAISANPHAM { get; set; }
        public virtual DbSet<MAUSAC> MAUSAC { get; set; }
        public virtual DbSet<NHANVIEN> NHANVIEN { get; set; }
        public virtual DbSet<NHASANXUAT> NHASANXUAT { get; set; }
        public virtual DbSet<SANPHAM> SANPHAM { get; set; }
        public virtual DbSet<GOPY> GOPY { get; set; }
    }
}
