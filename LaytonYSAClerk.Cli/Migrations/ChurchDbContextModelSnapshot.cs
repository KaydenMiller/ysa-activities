﻿// <auto-generated />
using System;
using LaytonYSAClerk.Cli;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LaytonYSAClerk.Cli.Migrations
{
    [DbContext(typeof(ChurchDbContext))]
    partial class ChurchDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("LaytonYSAClerk.Cli.Entities.Member", b =>
                {
                    b.Property<long>("MemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Address")
                        .HasColumnType("TEXT")
                        .HasColumnName("textAddress");

                    b.Property<DateOnly?>("Birthday")
                        .HasColumnType("TEXT")
                        .HasColumnName("birthdate");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<string>("Gender")
                        .HasColumnType("TEXT")
                        .HasColumnName("gender");

                    b.Property<string>("HouseholdPosistion")
                        .HasColumnType("TEXT")
                        .HasColumnName("householdPosition");

                    b.Property<DateOnly?>("MoveInDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("moveDateCalc");

                    b.Property<DateTime?>("NewMemberEmailSentDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("newMemberEmailSentDate");

                    b.Property<string>("Phone")
                        .HasColumnType("TEXT")
                        .HasColumnName("phone");

                    b.Property<string>("PriorUnit")
                        .HasColumnType("TEXT")
                        .HasColumnName("priorUnitName");

                    b.Property<string>("PriorUnitNumber")
                        .HasColumnType("TEXT")
                        .HasColumnName("priorUnitNumber");

                    b.HasKey("MemberId");

                    b.ToTable("members", (string)null);
                });

            modelBuilder.Entity("LaytonYSAClerk.Cli.Entities.Unit", b =>
                {
                    b.Property<long>("MemberId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("memberId");

                    b.Property<int>("UnitNumber")
                        .HasColumnType("INTEGER")
                        .HasColumnName("unitNumber");

                    b.Property<string>("LeaderCellPhone")
                        .HasColumnType("TEXT")
                        .HasColumnName("leaderCellPhone");

                    b.Property<string>("LeaderEmail")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("leaderEmail");

                    b.Property<string>("LeaderName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("leaderName");

                    b.Property<string>("PositionName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("positionName");

                    b.Property<string>("UnitTitle")
                        .HasColumnType("TEXT")
                        .HasColumnName("title");

                    b.HasKey("MemberId", "UnitNumber");

                    b.HasIndex("MemberId")
                        .IsUnique();

                    b.ToTable("units", (string)null);
                });

            modelBuilder.Entity("LaytonYSAClerk.Cli.Entities.Unit", b =>
                {
                    b.HasOne("LaytonYSAClerk.Cli.Entities.Member", "Member")
                        .WithOne("Unit")
                        .HasForeignKey("LaytonYSAClerk.Cli.Entities.Unit", "MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Member");
                });

            modelBuilder.Entity("LaytonYSAClerk.Cli.Entities.Member", b =>
                {
                    b.Navigation("Unit")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
