using SurveyNeeds.Learning;

// Create one survey object.
var survey = new SurveyResponse
{
    Id = 1,
    HouseholdSize = 4,
    EmploymentStatus = "part-time",
    ResponseText = "i lost my job and cannot afford food or electricity"
};

// Pass the survey text into our analysis function.
List<string> needs = AnalyzeNeeds(survey.ResponseText);

// Print the original survey.
Console.WriteLine("Survey:");
Console.WriteLine(survey.ResponseText);
Console.WriteLine("\nDetected needs:");
// Print every detected need.
foreach (string need in needs)
{
    Console.WriteLine(need);
}

// Takes survey text as input.
// Returns a list of detected needs.
static List<string> AnalyzeNeeds(string text)
{
    // Start with an empty result list.
    var needs = new List<string>();

    // Simple hard-coded rules.
    if (text.Contains("food"))
        needs.Add("food_assistance");

    if (text.Contains("job"))
        needs.Add("employment_support");

    if (text.Contains("electricity"))
        needs.Add("utility_assistance");

    if (text.Contains("car"))
        needs.Add("transportation_support");

    return needs;
}