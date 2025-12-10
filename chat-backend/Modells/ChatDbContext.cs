using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUserJoin> GroupUserJoins { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Group>()
                .HasOne(g => g.Creator)
                .WithMany()
                .HasForeignKey("CreatorId");

            builder.Entity<Message>()
                .HasOne(g => g.Sender)
                .WithMany()
                .HasForeignKey("SenderId");
            builder.Entity<Message>()
                .HasOne(g => g.Group)
                .WithMany()
                .HasForeignKey("GroupId");

            builder.ApplyConfiguration(new GroupEntityTypeConfiguration());
            builder.ApplyConfiguration(new UserEntityTypeConfiguration());
        }
    }

    public class GroupEntityTypeConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> entity)
        {
            entity.HasIndex(e => e.Id)
               .IsUnique();
        }
    }
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity
                .HasMany(p => p.Groups)
                .WithMany(p => p.Members)
                .UsingEntity<GroupUserJoin>(
                    j => j
                        .HasOne(pt => pt.Group)
                        .WithMany(t => t.MembersJoined)
                        .HasForeignKey(pt => pt.UserId),
                    j => j
                        .HasOne(pt => pt.User)
                        .WithMany(p => p.GroupsJoined)
                        .HasForeignKey(pt => pt.GroupId),
                    j => j
                        .HasKey(t => new { t.GroupId, t.UserId })
                );
        }
    }
}
