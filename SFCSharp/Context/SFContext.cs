using System;
using System.Collections.Generic;

namespace SFCSharp.Context
{
    /// <summary>
    /// SFCSharp 스크립트 실행 컨텍스트
    /// 변수 저장소와 메서드 정보를 관리합니다.
    /// </summary>
    public class SFContext
    {
        private readonly string _namespace;
        private readonly string _className;
        private readonly Dictionary<string, object> _variables;

        public string Namespace => _namespace;
        public string ClassName => _className;

        public SFContext(string namespaceName, string className)
        {
            _namespace = namespaceName ?? "SFCSharp.Scripts";
            _className = className ?? "UnknownScript";
            _variables = new Dictionary<string, object>();
        }

        /// <summary>
        /// 변수 설정
        /// </summary>
        public void SetVariable(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Variable name cannot be null or empty");

            _variables[name] = value;
        }

        /// <summary>
        /// 변수 조회
        /// </summary>
        public object GetVariable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Variable name cannot be null or empty");

            return _variables.ContainsKey(name) ? _variables[name] : null;
        }

        /// <summary>
        /// 변수 존재 여부 확인
        /// </summary>
        public bool HasVariable(string name)
        {
            return _variables.ContainsKey(name);
        }

        /// <summary>
        /// 변수 제거
        /// </summary>
        public bool RemoveVariable(string name)
        {
            return _variables.Remove(name);
        }

        /// <summary>
        /// 모든 변수 초기화
        /// </summary>
        public void ClearVariables()
        {
            _variables.Clear();
        }

        /// <summary>
        /// 현재 저장된 변수 개수
        /// </summary>
        public int VariableCount => _variables.Count;

        public override string ToString()
        {
            return $"SFContext(Namespace={_namespace}, Class={_className}, Variables={_variables.Count})";
        }
    }
}
