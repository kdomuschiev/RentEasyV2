namespace renteasy_api.Application.DTOs.Properties;

public class UpdatePaymentMethodsRequest
{
    public string? Iban { get; set; }
    public string? IrisPayPhoneNumber { get; set; }
    public string? RevolutMeLink { get; set; }
}
