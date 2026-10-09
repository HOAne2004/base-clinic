using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Models.Auth;
using MediatR;

namespace BaseClinic.Business.Services.Auth.Queries
{
    /// <summary>
    /// Query lấy danh sách session (thiết bị) đang active của Account.
    /// 
    /// LUỒNG:
    /// 1. Query RefreshTokens active của Account.
    /// 2. Map sang DTO.
    /// 3. Đánh dấu IsCurrent = true nếu session == CurrentSessionId (từ JWT).
    /// </summary>
    public record GetSessionsQuery(
        Guid AccountId,
        Guid CurrentSessionId
    ) : IRequest<List<SessionDto>>;

    public class GetSessionsQueryHandler
        : IRequestHandler<GetSessionsQuery, List<SessionDto>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public GetSessionsQueryHandler(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<List<SessionDto>> Handle(
            GetSessionsQuery request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Lấy active sessions ====================
            var sessions = await _refreshTokenRepository
                .GetActiveSessionsByAccountIdAsync(request.AccountId, cancellationToken);

            // ==================== BƯỚC 2: Map sang DTO ====================
            return sessions
                .Select(s => new SessionDto(
                    SessionId: s.Id,
                    DeviceInfo: s.DeviceInfo,
                    IpAddress: s.IpAddress,
                    CreatedAt: s.CreatedAt,
                    ExpiresAt: s.ExpiresAt,
                    IsCurrent: s.Id == request.CurrentSessionId,
                    IsTrusted: s.IsTrusted
                ))
                .ToList();
        }
    }
}