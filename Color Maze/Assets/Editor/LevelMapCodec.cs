using System;
using System.Collections.Generic;

/// <summary>
/// 关卡地图文本编解码器：把粘贴的纯文本归一化为等宽的合法字符行，
/// 或把字符行编码回可复制文本。只负责文本格式；
/// 起点、材料数量等关卡语义由 LevelValidator 检查。
/// </summary>
public static class LevelMapCodec
{
    /// <summary>合法的地图字符集；LevelValidator 的逐格检查复用同一份定义。</summary>
    public const string AllowedTiles = ".#SRBCME123456789";

    /// <summary>
    /// 归一化粘贴文本：统一 CRLF/CR 为 LF、去掉每行首尾空白、删除首尾空行，
    /// 再依次校验中间空行、行宽一致与未知字符。
    /// 成功时 rows 为等宽合法行（上到下）；失败时 rows 为 null，error 带行列位置。
    /// </summary>
    public static bool TryNormalize(string text, out string[] rows, out string error)
    {
        rows = null;

        if (string.IsNullOrWhiteSpace(text))
        {
            error = "地图为空：没有可解析的文本。";
            return false;
        }

        string[] lines = text
            .Replace("\r\n", "\n")
            .Replace('\r', '\n')
            .Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].Trim();
        }

        int first = 0;
        int last = lines.Length - 1;
        while (first <= last && lines[first].Length == 0)
        {
            first += 1;
        }

        while (last >= first && lines[last].Length == 0)
        {
            last -= 1;
        }

        if (first > last)
        {
            error = "地图为空：粘贴的内容只有空白行。";
            return false;
        }

        string[] map = new string[last - first + 1];
        Array.Copy(lines, first, map, 0, map.Length);
        int width = map[0].Length;

        for (int row = 0; row < map.Length; row++)
        {
            string line = map[row];
            int rowNumber = row + 1;

            // 空行判断必须先于行宽判断，否则中间空行会被报成“行宽 0”。
            if (line.Length == 0)
            {
                error = $"第 {rowNumber} 行是空行：地图中间不允许空行。";
                return false;
            }

            if (line.Length != width)
            {
                error =
                    $"第 {rowNumber} 行宽为 {line.Length}，" +
                    $"应为 {width}。";
                return false;
            }

            int column = FindInvalidTile(line);
            if (column >= 0)
            {
                error =
                    $"第 {rowNumber} 行第 {column + 1} 列" +
                    $"包含未知字符 '{line[column]}'。";
                return false;
            }
        }

        rows = map;
        error = null;
        return true;
    }

    /// <summary>把行数组编码为 LF 分隔的纯文本，用于复制与回读。</summary>
    public static string Encode(IEnumerable<string> rows)
    {
        return rows == null ? string.Empty : string.Join("\n", rows);
    }

    public static bool IsAllowedTile(char tile)
    {
        return AllowedTiles.IndexOf(tile) >= 0;
    }

    private static int FindInvalidTile(string line)
    {
        for (int column = 0; column < line.Length; column++)
        {
            if (!IsAllowedTile(line[column]))
            {
                return column;
            }
        }

        return -1;
    }
}
