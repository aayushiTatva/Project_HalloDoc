using System;
using System.Collections.Generic;
using Assignment.DBEntity.DataModels;
using Microsoft.EntityFrameworkCore;
using Task = Assignment.DBEntity.DataModels.Task;

namespace Assignment.DBEntity.DataContext;

public partial class TaskManagementSystemContext : DbContext
{
    public TaskManagementSystemContext()
    {
    }

    public TaskManagementSystemContext(DbContextOptions<TaskManagementSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID = postgres;Password=Aayushi03;Server=localhost;Port=5432;Database=TaskManagementSystemDB;Integrated Security=true;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Category_pkey");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Task_pkey");

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.Tasks).HasConstraintName("Task_CategoryId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
