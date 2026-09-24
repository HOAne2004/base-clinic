using MediatR;

namespace BaseClinic.Business.Services.Doctors.Commands
{
    public class CreateDoctorCommand : IRequest<Guid>
    {
        public Guid AccountId { get; set; }
        
    }
}
