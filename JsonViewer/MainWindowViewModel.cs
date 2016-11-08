using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PropertyChanged;

namespace JsonViewer
{
    [ImplementPropertyChanged]
    public class MainWindowViewModel
    {

        public string JsonText { get; set; }

        public RelayCommand FormatJsonCommand { get; private set; }


        public MainWindowViewModel()
        {
            FormatJsonCommand = new RelayCommand(FormatJson);
        }

        private void FormatJson(object p)
        {
            if (Clipboard.ContainsText() == false) return;

            var tb = (TextBox)p;

            try
            {
                var onlyJson = TrimToOnlyJson(Clipboard.GetText());

                JsonText = JToken.Parse(onlyJson).ToString(Formatting.Indented);
            }
            catch (Exception ex)
            {
                JsonText = ex.Message;
            }
            finally
            {
                tb.ScrollToHome();
            }

        }


        private string TrimToOnlyJson(string getText)
        {
            var match = Regex.Match(getText, "{(.*)}", RegexOptions.Compiled, TimeSpan.FromSeconds(5));
            return match.Success ? match.Captures[0].Value : string.Empty;
        }
    }
}
