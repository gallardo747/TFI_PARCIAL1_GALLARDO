﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Data;
using VETLY_BE.Entities;
using Microsoft.Extensions.Configuration;

namespace LOGISTICA_DAL.Data
{
    public partial class LOGISTICAContext : DbContext
    {
        public LOGISTICAContext()
        {
        }

        public LOGISTICAContext(DbContextOptions<LOGISTICAContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DocumentoProcesado> DocumentoProcesado { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-IAI1KNV;Initial Catalog=TFI_PARCIAL1;Integrated Security=True; TrustServerCertificate=True");
                
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                //optionsBuilder.UseSqlServer(configuration.GetConnectionString("VETLYDB"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<DocumentoProcesado>(entity =>
            {
                entity.ToTable("Documentos_Procesados_OK");

                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Nombre)
                                    .HasMaxLength(50)
                                    .IsUnicode(false)
                                    .HasColumnName("Nombre");
                entity.Property(e => e.Cuerpo)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Cuerpo");
                entity.Property(e => e.Fecha_Impresion).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }


        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}