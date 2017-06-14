using SimpleInjector;
using ViewModels.Dialogs;

namespace ViewModels
{
    /// <summary>
    /// Class provides assembly-wide access to services
    /// </summary>
    public static class Injector
    {
        public static Container Container { get; set; }

        public static VisualDialogContainer Dialogs { get; set; }

        public static ILocalizationVm Localization { get; set; }
    }
}
