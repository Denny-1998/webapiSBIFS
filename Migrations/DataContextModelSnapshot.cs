﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using webapiSBIFS.Model;

#nullable disable

namespace webapiSBIFS.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ActivityUser", b =>
                {
                    b.Property<int>("ActivitiesActivityID")
                        .HasColumnType("int");

                    b.Property<int>("ParticipantsUserID")
                        .HasColumnType("int");

                    b.HasKey("ActivitiesActivityID", "ParticipantsUserID");

                    b.HasIndex("ParticipantsUserID");

                    b.ToTable("ActivityUser");
                });

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.Property<int>("GroupsGroupID")
                        .HasColumnType("int");

                    b.Property<int>("ParticipantsUserID")
                        .HasColumnType("int");

                    b.HasKey("GroupsGroupID", "ParticipantsUserID");

                    b.HasIndex("ParticipantsUserID");

                    b.ToTable("GroupUser");
                });

            modelBuilder.Entity("webapiSBIFS.Model.Activity", b =>
                {
                    b.Property<int>("ActivityID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ActivityID"));

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GroupID")
                        .HasColumnType("int");

                    b.Property<int>("OwnerID")
                        .HasColumnType("int");

                    b.HasKey("ActivityID");

                    b.HasIndex("GroupID");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("webapiSBIFS.Model.Group", b =>
                {
                    b.Property<int>("GroupID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GroupID"));

                    b.Property<int?>("OwnerID")
                        .HasColumnType("int");

                    b.HasKey("GroupID");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("webapiSBIFS.Model.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Privilege")
                        .HasColumnType("int");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ActivityUser", b =>
                {
                    b.HasOne("webapiSBIFS.Model.Activity", null)
                        .WithMany()
                        .HasForeignKey("ActivitiesActivityID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("webapiSBIFS.Model.User", null)
                        .WithMany()
                        .HasForeignKey("ParticipantsUserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.HasOne("webapiSBIFS.Model.Group", null)
                        .WithMany()
                        .HasForeignKey("GroupsGroupID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("webapiSBIFS.Model.User", null)
                        .WithMany()
                        .HasForeignKey("ParticipantsUserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("webapiSBIFS.Model.Activity", b =>
                {
                    b.HasOne("webapiSBIFS.Model.Group", "Group")
                        .WithMany("Activities")
                        .HasForeignKey("GroupID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("webapiSBIFS.Model.Group", b =>
                {
                    b.Navigation("Activities");
                });
#pragma warning restore 612, 618
        }
    }
}
