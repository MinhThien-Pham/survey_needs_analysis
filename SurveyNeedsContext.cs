using Microsoft.EntityFrameworkCore;

namespace SurveyNeeds.Learning;

public class SurveyNeedsContext : DbContext
{
    public SurveyNeedsContext(DbContextOptions<SurveyNeedsContext> options) : base(options) { }

    public DbSet<SurveyResponse> Surveys => Set<SurveyResponse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SurveyResponse>(entity =>
        {
            // C# SurveyResponse maps to MySQL table "surveys".
            entity.ToTable("surveys");

            // Primary key.
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(s => s.HouseholdSize).HasColumnName("household_size").IsRequired();
            entity.Property(s => s.EmploymentStatus).HasColumnName("employment_status").HasMaxLength(100).IsRequired();
            entity.Property(s => s.ResponseText).HasColumnName("response_text").HasColumnType("text").IsRequired();
        });
    }
}