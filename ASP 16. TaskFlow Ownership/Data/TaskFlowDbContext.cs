using ASP_16._TaskFlow_Ownership.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP_16._TaskFlow_Ownership.Data;

public class TaskFlowDbContext : IdentityDbContext<ApplicationUser>
{
    public TaskFlowDbContext(DbContextOptions options) 
        : base(options)
    {}

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();

    // Fluent API

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Project
        modelBuilder.Entity<Project>(
            p =>
            {
                p.HasKey(p => p.Id);
                p.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);
                p.Property(p => p.Description)
                    .HasMaxLength(1000);
                p.Property(p => p.CreatedAt)
                    .IsRequired();
                p.Property(p => p.OwnerId)
                    .IsRequired()
                    .HasMaxLength(450);
                p.HasOne(p => p.Owner)
                    .WithMany()
                    .HasForeignKey(p => p.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
            );


        // TaskItem
        modelBuilder.Entity<TaskItem>(
            t =>
            {
                t.HasKey(t => t.Id);
                t.Property(t => t.Title)
                    .IsRequired()
                    .HasMaxLength(200);
                t.Property(t => t.Description)
                    .HasMaxLength(1000);
                t.Property(t => t.CreatedAt)
                    .IsRequired();
                t.Property(t => t.Status)
                   .IsRequired();
                t.Property(t => t.Priority)
                    .IsRequired();

                t.HasOne(t => t.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
            );

        // RefreshToken
        modelBuilder.Entity<RefreshToken>(
            rt =>
            {
                rt.HasKey(rt => rt.Id);
                rt.HasIndex(rt => rt.JwtId).IsUnique();
                rt.Property(rt => rt.JwtId)
                     .IsRequired()
                     .HasMaxLength(64);
                rt.Property(rt => rt.UserId)
                     .IsRequired()
                     .HasMaxLength(450);
            }
            );

        modelBuilder.Entity<ProjectMember>(
            m=>
            {
                m.HasKey(m => new { m.ProjectId, m.UserId });
                m.HasOne(m => m.Project)
                    .WithMany(p => p.Members)
                    .HasForeignKey(m => m.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
                m.HasOne(m => m.User)
                    .WithMany(u => u.ProjectMemberships)
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                m.Property(m => m.UserId)
                    .HasMaxLength(450);
            }
            );


    }
}
