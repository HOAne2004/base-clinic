using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Departments.Commands
{
    /// <summary>
    /// Bật/tắt đặt lịch trực tuyến cho khoa.
    /// 
    /// TÁC ĐỘNG:
    /// - true → bệnh nhân có thể book online vào khoa.
    /// - false → khoa chỉ tiếp nhận walk-in (không book online).
    /// 
    /// LƯU Ý: API này KHÔNG ảnh hưởng đến các appointment đã đặt.
    /// Chỉ ngăn các booking MỚI.
    /// </summary>
    public record SetDepartmentOnlineBookingCommand(
        Guid Id,
        bool Enabled
    ) : IRequest<bool>;

    public class SetDepartmentOnlineBookingCommandHandler
        : IRequestHandler<SetDepartmentOnlineBookingCommand, bool>
    {
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IUnitOfWork _unitOfWork;

        public SetDepartmentOnlineBookingCommandHandler(
            IDepartmentRepository departmentRepo,
            IUnitOfWork unitOfWork)
        {
            _departmentRepo = departmentRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            SetDepartmentOnlineBookingCommand request,
            CancellationToken cancellationToken)
        {
            var department = await _departmentRepo.GetByIdAsync(
                request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy khoa.");

            // Không cho bật đặt lịch khi khoa đang tạm ngưng
            if (request.Enabled && !department.IsActive)
                throw new DomainException(
                    "Không thể bật đặt lịch cho khoa đang tạm ngưng.");

            // Idempotent — nếu đã ở trạng thái này rồi thì bỏ qua
            if (department.OnlineBookingEnabled == request.Enabled)
                return true;

            department.SetOnlineBookingEnabled(request.Enabled);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}