namespace NPI.Client.Services
{
    public class ToastService
    {
        public event Action<string, string>? OnShow;

        public void ShowSuccess(string message)
            => OnShow?.Invoke(message, "bg-success");

        public void ShowError(string message)
            => OnShow?.Invoke(message, "bg-danger");

        public void ShowInfo(string message)
            => OnShow?.Invoke(message, "bg-info");

        public void ShowWarning(string message)
            => OnShow?.Invoke(message, "bg-warning");
    }


}
