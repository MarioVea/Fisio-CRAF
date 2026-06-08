using System;
using System.Data.Entity;
using System.IO;
using FisioCRAF.Models.Entidades;

namespace FisioCRAF.Models
{
    public class FisioCRAFTContext : DbContext
    {
        public FisioCRAFTContext() : base(ObtenerConnectionString())
        {
            // No inicializar ni crear base de datos, ya existe
            Database.SetInitializer<FisioCRAFTContext>(null);
        }

        private static string ObtenerConnectionString()
        {
            string rutacompleta = @"C:\conexion\FisioCRAF.txt";
            using (StreamReader file = new StreamReader(rutacompleta))
            {
                return file.ReadToEnd().Trim();
            }
        }

        // Tablas del Diagnóstico
        public DbSet<DiagnosticoEntity> Diagnosticos { get; set; }
        public DbSet<DetalleDiag> DetallesDiag { get; set; }
        public DbSet<DetalleEjerDiag> DetallesEjer { get; set; }
        public DbSet<DetalleCH> DetallesCH { get; set; }
        public DbSet<DetalleTrata> DetallesTrata { get; set; }

        // Tablas de catálogos
        public DbSet<EscalaDolor> EscalasDolor { get; set; }
        public DbSet<Consultorio> Consultorios { get; set; }
        public DbSet<Tratamiento> Tratamientos { get; set; }
        public DbSet<Ejercicio> Ejercicios { get; set; }
        public DbSet<CategoriaEjercicio> CategoriasEjercicio { get; set; }
        public DbSet<Lesion> Lesiones { get; set; }
        public DbSet<TipoLesiones> TipoLesiones { get; set; }
        public DbSet<Cita> Citas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Mapear entidades a tablas con esquemas específicos

            // Esquema Diagnostico
            modelBuilder.Entity<DiagnosticoEntity>()
                .ToTable("Diagnostico", "Diagnostico")
                .HasKey(d => d.id_Diag)
                .Property(d => d.id_Diag).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

            modelBuilder.Entity<DetalleDiag>()
                .ToTable("DetalleDiag", "Diagnostico")
                .HasKey(d => d.id_DetalleDiag)
                .Property(d => d.id_DetalleDiag).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

            modelBuilder.Entity<DetalleEjerDiag>()
                .ToTable("DetalleEjer", "Diagnostico")
                .HasKey(d => d.id_DetalleEjer)
                .Property(d => d.id_DetalleEjer).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

            modelBuilder.Entity<DetalleCH>()
                .ToTable("DetalleCH", "Diagnostico")
                .HasKey(d => d.id_DetalleCH)
                .Property(d => d.id_DetalleCH).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

            modelBuilder.Entity<DetalleTrata>()
                .ToTable("DetalleTrata", "Diagnostico")
                .HasKey(d => d.id_DetalleTrata)
                .Property(d => d.id_DetalleTrata).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

            // Esquema Salud
            modelBuilder.Entity<EscalaDolor>()
                .ToTable("EscalaDolor", "Salud")
                .HasKey(e => e.id_EscalaDolor);

            modelBuilder.Entity<Tratamiento>()
                .ToTable("Tratamiento", "Salud")
                .HasKey(t => t.id_Trata);

            // Esquema Ubicacion
            modelBuilder.Entity<Consultorio>()
                .ToTable("Consultorio", "Ubicacion")
                .HasKey(c => c.id_Consul);

            // Esquema Ejercicio
            modelBuilder.Entity<Ejercicio>()
                .ToTable("Ejercicios", "Ejercicio")
                .HasKey(e => e.id_Ejercicio);

            modelBuilder.Entity<CategoriaEjercicio>()
                .ToTable("CatEjercicio", "Ejercicio")
                .HasKey(c => c.id_CatEjer);

            // Esquema Lesion
            modelBuilder.Entity<Lesion>()
                .ToTable("Lesiones", "Lesion")
                .HasKey(l => l.id_Lesion);
            modelBuilder.Entity<Lesion>()
                .Ignore(l => l.Nombre_TipoLes);

            modelBuilder.Entity<TipoLesiones>()
                .ToTable("TipoLesiones", "Lesion")
                .HasKey(t => t.id_TipoLes);
            modelBuilder.Entity<TipoLesiones>()
                .Property(t => t.Nom_TipLes)
                .HasColumnName("Nom_TipoLes");

            // Esquema Datos
            modelBuilder.Entity<Cita>()
                .ToTable("Cita", "Datos")
                .HasKey(c => c.id_Cita);
            modelBuilder.Entity<Cita>()
                .Ignore(c => c.Nombre);

            base.OnModelCreating(modelBuilder);
        }
    }
}
