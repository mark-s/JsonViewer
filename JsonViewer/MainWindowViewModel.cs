using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Newtonsoft.Json.Linq;
using PropertyChanged;
using Formatting = Newtonsoft.Json.Formatting;

namespace JsonViewer
{
    [ImplementPropertyChanged]
    public class MainWindowViewModel
    {
        public string JsonText { get; set; } = "";

        public TextDocument Document => new TextDocument { Text = JsonText };
        public bool ShowEditor => !string.IsNullOrEmpty(JsonText);

        public RelayCommand FormatJsonCommand { get; private set; }
        public IHighlightingDefinition SyntaxHighlightDef { get; set; }


        public MainWindowViewModel()
        {
            // Bail out if we're in the designer
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;

            FormatJsonCommand = new RelayCommand(async _ => JsonText = await FormatJson(Clipboard.GetText()));

            LoadTheSyntaxFile();
        }

        private void LoadTheSyntaxFile()
        {
            var fileUri = new Uri("/json.xshd", UriKind.Relative);

            var resourceInfo = Application.GetResourceStream(fileUri);

            if (resourceInfo != null)
                using (var xmlTextReader = new XmlTextReader(resourceInfo.Stream))
                {
                    SyntaxHighlightDef = HighlightingLoader.Load(xmlTextReader, HighlightingManager.Instance);
                }
            else
                Trace.WriteLine($"Failed to load syntax Highlighting file from [{fileUri}]");
        }
        
        private Task<string> FormatJson(string text)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (Clipboard.ContainsText() == false) return string.Empty;

                    var onlyJson = TrimToOnlyJson(text);
                    return JToken.Parse(onlyJson).ToString(Formatting.Indented);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            });
        }

        private string TrimToOnlyJson(string text)
        {
            // Grab only the JSON, using the brackets as start and end points
            var match = Regex.Match(text, "{(.*)}", RegexOptions.Compiled, TimeSpan.FromSeconds(5));
            return match.Success ? match.Captures[0].Value : string.Empty;
        }
    }
}
