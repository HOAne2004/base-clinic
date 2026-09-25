using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Models;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using MediatR;
using System.Threading;

namespace BaseClinic.Business.Services.Doctors.Commands
{
    public record CreateDoctorCommand(
            // Thông tin Account
            string FullName,
            string PhoneNumber,
            string? Email,
            string Password,

            // Thông tin Doctor
            Guid DepartmentId,
            string? Avatar,
            string? FullAddress,
            bool? Gender,
            DateTime? DateOfBirth,
            string? IdentityNumber,
            string? LicenseNumber,
            decimal? ConsultationFee,
            string? Description
        ) : IRequest<Guid>;

    public class CreateDoctorCommandHandler : IRequestHandler<CreateDoctorCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IDoctorCodeGenerator _doctorCodeGenerator;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPasswordHasher _passwordHasher;

        public CreateDoctorCommandHandler(
            IUnitOfWork unitOfWork, 
            IAccountRepository accountRepository,
            IRoleRepository roleRepository,
            IDoctorCodeGenerator doctorCodeGenerator,
            IDoctorRepository doctorRepository,
            IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _doctorCodeGenerator = doctorCodeGenerator;
            _doctorRepository = doctorRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Guid> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
        {
            // Mở giao dịch vì quá trình này tác động đến nhiều bảng
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // BƯỚC 1: XÁC THỰC VÀ TẠO ACCOUNT
                var existingAccount = await _accountRepository.GetAccountByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
                if (existingAccount != null)
                {
                    throw new InvalidOperationException("Số điện thoại này đã được sử dụng cho một tài khoản khác.");
                }

                string hashedPassword = _passwordHasher.Hash(request.Password);

                var account = new Account(
                    fullName: request.FullName,
                    email: request.Email,
                    passwordHash: hashedPassword,
                    phoneNumber: request.PhoneNumber,
                    status: AccountStatus.Active, // Tự động active khi tạo từ Admin
                    isEmailVerified: false,
                    isPhoneNumberVerified: true // Mặc định true vì SĐT là định danh chính
                );

                _accountRepository.Add(account);

                // Cần lưu thay đổi để EF Core cấp phát GUID cho Account.Id
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // BƯỚC 2: TÌM VÀ GÁN ROLE "DOCTOR" CHO ACCOUNT VỪA TẠO
                var doctorRole = await _roleRepository.GetByNameAsync("Doctor", cancellationToken)
                    ?? throw new InvalidOperationException("Role hệ thống 'Doctor' chưa được khởi tạo trong Database.");

                account.AssignRole(doctorRole.Id);
                await _unitOfWork.SaveChangesAsync(cancellationToken); // Lưu role trung gian

                // BƯỚC 3: KHỞI TẠO HỒ SƠ DOCTOR
                string doctorCode = await _doctorCodeGenerator.GenerateAsync(cancellationToken);

                var doctor = new Doctor(
                    accountId: account.Id,
                    departmentId: request.DepartmentId,
                    doctorCode: doctorCode,
                    avatar: request.Avatar,
                    fullAddress: request.FullAddress,
                    gender: request.Gender,
                    dateOfBirth: request.DateOfBirth,
                    identityNumber: request.IdentityNumber,
                    licenseNumber: request.LicenseNumber,
                    consultationFee: request.ConsultationFee,
                    description: request.Description
                );

                _doctorRepository.Add(doctor);

                // BƯỚC 4: LƯU TOÀN BỘ SỰ THAY ĐỔI
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
