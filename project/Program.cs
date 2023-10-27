using Microsoft.EntityFrameworkCore;
using project.Models.Data;
using project.Repositories;
using project.Repository;
using project.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MyDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDBConnection"));
});//DB Connection Config.(MSSQL)

builder.Services.AddCors(options =>
{
    options.AddPolicy("Cors", p =>
        {
            p.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// ###Register your repository and service
builder.Services.AddScoped<IRecruitmentRepository, RecruitmentRepository>();
builder.Services.AddScoped<IRecruitmentService, RecruitmentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Cors");

app.UseAuthorization();

app.MapControllers();

app.Run();


//Repo @ Service