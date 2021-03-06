using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MeteoAlert.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeteoAlert.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<EmailMessage> EmailMessages { get; set; }

        public DbSet<Rubrica> Rubrica { get; set; }

        public DbSet<ContattoInviato> ContattoInviato { get; set; }

        public DbSet<Bollettino> Bollettino { get; set; }

        public DbSet<Allerta> Allerta { get; set; }
        public DbSet<OriginalMail> OriginalMail { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OriginalMail>().Property(m => m.Id).HasColumnType("UNIQUEIDENTIFIER ROWGUIDCOL").IsRequired();
            modelBuilder.Entity<OriginalMail>().Property(t => t.Mail).HasColumnType("VARBINARY(MAX) FILESTREAM");
            modelBuilder.Entity<EmailMessage>()
            .HasOne(b => b.OriginalMail)
            .WithOne(i => i.EmailMessage)
            .HasForeignKey<OriginalMail>(b => b.EmailMessageId);


            modelBuilder.Entity<Attachment>().Property(m => m.Id).HasColumnType("UNIQUEIDENTIFIER ROWGUIDCOL").IsRequired();
            modelBuilder.Entity<Attachment>().Property(t => t.Content).HasColumnType("VARBINARY(MAX) FILESTREAM");


            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.ClientSetNull;

            base.OnModelCreating(modelBuilder);
        }
    }
}
