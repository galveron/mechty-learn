using System.Text.Json.Serialization;
using Mechty_learn_backend.Data;
using Mechty_learn_backend.Models;
using Mechty_learn_backend.Repositories;
using Mechty_learn_backend.Repositories.EducationRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var config =
    new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .Build();

var builder = WebApplication.CreateBuilder(args);
var connectionString = config["ConnectionString"] ?? config["CONNECTIONSTRING"] ?? Environment.GetEnvironmentVariable("CONNECTIONSTRING");
Console.WriteLine("CS:" + Environment.GetEnvironmentVariable("CONNECTIONSTRING"));
Console.WriteLine("cs:" + config["ConnectionString"]);
Console.WriteLine("cs2:" + config["CONNECTIONSTRING"]);
ConfigureSwagger();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IKidsRepository, KidsRepository>();
builder.Services.AddTransient<IAdultsRepository, AdultsRepository>();
builder.Services.AddTransient<ILessonRepository, LessonRepository>();
builder.Services.AddTransient<IChapterRepository, ChapterRepository>();
builder.Services.AddTransient<IChapterPageRepository, ChapterPageRepository>();
builder.Services.AddTransient<IChapterPageSoundRepository, ChapterPageSoundRepository>();
builder.Services.AddTransient<IChapterPageTextRepository, ChapterPageTextRepository>();
builder.Services.AddTransient<IChapterPage3DModeRepository, ChapterPage3DModelRepository>();
builder.Services.AddTransient<IProgressRepository, ProgressRepository>();
builder.Services.AddDbContext<ApplicationDbContext>((options) =>
    options.UseSqlServer(connectionString));
Console.WriteLine(connectionString);

AddIdentity();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

void ConfigureSwagger()
{
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });
    });
}

void AddIdentity()
{
    builder.Services
        .AddIdentityCore<Adult>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>();
}
