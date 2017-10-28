﻿// <auto-generated />
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
    [DbContext(typeof(FileSharingContext))]
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

                    b.Property<int>("Action")
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IdObject")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<long>("IdUser");

                    b.Property<string>("Object")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Audit");
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.File", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Filename")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.Property<long?>("IdFolder");

                    b.Property<long?>("IdGroup");

                    b.Property<long>("IdUser");

                    b.Property<bool>("IsPublic");

                    b.Property<DateTime>("ModificationDate")
                        .HasColumnType("datetime");

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
                        .IsRequired()
                        .HasColumnType("varchar(200)");

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
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("IdAdmin");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasName("UK_Group");

                    b.ToTable("Group");
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.Session", b =>
                {
                    b.Property<string>("SecurityToken")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime>("DateLastAccess")
                        .HasColumnType("datetime");

                    b.Property<long>("IdUser");

                    b.HasKey("SecurityToken")
                        .HasName("PK_Session");

                    b.HasIndex("IdUser");

                    b.ToTable("Session");
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("Login")
                        .IsUnique()
                        .HasName("UK_User");

                    b.ToTable("User");
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.UserGroup", b =>
                {
                    b.Property<long>("IdUser");

                    b.Property<long>("IdGroup");

                    b.Property<DateTime?>("DateInclusionApproval")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("DateInclusionRequest")
                        .HasColumnType("datetime");

                    b.HasKey("IdUser", "IdGroup")
                        .HasName("PK_UserGroup");

                    b.HasIndex("IdGroup");

                    b.ToTable("UserGroup");
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.File", b =>
                {
                    b.HasOne("FileSharing.Persistence.Models.Folder", "Folder")
                        .WithMany("Files")
                        .HasForeignKey("IdFolder")
                        .HasConstraintName("FK_File_Folder")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FileSharing.Persistence.Models.Group", "Group")
                        .WithMany("Files")
                        .HasForeignKey("IdGroup")
                        .HasConstraintName("FK_File_Group");

                    b.HasOne("FileSharing.Persistence.Models.User", "User")
                        .WithMany("Files")
                        .HasForeignKey("IdUser")
                        .HasConstraintName("FK_File_User")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.Folder", b =>
                {
                    b.HasOne("FileSharing.Persistence.Models.Folder", "FolderRoot")
                        .WithMany("Folders")
                        .HasForeignKey("IdFolderRoot")
                        .HasConstraintName("FK_Folder_Folder")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FileSharing.Persistence.Models.User", "User")
                        .WithMany("Folders")
                        .HasForeignKey("IdUser")
                        .HasConstraintName("FK_Folder_User")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.Group", b =>
                {
                    b.HasOne("FileSharing.Persistence.Models.User", "Admin")
                        .WithMany("AdministrableGroups")
                        .HasForeignKey("IdAdmin")
                        .HasConstraintName("FK_Group_User")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.Session", b =>
                {
                    b.HasOne("FileSharing.Persistence.Models.User", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("IdUser")
                        .HasConstraintName("FK_Session_User")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FileSharing.Persistence.Models.UserGroup", b =>
                {
                    b.HasOne("FileSharing.Persistence.Models.Group", "Group")
                        .WithMany("Users")
                        .HasForeignKey("IdGroup")
                        .HasConstraintName("FK_UserGroup_Group")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FileSharing.Persistence.Models.User", "User")
                        .WithMany("Groups")
                        .HasForeignKey("IdUser")
                        .HasConstraintName("FK_UserGroup_User")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
