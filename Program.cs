using SurveyNeeds.Learning;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/analyze", (SurveyResponse survey) =>
{
    List<string> needs = AnalyzeNeeds(survey.ResponseText);

    return Results.Ok(new
    {
        needs = needs
    });
});

app.Run();

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