﻿// <auto-generated />
using System;
using Arvant.Entity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Arvant.Entity.Migrations
{
    [DbContext(typeof(ArvantEntityContext))]
    partial class ArvantEntityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Arvant.Entity.Structure.EntityStructure", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Caption")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("caption");

                    b.Property<string>("ColumnsJson")
                        .IsRequired()
                        .HasColumnType("json")
                        .HasColumnName("columns");

                    b.Property<string>("ForeignKeysJson")
                        .IsRequired()
                        .HasColumnType("json")
                        .HasColumnName("foreign_keys");

                    b.Property<string>("IndexesJson")
                        .IsRequired()
                        .HasColumnType("json")
                        .HasColumnName("indexes");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_entity_structure");

                    b.ToTable("entity_structure", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}