using System;
using System.Text;

namespace AdminTools.Extensions
{
    public static class ArraySegmentExtensions
    {
        public static string FormatArguments(this ArraySegment<string> sentence, int index)
        {
            StringBuilder sb = new();
        
            foreach (var word in sentence.Segment(index))
            {
                sb.Append(word);
                sb.Append(" ");
            }

            var msg = sb.ToString();
            return msg;
        }
    }
}