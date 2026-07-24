using MySql.Data.MySqlClient;
using SurveyNeeds.Learning;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string connectionString = builder.Configuration.GetConnectionString("SurveyNeeds") ?? throw new InvalidOperationException("Database connection string 'SurveyNeeds' was not found.");

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
    using var connection = new MySqlConnection(connectionString);
    connection.Open();

    string sql = "INSERT INTO surveys (household_size, employment_status, response_text) VALUES (@householdSize, @employmentStatus, @responseText);";
    using var command = new MySqlCommand(sql, connection);
    command.Parameters.AddWithValue("@householdSize", survey.HouseholdSize);
    command.Parameters.AddWithValue("@employmentStatus", survey.EmploymentStatus);
    command.Parameters.AddWithValue("@responseText", survey.ResponseText);
    command.ExecuteNonQuery();

    survey.Id = (int)command.LastInsertedId;
    return Results.Created($"/surveys/{survey.Id}", survey);
});


// Get all saved surveys.
app.MapGet("/surveys", () =>
{
    var surveys = new List<SurveyResponse>();

    using var connection = new MySqlConnection(connectionString);
    connection.Open();

    string sql = "SELECT id, household_size, employment_status, response_text FROM surveys;";
    using var command = new MySqlCommand(sql, connection);
    using MySqlDataReader reader = command.ExecuteReader();

    while (reader.Read())
    {
        var survey = new SurveyResponse
        {
            Id = reader.GetInt32("id"),
            HouseholdSize = reader.GetInt32("household_size"),
            EmploymentStatus = reader.GetString("employment_status"),
            ResponseText = reader.GetString("response_text")
        };
        surveys.Add(survey);
    }

    return Results.Ok(surveys);
});


// Get one survey by its ID.
app.MapGet("/surveys/{id:int}", (int id) =>
{
    using var connection = new MySqlConnection(connectionString);
    connection.Open();

    string sql = "SELECT id, household_size, employment_status, response_text FROM surveys WHERE id = @id;";
    using var command = new MySqlCommand(sql, connection);
    command.Parameters.AddWithValue("@id", id);
    using MySqlDataReader reader = command.ExecuteReader();

    if (!reader.Read())
        return Results.NotFound();

    var survey = new SurveyResponse
    {
        Id = reader.GetInt32("id"),
        HouseholdSize = reader.GetInt32("household_size"),
        EmploymentStatus = reader.GetString("employment_status"),
        ResponseText = reader.GetString("response_text")
    };

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