using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NetBB.Domain.Domains.Post;
using NetBB.Domain.Domains.User;
using NetBB.System.EventBus.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Infrastructure.Repositories
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> dbContextOptions) : DbContext(dbContextOptions), IDataProtectionKeyContext
    {
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;
        public DbSet<DatabaseCacheItem> DatabaseCacheItems { get; set; } = null!;
        public DbSet<AuthTicket> AuthTickets { get; set; } = null!;

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostHistory> PostHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataProtectionKey>(
                eb =>
                {
                    eb.ToTable("internal_data_protection");
                    eb.Property(u => u.Id).HasColumnName("key_id");
                    eb.Property(u => u.FriendlyName).HasColumnName("friendly_name");
                    eb.Property(u => u.Xml).HasColumnName("xml");
                    eb.HasKey(u => u.Id);
                }
                );
            modelBuilder.Entity<DatabaseCacheItem>(
                eb =>
                {
                    eb.ToTable("cache");
                    eb.Property(u => u.Id).HasColumnName("cache_id");
                    eb.Property(u => u.Key).HasColumnName("cache_key");
                    eb.Property(u => u.Value).HasColumnName("cache_value");
                    eb.Property(u => u.TimeStarted).HasColumnName("time_started");
                    eb.Property(u => u.TimeExpired).HasColumnName("time_expired");
                    eb.HasKey(u => u.Id);
                    eb.HasIndex(u => u.Key);
                });
            modelBuilder.Entity<AuthTicket>(
                eb =>
                {
                    eb.ToTable("auth_ticket");
                    eb.Property(u => u.Id).HasColumnName("id");
                    eb.Property(u => u.AuthScheme).HasColumnName("auth_scheme");
                    eb.Property(u => u.AuthKey).HasColumnName("auth_key");
                    eb.Property(u => u.TicketValue).HasColumnName("ticket_value");
                    eb.Property(u => u.UserId).HasColumnName("user_id");
                    eb.Property(u => u.LoginId).HasColumnName("login_id");
                    eb.Property(u => u.TimeStarted).HasColumnName("time_started");
                    eb.Property(u => u.TimeExpired).HasColumnName("time_expired");
                    eb.HasKey(u => u.Id);
                }
                );
            
            //===========================================================================================

            modelBuilder.Entity<User>(
                eb =>
                {
                    eb.ToTable("user");
                    eb.Property(u => u.Id).HasColumnName("user_id").IsRequired();
                    eb.Property(u => u.UserName).HasColumnName("username").IsRequired();
                    eb.Property(u => u.Password).HasColumnName("password").IsRequired();
                    eb.Property(u => u.NickName).HasColumnName("nickname").IsRequired();
                    eb.Property(u => u.Email).HasColumnName("email").IsRequired();
                    eb.Property(u => u.TimeCreated).HasColumnName("time_created").IsRequired();
                    eb.Property(u => u.TimeUpdated).HasColumnName("time_updated").IsRequired();
                    eb.HasKey(u => u.Id);
                }
                );
            modelBuilder.Entity<Post>(
                eb =>
                {
                    eb.ToTable("posts");
                    eb.Property(u => u.PostId).HasColumnName("post_id").IsRequired();
                    eb.Property(u => u.AuthorId).HasColumnName("author_id").IsRequired();
                    eb.Property(u => u.PostType).HasColumnName("post_type").IsRequired();
                    eb.Property(u => u.PostTitle).HasColumnName("title").IsRequired();
                    eb.Property(u => u.PostContent).HasColumnName("content").IsRequired();
                    eb.Property(u => u.TimeCreated).HasColumnName("time_created").IsRequired();
                    eb.Property(u => u.TimeUpdated).HasColumnName("time_updated").IsRequired();
                    eb.HasKey(u => u.PostId);
                }
                );
            modelBuilder.Entity<PostHistory>(
                eb =>
                {
                    eb.ToTable("post_history");
                    eb.Property(u => u.PostHistoryId).HasColumnName("post_history_id").IsRequired();
                    eb.Property(u => u.PostId).HasColumnName("post_id").IsRequired();
                    eb.Property(u => u.AuthorId).HasColumnName("author_id").IsRequired();
                    eb.Property(u => u.PostContentType).HasColumnName("post_type").IsRequired();
                    eb.Property(u => u.Title).HasColumnName("title").IsRequired();
                    eb.Property(u => u.Content).HasColumnName("content").IsRequired();
                    eb.Property(u => u.TimeCreated).HasColumnName("time_created").IsRequired();
                    eb.HasKey(u => u.PostHistoryId);
                }
                );
        }

        public DatabaseFacade RawDatabase()
        {
            return this.Database;
        }
    }
}
