using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Domain.Constants;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Doctors.Commands
{
    public record CreateDoctorCommand(
        // Account
        string FullName,
        string PhoneNumber,
        string? Email,
        string Password,

        // Doctor
        Guid DepartmentId,
        string? Slug,           // null = auto-generate từ FullName
        string? Avatar,
        string? FullAddress,
        bool? Gender,
        DateTime? DateOfBirth,
        string? IdentityNumber,
        string? LicenseNumber,
        decimal? ConsultationFee,
        string? Description
    ) : IRequest<Guid>;

    public class CreateDoctorCommandHandler
        : IRequestHandler<CreateDoctorCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IDoctorCodeGenerator _doctorCodeGenerator;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISlugGenerator _slugGenerator;

        public CreateDoctorCommandHandler(
            IUnitOfWork unitOfWork,
            IAccountRepository accountRepo,
            IRoleRepository roleRepo,
            IDoctorCodeGenerator doctorCodeGenerator,
            IDoctorRepository doctorRepo,
            IDepartmentRepository departmentRepo,
            IPasswordHasher passwordHasher,
            ISlugGenerator slugGenerator)
        {
            _unitOfWork = unitOfWork;
            _accountRepo = accountRepo;
            _roleRepo = roleRepo;
            _doctorCodeGenerator = doctorCodeGenerator;
            _doctorRepo = doctorRepo;
            _departmentRepo = departmentRepo;
            _passwordHasher = passwordHasher;
            _slugGenerator = slugGenerator;
        }

        public async Task<Guid> Handle(
            CreateDoctorCommand request,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // ==================== BƯỚC 1: Validate ====================
                // Check Department tồn tại + active
                var department = await _departmentRepo.GetByIdAsync(
                    request.DepartmentId, cancellationToken)
                    ?? throw new NotFoundException("Không tìm thấy khoa.");

                if (!department.IsActive)
                    throw new DomainException(
                        "Không thể tạo bác sĩ cho khoa đang tạm ngưng.");

                // Check SĐT chưa tồn tại
                var exists = await _accountRepo.ExistsByPhoneAsync(
                    request.PhoneNumber, cancellationToken);

                if (exists)
                    throw new DomainException(
                        "Số điện thoại này đã được sử dụng cho một tài khoản khác.");

                // ==================== BƯỚC 2: Tạo Account ====================
                var hashedPassword = _passwordHasher.Hash(request.Password);

                var account = new Account(
                    fullName: request.FullName,
                    phoneNumber: request.PhoneNumber,
                    passwordHash: hashedPassword,
                    email: request.Email);

                account.VerifyPhoneNumber();
                account.Activate();

                _accountRepo.Add(account);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // ==================== BƯỚC 3: Gán Role Doctor ====================
                var doctorRole = await _roleRepo.GetByCodeAsync(
                    SystemRoles.Doctor, cancellationToken)
                    ?? throw new DomainException(
                        $"Role '{SystemRoles.Doctor}' chưa được khởi tạo.");

                account.AssignRole(doctorRole.Id);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // ==================== BƯỚC 4: Sinh DoctorCode + Slug ====================
                string doctorCode = await _doctorCodeGenerator
                    .GenerateAsync(cancellationToken);

                // Slug: ưu tiên input, fallback về FullName
                var slugSource = !string.IsNullOrWhiteSpace(request.Slug)
                    ? request.Slug
                    : request.FullName;

                var uniqueSlug = await _slugGenerator.GenerateUniqueDoctorSlugAsync(
                    input: slugSource,
                    excludeId: null,
                    cancellationToken: cancellationToken);

                // ==================== BƯỚC 5: Tạo Doctor ====================
                var doctor = new Doctor(
                    accountId: account.Id,
                    departmentId: request.DepartmentId,
                    doctorCode: doctorCode,
                    licenseNumber: request.LicenseNumber ?? string.Empty,   // nullable
                    licenseIssueDate: null,
                    licenseExpiryDate: null,
                    licenseIssuedBy: null,
                    slug: uniqueSlug);

                doctor.UpdateProfile(
                    avatar: request.Avatar,
                    fullAddress: request.FullAddress,
                    gender: request.Gender,
                    dateOfBirth: request.DateOfBirth,
                    identityNumber: request.IdentityNumber,
                    description: request.Description);

                if (request.ConsultationFee.HasValue)
                    doctor.UpdateConsultationFee(request.ConsultationFee.Value);

                _doctorRepo.Add(doctor);

                // ==================== BƯỚC 6: Commit ====================
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return doctor.Id;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}