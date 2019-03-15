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
        public DbSet<BotConversationMember> BotConversationMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BotConversation>().ToTable("BotConversation");
            modelBuilder.Entity<BotConversation>(entity =>
            {
                entity.HasKey(e => new { e.GroupID });
                entity.Property(e => e.Name).HasColumnType("VARCHAR(500)").IsRequired();
                entity.Property(e => e.UpdatedBy).HasColumnType("VARCHAR(50)").IsRequired();
                entity.Property(e => e.LastUpdate).IsRequired();
                entity.Property(e => e.ChannelID).HasColumnType("VARCHAR(45)").IsRequired(false);
                entity.Property(e => e.GroupID).HasColumnType("INT(11)")
                .ValueGeneratedOnAdd().IsRequired();
                entity.Property(e => e.ConversationID).HasColumnType("VARCHAR(70)").IsRequired();
                entity.Property(e => e.ServiceUrl).HasColumnType("VARCHAR(500)").IsRequired(false);
            });

            modelBuilder.Entity<BotConversationMember>().ToTable("BotCoversationMember");
            modelBuilder.Entity<BotConversationMember>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasColumnType("INT(11)").IsRequired();
                entity.Property(e => e.Name).HasColumnType("VARCHAR(50)").IsRequired();
                entity.Property(e => e.LastUpdate).IsRequired();
                entity.Property(e => e.IsBot).HasColumnType("BIT(1)").IsRequired();
                entity.Property(e => e.IsActive).HasColumnType("BIT(1)").IsRequired();
                entity.Property(e => e.ConversationID).HasColumnType("VARCHAR(70)").IsRequired();
                entity.Property(e => e.MemberId).HasColumnType("VARCHAR(70)").IsRequired();
                entity.Property(e => e.UpdatedBy).HasColumnType("VARCHAR(50)");

            });
        }
    }
}
