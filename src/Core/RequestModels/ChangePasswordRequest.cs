namespace Core.RequestModels;

public class ChangePasswordRequest
{
    public string Email { get; set; } = default!;
    public string CurrentPassword { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
    public string ConfirmNewPassword { get; set; } = default!;
}
