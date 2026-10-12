using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace BaseClinic.UnitTests.Domain.Entities
{
    public class MergeRollbackRequestTests
    {
        private static readonly Guid ValidMergeLogId = Guid.NewGuid();
        private static readonly Guid ValidRequesterId = Guid.NewGuid();
        private static readonly Guid ValidReviewerId = Guid.NewGuid();

        // ==================== CREATE ====================

        [Fact]
        public void Create_ValidInput_ReturnsPendingRequest()
        {
            // Act
            var request = MergeRollbackRequest.Create(
                ValidMergeLogId,
                ValidRequesterId,
                "Merge nhầm 2 người khác nhau");

            // Assert
            request.PatientMergeLogId.Should().Be(ValidMergeLogId);
            request.RequestedByAccountId.Should().Be(ValidRequesterId);
            request.Reason.Should().Be("Merge nhầm 2 người khác nhau");
            request.Status.Should().Be(MergeRollbackRequestStatus.Pending);
            request.ReviewedAt.Should().BeNull();
            request.ReviewedByAccountId.Should().BeNull();
        }

        [Fact]
        public void Create_EmptyMergeLogId_ThrowsDomainException()
        {
            // Act
            Action act = () => MergeRollbackRequest.Create(
                Guid.Empty, ValidRequesterId, "Lý do");

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("*PatientMergeLogId*");
        }

        [Fact]
        public void Create_EmptyRequesterId_ThrowsDomainException()
        {
            Action act = () => MergeRollbackRequest.Create(
                ValidMergeLogId, Guid.Empty, "Lý do");

            act.Should().Throw<DomainException>()
                .WithMessage("*RequesterAccountId*");
        }

        [Fact]
        public void Create_EmptyReason_ThrowsDomainException()
        {
            Action act = () => MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "");

            act.Should().Throw<DomainException>()
                .WithMessage("*Lý do*");
        }

        [Fact]
        public void Create_WhitespaceReason_ThrowsDomainException()
        {
            Action act = () => MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "   ");

            act.Should().Throw<DomainException>()
                .WithMessage("*Lý do*");
        }

        [Fact]
        public void Create_ReasonTooLong_ThrowsDomainException()
        {
            var longReason = new string('a', 501);

            Action act = () => MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, longReason);

            act.Should().Throw<DomainException>()
                .WithMessage("*500*");
        }

        [Fact]
        public void Create_ReasonWithWhitespace_TrimsReason()
        {
            var request = MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "  Lý do có space  ");

            request.Reason.Should().Be("Lý do có space");
        }

        // ==================== APPROVE ====================

        [Fact]
        public void Approve_PendingRequest_UpdatesStatus()
        {
            // Arrange
            var request = MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "Lý do");

            // Act
            request.Approve(ValidReviewerId, "OK", "[{\"test\":1}]");

            // Assert
            request.Status.Should().Be(MergeRollbackRequestStatus.Approved);
            request.ReviewedByAccountId.Should().Be(ValidReviewerId);
            request.ReviewedAt.Should().NotBeNull();
            request.ReviewNote.Should().Be("OK");
            request.ConflictResolutions.Should().Be("[{\"test\":1}]");
        }

        [Fact]
        public void Approve_AlreadyApproved_ThrowsDomainException()
        {
            // Arrange
            var request = MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "Lý do");
            request.Approve(ValidReviewerId, "OK", null);

            // Act
            Action act = () => request.Approve(ValidReviewerId, "OK lần 2", null);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("*Pending*");
        }

        [Fact]
        public void Approve_RejectedRequest_ThrowsDomainException()
        {
            var request = MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "Lý do");
            request.Reject(ValidReviewerId, "Không hợp lệ");

            Action act = () => request.Approve(ValidReviewerId, "OK", null);

            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Approve_EmptyReviewerId_ThrowsDomainException()
        {
            var request = MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "Lý do");

            Action act = () => request.Approve(Guid.Empty, "OK", null);

            act.Should().Throw<DomainException>()
                .WithMessage("*ReviewerAccountId*");
        }

        [Fact]
        public void Approve_NullReviewNote_AcceptsNull()
        {
            var request = MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "Lý do");

            request.Approve(ValidReviewerId, null, null);

            request.ReviewNote.Should().BeNull();
            request.ConflictResolutions.Should().BeNull();
        }

        // ==================== REJECT ====================

        [Fact]
        public void Reject_PendingRequest_UpdatesStatus()
        {
            var request = MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "Lý do");

            request.Reject(ValidReviewerId, "Không đủ căn cứ");

            request.Status.Should().Be(MergeRollbackRequestStatus.Rejected);
            request.ReviewedByAccountId.Should().Be(ValidReviewerId);
            request.ReviewedAt.Should().NotBeNull();
            request.ReviewNote.Should().Be("Không đủ căn cứ");
        }

        [Fact]
        public void Reject_EmptyReviewNote_ThrowsDomainException()
        {
            var request = MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "Lý do");

            Action act = () => request.Reject(ValidReviewerId, "");

            act.Should().Throw<DomainException>()
                .WithMessage("*Lý do từ chối*");
        }

        [Fact]
        public void Reject_AlreadyApproved_ThrowsDomainException()
        {
            var request = MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "Lý do");
            request.Approve(ValidReviewerId, "OK", null);

            Action act = () => request.Reject(ValidReviewerId, "Lý do reject");

            act.Should().Throw<DomainException>();
        }

        // ==================== CANCEL ====================

        [Fact]
        public void Cancel_ByRequester_PendingRequest_UpdatesStatus()
        {
            var request = MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "Lý do");

            request.Cancel(ValidRequesterId);

            request.Status.Should().Be(MergeRollbackRequestStatus.Cancelled);
        }

        [Fact]
        public void Cancel_ByDifferentAccount_ThrowsDomainException()
        {
            var request = MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "Lý do");

            var otherAccountId = Guid.NewGuid();
            Action act = () => request.Cancel(otherAccountId);

            act.Should().Throw<DomainException>()
                .WithMessage("*người gửi*");
        }

        [Fact]
        public void Cancel_AlreadyApproved_ThrowsDomainException()
        {
            var request = MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "Lý do");
            request.Approve(ValidReviewerId, "OK", null);

            Action act = () => request.Cancel(ValidRequesterId);

            act.Should().Throw<DomainException>()
                .WithMessage("*Pending*");
        }

        [Fact]
        public void Cancel_AlreadyCancelled_ThrowsDomainException()
        {
            var request = MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "Lý do");
            request.Cancel(ValidRequesterId);

            Action act = () => request.Cancel(ValidRequesterId);

            act.Should().Throw<DomainException>();
        }

        // ==================== STATE MACHINE ====================

        [Theory]
        [InlineData("approve", MergeRollbackRequestStatus.Approved)]
        [InlineData("reject", MergeRollbackRequestStatus.Rejected)]
        [InlineData("cancel", MergeRollbackRequestStatus.Cancelled)]
        public void Transition_FromPending_ToTarget(string action,
            MergeRollbackRequestStatus expectedStatus)
        {
            var request = MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "Lý do");

            switch (action)
            {
                case "approve":
                    request.Approve(ValidReviewerId, "OK", null);
                    break;
                case "reject":
                    request.Reject(ValidReviewerId, "Không OK");
                    break;
                case "cancel":
                    request.Cancel(ValidRequesterId);
                    break;
            }

            request.Status.Should().Be(expectedStatus);
        }

        [Theory]
        [InlineData(MergeRollbackRequestStatus.Approved)]
        [InlineData(MergeRollbackRequestStatus.Rejected)]
        [InlineData(MergeRollbackRequestStatus.Cancelled)]
        public void AllTransitions_FromTerminalState_ThrowException(
            MergeRollbackRequestStatus terminalState)
        {
            var request = MergeRollbackRequest.Create(
                ValidMergeLogId, ValidRequesterId, "Lý do");

            // Đưa về terminal state
            switch (terminalState)
            {
                case MergeRollbackRequestStatus.Approved:
                    request.Approve(ValidReviewerId, "OK", null);
                    break;
                case MergeRollbackRequestStatus.Rejected:
                    request.Reject(ValidReviewerId, "Không OK");
                    break;
                case MergeRollbackRequestStatus.Cancelled:
                    request.Cancel(ValidRequesterId);
                    break;
            }

            // Thử transition tiếp → đều throw
            Action approve = () => request.Approve(ValidReviewerId, "OK", null);
            Action reject = () => request.Reject(ValidReviewerId, "Không OK");
            Action cancel = () => request.Cancel(ValidRequesterId);

            approve.Should().Throw<DomainException>();
            reject.Should().Throw<DomainException>();
            cancel.Should().Throw<DomainException>();
        }
    }
}