using CourseManager.Data;
using CourseManager.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Scoped builder lines:
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SubjectService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<EnrollmentService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<NotificationService>();


builder.Services.AddHostedService<CourseManager.BackgroundServices.NotificationBackgroundService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed the database if empty
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DatabaseSeeder.Seed(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();