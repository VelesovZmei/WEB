using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using Models;

namespace WEB.Data
{
    public class ApplicationDbContext : IdentityDbContext<WebUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<New> News { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<WebUser> WebUsers { get; set; }
        public DbSet<Afinn> Afinns { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Identity scaffolded entities

            builder.Entity<IdentityRole>(b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("ConcurrencyStamp")
                    .IsConcurrencyToken()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Name")
                    .HasColumnType("nvarchar(256)")
                    .HasMaxLength(256);

                b.Property<string>("NormalizedName")
                    .HasColumnType("nvarchar(256)")
                    .HasMaxLength(256);

                b.HasKey("Id");

                b.HasIndex("NormalizedName")
                    .IsUnique()
                    .HasName("RoleNameIndex")
                    .HasFilter("[NormalizedName] IS NOT NULL");

                b.ToTable("AspNetRoles");
            });

            builder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int")
                    .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                b.Property<string>("ClaimType")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("ClaimValue")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("RoleId")
                    .IsRequired()
                    .HasColumnType("nvarchar(450)");

                b.HasKey("Id");

                b.HasIndex("RoleId");

                b.ToTable("AspNetRoleClaims");

                b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            builder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int")
                    .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                b.Property<string>("ClaimType")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("ClaimValue")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("UserId")
                    .IsRequired()
                    .HasColumnType("nchar(36)");

                b.HasKey("Id");

                b.HasIndex("UserId");

                b.ToTable("AspNetUserClaims");

                b.HasOne("Models.WebUser", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            builder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
            {
                b.Property<string>("LoginProvider")
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("ProviderKey")
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("ProviderDisplayName")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("UserId")
                    .IsRequired()
                    .HasColumnType("nchar(36)");

                b.HasKey("LoginProvider", "ProviderKey");

                b.HasIndex("UserId");

                b.ToTable("AspNetUserLogins");

                b.HasOne("Models.WebUser", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            builder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
            {
                b.Property<string>("UserId")
                    .HasColumnType("nchar(36)");

                b.Property<string>("RoleId")
                    .HasColumnType("nvarchar(450)");

                b.HasKey("UserId", "RoleId");

                b.HasIndex("RoleId");

                b.ToTable("AspNetUserRoles");

                b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("Models.WebUser", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            builder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
            {
                b.Property<string>("UserId")
                    .HasColumnType("nchar(36)");

                b.Property<string>("LoginProvider")
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("Name")
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("Value")
                    .HasColumnType("nvarchar(max)");

                b.HasKey("UserId", "LoginProvider", "Name");

                b.ToTable("AspNetUserTokens");

                b.HasOne("Models.WebUser", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            #endregion Identity scaffolded entities

            builder.Entity<WebUser>(a =>
            {
                a.ToTable("WebUsers");
                a.HasKey(s => s.Id);
                a.Property(s => s.Id).HasMaxLength(36).IsFixedLength().ValueGeneratedNever();
                a.Property(s => s.ConcurrencyStamp).HasMaxLength(36).IsFixedLength().IsRequired().IsConcurrencyToken();
            });

            builder.Entity<New>(a =>
            {
                a.HasKey(s => s.Id);
                a.HasIndex(s => s.DatePosted);
                a.HasIndex(s => s.Score);
                a.Property(s => s.Id).HasMaxLength(36).IsFixedLength().ValueGeneratedNever();
                a.Property(s => s.Head).HasMaxLength(4000).IsRequired();
                a.Property(s => s.Text).IsRequired();
                a.Property(s => s.SourceURL).HasMaxLength(1024);
                a.Property(s => s.Author).HasMaxLength(128).IsRequired();
            });

            builder.Entity<Comment>(a =>
            {
                a.HasKey(s => s.Id);
                a.Property(s => s.ConcurrencyStamp).HasMaxLength(36).IsFixedLength().IsRequired().IsConcurrencyToken();
                a.Property(s => s.Content).HasMaxLength(4000).IsRequired();
                a.Property(s => s.UserName).HasMaxLength(256);
                a.Property(s => s.NewId).HasMaxLength(36).IsFixedLength().IsRequired();
                a.Property(s => s.WebUserId).HasMaxLength(36).IsFixedLength();
                a.HasOne(s => s.WebUser)
                    .WithMany(d => d.Comments)
                    .HasForeignKey(f => f.WebUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                a.HasOne(s => s.New)
                    .WithMany(d => d.Comments)
                    .HasForeignKey(f => f.NewId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Afinn>(a =>
            {
                a.HasKey(s => new { s.Word, s.Culture });
                a.Property(s => s.Word).HasMaxLength(64).IsRequired().ValueGeneratedNever();
                a.Property(s => s.Culture).HasMaxLength(5).IsFixedLength().IsRequired().ValueGeneratedNever();
            });
        }
    }
}
