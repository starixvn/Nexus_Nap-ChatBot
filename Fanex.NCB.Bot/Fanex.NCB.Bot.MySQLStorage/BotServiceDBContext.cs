using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Fanex.NCB.Bot.Models.DataDomain;
using Fanex.NCB.Bot.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Fanex.NCB.Bot.MySQLStorage
{
    public class BotServiceDBContext : DbContext
    {
        public BotServiceDBContext(DbContextOptions<BotServiceDBContext> options)
             : base(options)
        {
        }
        public DbSet<BotConversation> BotConversations { get; set; }
        // public DbSet<BotConversationMember> ConversationMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BotConversation>().ToTable("BotConversation");
            modelBuilder.Entity<BotConversation>(entity =>
            {
            entity.HasKey(e => new { e.GroupID, e.ConversationID });
            entity.Property(e => e.Name).HasColumnType("VARCHAR(500)").IsRequired();
            entity.Property(e => e.IsBotActive).HasColumnType("BIT(1)").IsRequired();
            entity.Property(e => e.BotName).HasColumnType("VARCHAR(50)").IsRequired();
            entity.Property(e => e.LastUpdate).IsRequired();
            entity.Property(e => e.ChannelID).HasColumnType("VARCHAR(45)").IsRequired(false);
                entity.Property(e => e.GroupID).HasColumnType("INT(11)")
                .ValueGeneratedOnAdd().IsRequired();
                entity.Property(e => e.ConversationID).HasColumnType("VARCHAR(70)").IsRequired();
                entity.Property(e => e.ServiceUrl).HasColumnType("VARCHAR(500)").IsRequired(false);
            });

            /* modelBuilder.Entity<BotConversationMember>(entity =>
             {
                 entity.HasKey(e => e.ISBN);
                 entity.Property(e => e.Title).IsRequired();
                 entity.HasOne(d => d.Publisher)
                   .WithMany(p => p.Books);
             });*/
        }
    }
}
