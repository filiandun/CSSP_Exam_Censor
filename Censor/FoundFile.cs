using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Censor
{
    public enum FileSaveOption
    {
        SaveCopy,
        ReplaceOriginal
    }

    internal class FoundFile
    {
        private Image icon;
        private FileInfo fileInfo;

        private FileSaveOption fileSaveOption;

        private Dictionary<string, int> countCensorWord;

        public FoundFile(string path)
        {
            this.fileInfo = new FileInfo(path);

            this.fileSaveOption = FileSaveOption.SaveCopy;
            this.countCensorWord = new Dictionary<string, int>();
        }

        public FileInfo FileInfo
        {
            get
            {
                return this.fileInfo;
            }
        }

        public FileSaveOption FileSaveOption
        {
            get { return this.fileSaveOption; }
            set { this.fileSaveOption = value; }
        }

        public int CountCensorWord(string censorWord)
        {
            if (!this.countCensorWord.ContainsKey(censorWord))
            {
                return 0;
            }
            else
            {
                return this.countCensorWord[censorWord];
            }
        }

        public void AddCensorWord(string censorWord, int count)
        {
            if (!this.countCensorWord.ContainsKey(censorWord))
            {
                this.countCensorWord.Add(censorWord, count);
            }
        }

    }
}
