using System.Windows;
using System.Windows.Documents;

namespace PāPēi_1._0
{
    /// <summary>
    /// Interaction logic for PDFViewer.xaml
    /// </summary>
    public partial class PDFViewer : Window
    {
        public PDFViewer()
        {
            InitializeComponent();
            DataContext = this;
        }

        public IDocumentPaginatorSource PreviewDocument
        { get; set; }
    }
}
