using StreamingPlatform.Models.Enums;

namespace StreamingPlatform.Dtos.Response
{
    public class PlanResponse(string name, double monthlyFee, int numberOfMinutes, PlanStatus status)
    {
        public string PlanName { get; set; } = name;

        public double MonthlyFee { get; set; } = monthlyFee;

        public int NumberOfMinutes { get; set; } = numberOfMinutes;

        public PlanStatus Status { get; set; } = status;
    }
}
