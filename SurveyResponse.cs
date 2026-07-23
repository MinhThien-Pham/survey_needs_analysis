namespace SurveyNeeds.Learning;

public class SurveyResponse
{
    public int Id { get; set; }
    public int HouseholdSize { get; set; }
    public string EmploymentStatus { get; set; } = "";
    public string ResponseText { get; set; } = "";
}