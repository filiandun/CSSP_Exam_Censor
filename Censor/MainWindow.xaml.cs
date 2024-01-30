using System;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using System.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;

using Winforms = System.Windows.Forms;
using WpfUi = Wpf.Ui.Appearance;

namespace Censor
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource findCancellationTokenSource;

        private FoundFileList foundFileList;
        private CensorWordList censorWordList;

        private FileProcessor fileProcessor;
        private CensorProcessor censorProcessor;
        private Report report;

        public MainWindow()
        {
            this.InitializeComponent();

            this.InitializeDirectoryComboBox();
            this.InitializeFileFormatComboBox();

            ProgressManager.Initialize(this.progressBar);

            WpfUi.Theme.Apply(WpfUi.ThemeType.Dark, WpfUi.BackgroundType.Tabbed, true); // тёмная тема

            this.saveCopyFileOptionRadioButton.Visibility = Visibility.Hidden;
            this.replaceOriginalFileOptionRadioButton.Visibility = Visibility.Hidden;

            this.findCancellationTokenSource = new CancellationTokenSource();

            this.foundFileList = new FoundFileList(this.foundFileListBox);
            this.censorWordList = new CensorWordList(this.censorWordListBox);

            this.fileProcessor = new FileProcessor(this.foundFileList);
            this.censorProcessor = new CensorProcessor(this.foundFileList, this.censorWordList);
            this.report = new Report(this.fileContentRichTextBox, this.foundFileList, this.censorWordList);
        }



        private void InitializeDirectoryComboBox()
        {
            this.directoryPathComboBox.Items.Add("Поиск на всех накопителях");
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                this.directoryPathComboBox.Items.Add(drive.Name);
            }
            this.directoryPathComboBox.Items.Add(@"D:\Downloads\Блокноты");
        }

        private void InitializeFileFormatComboBox()
        {
            this.fileFormatComboBox.Items.Add("Все перечисленные");
            this.fileFormatComboBox.Items.Add(".txt");
            this.fileFormatComboBox.Items.Add(".doc");
            this.fileFormatComboBox.Items.Add(".dot");
            this.fileFormatComboBox.Items.Add(".docx");
            this.fileFormatComboBox.Items.Add(".pdf");
            this.fileFormatComboBox.Items.Add(".html");
        }

 

        private void filesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.saveReportButton.Visibility == Visibility.Visible)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Вы уверены, что не хотите сохранить отчёт?", "Подтверждение", MessageBoxButton.YesNo);
                
                if (messageBoxResult == MessageBoxResult.No)
                {
                    return;
                }
            }

            // RADIOBUTTON
            this.radioButtonsUniformGrid.Visibility = Visibility.Visible;
            this.saveReportButton.Visibility = Visibility.Hidden;

            this.saveCopyFileOptionRadioButton.Visibility = Visibility.Hidden;
            this.replaceOriginalFileOptionRadioButton.Visibility = Visibility.Hidden;
            //

            this.ApplyRichTextBoxStyles();

            int indexSelectedFile = this.foundFileListBox.SelectedIndex;

            if (indexSelectedFile != -1)
            {
                string pathSelectedFile = this.foundFileList[indexSelectedFile].FileInfo.FullName;

                try
                {
                    TextRange textRange = new TextRange(this.fileContentRichTextBox.Document.ContentStart, this.fileContentRichTextBox.Document.ContentEnd);

                    using (FileStream fileStream = new FileStream(pathSelectedFile, FileMode.Open))
                    {
                        if (Path.GetExtension(pathSelectedFile).ToLower() == ".txt")
                        {
                            textRange.Load(fileStream, DataFormats.Text);
                        }
                    }

                    // RADIOBUTTON
                    this.saveCopyFileOptionRadioButton.Visibility = Visibility.Visible;
                    this.replaceOriginalFileOptionRadioButton.Visibility = Visibility.Visible;

                    if (this.foundFileList[indexSelectedFile].FileSaveOption == FileSaveOption.SaveCopy)
                    {
                        this.saveCopyFileOptionRadioButton.IsChecked = true;
                    }
                    else if (this.foundFileList[indexSelectedFile].FileSaveOption == FileSaveOption.ReplaceOriginal)
                    {
                        this.replaceOriginalFileOptionRadioButton.IsChecked = true;
                    }
                    //
                }
                catch (Exception ex)
                {
                    InfoBox.Show(ex.Message, "Критическая ошибка");
                }
            }
        }


        private async void startFindButton_Click(object sender, RoutedEventArgs e)
        {
            string path = this.directoryPathComboBox.Text;
            string fileFormat = this.fileFormatComboBox.Text;

            // Обработка ошибок в полях
            if (string.IsNullOrWhiteSpace(path))
            {
                InfoBox.Show("Вы оставили поле с путём пустым!", "Внимание");

                return;
            }

            if (!Directory.Exists(path))
            {
                InfoBox.Show($"Указанный путь ({path}) не найден!", "Внимание");

                return;
            }

            if (string.IsNullOrWhiteSpace(fileFormat))
            {
                InfoBox.Show("Вы оставили поле с форматом пустым!", "Внимание");

                return;
            }

            // Обработка ввода с полей
            if (this.directoryPathComboBox.Text == "Поиск на всех накопителях")
            {
                // TO DO
                // Думаю, что нужно асинхронно запускать this.FileFinderAsync для каждого накопителя
            }

            switch (fileFormat)
            {
                case "Все перечисленные": fileFormat = "*.*"; break;
                case ".txt": fileFormat = "*.txt"; break;
                case ".doc": fileFormat = "*.doc"; break;
                case ".dot": fileFormat = "*.dot"; break;
                case ".docx": fileFormat = "*.docx"; break;
                case ".pdf": fileFormat = "*.pdf"; break;
                case ".html": fileFormat = "*.html"; break;
            }

            // ТУТ НУЖНО ЗАБЛОКИРОВАТЬ ВСЁ, КРОМЕ ОСТАНОВКИ ПОИСКА ФАЙЛОВ

            this.progressBar.Value = 0;
            await this.fileProcessor.FileFinderAsync(path, fileFormat); // рекурсивный поиск

            this.fileCountTextBlock.Text = $"{this.foundFileList.Count()}";

            // ТУТ НУЖНО РАЗБЛОКИРОВАТЬ ВСЁ, КРОМЕ ОСТАНОВКИ ПОИСКА ФАЙЛОВ
        }


        private void stopFindButton_Click(object sender, RoutedEventArgs e)
        {
            this.findCancellationTokenSource.Cancel();
        }


        private void choiceDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            Winforms.FolderBrowserDialog folderBrowserDialog = new Winforms.FolderBrowserDialog();
            Winforms.DialogResult dialogResult = folderBrowserDialog.ShowDialog();

            if (dialogResult == Winforms.DialogResult.OK)
            {
                this.directoryPathComboBox.Text = folderBrowserDialog.SelectedPath;
            }
        }


        private void addFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Текстовый файл (*.txt)|*.txt" };
            bool? dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == true)
            {
                string pathNewFile = openFileDialog.FileName;

                this.foundFileList.Add(new FoundFile(pathNewFile));
            }

            this.fileCountTextBlock.Text = $"{this.foundFileList.Count()}";
        }

        private void delFileButton_Click(object sender, RoutedEventArgs e)
        {
            int indexSelectedFile = this.foundFileListBox.SelectedIndex;

            this.foundFileList.RemoveAt(indexSelectedFile);
            this.fileCountTextBlock.Text = $"{this.foundFileList.Count()}";
        }

        private void clsFileButton_Click(object sender, RoutedEventArgs e)
        {
            this.foundFileList.Clear();
            this.fileCountTextBlock.Text = $"{this.foundFileList.Count()}";
        }





        private void openFileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == true)
            {
                try
                {
                    string censorFilePath = openFileDialog.FileName;
                    List<string> allLines = File.ReadAllLines(censorFilePath).Where(l => !String.IsNullOrWhiteSpace(l)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                    this.censorWordList.AddRange(allLines);
                }
                catch (Exception ex)
                {
                    InfoBox.Show(ex.Message, "Критическая ошибка");
                }

                this.wordCountTextBlock.Text = $"{this.censorWordList.Count()}";
            }
        }

        private void addCensorWordButton_Click(object sender, RoutedEventArgs e)
        {
            this.censorWordList.Add("Недоделал");
            this.wordCountTextBlock.Text = $"{this.censorWordList.Count()}";
        }

        private void delCensorWordButton_Click(object sender, RoutedEventArgs e)
        {
            int indexSelectedCensorWord = this.censorWordListBox.SelectedIndex;

            this.censorWordList.RemoveAt(indexSelectedCensorWord);
            this.wordCountTextBlock.Text = $"{this.censorWordList.Count()}";
        }

        private void clsCensorWordsListButton_Click(object sender, RoutedEventArgs e)
        {
            this.censorWordList.Clear();
            this.wordCountTextBlock.Text = $"{this.censorWordList.Count()}";
        }


        private async void startCensorshipButton_Click(object sender, RoutedEventArgs e)
        {
            await this.censorProcessor.Censor();

            this.ApplyRichTextBoxStyles();

            // TO DO
            // Хочу, чтобы в RichTextBox прям построчно добавлялся отчёт, но тогда нужно переделать.
            // 100% нужно переделать, чтобы прям построчно добавлялся отчёт, иначе тут всё ложится (надолго зависает), когда очень строк получается для добавления в RichTextBox

            // Переделал, но оно всё равно как бы зависает, из-за того, что через основной поток только добавлять строки можно в richTextBox.
            await this.report.GenerateReport();

            this.radioButtonsUniformGrid.Visibility = Visibility.Hidden;
            this.saveReportButton.Visibility = Visibility.Visible;
        }





        private void fileOptionRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (this.foundFileList != null) // КОСТЫЛЬ
                                            // так как вылетает исключение, что this.foundFileList == null, так как radioButton изменяется ещё до инициализации всего
            {
                int indexSelectedFile = this.foundFileListBox.SelectedIndex;

                if (this.saveCopyFileOptionRadioButton.IsChecked == true)
                {
                    this.foundFileList[indexSelectedFile].FileSaveOption = FileSaveOption.SaveCopy;
                }
                else if (this.replaceOriginalFileOptionRadioButton.IsChecked == true)
                {
                    this.foundFileList[indexSelectedFile].FileSaveOption = FileSaveOption.ReplaceOriginal;
                }
            }
        }

        private void saveReportButton_Click(object sender, RoutedEventArgs e)
        {
            this.report.SaveReportFile(this.fileContentRichTextBox.Document); // сохранение отчёта
        }


        // Задаёт стиль для RichTextBox
        private void ApplyRichTextBoxStyles()
        {
            FlowDocument newFlowDocument = new FlowDocument();

            FontFamily fontFamily = new FontFamily("Segoe UI Variable");
            double fontSize = 14;

            newFlowDocument.FontFamily = fontFamily;
            newFlowDocument.FontSize = fontSize;

            this.fileContentRichTextBox.Document = newFlowDocument;
        }
    }
}
