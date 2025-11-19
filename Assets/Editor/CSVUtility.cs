using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static class CSVUtility
{
    public static List<Dictionary<string, string>> ParseCSV(string filePath)
    {
        List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

        if (!File.Exists(filePath))
        {
            Debug.LogError($"CSV file not found: {filePath}");
            return result;
        }

        string[] lines = File.ReadAllLines(filePath);
        if (lines.Length < 2) return result;

        // 解析表头
        string[] headers = ParseCSVLine(lines[0]);

        // 解析数据行
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = ParseCSVLine(lines[i]);
            Dictionary<string, string> row = new Dictionary<string, string>();

            // 确保每行数据与表头对应
            for (int j = 0; j < Mathf.Min(headers.Length, values.Length); j++)
            {
                string value = values[j].Trim();
                row[headers[j]] = value;
            }

            result.Add(row);
        }

        return result;
    }

    private static string[] ParseCSVLine(string line)
    {
        List<string> values = new List<string>();
        int position = 0;
        bool inQuotes = false;
        string current = "";

        while (position < line.Length)
        {
            char c = line[position];

            if (c == '"')
            {
                if (inQuotes && position < line.Length - 1 && line[position + 1] == '"')
                {
                    // 转义引号
                    current += '"';
                    position++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                values.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }

            position++;
        }

        values.Add(current);
        return values.ToArray();
    }
}