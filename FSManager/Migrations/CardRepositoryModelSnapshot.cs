﻿// <auto-generated />
using FSManager.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FSManager.Migrations
{
    [DbContext(typeof(CardRepository))]
    partial class CardRepositoryModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FSManager.Shared.Models.CardCollection", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("text");

                    b.HasKey("Key");

                    b.ToTable("CardCollections", (string)null);
                });

            modelBuilder.Entity("FSManager.Shared.Models.CardModel", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("text");

                    b.Property<int>("Attack")
                        .HasColumnType("integer");

                    b.Property<string>("CollectionKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Evasion")
                        .HasColumnType("integer");

                    b.Property<int>("Health")
                        .HasColumnType("integer");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RewardsText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Script")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SoulValue")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Key");

                    b.HasIndex("CollectionKey");

                    b.ToTable("Cards", (string)null);
                });

            modelBuilder.Entity("FSManager.Shared.Models.CardRelation", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ID"));

                    b.Property<string>("RelatedCardKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RelatedToKey")
                        .HasColumnType("text");

                    b.Property<int>("RelationType")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.HasIndex("RelatedCardKey");

                    b.HasIndex("RelatedToKey");

                    b.ToTable("CardRelations", (string)null);
                });

            modelBuilder.Entity("FSManager.Shared.Models.CardModel", b =>
                {
                    b.HasOne("FSManager.Shared.Models.CardCollection", "Collection")
                        .WithMany("Cards")
                        .HasForeignKey("CollectionKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Collection");
                });

            modelBuilder.Entity("FSManager.Shared.Models.CardRelation", b =>
                {
                    b.HasOne("FSManager.Shared.Models.CardModel", "RelatedCard")
                        .WithMany("RelatedTo")
                        .HasForeignKey("RelatedCardKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FSManager.Shared.Models.CardModel", "RelatedTo")
                        .WithMany("Relations")
                        .HasForeignKey("RelatedToKey");

                    b.Navigation("RelatedCard");

                    b.Navigation("RelatedTo");
                });

            modelBuilder.Entity("FSManager.Shared.Models.CardCollection", b =>
                {
                    b.Navigation("Cards");
                });

            modelBuilder.Entity("FSManager.Shared.Models.CardModel", b =>
                {
                    b.Navigation("RelatedTo");

                    b.Navigation("Relations");
                });
#pragma warning restore 612, 618
        }
    }
}
