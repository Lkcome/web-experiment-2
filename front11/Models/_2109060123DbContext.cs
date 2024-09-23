using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace front11.Models;

public partial class _2109060123DbContext : DbContext
{
    public _2109060123DbContext()
    {
    }

    public _2109060123DbContext(DbContextOptions<_2109060123DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Selectcoursetime> Selectcoursetimes { get; set; }

    public virtual DbSet<SelectedCourse> SelectedCourses { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=selectcourse;user id=root;password=200301071314Lk;character set=utf8;allow zero datetime=true;convert zero datetime=true;pooling=true;maximumpoolsize=3000", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.34-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Aid).HasName("PRIMARY");

            entity.ToTable("admins");

            entity.Property(e => e.Aid)
                .HasMaxLength(50)
                .HasComment("管理员代码（唯一标识）");
            entity.Property(e => e.Aaccount)
                .HasMaxLength(50)
                .HasComment("管理员账号");
            entity.Property(e => e.Apassword)
                .HasMaxLength(50)
                .HasComment("账号密码");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Cid).HasName("PRIMARY");

            entity.ToTable("course");

            entity.Property(e => e.Cid)
                .HasMaxLength(15)
                .HasComment("	课程代码（唯一标识）");
            entity.Property(e => e.Cclassroom)
                .HasMaxLength(50)
                .HasComment("上课教室");
            entity.Property(e => e.Cname)
                .HasMaxLength(50)
                .HasComment("课程名称");
            entity.Property(e => e.Cscore)
                .HasMaxLength(15)
                .HasComment("学分");
            entity.Property(e => e.Csem)
                .HasMaxLength(20)
                .HasComment("开课学期");
            entity.Property(e => e.Cteacher)
                .HasMaxLength(50)
                .HasComment("上课老师");
            entity.Property(e => e.Ctime)
                .HasMaxLength(50)
                .HasComment("上课时间");
        });

        modelBuilder.Entity<Selectcoursetime>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("selectcoursetime");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.SelectionEndTime).HasColumnType("datetime");
            entity.Property(e => e.SelectionStartTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<SelectedCourse>(entity =>
        {
            entity.HasKey(e => new { e.Cid, e.Sid })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("selected_course");

            entity.HasIndex(e => e.Sid, "outdoor1");

            entity.Property(e => e.Cid)
                .HasMaxLength(15)
                .HasComment("课程代码(主键加外键)");
            entity.Property(e => e.Sid)
                .HasMaxLength(12)
                .HasComment("学号(主键加外键)");
            entity.Property(e => e.ScDate)
                .HasComment("选课日期")
                .HasColumnType("datetime");

            entity.HasOne(d => d.CidNavigation).WithMany(p => p.SelectedCourses)
                .HasForeignKey(d => d.Cid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("outdoor");

            entity.HasOne(d => d.SidNavigation).WithMany(p => p.SelectedCourses)
                .HasForeignKey(d => d.Sid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("outdoor1");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Sid).HasName("PRIMARY");

            entity.ToTable("student");

            entity.Property(e => e.Sid)
                .HasMaxLength(12)
                .HasComment("学号（唯一标识）");
            entity.Property(e => e.Sclass)
                .HasMaxLength(50)
                .HasComment("班级");
            entity.Property(e => e.Sname)
                .HasMaxLength(50)
                .HasComment("姓名");
            entity.Property(e => e.Spassword)
                .HasMaxLength(20)
                .HasComment("初始密码");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
