﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess.Sqlite.Migrations
{
    [DbContext(typeof(SqliteContext))]
    partial class SqliteContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.4");

            modelBuilder.Entity("Aris.Moe.OverlayTranslate.Server.DataAccess.MachineTranslationModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<int>("MachineOcrId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Provider")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("MachineOcrId");

                    b.ToTable("MachineTranslations");
                });

            modelBuilder.Entity("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.AddressableSpatialTextModel", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BasedOnSpatialOcrText")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("Language")
                        .HasColumnType("TEXT");

                    b.Property<int?>("MachineOcrId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("MachineTranslationId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BasedOnSpatialOcrText");

                    b.HasIndex("MachineOcrId");

                    b.HasIndex("MachineTranslationId");

                    b.ToTable("SpatialTexts");
                });

            modelBuilder.Entity("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.ConsolidatedMachineOcrModel", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Consolidation")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("RawId")
                        .IsRequired()
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RawId");

                    b.ToTable("ConsolidatedMachineOcrs");
                });

            modelBuilder.Entity("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.ImageReferenceModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("OriginalUrl")
                        .HasColumnType("TEXT");

                    b.Property<double>("QualityScore")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.RawMachineOcrModel", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ForImage")
                        .HasColumnType("TEXT");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Provider")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Texts")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ForImage");

                    b.ToTable("RawMachineOcrs");
                });

            modelBuilder.Entity("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.VoteModel", b =>
                {
                    b.Property<int>("For")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ChangedOn")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TEXT");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("For", "UserId");

                    b.ToTable("Votes");
                });

            modelBuilder.Entity("Aris.Moe.OverlayTranslate.Server.DataAccess.MachineTranslationModel", b =>
                {
                    b.HasOne("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.ConsolidatedMachineOcrModel", "MachineOcr")
                        .WithMany()
                        .HasForeignKey("MachineOcrId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MachineOcr");
                });

            modelBuilder.Entity("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.AddressableSpatialTextModel", b =>
                {
                    b.HasOne("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.AddressableSpatialTextModel", null)
                        .WithMany()
                        .HasForeignKey("BasedOnSpatialOcrText");

                    b.HasOne("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.ConsolidatedMachineOcrModel", "MachineOcr")
                        .WithMany("Texts")
                        .HasForeignKey("MachineOcrId");

                    b.HasOne("Aris.Moe.OverlayTranslate.Server.DataAccess.MachineTranslationModel", null)
                        .WithMany("Texts")
                        .HasForeignKey("MachineTranslationId");

                    b.OwnsOne("Aris.Moe.OverlayTranslate.Server.ViewModel.RectangleModel", "Rectangle", b1 =>
                        {
                            b1.Property<int>("AddressableSpatialTextModelId")
                                .HasColumnType("INTEGER");

                            b1.HasKey("AddressableSpatialTextModelId");

                            b1.ToTable("SpatialTexts");

                            b1.WithOwner()
                                .HasForeignKey("AddressableSpatialTextModelId");

                            b1.OwnsOne("Aris.Moe.OverlayTranslate.Server.ViewModel.PointModel", "BottomRight", b2 =>
                                {
                                    b2.Property<int>("RectangleModelAddressableSpatialTextModelId")
                                        .HasColumnType("INTEGER");

                                    b2.Property<int>("X")
                                        .HasColumnType("INTEGER");

                                    b2.Property<int>("Y")
                                        .HasColumnType("INTEGER");

                                    b2.HasKey("RectangleModelAddressableSpatialTextModelId");

                                    b2.ToTable("SpatialTexts");

                                    b2.WithOwner()
                                        .HasForeignKey("RectangleModelAddressableSpatialTextModelId");
                                });

                            b1.OwnsOne("Aris.Moe.OverlayTranslate.Server.ViewModel.PointModel", "TopLeft", b2 =>
                                {
                                    b2.Property<int>("RectangleModelAddressableSpatialTextModelId")
                                        .HasColumnType("INTEGER");

                                    b2.Property<int>("X")
                                        .HasColumnType("INTEGER");

                                    b2.Property<int>("Y")
                                        .HasColumnType("INTEGER");

                                    b2.HasKey("RectangleModelAddressableSpatialTextModelId");

                                    b2.ToTable("SpatialTexts");

                                    b2.WithOwner()
                                        .HasForeignKey("RectangleModelAddressableSpatialTextModelId");
                                });

                            b1.Navigation("BottomRight")
                                .IsRequired();

                            b1.Navigation("TopLeft")
                                .IsRequired();
                        });

                    b.Navigation("MachineOcr");

                    b.Navigation("Rectangle")
                        .IsRequired();
                });

            modelBuilder.Entity("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.ConsolidatedMachineOcrModel", b =>
                {
                    b.HasOne("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.RawMachineOcrModel", "Raw")
                        .WithMany()
                        .HasForeignKey("RawId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Raw");
                });

            modelBuilder.Entity("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.ImageReferenceModel", b =>
                {
                    b.OwnsOne("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.ImageInfoModel", "Info", b1 =>
                        {
                            b1.Property<Guid>("ImageReferenceModelId")
                                .HasColumnType("TEXT");

                            b1.Property<ulong>("AverageHash")
                                .HasColumnType("INTEGER");

                            b1.Property<ulong>("DifferenceHash")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Height")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("MimeType")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<ulong>("PerceptualHash")
                                .HasColumnType("INTEGER");

                            b1.Property<byte[]>("Sha256Hash")
                                .IsRequired()
                                .HasColumnType("BLOB");

                            b1.Property<int>("Width")
                                .HasColumnType("INTEGER");

                            b1.HasKey("ImageReferenceModelId");

                            b1.HasIndex("AverageHash");

                            b1.HasIndex("Sha256Hash");

                            b1.ToTable("Images");

                            b1.WithOwner()
                                .HasForeignKey("ImageReferenceModelId");
                        });

                    b.Navigation("Info")
                        .IsRequired();
                });

            modelBuilder.Entity("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.RawMachineOcrModel", b =>
                {
                    b.HasOne("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.ImageReferenceModel", null)
                        .WithMany()
                        .HasForeignKey("ForImage")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.VoteModel", b =>
                {
                    b.HasOne("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.AddressableSpatialTextModel", null)
                        .WithMany()
                        .HasForeignKey("For")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Aris.Moe.OverlayTranslate.Server.DataAccess.MachineTranslationModel", b =>
                {
                    b.Navigation("Texts");
                });

            modelBuilder.Entity("Aris.Moe.OverlayTranslate.Server.DataAccess.Model.ConsolidatedMachineOcrModel", b =>
                {
                    b.Navigation("Texts");
                });
#pragma warning restore 612, 618
        }
    }
}
