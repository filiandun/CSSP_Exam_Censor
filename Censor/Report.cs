using System;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;

namespace Censor
{
    internal class Report
    {
        private RichTextBox fileContentRichTextBox;

        private FoundFileList foundFileList;
        private CensorWordList censorWordList;

        public Report(RichTextBox fileContentRichTextBox, FoundFileList foundFileList, CensorWordList censorWordList)
        {
            this.fileContentRichTextBox = fileContentRichTextBox;

            this.foundFileList = foundFileList;
            this.censorWordList = censorWordList;
        }

        public async Task GenerateReport()
        {
            StringBuilder report = new StringBuilder();
            this.fileContentRichTextBox.AppendText("Отчёт по файлам: ");
            //report.AppendLine("Отчёт по файлам: ");

            await Task.Run(() =>
            {
                foreach (FoundFile foundFile in this.foundFileList)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.fileContentRichTextBox.AppendText($"\n\nФайл: {foundFile.FileInfo.Name} ({foundFile.FileInfo.FullName})");
                    });
                    //report.AppendLine($"\nФайл: {foundFile.FileInfo.Name} ({foundFile.FileInfo.FullName})");
                    foreach (string censorWord in this.censorWordList)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            this.fileContentRichTextBox.AppendText($"\n\tСлово \"{censorWord}\" повторяется {foundFile.CountCensorWord(censorWord)} раз(а).");
                        });
                        //report.AppendLine($"\tСлово \"{censorWord}\" повторяется {foundFile.CountCensorWord(censorWord)} раз(а).");
                    }
                }
            });
        }

        public void SaveReportFile(FlowDocument flowDocument)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Текстовый файл (*.txt)|*.txt",
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                TextRange textRange = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                using (FileStream fileStream = File.Create(saveFileDialog.FileName))
                {
                    if (Path.GetExtension(saveFileDialog.FileName).ToLower() == ".txt")
                    {
                        textRange.Save(fileStream, DataFormats.Text);
                    }
                }
            }
        }
    }
}
