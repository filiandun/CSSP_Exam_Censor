using System;
using System.Collections;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Censor
{
    internal class FoundFileList : IEnumerable<FoundFile>
    {
        private List<FoundFile> foundFiles;
        private ListBox foundFileListBox;

        public FoundFileList(ListBox listBox)
        {
            this.foundFiles = new List<FoundFile>();
            this.foundFileListBox = listBox;
        }

        public IEnumerator<FoundFile> GetEnumerator()
        {
            return this.foundFiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.foundFiles.GetEnumerator();
        }

        public FoundFile this[int index]
        {
            get
            {
                return this.foundFiles[index];
            }
        }

        public void Add(FoundFile foundFile)
        {
            if (!this.foundFiles.Contains(foundFile))
            {
                this.foundFileListBox.Items.Add(ListBoxItems.CreateFoundFileItem(foundFile));
                this.foundFiles.Add(foundFile);
            }
        }

        public void Remove(FoundFile foundFile)
        {
            if (this.foundFiles.Contains(foundFile))
            {
                this.foundFileListBox.Items.Add(ListBoxItems.CreateFoundFileItem(foundFile));
                this.foundFiles.Remove(foundFile);
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index <= this.foundFiles.Count)
            {
                this.foundFileListBox.Items.RemoveAt(index);
                this.foundFiles.RemoveAt(index);
            }
        }

        public void Clear()
        {
            if (this.foundFiles.Count > 0)
            {
                this.foundFileListBox.Items.Clear();
                this.foundFiles.Clear();
            }
        }

        public int Count()
        {
            return this.foundFiles.Count;
        }
    }
}
