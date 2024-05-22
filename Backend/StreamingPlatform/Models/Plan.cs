using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Models.Enums;

namespace StreamingPlatform.Models
{
    /// <summary>
    /// Represents a Plan.
    /// </summary>
    [Table("Plans")]
    [Index(nameof(PlanName), IsUnique = true)]

    public sealed class Plan
    {

        public Plan()
        {
            this.PlanName = string.Empty;
            this.MonthlyFee = 0;
            this.NumberOfMinutes = 0;
            this.Status = PlanStatus.Active;
        }

        public Plan(string planName, double montlyFee, int numberOfMinutes)
        {
            this.PlanId = Guid.NewGuid();
            this.PlanName = planName;
            this.MonthlyFee = montlyFee;
            this.NumberOfMinutes = numberOfMinutes;
            this.Status = PlanStatus.Active;
        }

        /// <summary>
        /// Represents an unique identifier for the Plan.
        /// </summary>
        [Key]
        public Guid PlanId { get; set; }

        /// <summary>
        /// Represents the name of the Plan.
        /// </summary>
        [Required(ErrorMessage = "Plan name is required.")]
        [MaxLength(50, ErrorMessage = "Plan name is too long. Max length is 50 characters.")]
        public string PlanName { get; set; }

        /// <summary>
        /// Represents the monthly fee of the Plan.
        /// </summary>
        /// 
        [Required(ErrorMessage = "Monthly fee is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Monthly fee must be greater than 0.")]
        public double MonthlyFee { get; set; }

        /// <summary>
        /// Represents the number of minutes of the Plan.
        /// </summary>
        [Required(ErrorMessage = "Number of minutes is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Number of minutes must be greater than 0.")]
        public int NumberOfMinutes { get; set; }

        /// <summary>
        /// Represents the status of the Plan. Can be Active or Inactive.
        /// </summary>
        [Required(ErrorMessage = "Status is required.")]
        [EnumDataType(typeof(PlanStatus), ErrorMessage = "Invalid status.")]
        public PlanStatus Status { get; set; }

    }
}
