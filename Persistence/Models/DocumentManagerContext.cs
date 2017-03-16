using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace DocumentManager.Persistence.Models
{
	public partial class DocumentManagerContext : DbContext
	{
		public virtual DbSet<Audit> Audit { get; set; }
		public virtual DbSet<Document> Document { get; set; }
		public virtual DbSet<Group> Group { get; set; }
		public virtual DbSet<Session> Session { get; set; }
		public virtual DbSet<User> User { get; set; }
		public virtual DbSet<UserGroup> UserGroup { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json");
			var configuration = builder.Build();

			optionsBuilder.UseSqlServer($"{configuration["ConnectionStrings:DocumentManagerDatabase"]}");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Audit>(entity =>
			{
				entity.Property(e => e.Action)
					.IsRequired()
					.HasColumnType("varchar(200)");

				entity.Property(e => e.Date).HasColumnType("datetime");

				entity.Property(e => e.Object)
					.IsRequired()
					.HasColumnType("varchar(50)");

				entity.HasOne(d => d.IdUserNavigation)
					.WithMany(p => p.Audit)
					.HasForeignKey(d => d.IdUser)
					.OnDelete(DeleteBehavior.Restrict)
					.HasConstraintName("FK_Audit_User");
			});

			modelBuilder.Entity<Document>(entity =>
			{
				entity.Property(e => e.Filename)
					.IsRequired()
					.HasColumnType("varchar(200)");

				entity.HasOne(d => d.IdGroupNavigation)
					.WithMany(p => p.Document)
					.HasForeignKey(d => d.IdGroup)
					.HasConstraintName("FK_Document_Group");

				entity.HasOne(d => d.IdUserNavigation)
					.WithMany(p => p.Document)
					.HasForeignKey(d => d.IdUser)
					.OnDelete(DeleteBehavior.Restrict)
					.HasConstraintName("FK_Document_User");
			});

			modelBuilder.Entity<Group>(entity =>
			{
				entity.HasIndex(e => e.Name)
					.HasName("UK_Group")
					.IsUnique();

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)");

				entity.HasOne(d => d.IdAdminNavigation)
					.WithMany(p => p.Group)
					.HasForeignKey(d => d.IdAdmin)
					.OnDelete(DeleteBehavior.Restrict)
					.HasConstraintName("FK_Group_User");
			});

			modelBuilder.Entity<Session>(entity =>
			{
				entity.HasKey(e => e.SecurityToken)
					.HasName("PK_Session");

				entity.Property(e => e.SecurityToken).HasColumnType("varchar(200)");

				entity.Property(e => e.DateLastAccess).HasColumnType("datetime");

				entity.HasOne(d => d.IdUserNavigation)
					.WithMany(p => p.Session)
					.HasForeignKey(d => d.IdUser)
					.OnDelete(DeleteBehavior.Restrict)
					.HasConstraintName("FK_Session_User");
			});

			modelBuilder.Entity<User>(entity =>
			{
				entity.HasIndex(e => e.Login)
					.HasName("UK_User")
					.IsUnique();

				entity.Property(e => e.FirstName)
					.IsRequired()
					.HasColumnType("varchar(50)");

				entity.Property(e => e.LastName)
					.IsRequired()
					.HasColumnType("varchar(50)");

				entity.Property(e => e.Login)
					.IsRequired()
					.HasColumnType("varchar(50)");

				entity.Property(e => e.Password)
					.IsRequired()
					.HasColumnType("varchar(200)");
			});

			modelBuilder.Entity<UserGroup>(entity =>
			{
				entity.HasKey(e => new { e.IdUser, e.IdGroup })
					.HasName("PK_UserGroup");

				entity.Property(e => e.DateInclusionApproval).HasColumnType("datetime");

				entity.Property(e => e.DateInclusionRequest).HasColumnType("datetime");

				entity.HasOne(d => d.IdGroupNavigation)
					.WithMany(p => p.UserGroup)
					.HasForeignKey(d => d.IdGroup)
					.OnDelete(DeleteBehavior.Restrict)
					.HasConstraintName("FK_UserGroup_Group");

				entity.HasOne(d => d.IdUserNavigation)
					.WithMany(p => p.UserGroup)
					.HasForeignKey(d => d.IdUser)
					.OnDelete(DeleteBehavior.Restrict)
					.HasConstraintName("FK_UserGroup_User");
			});
		}
	}
}