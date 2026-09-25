using BaseClinic.Business.Interfaces;
using MediatR;

namespace BaseClinic.Business.Services.Doctors.Commands
{
    public record DeleteDoctorCommand(Guid Id) : IRequest<bool>;

    public class DeleteDoctorCommandHandler : IRequestHandler<DeleteDoctorCommand, bool>
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAccountRepository _accountRepository; // Thêm AccountRepo
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDoctorCommandHandler(
            IDoctorRepository doctorRepository,
            IAccountRepository accountRepository,
            IUnitOfWork unitOfWork)
        {
            _doctorRepository = doctorRepository;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteDoctorCommand request, CancellationToken cancellation)
        {
            await _unitOfWork.BeginTransactionAsync(cancellation);
            try
            {
                var doctor = await _doctorRepository.GetByIdAsync(request.Id, cancellation);
                if (doctor == null)
                {
                    throw new InvalidOperationException("Bác sĩ không tồn tại.");
                }

                // 1. Lấy Account liên kết và khóa nó lại (thay vì xóa cứng Account)
                var account = await _accountRepository.GetByIdAsync(doctor.AccountId, cancellation);
                if (account != null)
                {
                    account.Deactivate(); 
                }

                // 2. Xóa hồ sơ Bác sĩ
                _doctorRepository.Remove(doctor); 

                await _unitOfWork.CommitTransactionAsync(cancellation);
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellation);
                throw;
            }
        }
    }
}