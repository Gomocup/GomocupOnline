using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SocketUploader
{
    public class UploaderItem
    {
        public string SourceFileName { get; set; }
        public string TargetFileName { get; set; }      
    }

    public class UploaderConfig
    {
        public string Url { get; set; }

        public List<UploaderItem> Items = new List<UploaderItem>();

        public UploaderConfig(string path)
        {
            XDocument doc = XDocument.Load(path);

            Url = doc.Root.Element("url").Value;

            var elements = doc.Root.Element("items").Elements();

            foreach (XElement item in elements)
            {               
                UploaderItem uploaderItem = new UploaderItem()
                {
                    SourceFileName = item.Element("source").Value.ToLower(),
                    TargetFileName = item.Element("target").Value,
                };

                Items.Add(uploaderItem);
            }

        }
   
    }
}
