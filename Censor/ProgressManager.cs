using System;
using System.Windows.Controls;

namespace Censor
{
    internal class ProgressManager
    {
        private static ProgressBar progressBar;

        public static void Initialize(ProgressBar progressBar)
        {
            ProgressManager.progressBar = progressBar;
        }

        public static void ReportProgress(int value)
        {
            if (progressBar != null)
            {
                progressBar.Dispatcher.Invoke(() => progressBar.Value = value);
            }
        }
    }
}
