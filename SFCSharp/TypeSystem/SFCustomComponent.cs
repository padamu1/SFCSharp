using SFCSharp.Execution.UnityExec;
using System;
using System.Collections.Generic;

namespace SFCSharp.TypeSystem
{
    /// <summary>
    /// 사용자 정의 컴포넌트
    /// MOD에서 정의한 커스텀 타입의 인스턴스를 나타냅니다.
    /// Dictionary 기반의 프로퍼티 시스템으로 AOT 안전합니다.
    /// </summary>
    public class SFCustomComponent : SFComponent
    {
        private readonly string _typeName;
        private readonly Dictionary<string, object> _properties;

        public override string ComponentTypeName => _typeName;

        public SFCustomComponent(string typeName, Dictionary<string, object> defaultProperties = null)
        {
            _typeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
            _properties = new Dictionary<string, object>();

            if (defaultProperties != null)
            {
                foreach (var kvp in defaultProperties)
                {
                    _properties[kvp.Key] = kvp.Value;
                }
            }
        }

        /// <summary>
        /// 프로퍼티 값을 설정합니다.
        /// </summary>
        public void SetProperty(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Property name cannot be null or empty");

            _properties[name] = value;
        }

        /// <summary>
        /// 프로퍼티 값을 조회합니다.
        /// </summary>
        public object? GetProperty(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Property name cannot be null or empty");

            return _properties.ContainsKey(name) ? _properties[name] : null;
        }

        /// <summary>
        /// 프로퍼티가 존재하는지 확인합니다.
        /// </summary>
        public bool HasProperty(string name)
        {
            return _properties.ContainsKey(name);
        }

        /// <summary>
        /// 프로퍼티를 제거합니다.
        /// </summary>
        public bool RemoveProperty(string name)
        {
            return _properties.Remove(name);
        }

        /// <summary>
        /// 전체 프로퍼티 이름 목록을 반환합니다.
        /// </summary>
        public IReadOnlyCollection<string> GetPropertyNames()
        {
            return _properties.Keys;
        }

        public override string ToString() => $"CustomComponent({_typeName}, props={_properties.Count})";
    }
}
