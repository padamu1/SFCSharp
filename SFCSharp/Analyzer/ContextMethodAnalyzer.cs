using System;
using System.Collections.Generic;

namespace SFCSharp.Analyzer
{
    public class ContextMethodAnalyzer
    {
        public static void Parse(string script, out string method, out List<object> param)
        {
            int paramStart = script.IndexOf('(');

            method = script.Substring(0, paramStart);

            script = script.Remove(0, paramStart + 1);

            int paramEnd = script.LastIndexOf(')');

            script = script.Remove(paramEnd, script.Length - paramEnd);

            param = GetParam(script.Trim());
        }

        private static List<object> GetParam(string script)
        {
            List<object> param = new List<object>();

            while (script.Length > 0)
            {
                char character = script[0];

                script = script.Remove(0, 1);
                if (character == '"')
                {
                    int index = 0;
                    while (true)
                    {
                        if (script[index] == '"')
                        {
                            break;
                        }

                        ++index;
                    }

                    param.Add(script.Substring(0, index));
                    script = script.Remove(0, index + 1);
                }
                // new 또는 null
                else if (character == 'n')
                {

                }
                else if (character == ' ' || character == ',')
                {

                }
                else
                {
                    int seperatorIndex = script.IndexOf(',');

                    param.Add(string.Concat(character, script.Substring(0, seperatorIndex)));
                    script = script.Remove(0, seperatorIndex);
                }
            }

            return param;
        }
    }
}
