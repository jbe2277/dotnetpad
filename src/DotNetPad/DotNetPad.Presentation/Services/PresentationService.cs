using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using Waf.DotNetPad.Applications.Services;

namespace Waf.DotNetPad.Presentation.Services
{
    [Export(typeof(IPresentationService))]
    internal class PresentationService : IPresentationService
    {
        public void Initialize()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }
    }
}
