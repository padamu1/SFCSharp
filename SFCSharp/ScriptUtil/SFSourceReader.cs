using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace SFCSharp.ScriptUtil
{
    public static class SFSourceReader
    {
        public static List<string> GetScriptsFromFolderPath(string folderPath)
        {
            // 폴더 경로에서 모든 .cs 파일을 가져옴
            if (Directory.Exists(folderPath))
            {
                var files = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

                List<string> sources = new List<string>();
                foreach (var file in files)
                {
                    if (GetScriptFromFilePath(file, out string script))
                    {
                        sources.Add(script);
                    }
                }

                return sources;
            }
            else
            {
                throw new DirectoryNotFoundException($"The directory '{folderPath}' does not exist.");
            }
        }

        public static bool GetScriptFromFilePath(string filePath, out string script)
        {
            script = File.ReadAllText(filePath);

            if (HasAttribute(script, "SFCSharp"))
            {
                return true;
            }

            return false;
        }

        public static bool HasAttribute(string script, string attributeName)
        {
            // 특정 Attribute가 붙어 있는지 확인하는 정규식
            string pattern = $@"\[{attributeName}\]";
            Regex regex = new Regex(pattern);

            // 파일 내에서 Attribute가 있는지 확인
            return regex.IsMatch(script);
        }
    }
}
