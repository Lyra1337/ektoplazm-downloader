using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EktoplazmExtractor.ViewModels
{
    public class DownloadType
    {
        public string Key { get; }
        public int Count { get; }
        public string DisplayName { get; }

        public DownloadType(string key, int count)
        {
            this.Key = key;
            this.Count = count;
            this.DisplayName = $"{key} ({count})";
        }
    }
}