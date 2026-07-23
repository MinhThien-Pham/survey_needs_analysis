using SurveyNeeds.Learning;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory storage.
// Data exists only while the server is running.
var surveys = new List<SurveyResponse>();
var nextId = 1;

app.MapPost("/analyze", (SurveyResponse survey) =>
{
    List<string> needs = AnalyzeNeeds(survey.ResponseText);

    return Results.Ok(new
    {
        needs = needs
    });
});

app.MapPost("/surveys", (SurveyResponse survey) =>
{
    survey.Id = nextId;
    nextId++;

    surveys.Add(survey);

    return Results.Created($"/surveys/{survey.Id}", survey);
});


// Get all saved surveys.
app.MapGet("/surveys", () =>
{
    return Results.Ok(surveys);
});


// Get one survey by its ID.
app.MapGet("/surveys/{id:int}", (int id) =>
{
    SurveyResponse? survey =
        surveys.FirstOrDefault(s => s.Id == id);

    if (survey is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(survey);
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