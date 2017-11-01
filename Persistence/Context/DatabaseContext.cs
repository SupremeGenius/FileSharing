using FileSharing.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace FileSharing.Persistence.Context
{
    public class DatabaseContext : DbContext
	{
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options) { }

        public virtual DbSet<Audit> Audit { get; set; }
		public virtual DbSet<File> File { get; set; }
		public virtual DbSet<Folder> Folder { get; set; }
		public virtual DbSet<Group> Group { get; set; }
		public virtual DbSet<Session> Session { get; set; }
		public virtual DbSet<User> User { get; set; }
		public virtual DbSet<UserGroup> UserGroup { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Audit>(entity =>
            {
                entity.Property(e => e.Action)
                    .IsRequired();

                entity.Property(e => e.Date);

                entity.Property(e => e.Description)
                    .IsRequired();

                entity.Property(e => e.IdObject)
                    .IsRequired();

                entity.Property(e => e.Object)
                    .IsRequired();
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.Property(e => e.Filename)
                    .IsRequired();

                entity.Property(e => e.ContentType)
                    .IsRequired();

                entity.Property(e => e.CreationDate)
                    .IsRequired();

                entity.Property(e => e.ModificationDate)
                    .IsRequired();

                entity.HasOne(d => d.Folder)
                    .WithMany(p => p.Files)
                    .HasForeignKey(d => d.IdFolder)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Files)
                    .HasForeignKey(d => d.IdGroup);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Files)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Folder>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired();

                entity.HasOne(d => d.FolderRoot)
                    .WithMany(p => p.Folders)
                    .HasForeignKey(d => d.IdFolderRoot);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Folders)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.AdministrableGroups)
                    .HasForeignKey(d => d.IdAdmin)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(e => e.SecurityToken);

                entity.Property(e => e.SecurityToken);

                entity.Property(e => e.DateLastAccess);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Login)
                    .IsUnique();

                entity.Property(e => e.FirstName)
                    .IsRequired();

                entity.Property(e => e.LastName)
                    .IsRequired();

                entity.Property(e => e.Login)
                    .IsRequired();

                entity.Property(e => e.Password)
                    .IsRequired();
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasKey(e => new { e.IdUser, e.IdGroup });

                entity.Property(e => e.DateInclusionApproval);

                entity.Property(e => e.DateInclusionRequest);

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.IdGroup)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Groups)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}