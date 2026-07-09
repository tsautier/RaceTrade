namespace RaceTrader
{
    public class ChangelogForm : HelpForm
    {
        public ChangelogForm()
            : base("RaceTrader Changelog", "RaceTrader Changelog", GetChangelogContent())
        {
        }
    }
}
