using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EktoplazmExtractor.Services
{
    internal sealed class Album
    {
        public string Name { get; }
        public string Url { get; }
        public Dictionary<string, string> Downloads { get; }

        public Album(string name, string url, Dictionary<string, string> downloads)
        {
            this.Name = name;
            this.Url = url;
            this.Downloads = downloads;
        }
    }
}