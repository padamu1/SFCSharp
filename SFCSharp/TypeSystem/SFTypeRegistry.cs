using SFCSharp.Execution.UnityExec;
using System;
using System.Collections.Generic;

namespace SFCSharp.TypeSystem
{
    /// <summary>
    /// 타입 레지스트리 (싱글톤)
    /// 모든 컴포넌트 타입과 인터페이스의 등록/조회를 관리합니다.
    /// IL2CPP 환경에서도 안전하게 동작합니다.
    /// </summary>
    public class SFTypeRegistry
    {
        private static SFTypeRegistry _instance;
        private static readonly object _lock = new object();

        private readonly Dictionary<string, SFTypeInfo> _types;
        private readonly Dictionary<string, SFInterfaceInfo> _interfaces;
        private readonly Dictionary<string, Func<SFComponent>> _builtInFactories;

        public static SFTypeRegistry Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SFTypeRegistry();
                        }
                    }
                }
                return _instance;
            }
        }

        private SFTypeRegistry()
        {
            _types = new Dictionary<string, SFTypeInfo>();
            _interfaces = new Dictionary<string, SFInterfaceInfo>();
            _builtInFactories = new Dictionary<string, Func<SFComponent>>();
            RegisterBuiltInTypes();
        }

        /// <summary>
        /// 빌트인 컴포넌트 타입들을 등록합니다.
        /// </summary>
        private void RegisterBuiltInTypes()
        {
            // 기본 컴포넌트 타입
            _types["Component"] = new SFTypeInfo("Component", null, null, isBuiltIn: true);

            // UI 컴포넌트
            _types["Text"] = new SFTypeInfo("Text", "Component", null, isBuiltIn: true);
            _builtInFactories["Text"] = () => new SFText();

            _types["Image"] = new SFTypeInfo("Image", "Component", null, isBuiltIn: true);
            _builtInFactories["Image"] = () => new SFImage();

            // 렌더러
            _types["SpriteRenderer"] = new SFTypeInfo("SpriteRenderer", "Component", null, isBuiltIn: true);
            _builtInFactories["SpriteRenderer"] = () => new SFSpriteRenderer();
        }

        /// <summary>
        /// 인터페이스를 등록합니다.
        /// </summary>
        public void RegisterInterface(SFInterfaceInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            if (string.IsNullOrEmpty(info.InterfaceName))
                throw new ArgumentException("Interface name cannot be null or empty");

            // 부모 인터페이스 존재 확인
            foreach (var parent in info.ParentInterfaces)
            {
                if (!_interfaces.ContainsKey(parent))
                    throw new ArgumentException($"Parent interface not found: {parent}");
            }

            _interfaces[info.InterfaceName] = info;
        }

        /// <summary>
        /// 커스텀 타입을 등록합니다.
        /// </summary>
        public void RegisterType(SFTypeInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            if (string.IsNullOrEmpty(info.TypeName))
                throw new ArgumentException("Type name cannot be null or empty");

            // 부모 타입 존재 확인
            if (info.BaseTypeName != null && !_types.ContainsKey(info.BaseTypeName))
                throw new ArgumentException($"Base type not found: {info.BaseTypeName}");

            // 인터페이스 존재 확인
            foreach (var ifaceName in info.InterfaceNames)
            {
                if (!_interfaces.ContainsKey(ifaceName))
                    throw new ArgumentException($"Interface not found: {ifaceName}");
            }

            _types[info.TypeName] = info;
        }

        /// <summary>
        /// 타입에 기본 프로퍼티를 추가합니다.
        /// </summary>
        public void DefineProperty(string typeName, string propertyName, object defaultValue)
        {
            if (!_types.ContainsKey(typeName))
                throw new ArgumentException($"Type not found: {typeName}");

            var typeInfo = _types[typeName];
            if (typeInfo.IsBuiltIn)
                throw new InvalidOperationException($"Cannot add properties to built-in type: {typeName}");

            typeInfo.DefaultProperties[propertyName] = defaultValue;
        }

        /// <summary>
        /// 빌트인 타입에 인터페이스를 추가합니다.
        /// 이미 등록된 빌트인 타입이 특정 인터페이스를 구현하도록 선언합니다.
        /// </summary>
        public void AddInterfaceToType(string typeName, string interfaceName)
        {
            if (!_types.ContainsKey(typeName))
                throw new ArgumentException($"Type not found: {typeName}");
            if (!_interfaces.ContainsKey(interfaceName))
                throw new ArgumentException($"Interface not found: {interfaceName}");

            var oldInfo = _types[typeName];
            var newInterfaces = new List<string>(oldInfo.InterfaceNames);
            if (!newInterfaces.Contains(interfaceName))
            {
                newInterfaces.Add(interfaceName);
            }

            _types[typeName] = new SFTypeInfo(
                oldInfo.TypeName,
                oldInfo.BaseTypeName,
                newInterfaces,
                oldInfo.IsBuiltIn
            );
        }

        /// <summary>
        /// 컴포넌트 인스턴스를 생성합니다.
        /// </summary>
        public SFComponent CreateComponent(string typeName)
        {
            if (!_types.ContainsKey(typeName))
                throw new ArgumentException($"Unknown component type: {typeName}");

            // 빌트인 타입
            if (_builtInFactories.ContainsKey(typeName))
            {
                return _builtInFactories[typeName]();
            }

            // 커스텀 타입
            var typeInfo = _types[typeName];
            return new SFCustomComponent(typeName, typeInfo.DefaultProperties);
        }

        /// <summary>
        /// derivedType이 baseType의 하위 타입인지 확인합니다.
        /// </summary>
        public bool IsSubclassOf(string derivedType, string baseType)
        {
            if (derivedType == baseType)
                return true;

            if (!_types.ContainsKey(derivedType))
                return false;

            string current = derivedType;
            while (current != null)
            {
                if (current == baseType)
                    return true;

                if (!_types.ContainsKey(current))
                    return false;

                current = _types[current].BaseTypeName;
            }

            return false;
        }

        /// <summary>
        /// 타입이 특정 인터페이스를 구현하는지 확인합니다.
        /// 상속 체인의 모든 타입과 인터페이스 상속을 검사합니다.
        /// </summary>
        public bool ImplementsInterface(string typeName, string interfaceName)
        {
            if (!_types.ContainsKey(typeName))
                return false;
            if (!_interfaces.ContainsKey(interfaceName))
                return false;

            // 상속 체인을 따라 올라가며 검사
            string current = typeName;
            while (current != null)
            {
                if (!_types.ContainsKey(current))
                    break;

                var typeInfo = _types[current];

                foreach (var ifaceName in typeInfo.InterfaceNames)
                {
                    if (IsInterfaceAssignableTo(ifaceName, interfaceName))
                        return true;
                }

                current = typeInfo.BaseTypeName;
            }

            return false;
        }

        /// <summary>
        /// 인터페이스 상속 관계를 확인합니다.
        /// derived 인터페이스가 target 인터페이스이거나 target을 상속하는지 검사합니다.
        /// </summary>
        private bool IsInterfaceAssignableTo(string derived, string target)
        {
            if (derived == target)
                return true;

            if (!_interfaces.ContainsKey(derived))
                return false;

            var info = _interfaces[derived];
            foreach (var parent in info.ParentInterfaces)
            {
                if (IsInterfaceAssignableTo(parent, target))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 타입 또는 인터페이스에 할당 가능한지 확인합니다.
        /// </summary>
        public bool IsAssignableTo(string typeName, string targetName)
        {
            // 인터페이스 체크
            if (_interfaces.ContainsKey(targetName))
            {
                return ImplementsInterface(typeName, targetName);
            }

            // 타입 체크
            return IsSubclassOf(typeName, targetName);
        }

        /// <summary>
        /// 타입 정보를 조회합니다.
        /// </summary>
        public SFTypeInfo? GetTypeInfo(string typeName)
        {
            return _types.ContainsKey(typeName) ? _types[typeName] : null;
        }

        /// <summary>
        /// 인터페이스 정보를 조회합니다.
        /// </summary>
        public SFInterfaceInfo? GetInterfaceInfo(string interfaceName)
        {
            return _interfaces.ContainsKey(interfaceName) ? _interfaces[interfaceName] : null;
        }

        /// <summary>
        /// 타입이 등록되어 있는지 확인합니다.
        /// </summary>
        public bool HasType(string typeName)
        {
            return _types.ContainsKey(typeName);
        }

        /// <summary>
        /// 인터페이스가 등록되어 있는지 확인합니다.
        /// </summary>
        public bool HasInterface(string interfaceName)
        {
            return _interfaces.ContainsKey(interfaceName);
        }

        /// <summary>
        /// 등록된 모든 타입 이름을 반환합니다.
        /// </summary>
        public IReadOnlyCollection<string> GetAllTypeNames()
        {
            return _types.Keys;
        }

        /// <summary>
        /// 등록된 모든 인터페이스 이름을 반환합니다.
        /// </summary>
        public IReadOnlyCollection<string> GetAllInterfaceNames()
        {
            return _interfaces.Keys;
        }
    }
}
