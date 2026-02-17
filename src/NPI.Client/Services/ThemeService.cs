namespace NPI.Client.Services
{
    public class ThemeService
    {
        public bool IsDarkMode { get; private set; }

        public event Action? OnChange;

        public void ToggleTheme()
        {
            IsDarkMode = !IsDarkMode;
            NotifyStateChanged();
        }

        public void SetTheme(bool isDark)
        {
            IsDarkMode = isDark;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }

}
