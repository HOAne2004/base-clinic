using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Models.Patient;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Patients.Commands
{
    /// <summary>
    /// Cập nhật hồ sơ BN.
    /// 
    /// LƯU Ý: Controller cần check ownership nếu user là Patient.
    /// Nếu user là Admin/Lễ tân → sửa mọi hồ sơ.
    /// </summary>
    public record UpdatePatientCommand(Guid Id, UpdatePatientDto Dto)
        : IRequest<Unit>; // unit tra ve kieu rong (tuong tu void)

    public class UpdatePatientCommandHandler
        : IRequestHandler<UpdatePatientCommand, Unit>
    {
        private readonly IPatientRepository _patientRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePatientCommandHandler(
            IPatientRepository patientRepo,
            IUnitOfWork unitOfWork)
        {
            _patientRepo = patientRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            UpdatePatientCommand request,
            CancellationToken cancellationToken)
        {
            var patient = await _patientRepo.GetByIdAsync(request.Id, cancellationToken);

            if (patient == null)
                throw new NotFoundException($"Không tìm thấy bệnh nhân với Id {request.Id}.");

            patient.UpdateProfile(
                fullName: request.Dto.FullName,
                avatarUrl: request.Dto.AvatarUrl,
                address: request.Dto.Address,
                identityNumber: request.Dto.IdentityNumber,
                gender: request.Dto.Gender,
                dateOfBirth: request.Dto.DateOfBirth,
                healthInsuranceNumber: request.Dto.HealthInsuranceNumber);

            _patientRepo.Update(patient);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}