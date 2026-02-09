using System.Collections.Generic;

namespace SFCSharp.TypeSystem
{
    /// <summary>
    /// 인터페이스 정의 정보
    /// MOD에서 정의한 인터페이스의 메타데이터를 관리합니다.
    /// </summary>
    public class SFInterfaceInfo
    {
        /// <summary>
        /// 인터페이스 이름
        /// </summary>
        public string InterfaceName { get; }

        /// <summary>
        /// 부모 인터페이스 목록 (인터페이스 상속)
        /// </summary>
        public IReadOnlyList<string> ParentInterfaces { get; }

        public SFInterfaceInfo(string interfaceName, List<string> parentInterfaces = null)
        {
            InterfaceName = interfaceName;
            ParentInterfaces = parentInterfaces ?? new List<string>();
        }

        public override string ToString() => $"Interface({InterfaceName})";
    }
}
