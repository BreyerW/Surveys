using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Surveys.Model
{
    public partial class surveyContext : DbContext
    {
        public surveyContext()
        {
        }

        public surveyContext(DbContextOptions<surveyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PredefinedAnswer> PredefinedAnswers { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SubmittedSurvey> SubmittedSurveys { get; set; }
        public virtual DbSet<SubmittedSurveyAnswer> SubmittedSurveyAnswers { get; set; }
        public virtual DbSet<Survey> Surveys { get; set; }
        public virtual DbSet<SurveysContent> SurveysContents { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-TOTDOGM\\SQLEXPRESS;Database=survey;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Polish_CI_AS");

            modelBuilder.Entity<PredefinedAnswer>(entity =>
            {
                entity.ToTable("predefined_answers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Answer).HasColumnName("answer");

                entity.Property(e => e.IdQuestion).HasColumnName("id_question");

                entity.HasOne(d => d.IdQuestionNavigation)
                    .WithMany(p => p.PredefinedAnswers)
                    .HasForeignKey(d => d.IdQuestion)
                    .HasConstraintName("FK_predefined_answers_questions");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("questions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Question1).HasColumnName("question");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("role_name");
            });

            modelBuilder.Entity<SubmittedSurvey>(entity =>
            {
                entity.ToTable("submitted_surveys");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdSurvey).HasColumnName("id_survey");

                entity.HasOne(d => d.IdSurveyNavigation)
                    .WithMany(p => p.SubmittedSurveys)
                    .HasForeignKey(d => d.IdSurvey)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_submitted_surveys_surveys");
            });

            modelBuilder.Entity<SubmittedSurveyAnswer>(entity =>
            {
                entity.HasKey(table => new
                {
                    table.IdSubmittedSurvey,
                    table.IdQuestion
                });

                entity.ToTable("submitted_survey_answers");

                entity.Property(e => e.Answers).HasColumnName("answers");

                entity.Property(e => e.Hash)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasColumnName("hash");

                entity.Property(e => e.IdQuestion).HasColumnName("id_question");

                entity.Property(e => e.IdSubmittedSurvey).HasColumnName("id_submitted_survey");

                entity.HasOne(d => d.IdQuestionNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.IdQuestion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_submitted_survey_answers_questions");

                entity.HasOne(d => d.IdSubmittedSurveyNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.IdSubmittedSurvey)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_submitted_survey_answers_submitted_surveys");
            });

            modelBuilder.Entity<Survey>(entity =>
            {
                entity.ToTable("surveys");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.MaxBirthYear).HasColumnName("max_birth_year");

                entity.Property(e => e.MinBirthYear).HasColumnName("min_birth_year");

                entity.Property(e => e.Sex)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("sex");

                entity.Property(e => e.Topic).HasColumnName("topic");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Surveys)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_surveys_users");
            });

            modelBuilder.Entity<SurveysContent>(entity =>
            {
                entity.HasKey(table => new
                {
                    table.IdSurvey,
                    table.IdQuestion
                });

                entity.ToTable("surveys_content");

                entity.Property(e => e.AllowMultipleAnswers).HasColumnName("allow_multiple_answers");

                entity.Property(e => e.IdQuestion).HasColumnName("id_question");

                entity.Property(e => e.IdSurvey).HasColumnName("id_survey");

                entity.Property(e => e.Required).HasColumnName("required");

                entity.HasOne(d => d.IdQuestionNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.IdQuestion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_surveys_content_questions");

                entity.HasOne(d => d.IdSurveyNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.IdSurvey)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_surveys_content_surveys");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BirthYear)
                    .IsUnicode(false)
                    .HasColumnName("birth_year");

                entity.Property(e => e.IdRole).HasColumnName("id_role");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Sex)
                    .IsUnicode(false)
                    .HasColumnName("sex");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("username");

                entity.HasOne(d => d.IdRoleNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.IdRole)
                    .HasConstraintName("FK_users_roles");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
