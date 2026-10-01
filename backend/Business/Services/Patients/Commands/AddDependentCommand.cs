using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using MediatR;

namespace BaseClinic.Business.Services.Patients.Commands
{
    public record AddDependentCommand(
        // ID của tài khoản đang đăng nhập (Người gửi yêu cầu)
        Guid AccountId,

        // Thông tin người thân cần thêm
        string FullName,
        string PhoneNumber,
        DateTime? DateOfBirth,
        bool? Gender,

        // Mối quan hệ (Ví dụ: Vợ, Chồng, Con cái...)
        PatientRelationshipType RelationshipType
    ) : IRequest<string>; // Trả về chuỗi thông báo kết quả (Vì kết quả có thể là "Đã thêm" hoặc "Chờ xác nhận")

    public class AddDependentCommandHandler : IRequestHandler<AddDependentCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IPatientDelegationRepository _patientDelegationRepository;
        private readonly IPatientCodeGenerator _patientCodeGenerator;

        public AddDependentCommandHandler(
            IUnitOfWork unitOfWork,
            IAccountRepository accountRepository,
            IPatientRepository patientRepository,
            IPatientDelegationRepository patientDelegationRepository,
            IPatientCodeGenerator patientCodeGenerator)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _patientRepository = patientRepository;
            _patientDelegationRepository = patientDelegationRepository;
            _patientCodeGenerator = patientCodeGenerator;
        }

        public async Task<string> Handle(AddDependentCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var existingAccount = await _accountRepository.GetAccountByPhoneNumberAsync(request.PhoneNumber, cancellationToken);

                if (existingAccount != null)
                {
                    // LUỒNG A: NGƯỜI THÂN ĐÃ CÓ TÀI KHOẢN APP

                    // Lấy hồ sơ gốc của tài khoản đó
                    var targetPatient = await _patientRepository.GetPrimaryPatientByAccountIdAsync(existingAccount.Id, cancellationToken)
                        ?? throw new InvalidOperationException("Tài khoản người thân bị lỗi dữ liệu (không tìm thấy hồ sơ gốc).");

                    // Kiểm tra xem đã gửi yêu cầu trước đó chưa để tránh duplicate
                    bool isExists = await _patientDelegationRepository.CheckDelegationExistAsync(targetPatient.Id, request.AccountId, cancellationToken);
                    if (isExists) throw new InvalidOperationException("Yêu cầu đã được gửi trước đó.");

                    // Tạo giấy phép trạng thái PENDING (Chờ B xác nhận)
                    var delegation = new PatientDelegation(
                        targetPatientId: targetPatient.Id,
                        observerAccountId: request.AccountId,
                        relationshipType: request.RelationshipType,
                        status: DelegationStatus.Pending);

                    _patientDelegationRepository.AddDelegation(delegation);
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);
                    return $"Đã gửi yêu cầu liên kết đến số điện thoại {request.PhoneNumber}. Vui lòng chờ xác nhận.";
                }
                else
                {
                    // LUỒNG B: NGƯỜI THÂN CHƯA CÓ TÀI KHOẢN APP (Tạo mới hoàn toàn)
                    string patientCode = await _patientCodeGenerator.GenerateAsync(cancellationToken);

                    var newDependent = new Patient(
                        patientCode: patientCode,
                        accountId: null, // Chưa có tài khoản
                        fullName: request.FullName,
                        isPrimary: false);

                    newDependent.UpdateProfile(
                        fullName: request.FullName,
                        avatar: null,
                        fullAddress: null,
                        identityNumber: null,
                        gender: request.Gender,
                        dateOfBirth: request.DateOfBirth);

                    _patientRepository.Add(newDependent);
                    // Lưu để EF Core sinh ra Id cho newDependent
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // Tạo giấy phép trạng thái ACCEPTED (Vì A tự tạo nên A có quyền luôn)
                    var delegation = new PatientDelegation(
                        targetPatientId: newDependent.Id,
                        observerAccountId: request.AccountId,
                        relationshipType: request.RelationshipType,
                        status: DelegationStatus.Accepted);

                    _patientDelegationRepository.AddDelegation(delegation);
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    return "Thêm hồ sơ người thân thành công. Bạn đã có thể đặt lịch cho hồ sơ này.";
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw new Exception("Có lỗi: ", ex);
            }
        }
    }
}
