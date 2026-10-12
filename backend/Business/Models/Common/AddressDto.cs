namespace BaseClinic.Business.Models.Common
{
    /// <summary>
    /// DTO cho Address Value Object.
    /// Dùng trong response để FE prefill form edit.
    /// </summary>
    public record AddressDto(
        string Tinh,
        string Xa,
        string Duong,
        string SoNha
    );
}