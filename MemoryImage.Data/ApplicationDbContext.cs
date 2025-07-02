using Microsoft.EntityFrameworkCore;
using MemoryImage.Models;

namespace MemoryImage.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration - không thay đổi
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Bio).IsRequired(false);
                entity.Property(e => e.ProfilePicture).HasDefaultValue("/images/pf.png");
                entity.Property(e => e.IsAdmin).HasDefaultValue(false);
            });
            
            // Friendship configuration
            modelBuilder.Entity<Friendship>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Khi User bị xóa, Friendship do họ gửi sẽ bị xóa
                entity.HasOne(f => f.Requester)
                      .WithMany(u => u.SentFriendRequests)
                      .HasForeignKey(f => f.RequesterId)
                      .OnDelete(DeleteBehavior.Cascade); 
                      
                // Khi User bị xóa, Friendship họ nhận sẽ KHÔNG tự động xóa để tránh lỗi chu trình
                entity.HasOne(f => f.Receiver)
                      .WithMany(u => u.ReceivedFriendRequests)
                      .HasForeignKey(f => f.ReceiverId)
                      .OnDelete(DeleteBehavior.NoAction);
                      
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => new { e.RequesterId, e.ReceiverId }).IsUnique();
            });

            // Post configuration
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Content).HasMaxLength(500).IsRequired(false);
                entity.Property(e => e.ImageUrl).IsRequired(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                
                // Khi User bị xóa, các Post của họ cũng bị xóa
                entity.HasOne(p => p.User)
                    .WithMany(u => u.Posts) // Chỉ định rõ navigation property
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Like configuration
            modelBuilder.Entity<Like>(entity =>
            {
                 // Khi Post bị xóa, Like liên quan cũng bị xóa
                entity.HasOne(l => l.Post)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(l => l.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Khi User (người thực hiện like) bị xóa, KHÔNG tự động xóa Like để tránh lỗi
                entity.HasOne(l => l.User)
                    .WithMany(u => u.Likes)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            
            // Comment configuration
            modelBuilder.Entity<Comment>(entity =>
            {
                // Khi Post bị xóa, Comment liên quan cũng bị xóa
                entity.HasOne(c => c.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(c => c.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Khi User (người thực hiện comment) bị xóa, KHÔNG tự động xóa Comment để tránh lỗi
                entity.HasOne(c => c.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
