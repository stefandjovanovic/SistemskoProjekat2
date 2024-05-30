using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SistemskoProjekat2.Cache
{
    public class Node
    {
        public Node? Prev { get; set; }
        public Node? Next { get; set; }
        public string Key { get; set; }
        public string Data { get; set; }

        public Node(string Key, string Data)
        {
            this.Key = Key;
            this.Data = Data;
        }

       
    }
}
