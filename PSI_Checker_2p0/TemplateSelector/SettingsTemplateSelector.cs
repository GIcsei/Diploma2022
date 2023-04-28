using System.Windows;
using System.Windows.Controls;

namespace PSI_Checker_2p0
{
    public class SettingsTemplateSelector : DataTemplateSelector
    {
        #region Properties
        public DataTemplate StringTemplate { get; set; }
        public DataTemplate BooleanTemplate { get; set; }
        public DataTemplate NumberTemplate { get; set; }
        #endregion
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var selectedTemplate = StringTemplate;
            //var setting = item as SettingsHelper;
            if (item == null) { return selectedTemplate; }

            switch ("")
            {
                case "String":
                    selectedTemplate = StringTemplate;
                    break;
                case "Boolean":
                    selectedTemplate = BooleanTemplate;
                    break;
                default:
                    selectedTemplate = NumberTemplate;
                    break;
            }
            return selectedTemplate;
        }
    }
}
