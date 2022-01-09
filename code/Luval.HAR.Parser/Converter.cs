using System;

namespace Luval.HAR.Parser
{
    public class Converter
    {
        
        public Converter(string content)
        {
            Content = content;
        }

        public string Content { get; private set; }

        public string ToCsv()
        {

        }
    }
}
