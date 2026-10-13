using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Business.Services;
using BaseClinic.Business.Services.Auth.Commands;
using BaseClinic.Business.Settings;
using BaseClinic.DataAccess;
using BaseClinic.DataAccess.Repositories;
using BaseClinic.DataAccess.Services;
using BaseClinic.DataAccess.UnitOfWork;
using BaseClinic.Domain.Entities;
using BaseClinic.Presentation.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. ĐĂNG KÝ CÁC DỊCH VỤ CỐT LÕI (Infrastructure Services)
// ============================================================

// 1.1. Controllers + MemoryCache
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

// 1.2. HttpContextAccessor — cần thiết để truy cập HttpContext từ service (ví dụ CurrentUserService)
builder.Services.AddHttpContextAccessor();

// 1.3. Problem Details — chuẩn hóa response lỗi theo RFC 7807
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// ============================================================
// 2. CẤU HÌNH DATABASE (EF Core)
// ============================================================
builder.Services.AddDbContext<ClinicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions =>
    {
        // Chỉ định assembly chứa Migrations (tránh lỗi khi DbContext nằm ở project khác)
        sqlOptions.MigrationsAssembly(typeof(ClinicDbContext).Assembly.FullName);
    }));

// ============================================================
// 3. ĐĂNG KÝ MEDIATR (CQRS)
// ============================================================
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));

// Bind OcrOptions 
builder.Services.Configure<OcrOptions>(
    builder.Configuration.GetSection(OcrOptions.SectionName));

// ============================================================
// 4. ĐĂNG KÝ REPOSITORIES & UNIT OF WORK (DI)
// ============================================================

// 4.1. Các service
builder.Services.AddScoped<IAppointmentCodeGenerator, AppointmentCodeGenerator>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IDoctorCodeGenerator, DoctorCodeGenerator>();
builder.Services.AddScoped<IPatientCodeGenerator, PatientCodeGenerator>();
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddScoped<IOcrService, TesseractOcrService>();
builder.Services.AddScoped<ISlugGenerator, SlugGenerator>();
builder.Services.AddScoped<ISmsService, FakeSmsService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 4.2. Đăng ký Repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IEncounterRepository, EncounterRepository>();
builder.Services.AddScoped<IRepository<MergeRollbackRequest>, Repository<MergeRollbackRequest>>();
builder.Services.AddScoped<IOtpCodeRepository, OtpCodeRepository>();
builder.Services.AddScoped<IPatientDelegationRepository, PatientDelegationRepository>();
builder.Services.AddScoped<IRepository<PatientMergeLog>, Repository<PatientMergeLog>>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IQueueRepository, QueueRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IRoleRepository, RoleRepository>();



// ============================================================
// 5. CẤU HÌNH AUTHENTICATION / AUTHORIZATION
// ============================================================

// 5.1. Bind cấu hình JwtOptions từ appsettings.json

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtSettings"));

// 5.2. Đăng ký các service phục vụ Auth
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

// 5.3. Cấu hình Authentication với JwtBearer
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(options =>
{
    // Đặt JwtBearer làm scheme mặc định cho cả Authenticate / Challenge / Forbid
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
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

    // Tùy chỉnh response 401 để trả về ProblemDetails thay vì body rỗng
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

// 5.4. Authorization — khai báo tường minh để dễ mở rộng policy sau này
builder.Services.AddAuthorization();

// ============================================================
// 6. CẤU HÌNH CORS
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJsApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // URL của ứng dụng Next.js
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Bắt buộc nếu cần gửi cookie hoặc authorization header
        });
});

// ============================================================
// 7. CẤU HÌNH SWAGGER / OPENAPI
// ============================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Định nghĩa scheme Bearer để Swagger UI hiện nút "Authorize"
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",           // Header name phải là Authorization
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,   // Dùng Http scheme
        Scheme = "bearer",                // Chữ 'bearer' viết thường
        BearerFormat = "JWT"
    });

    // Yêu cầu scheme Bearer cho toàn bộ endpoint (Swagger UI sẽ tự đính token)
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

// ============================================================
// BUILD APP
// ============================================================

var app = builder.Build();

// ============================================================
// 8. CẤU HÌNH HTTP REQUEST PIPELINE (Middleware)
// ============================================================

// 8.1. Swagger chỉ bật ở môi trường Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 8.2. Redirect HTTP → HTTPS
app.UseHttpsRedirection();

// 8.3. Exception Handler — đặt sớm để bắt mọi exception từ các middleware phía sau
app.UseExceptionHandler();

// 8.4. CORS — phải chạy TRƯỚC Authentication/Authorization
app.UseCors("AllowNextJsApp");

// 8.5. Authentication & Authorization
app.UseAuthentication();
app.UseMiddleware<SecurityStampValidationMiddleware>();
app.UseAuthorization();

// 8.6. Map Controllers (đồng thời thêm routing)
app.MapControllers();

// ============================================================
// 9. MIGRATE DATABASE + SEED DỮ LIỆU KHI KHỞI ĐỘNG
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ClinicDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();

        // Đảm bảo database đã được tạo và update mới nhất
        await context.Database.MigrateAsync();

        // Gọi hàm quét và nạp quyền
        await BaseClinic.DataAccess.Seeding.DatabaseSeeder.SeedAllAsync(context, passwordHasher);
    }
    catch (Exception ex)
    {
        // Có thể thêm logger tại đây nếu cần ghi nhận lỗi khởi tạo
        Console.WriteLine($"Lỗi khi chạy Seeder: {ex.Message}");
    }
}

app.Run();
