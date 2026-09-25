using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Services.Auth.Commands;
using BaseClinic.Business.Settings;
using BaseClinic.DataAccess;
using BaseClinic.DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure the database connection
builder.Services.AddDbContext<ClinicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions =>
    {
        // Trỏ đến project chứa file DbContext nếu nó khác với project Web khởi chạy
        sqlOptions.MigrationsAssembly(typeof(ClinicDbContext).Assembly.FullName);
    }));

// Đăng ký MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));

// Kích hoạt chuẩn Problem Details của .NET
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<BaseClinic.Presentation.Middlewares.GlobalExceptionHandler>();

// Đăng ký Repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDoctorCodeGenerator, DoctorCodeGenerator>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IEncounterRepository, EncounterRepository>();
builder.Services.AddScoped<IPatientCodeGenerator, PatientCodeGenerator>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IQueueRepository, QueueRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// 1. Đăng ký UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 2. Đăng ký cấu hình JwtOptions từ appsettings.json (Dùng cho JwtProvider của dự án cũ)
// Giả định class JwtOptions của bạn có hằng số SectionName = "JwtSettings"
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtSettings"));

// 3. Đăng ký các Service xử lý Auth
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

// 4. Cấu hình Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",           // Header name phải là Authorization
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,   // Dùng Http scheme
        Scheme = "bearer",                // Chữ 'bearer' viết thường
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

// 5. Đăng ký cấu hình Authentication và JwtBearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("Missing SecretKey")))
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            // Hủy bỏ response 401 rỗng mặc định của ASP.NET Core
            context.HandleResponse();

            // Tự thiết lập cấu trúc ProblemDetails
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var problemDetails = new
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Chưa xác thực.",
                Detail = "Vui lòng đăng nhập để sử dụng tính năng này."
            };

            // Ghi JSON ra luồng phản hồi
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ClinicDbContext>();

        // Đảm bảo database đã được tạo và update mới nhất
        await context.Database.MigrateAsync();

        // Gọi hàm quét và nạp quyền
        await BaseClinic.DataAccess.Seeding.PermissionSeeder.SeedPermissionsAsync(context);
    }
    catch (Exception ex)
    {
        // Có thể thêm logger tại đây nếu cần ghi nhận lỗi khởi tạo
        Console.WriteLine($"Lỗi khi chạy Seeder: {ex.Message}");
    }
}

app.Run();
