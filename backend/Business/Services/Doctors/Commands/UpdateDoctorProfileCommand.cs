using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Doctors.Commands
{
    /// <summary>
    /// Cập nhật PROFILE bác sĩ — chỉ các field cá nhân cơ bản.
    /// Không động đến: License, Fee, Workload, Department.
    /// </summary>
    public record UpdateDoctorProfileCommand(
        Guid Id,
        string? Avatar,
        string? FullAddress,
        bool? Gender,
        DateTime? DateOfBirth,
        string? IdentityNumber,
        string? Description
    ) : IRequest<bool>;

    public class UpdateDoctorProfileCommandHandler
        : IRequestHandler<UpdateDoctorProfileCommand, bool>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDoctorProfileCommandHandler(
            IDoctorRepository doctorRepo,
            IUnitOfWork unitOfWork)
        {
            _doctorRepo = doctorRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            UpdateDoctorProfileCommand request,
            CancellationToken cancellationToken)
        {
            var doctor = await _doctorRepo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy bác sĩ.");

            doctor.UpdateProfile(
                avatar: request.Avatar,
                fullAddress: request.FullAddress,
                gender: request.Gender,
                dateOfBirth: request.DateOfBirth,
                identityNumber: request.IdentityNumber,
                description: request.Description);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}