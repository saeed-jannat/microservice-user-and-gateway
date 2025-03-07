using System;
using System.Collections.Generic;
using identity.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace identity.Infrastructure.Data;

public partial class SilbargBaseIdentityContext : DbContext
{
    public SilbargBaseIdentityContext()
    {
    }

    public SilbargBaseIdentityContext(DbContextOptions<SilbargBaseIdentityContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Accessibility> Accessibilities { get; set; }
    public virtual DbSet<Claim> Claims { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleParentChild> RoleParentChildren { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserParentChild> UserParentChildren { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserRoleClaim> UserRoleClaims { get; set; }

    public virtual DbSet<Year> Years { get; set; }

    public virtual DbSet<Zone> Zones { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Persian_100_CI_AS_WS_SC_UTF8");

        modelBuilder.Entity<Accessibility>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Accessib__3214EC07A9D2054D");

            entity.ToTable("Accessibility");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.Controller).HasMaxLength(50);
            entity.Property(e => e.FaActionName).HasMaxLength(50);

            entity.HasOne(d => d.Claim).WithMany(p => p.Accessibilities)
                .HasForeignKey(d => d.ClaimId)
                .HasConstraintName("FK__Accessibi__Claim__3A81B327");
        });

       

        modelBuilder.Entity<Claim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Claim__3214EC07C3A37A57");

            entity.ToTable("Claim");

            entity.HasIndex(e => e.EnName, "UQ__Claim__511966336D3CF7D7").IsUnique();

            entity.HasIndex(e => e.FaName, "UQ__Claim__E6A5192AABA4A0B6").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.EnName).HasMaxLength(100);
            entity.Property(e => e.FaName).HasMaxLength(50);
            entity.Property(e => e.Path).HasMaxLength(100);
            entity.Property(e => e.Verb).HasMaxLength(50);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Claim__ParentId__6C190EBB");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Zone__3214EC07C883E8EB");

            entity.ToTable("Organization");

            entity.HasIndex(e => new { e.Name, e.Id }, "UQ__Zone__1054CA36C5623351").IsUnique();

            entity.HasIndex(e => e.MemoryId, "UQ__Zone__9A4986D5CE74E634").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.MemoryId).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.NationalCode).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Zone).WithMany(p => p.Organizations)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("FK__Organizat__ZoneI__2FCF1A8A");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC074D2B1639");

            entity.ToTable("Role");

            entity.HasIndex(e => new { e.Name, e.Id }, "UQ__Role__1054CA367848419B").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasMany(d => d.Claims).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RoleClaim",
                    r => r.HasOne<Claim>().WithMany()
                        .HasForeignKey("ClaimId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RoleClaims_Claim"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RoleClaims_Role"),
                    j =>
                    {
                        j.HasKey("RoleId", "ClaimId").HasName("PK__RoleClai__24082F23DA17CCD2");
                        j.ToTable("RoleClaims");
                    });
        });

        modelBuilder.Entity<RoleParentChild>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RolePare__3214EC0748D9ABDF");

            entity.ToTable("RoleParentChild");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.ChildRole).WithMany(p => p.RoleParentChildChildRoles)
                .HasForeignKey(d => d.ChildRoleId)
                .HasConstraintName("FK__RoleParen__Child__3D5E1FD2");

            entity.HasOne(d => d.ParentRole).WithMany(p => p.RoleParentChildParentRoles)
                .HasForeignKey(d => d.ParentRoleId)
                .HasConstraintName("FK__RoleParen__Paren__3E52440B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07B3744F98");

            entity.ToTable("User");

            entity.HasIndex(e => e.PhoneNumber, "UQ__User__85FB4E386131E5F2").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__User__UserName").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.NationalCode).HasMaxLength(50);
            entity.Property(e => e.PassWord).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.SubscriptionEndDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(255);

            entity.HasOne(d => d.Zone).WithMany(p => p.Users)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("FK__User__Zone__339FAB6E");
        });

        modelBuilder.Entity<UserParentChild>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserPare__3214EC07B4C413BE");

            entity.ToTable("UserParentChild");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.ChildUser).WithMany(p => p.UserParentChildChildUsers)
                .HasForeignKey(d => d.ChildUserId)
                .HasConstraintName("FK__UserParen__Child__403A8C7D");

            entity.HasOne(d => d.ParentUser).WithMany(p => p.UserParentChildParentUsers)
                .HasForeignKey(d => d.ParentUserId)
                .HasConstraintName("FK__UserParen__Paren__412EB0B6");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC0797C633C3");

            entity.ToTable("UserRole");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Organization).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK__UserRole__OrganizationId__32AB8735");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__UserRole__RoleId__4222D4EF");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__UserRole__UserId__4316F928");
        });

        modelBuilder.Entity<UserRoleClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC0798E53030");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Claims).WithMany(p => p.UserRoleClaims)
                .HasForeignKey(d => d.ClaimsId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__UserRoleC__Claim__44FF419A");

            entity.HasOne(d => d.UserRole).WithMany(p => p.UserRoleClaims)
                .HasForeignKey(d => d.UserRoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__UserRoleC__UserR__45F365D3");

            entity.HasMany(d => d.Years).WithMany(p => p.UserRoleClaims)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRoleClaimYear",
                    r => r.HasOne<Year>().WithMany()
                        .HasForeignKey("YearId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserRoleC__YearI__6B24EA82"),
                    l => l.HasOne<UserRoleClaim>().WithMany()
                        .HasForeignKey("UserRoleClaimId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserRoleC__UserR__6A30C649"),
                    j =>
                    {
                        j.HasKey("UserRoleClaimId", "YearId").HasName("PK__UserRole__4030D70E827B70D9");
                        j.ToTable("UserRoleClaimYears");
                    });
        });

        modelBuilder.Entity<Year>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Years__3214EC072F627DF4");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasMany(d => d.UserRoles).WithMany(p => p.Years)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRoleYear",
                    r => r.HasOne<UserRole>().WithMany()
                        .HasForeignKey("UserRoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserRoleY__UserR__7A672E12"),
                    l => l.HasOne<Year>().WithMany()
                        .HasForeignKey("YearId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserRoleY__YearI__7B5B524B"),
                    j =>
                    {
                        j.HasKey("YearId", "UserRoleId").HasName("PK__UserRole__80E3606E14E486A6");
                        j.ToTable("UserRoleYears");
                    });
        });

        modelBuilder.Entity<Zone>(entity =>
        {
            entity.HasKey(e => e.ZoneId).HasName("PK__CountryD__60166795F5AFC17F");

            entity.ToTable("Zone");

            entity.HasIndex(e => e.FileId, "UQ__CountryD__6F0F989E84958081").IsUnique();

            entity.Property(e => e.ZoneId)
                .ValueGeneratedNever()
                .HasColumnName("ZoneID");
            entity.Property(e => e.Bakhsh).HasColumnName("bakhsh");
            entity.Property(e => e.Bakhshname)
                .HasMaxLength(100)
                .HasColumnName("bakhshname");
            entity.Property(e => e.BreakupDate)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.City).HasColumnName("city");
            entity.Property(e => e.Cityname)
                .HasMaxLength(100)
                .HasColumnName("cityname");
            entity.Property(e => e.EstablishDate)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Etepass)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("etepass");
            entity.Property(e => e.Eteuser)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("eteuser");
            entity.Property(e => e.FileId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("FileID");
            entity.Property(e => e.HczoneLevelCode).HasColumnName("HCZoneLevelCode");
            entity.Property(e => e.HczoneLevelName)
                .HasMaxLength(255)
                .HasColumnName("HCZoneLevelName");
            entity.Property(e => e.InsertDate).HasColumnType("smalldatetime");
            entity.Property(e => e.InsertUser)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsLockWhenInfoNotComplete).HasDefaultValue(false);
            entity.Property(e => e.Jameiat).HasColumnName("jameiat");
            entity.Property(e => e.Khanevar).HasColumnName("khanevar");
            entity.Property(e => e.Lat).HasColumnName("lat");
            entity.Property(e => e.LatinName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Lng).HasColumnName("lng");
            entity.Property(e => e.Man).HasColumnName("man");
            entity.Property(e => e.MojavezDate)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MojavezTasis).HasMaxLength(100);
            entity.Property(e => e.NationalCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Ostan).HasColumnName("ostan");
            entity.Property(e => e.Ostanname)
                .HasMaxLength(100)
                .HasColumnName("ostanname");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.Parentname)
                .HasMaxLength(100)
                .HasColumnName("parentname");
            entity.Property(e => e.PishsShomareh)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StaticIp)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TarikhSabt)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("smalldatetime");
            entity.Property(e => e.UpdateUser)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.VillageCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Woman).HasColumnName("woman");
            entity.Property(e => e.ZoneCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ZoneName).HasMaxLength(50);
            entity.Property(e => e.ZoneStatusName).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
