using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using System.ComponentModel.DataAnnotations;

namespace MediAI.Data
{
    public abstract class User:IdentityUser
    {
        private readonly ApplicationDbContext? db;
        /*public User()
        {

        }*/

        protected User()
        {
            
        }

        public User(ApplicationDbContext context)
        {
            this.db = context;
        }

        [Required]
        [StringLength(100)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string? LastName { get; set; }

        [NotMapped]
        public string? FullName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName))
                    return FirstName.Trim() + " " + LastName.Trim();
                else //if(!string.IsNullOrWhiteSpace(this.Id))
                {
                    return this.Email;
                }
                //return string.Empty;
            }
        }

        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;        
        public UserType UserType { get; set; }

        public Patient ToPatient()
        {
            return new Patient(db)
            {
                Id = this.Id,
                AccessFailedCount = this.AccessFailedCount,                
                ConcurrencyStamp = this.ConcurrencyStamp,
                CreatedAt = this.CreatedAt,
                Email = this.Email,
                EmailConfirmed = this.EmailConfirmed,               
                FirstName = this.FirstName,
                LastName = this.LastName,
                //FullName = this.FullName,
                IsActive = this.IsActive,
                LockoutEnabled = this.LockoutEnabled,
                LockoutEnd = this.LockoutEnd,
                //Name=this.Name,
                NormalizedEmail = this.NormalizedEmail,
                PasswordHash = this.PasswordHash,
                NormalizedUserName = this.NormalizedUserName,
                PhoneNumber = this.PhoneNumber,
                PhoneNumberConfirmed = this.PhoneNumberConfirmed,
                ProfileImageUrl = this.ProfileImageUrl,
                SecurityStamp = this.SecurityStamp,                
                TwoFactorEnabled = this.TwoFactorEnabled,
                UserName = this.UserName,               
            };
        }

        public Doctor ToDoctor()
        {
            return new Doctor(db)
            {
                Id = this.Id,
                AccessFailedCount = this.AccessFailedCount,                
                ConcurrencyStamp = this.ConcurrencyStamp,
                CreatedAt = this.CreatedAt,
                Email = this.Email,
                EmailConfirmed = this.EmailConfirmed,                
                FirstName = this.FirstName,
                LastName = this.LastName,
                //FullName = this.FullName,
                IsActive = this.IsActive,
                LockoutEnabled = this.LockoutEnabled,
                LockoutEnd = this.LockoutEnd,
                //Name=this.Name,
                NormalizedEmail = this.NormalizedEmail,
                PasswordHash = this.PasswordHash,
                NormalizedUserName = this.NormalizedUserName,
                PhoneNumber = this.PhoneNumber,
                PhoneNumberConfirmed = this.PhoneNumberConfirmed,
                ProfileImageUrl = this.ProfileImageUrl,
                SecurityStamp = this.SecurityStamp,                
                TwoFactorEnabled = this.TwoFactorEnabled,
                UserName = this.UserName,                
            };
        }
    }

    public enum Gender
    {
        Male,
        Female
    }

    public class Patient : User
    {
        public Patient()
        {
            
        }

        public Patient(ApplicationDbContext dbContext):base(dbContext)
        {
            
        }

        public string? BloodType { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string? Allergies { get; set; }
        public string? ChronicDiseases { get; set; }
        public string? InsuranceCompany { get; set; }
        public string? InsurancePolicyNumber { get; set; }

        public ICollection<Diagnosis>? GetDiagnosis()
        {
            return new List<Diagnosis>();
        }

        public ICollection<MedicalReport>? MedicalReports { get; set; }
    }

    public class Doctor : User
    {
        public Doctor()
        {
            
        }

        public Doctor(ApplicationDbContext dbContext):base(dbContext)
        {
            
        }

        [StringLength(50)]
        public string? LicenseNumber { get; set; }

        public int? YearsOfExperience { get; set; }

        public ICollection<Diagnosis>? GetDiagnosiss()
        {
            return new List<Diagnosis>();
        }
        public ICollection<MedicalReport>? MedicalReports { get; set; }
    }

    public class Admin : User
    {
        public Admin(ApplicationDbContext dbContext) : base(dbContext)
        {
            
        }

        public AccessLevel AccessLevel { get; set; }
    }

    public enum AccessLevel
    {
        Full,
        Partial,
        ReadOnly
    }

    public enum UserType
    {
        Patient,
        Doctor,
        Admin
    }

    public class ApplicationDbContext : IdentityDbContext//<User, IdentityRole<string>, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Admin> Admins { get; set; }        
        public DbSet<Diagnosis> Diagnosis { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }        
        public DbSet<MedicalReport> MedicalReports { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This is crucial for Identity to work

            // Configure inheritance
            /*modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Patient>(UserType.Patient)
                .HasValue<Doctor>(UserType.Doctor)
                .HasValue<Admin>(UserType.Admin);

            // Configure relationships
            modelBuilder.Entity<Diagnosis>()
                .HasOne(d => d.Patient)
                .WithMany(p => p.Diagnosiss)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Diagnosis>()
                .HasOne(d => d.Doctor)
                .WithMany(d => d.Diagnosiss)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add indexes
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.LicenseNumber)
                .IsUnique();

            // Configure Identity table names (optional)
            modelBuilder.Entity<User>().ToTable("AspNetUsers");
            modelBuilder.Entity<IdentityRole<string>>().ToTable("AspNetRoles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins").HasKey(l => new { l.LoginProvider, l.ProviderKey });
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens").HasKey(t => new { t.UserId, t.LoginProvider, t.Name });*/

            modelBuilder.Entity<DiagnosisSymptom>()
                .HasKey(ds => new { ds.DiagnosisId, ds.SymptomId });

            modelBuilder.Entity<DiagnosisSymptom>()
                .HasOne(ds => ds.Diagnosis)
                .WithMany(d => d.DiagnosisSymptoms)
                .HasForeignKey(ds => ds.DiagnosisId);

            modelBuilder.Entity<DiagnosisSymptom>()
                .HasOne(ds => ds.Symptom)
                .WithMany(s => s.DiagnosisSymptoms)
                .HasForeignKey(ds => ds.SymptomId);

            modelBuilder.Entity<Diagnosis>()
                .Property(d => d.ConfidenceLevel)
                .HasPrecision(10, 6); // مثال: 10 أرقام إجمالية، 6 بعد الفاصلة
        }
    }
}
