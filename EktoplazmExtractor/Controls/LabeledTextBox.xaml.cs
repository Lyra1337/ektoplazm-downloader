using System.Windows;
using System.Windows.Controls;

namespace EktoplazmExtractor.Controls
{
    public partial class LabeledTextBox : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                name: nameof(LabeledTextBox.Text),
                propertyType: typeof(string),
                ownerType: typeof(LabeledTextBox),
                typeMetadata: new FrameworkPropertyMetadata(
                    defaultValue: null,
                    flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                )
            );

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register(
                name: nameof(LabeledTextBox.Caption),
                propertyType: typeof(string),
                ownerType: typeof(LabeledTextBox),
                typeMetadata: new FrameworkPropertyMetadata(
                    defaultValue: null
                )
            );

        public string Text
        {
            get
            {
                return (string)this.GetValue(LabeledTextBox.TextProperty);
            }
            set
            {
                this.SetValue(LabeledTextBox.TextProperty, value);
            }
        }

        public string Caption
        {
            get
            {
                return (string)this.GetValue(LabeledTextBox.CaptionProperty);
            }
            set
            {
                this.SetValue(LabeledTextBox.CaptionProperty, value);
            }
        }

        public LabeledTextBox()
        {
            this.InitializeComponent();
        }
    }
}