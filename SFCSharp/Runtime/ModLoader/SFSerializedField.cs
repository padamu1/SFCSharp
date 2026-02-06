using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SFCSharp.Runtime.ModLoader
{
    /// <summary>
    /// 직렬화된 필드 정보
    /// 모더가 인스펙터에서 설정한 [SerializeField] 값을 저장합니다.
    /// </summary>
    public class SFSerializedField
    {
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{FieldType} {FieldName} = {Value}";
        }
    }

    /// <summary>
    /// 스크립트 텍스트에서 SerializeField 메타데이터를 파싱하고,
    /// 런타임에 변수로 주입하는 유틸리티
    /// </summary>
    public static class SFSerializedFieldParser
    {
        /// <summary>
        /// 스크립트 텍스트에서 SerializeField 메타데이터를 파싱합니다.
        /// 형식: // @field:타입 이름 = 값
        /// </summary>
        public static List<SFSerializedField> Parse(string scriptText)
        {
            var fields = new List<SFSerializedField>();

            if (string.IsNullOrEmpty(scriptText))
                return fields;

            string[] lines = scriptText.Split('\n');
            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (!trimmed.StartsWith("// @field:"))
                    continue;

                string fieldData = trimmed.Substring("// @field:".Length).Trim();

                // 형식: 타입 이름 = 값
                var match = Regex.Match(fieldData, @"^(\w+)\s+(\w+)\s*=\s*(.+)$");
                if (match.Success)
                {
                    fields.Add(new SFSerializedField
                    {
                        FieldType = match.Groups[1].Value,
                        FieldName = match.Groups[2].Value,
                        Value = match.Groups[3].Value.Trim()
                    });
                }
            }

            return fields;
        }

        /// <summary>
        /// 파싱된 필드 값을 적절한 타입으로 변환합니다.
        /// </summary>
        public static object ConvertValue(SFSerializedField field)
        {
            if (field == null || field.Value == null)
                return null;

            switch (field.FieldType.ToLower())
            {
                case "int":
                    return int.TryParse(field.Value, out int intVal) ? intVal : 0;

                case "float":
                    return float.TryParse(field.Value, out float floatVal) ? floatVal : 0f;

                case "double":
                    return double.TryParse(field.Value, out double doubleVal) ? doubleVal : 0.0;

                case "bool":
                    return bool.TryParse(field.Value, out bool boolVal) && boolVal;

                case "string":
                    string strVal = field.Value;
                    if ((strVal.StartsWith("\"") && strVal.EndsWith("\"")) ||
                        (strVal.StartsWith("'") && strVal.EndsWith("'")))
                    {
                        return strVal.Substring(1, strVal.Length - 2);
                    }
                    return strVal;

                default:
                    return field.Value;
            }
        }

        /// <summary>
        /// 파싱된 필드들을 컨텍스트 변수로 주입합니다.
        /// </summary>
        public static Dictionary<string, object> ConvertAll(List<SFSerializedField> fields)
        {
            var result = new Dictionary<string, object>();
            foreach (var field in fields)
            {
                result[field.FieldName] = ConvertValue(field);
            }
            return result;
        }
    }
}
