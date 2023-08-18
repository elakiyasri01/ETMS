using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Employee_TMS.Entities
{
    public partial class ETMSContext : DbContext
    {
        public ETMSContext()
        {
        }

        public ETMSContext(DbContextOptions<ETMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Report> Reports { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=ZSCHN01LP0459;user=sa;password=Password@123;database=ETMS;TrustServerCertificate=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin");

                entity.Property(e => e.AdminId)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.AdminPassword).HasMaxLength(100);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");

                entity.Property(e => e.EmployeeId)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Department)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Designation)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EmployeePassword).HasMaxLength(100);

                entity.Property(e => e.FullName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gender)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Passwordchanged).HasColumnType("date");

                entity.Property(e => e.ProfileImageName).HasMaxLength(50);

                entity.Property(e => e.ReportingTo)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SecurityAnswer)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SecurityQuestion)
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.EmployeeSno)
                    .HasName("PK__Report__854A933A32B524FD");

                entity.ToTable("Report");

                entity.Property(e => e.CheckIn).HasColumnType("datetime");

                entity.Property(e => e.CheckOut).HasColumnType("datetime");

                entity.Property(e => e.EmployeeId)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.WorkedHours).HasColumnName("Worked_Hours");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK__Report__Employee__2D27B809");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
