using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Doctors.Commands
{
    /// <summary>
    /// Doctor TỰ sửa profile của mình.
    /// CHỈ các field: Avatar, Biography, Languages, Description, IsAvailableForOnline.
    /// </summary>
    public record UpdateDoctorSelfProfileCommand(
        Guid AccountId,          // Từ JWT
        string? Avatar,
        string? Biography,
        string? Languages,
        string? Description,
        bool IsAvailableForOnline
    ) : IRequest<bool>;

    public class UpdateDoctorSelfProfileCommandHandler
        : IRequestHandler<UpdateDoctorSelfProfileCommand, bool>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDoctorSelfProfileCommandHandler(
            IDoctorRepository doctorRepo,
            IUnitOfWork unitOfWork)
        {
            _doctorRepo = doctorRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            UpdateDoctorSelfProfileCommand request,
            CancellationToken cancellationToken)
        {
            var doctor = await _doctorRepo.GetByAccountIdAsync(
                request.AccountId, cancellationToken)
                ?? throw new NotFoundException(
                    "Tài khoản này chưa được liên kết với hồ sơ bác sĩ.");

            doctor.UpdateSelfProfile(
                avatar: request.Avatar,
                biography: request.Biography,
                languages: request.Languages,
                description: request.Description,
                isAvailableForOnline: request.IsAvailableForOnline);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}