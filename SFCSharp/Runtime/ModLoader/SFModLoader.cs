using System;
using System.Collections.Generic;

namespace SFCSharp.Runtime.ModLoader
{
    /// <summary>
    /// MOD 런타임 로더
    /// 스크립트 텍스트를 받아 파싱하고 실행 가능한 SFLoadedMod를 생성합니다.
    /// </summary>
    public class SFModLoader
    {
        /// <summary>
        /// 단일 스크립트 텍스트로부터 MOD를 로드합니다.
        /// </summary>
        /// <param name="scriptName">스크립트 이름</param>
        /// <param name="scriptText">스크립트 텍스트 내용</param>
        /// <returns>로드된 MOD</returns>
        public SFLoadedMod LoadFromText(string scriptName, string scriptText)
        {
            if (string.IsNullOrEmpty(scriptName))
                throw new ArgumentException("Script name cannot be null or empty");
            if (string.IsNullOrEmpty(scriptText))
                throw new ArgumentException("Script text cannot be null or empty");

            var info = new SFModInfo
            {
                ModId = scriptName,
                ModName = scriptName,
                Version = "1.0.0",
                Author = "Unknown",
                ScriptNames = new List<string> { scriptName }
            };

            var scripts = new Dictionary<string, string>
            {
                { scriptName, scriptText }
            };

            return new SFLoadedMod(info, scripts);
        }

        /// <summary>
        /// MOD 정보와 여러 스크립트로부터 MOD를 로드합니다.
        /// </summary>
        /// <param name="modInfo">MOD 메타데이터</param>
        /// <param name="scripts">스크립트 이름 → 스크립트 텍스트 딕셔너리</param>
        /// <returns>로드된 MOD</returns>
        public SFLoadedMod Load(SFModInfo modInfo, Dictionary<string, string> scripts)
        {
            if (modInfo == null)
                throw new ArgumentNullException(nameof(modInfo));
            if (scripts == null || scripts.Count == 0)
                throw new ArgumentException("At least one script is required");

            return new SFLoadedMod(modInfo, scripts);
        }
    }
}
