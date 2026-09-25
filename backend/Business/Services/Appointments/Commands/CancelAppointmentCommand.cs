using BaseClinic.Business.Interfaces;
using MediatR;

namespace BaseClinic.Business.Services.Appointments.Commands
{
    public record CancelAppointmentCommand(Guid Id, string CancelReason) : IRequest<bool>;

    public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, bool>
    {
        private readonly IAppointmentRepository _appointmentsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IUnitOfWork unitOfWork)
        {
            _appointmentsRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(CancelAppointmentCommand request, CancellationToken cancellation)
        {
            var appointment = await _appointmentsRepository.GetByIdAsync(request.Id, cancellation);
            if (appointment == null)
            {
                throw new InvalidOperationException("Lịch hẹn không tồn tại.");
            }

            appointment.Cancel(request.CancelReason);
            await _unitOfWork.SaveChangesAsync(cancellation);
            return true;
        }
    }
}
