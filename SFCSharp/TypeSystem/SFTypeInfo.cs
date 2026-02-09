using System.Collections.Generic;

namespace SFCSharp.TypeSystem
{
    /// <summary>
    /// 타입 정의 정보
    /// 컴포넌트 타입의 상속 관계 및 구현 인터페이스를 관리합니다.
    /// </summary>
    public class SFTypeInfo
    {
        /// <summary>
        /// 타입 이름
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// 부모 타입 이름 (null이면 최상위 타입)
        /// </summary>
        public string BaseTypeName { get; }

        /// <summary>
        /// 구현하는 인터페이스 목록
        /// </summary>
        public IReadOnlyList<string> InterfaceNames { get; }

        /// <summary>
        /// 프로퍼티 기본값
        /// </summary>
        public Dictionary<string, object> DefaultProperties { get; }

        /// <summary>
        /// 빌트인 타입 여부
        /// </summary>
        public bool IsBuiltIn { get; }

        public SFTypeInfo(string typeName, string baseTypeName, List<string> interfaceNames, bool isBuiltIn = false)
        {
            TypeName = typeName;
            BaseTypeName = baseTypeName;
            InterfaceNames = interfaceNames ?? new List<string>();
            DefaultProperties = new Dictionary<string, object>();
            IsBuiltIn = isBuiltIn;
        }

        public override string ToString()
        {
            string baseStr = BaseTypeName != null ? $" : {BaseTypeName}" : "";
            string ifaceStr = InterfaceNames.Count > 0 ? $" [{string.Join(", ", InterfaceNames)}]" : "";
            return $"Type({TypeName}{baseStr}{ifaceStr})";
        }
    }
}
