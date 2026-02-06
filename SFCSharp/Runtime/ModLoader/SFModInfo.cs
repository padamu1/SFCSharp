using System.Collections.Generic;

namespace SFCSharp.Runtime.ModLoader
{
    /// <summary>
    /// MOD 메타데이터 정보
    /// </summary>
    public class SFModInfo
    {
        public string ModId { get; set; }
        public string ModName { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// MOD에 포함된 스크립트 이름 목록
        /// </summary>
        public List<string> ScriptNames { get; set; } = new List<string>();

        public override string ToString()
        {
            return $"{ModName} v{Version} by {Author} (id: {ModId})";
        }
    }
}
