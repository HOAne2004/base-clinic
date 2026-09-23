using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Constants;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using MediatR;

namespace BaseClinic.Business.Services.Auth.Commands
{
    public class RegisterCommand : IRequest<bool>
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IPatientCodeGenerator _patientCodeGenerator;

        public RegisterCommandHandler(
            IUnitOfWork unitOfWork,
            IAccountRepository accountRepository,
            IRoleRepository roleRepository,
            IPatientRepository patientRepository,
            IPatientCodeGenerator patientCodeGenerator)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _patientRepository = patientRepository;
            _patientCodeGenerator = patientCodeGenerator;
        }

        public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Mở Transaction đảm bảo tính toàn vẹn (Hoặc thành công tất cả, hoặc không có gì được lưu)
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // --- BƯỚC 1: TẠO ACCOUNT ---
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                var account = new Account(
                    fullName: request.FullName,
                    email: null,
                    passwordHash: passwordHash,
                    phoneNumber: request.PhoneNumber,
                    status: AccountStatus.Active,
                    isEmailVerified: false,
                    isPhoneNumberVerified: false); //[cite: 23]

                // --- BƯỚC 2: GÁN ROLE "Patient" CHO ACCOUNT ---
                // Cần truy vấn để lấy ID của Role Patient từ database thay vì fix cứng Guid
                var patientRole = await _roleRepository.GetByNameAsync(SystemRoles.Patient, cancellationToken)
                    ?? throw new InvalidOperationException($"Hệ thống bị thiếu System Role cốt lõi: {SystemRoles.Patient}. Vui lòng kiểm tra lại quá trình Seeding cơ sở dữ liệu.");

                // Gọi method Domain để gán quyền (Tự động thêm vào danh sách _accountRoles)
                account.AssignRole(patientRole.Id); //[cite: 23]

                _accountRepository.Add(account);

                // --- BƯỚC 3: TẠO HỒ SƠ PATIENT TỰ ĐỘNG ---
                string patientCode = await _patientCodeGenerator.GenerateAsync(cancellationToken);

                // Thuộc tính Id của account đã được EF Core tự sinh ra ngay khi khởi tạo Object
                var patient = new Patient(
                    patientCode: patientCode,
                    accountId: account.Id,
                    fullName: request.FullName);

                _patientRepository.Add(patient);

                // --- BƯỚC 4: LƯU VÀ COMMIT GIAO DỊCH ---
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return true;
            }
            catch
            {
                // Nếu quá trình sinh mã lỗi hoặc lưu CSDL lỗi, toàn bộ thao tác sẽ bị hủy
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}