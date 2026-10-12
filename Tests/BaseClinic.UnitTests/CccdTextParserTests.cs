using BaseClinic.Business.Services.Ocr;
using Xunit;
using FluentAssertions;

namespace BaseClinic.Tests.BaseClinic.UnitTests
{
    public class CccdTextParserTests
    {
        [Fact]
        public void Parse_ValidCccdText_ReturnsAllFields()
        {
            // Arrange
                        var rawText = @"
            CĂN CƯỚC CÔNG DÂN
            Số: 001199001234
            Họ và tên: NGUYỄN VĂN A
            Ngày sinh: 15/01/1990      Giới tính: Nam
            Quốc tịch: Việt Nam
            Quê quán: Hà Nội
            Nơi thường trú: 123 Lê Lợi, Phường 1, Hà Nội
            Có giá trị đến: 15/01/2030";

            // Act
            var result = CccdTextParser.Parse(rawText, 0.85f, 0.5f);

            // Assert
            result.IdentityNumber.Should().Be("001199001234");
            result.FullName.Should().Be("NGUYỄN VĂN A");
            result.DateOfBirth.Should().Be(new DateTime(1990, 1, 15));
            result.Gender.Should().BeTrue();
            result.RequiresManualReview.Should().BeFalse();
        }

        [Fact]
        public void Parse_EmptyText_ReturnsAllNull()
        {
            var result = CccdTextParser.Parse("", 0.0f, 0.5f);

            result.IdentityNumber.Should().BeNull();
            result.FullName.Should().BeNull();
        }
    }
}
