using System;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace Bug6842
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a FlowDocument with an embedded image
            FlowDocument flowDocument = new FlowDocument
            {
                Blocks =
                {
                    new Paragraph
                    {
                        // ** this line causes the exception **
                        Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/sample.bmp"))),

                        Inlines =
                        {
                            new Run { Text = "test" },
                        },
                    },
                }
            };
            
            // create a temp XPS document
            var tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            var tempDocument = new XpsDocument(tempFilePath, FileAccess.ReadWrite);
            var writer = XpsDocument.CreateXpsDocumentWriter(tempDocument);

            // Write the FlowDocument to the XPS document
            writer.Write(((IDocumentPaginatorSource) flowDocument).DocumentPaginator);

            // Get a default print ticket from the default printer
            LocalPrintServer localPrintServer = new LocalPrintServer();
            PrintQueue printQueue = LocalPrintServer.GetDefaultPrintQueue();
            PrintTicket printTicket = printQueue.DefaultPrintTicket;

            // Create an XpsDocumentWriter object for the print queue.
            XpsDocumentWriter printWriter = PrintQueue.CreateXpsDocumentWriter(printQueue);

            // ** this throws the exception **
            printWriter.Write(tempDocument.GetFixedDocumentSequence(), printTicket);

            // avoiding the temporary document prints successfully
            // xpsDocumentWriter.Write(((IDocumentPaginatorSource) flowDocument).DocumentPaginator, printTicket);
        }
    }
}
