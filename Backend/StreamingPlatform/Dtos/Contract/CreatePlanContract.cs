using System.ComponentModel.DataAnnotations;

namespace StreamingPlatform.Dtos.Contract
{
    public class CreatePlanContract
    {
        [Required(ErrorMessage = "Plan name is required.")]
        required public string PlanName { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Monthly fee must be greater than 0.")]
        [Required(ErrorMessage = "Monthly fee is required.")]
        public double MonthlyFee { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Number of minutes must be greater than 0.")]
        [Required(ErrorMessage = "Number of minutes is required.")]
        public int NumberOfMinutes { get; set; }
    }
}
