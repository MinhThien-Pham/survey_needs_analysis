using Microsoft.EntityFrameworkCore;
using SurveyNeeds.Learning;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("SurveyNeeds") ?? throw new InvalidOperationException("Database connection string 'SurveyNeeds' was not found.");
builder.Services.AddDbContext<SurveyNeedsContext>(options => options.UseMySQL(connectionString));
var app = builder.Build();

app.MapPost("/analyze", (SurveyResponse survey) =>
{
    List<string> needs = AnalyzeNeeds(survey.ResponseText);

    return Results.Ok(new
    {
        needs = needs
    });
});

app.MapPost("/surveys", (SurveyResponse survey, SurveyNeedsContext db) =>
{
    db.Surveys.Add(survey);
    db.SaveChanges();
    return Results.Created($"/surveys/{survey.Id}", survey);
});


// Get all saved surveys.
app.MapGet("/surveys", (SurveyNeedsContext db) =>
{
    List<SurveyResponse> surveys = db.Surveys.ToList();
    return Results.Ok(surveys);
});


// Get one survey by its ID.
app.MapGet("/surveys/{id:int}", (int id, SurveyNeedsContext db) =>
{
    SurveyResponse? survey = db.Surveys.Find(id);
    if (survey == null)
        return Results.NotFound();

    return Results.Ok(survey);
});

// Load a saved survey from MySQL and analyze its text.
app.MapPost("/surveys/{id:int}/analyze", (int id, SurveyNeedsContext db) =>
{
    // Find the saved survey by its primary key.
    SurveyResponse? survey = db.Surveys.Find(id);

    if (survey is null)
        return Results.NotFound();

    // Reuse the rule-based analysis from Step 1.
    List<string> needs = AnalyzeNeeds(survey.ResponseText);

    return Results.Ok(new
    {
        surveyId = survey.Id,
        needs
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