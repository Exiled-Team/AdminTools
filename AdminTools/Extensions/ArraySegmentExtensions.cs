namespace AdminTools.Extensions
{
    using System;
    using System.Text;

    public static class ArraySegmentExtensions
    {
        public static string FormatArguments(this ArraySegment<string> sentence, int index)
        {
            StringBuilder sb = new();

            foreach (string word in sentence.Segment(index))
            {
                sb.Append(word);
                sb.Append(" ");
            }

            string msg = sb.ToString();
            return msg;
        }
    }
}