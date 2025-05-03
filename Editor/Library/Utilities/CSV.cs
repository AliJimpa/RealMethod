using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RealMethod
{
    static class CSV
    {
        public static string[] ParseCSVRow(string csvLine)
        {
            return ParseSingleLine(csvLine).ToArray();
        }

        public static List<string[]> ParseCSV(string csvText)
        {
            List<string[]> rows = new List<string[]>();
            // Split text into lines (handling different OS newline formats)
            string[] lines = csvText.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                rows.Add(ParseSingleLine(line).ToArray());
            }

            return rows;
        }

        public static string[] ParseCSVToFlatArray(string csvText)
        {
            List<string> cells = new List<string>();
            string[] lines = csvText.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                cells.AddRange(ParseSingleLine(line));
            }

            return cells.ToArray();
        }

        // This method parses a single line of CSV text and returns a list of strings.
        private static List<string> ParseSingleLine(string csvLine)
        {
            List<string> result = new List<string>();
            string pattern = @"(?:^|,)(?:(?:""((?:[^""]|"""")*)"")|([^,""]*))";

            foreach (Match match in Regex.Matches(csvLine, pattern))
            {
                string value = match.Groups[1].Success ? match.Groups[1].Value.Replace("\"\"", "\"") : match.Groups[2].Value;
                result.Add(value.Trim());
            }

            return result;
        }
    }
}