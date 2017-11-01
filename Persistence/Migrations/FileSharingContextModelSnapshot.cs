﻿// <auto-generated />
using FileSharing.Persistence.Context;
using FileSharing.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace FileSharing.Persistence.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class FileSharingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FileSharing.Persistence.Models.Audit", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Action");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("IdObject")
                        .IsRequired();

                    b.Property<long>("IdUser");

                    b.Property<string>("Object")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Audit");
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.File", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContentType")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Filename")
                        .IsRequired();

                    b.Property<long?>("IdFolder");

                    b.Property<long?>("IdGroup");

                    b.Property<long>("IdUser");

                    b.Property<bool>("IsPublic");

                    b.Property<DateTime>("ModificationDate");

                    b.HasKey("Id");

                    b.HasIndex("IdFolder");

                    b.HasIndex("IdGroup");

                    b.HasIndex("IdUser");

                    b.ToTable("File");
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.Folder", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("IdFolderRoot");

                    b.Property<long>("IdUser");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("IdFolderRoot");

                    b.HasIndex("IdUser");

                    b.ToTable("Folder");
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.Group", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("IdAdmin");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("IdAdmin");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Group");
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.Session", b =>
                {
                    b.Property<string>("SecurityToken")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateLastAccess");

                    b.Property<long>("IdUser");

                    b.HasKey("SecurityToken");

                    b.HasIndex("IdUser");

                    b.ToTable("Session");
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Login")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.UserGroup", b =>
                {
                    b.Property<long>("IdUser");

                    b.Property<long>("IdGroup");

                    b.Property<DateTime?>("DateInclusionApproval");

                    b.Property<DateTime>("DateInclusionRequest");

                    b.HasKey("IdUser", "IdGroup");

                    b.HasIndex("IdGroup");

                    b.ToTable("UserGroup");
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.File", b =>
                {
                    b.HasOne("FileSharing.Persistence.Models.Folder", "Folder")
                        .WithMany("Files")
                        .HasForeignKey("IdFolder")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FileSharing.Persistence.Models.Group", "Group")
                        .WithMany("Files")
                        .HasForeignKey("IdGroup");

                    b.HasOne("FileSharing.Persistence.Models.User", "User")
                        .WithMany("Files")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.Folder", b =>
                {
                    b.HasOne("FileSharing.Persistence.Models.Folder", "FolderRoot")
                        .WithMany("Folders")
                        .HasForeignKey("IdFolderRoot");

                    b.HasOne("FileSharing.Persistence.Models.User", "User")
                        .WithMany("Folders")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.Group", b =>
                {
                    b.HasOne("FileSharing.Persistence.Models.User", "Admin")
                        .WithMany("AdministrableGroups")
                        .HasForeignKey("IdAdmin")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.Session", b =>
                {
                    b.HasOne("FileSharing.Persistence.Models.User", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.UserGroup", b =>
                {
                    b.HasOne("FileSharing.Persistence.Models.Group", "Group")
                        .WithMany("Users")
                        .HasForeignKey("IdGroup")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FileSharing.Persistence.Models.User", "User")
                        .WithMany("Groups")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
