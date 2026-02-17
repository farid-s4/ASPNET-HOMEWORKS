namespace InvoiceManager.DTO.CustomerDTOs
{
    public class CustomerResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int InvoicesCount { get; set; }
    }
}
