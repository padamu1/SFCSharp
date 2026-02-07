using System;
using System.Collections.Generic;

namespace SFCSharp.Execution.UnityExec
{
    /// <summary>
    /// UnityEngine 네임스페이스의 기본 타입들을 래핑하는 클래스
    /// SFCSharp에서 Unity의 주요 클래스와 메서드를 지원하기 위한 인터페이스를 제공합니다.
    /// </summary>

    #region Vector3

    /// <summary>
    /// UnityEngine.Vector3를 래핑하는 클래스
    /// </summary>
    public class SFVector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public SFVector3() : this(0, 0, 0) { }

        public SFVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static SFVector3 zero => new SFVector3(0, 0, 0);
        public static SFVector3 one => new SFVector3(1, 1, 1);
        public static SFVector3 up => new SFVector3(0, 1, 0);
        public static SFVector3 down => new SFVector3(0, -1, 0);
        public static SFVector3 left => new SFVector3(-1, 0, 0);
        public static SFVector3 right => new SFVector3(1, 0, 0);
        public static SFVector3 forward => new SFVector3(0, 0, 1);
        public static SFVector3 back => new SFVector3(0, 0, -1);

        public float magnitude => (float)Math.Sqrt(x * x + y * y + z * z);

        public SFVector3 normalized
        {
            get
            {
                float mag = magnitude;
                if (mag == 0)
                    return zero;
                return new SFVector3(x / mag, y / mag, z / mag);
            }
        }

        public static SFVector3 operator +(SFVector3 a, SFVector3 b)
            => new SFVector3(a.x + b.x, a.y + b.y, a.z + b.z);

        public static SFVector3 operator -(SFVector3 a, SFVector3 b)
            => new SFVector3(a.x - b.x, a.y - b.y, a.z - b.z);

        public static SFVector3 operator *(SFVector3 a, float scalar)
            => new SFVector3(a.x * scalar, a.y * scalar, a.z * scalar);

        public static SFVector3 operator *(float scalar, SFVector3 a)
            => new SFVector3(a.x * scalar, a.y * scalar, a.z * scalar);

        public static float Distance(SFVector3 a, SFVector3 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public static float Dot(SFVector3 a, SFVector3 b)
            => a.x * b.x + a.y * b.y + a.z * b.z;

        public static SFVector3 Cross(SFVector3 a, SFVector3 b)
            => new SFVector3(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x);

        public override string ToString() => $"({x}, {y}, {z})";
    }

    #endregion

    #region Quaternion

    /// <summary>
    /// UnityEngine.Quaternion를 래핑하는 클래스
    /// </summary>
    public class SFQuaternion
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float w { get; set; }

        public SFQuaternion() : this(0, 0, 0, 1) { }

        public SFQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static SFQuaternion identity => new SFQuaternion(0, 0, 0, 1);

        /// <summary>
        /// Euler 각도로부터 Quaternion 생성 (도 단위)
        /// </summary>
        public static SFQuaternion Euler(float x, float y, float z)
        {
            // 단순화된 Euler 변환 (실제 구현은 더 복잡함)
            float cx = (float)Math.Cos(x * Math.PI / 360);
            float cy = (float)Math.Cos(y * Math.PI / 360);
            float cz = (float)Math.Cos(z * Math.PI / 360);
            float sx = (float)Math.Sin(x * Math.PI / 360);
            float sy = (float)Math.Sin(y * Math.PI / 360);
            float sz = (float)Math.Sin(z * Math.PI / 360);

            return new SFQuaternion(
                sx * cy * cz - cx * sy * sz,
                cx * sy * cz + sx * cy * sz,
                cx * cy * sz - sx * sy * cz,
                cx * cy * cz + sx * sy * sz);
        }

        public override string ToString() => $"({x}, {y}, {z}, {w})";
    }

    #endregion

    #region Color

    /// <summary>
    /// UnityEngine.Color를 래핑하는 클래스
    /// </summary>
    public class SFColor
    {
        public float r { get; set; }
        public float g { get; set; }
        public float b { get; set; }
        public float a { get; set; }

        public SFColor() : this(0, 0, 0, 1) { }

        public SFColor(float r, float g, float b, float a = 1f)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static SFColor red => new SFColor(1, 0, 0, 1);
        public static SFColor green => new SFColor(0, 1, 0, 1);
        public static SFColor blue => new SFColor(0, 0, 1, 1);
        public static SFColor white => new SFColor(1, 1, 1, 1);
        public static SFColor black => new SFColor(0, 0, 0, 1);
        public static SFColor yellow => new SFColor(1, 0.92f, 0.016f, 1);
        public static SFColor cyan => new SFColor(0, 1, 1, 1);
        public static SFColor magenta => new SFColor(1, 0, 1, 1);
        public static SFColor gray => new SFColor(0.5f, 0.5f, 0.5f, 1);
        public static SFColor clear => new SFColor(0, 0, 0, 0);

        public override string ToString() => $"RGBA({r}, {g}, {b}, {a})";
    }

    #endregion

    #region Component

    /// <summary>
    /// UnityEngine.Component 기반 클래스
    /// 모든 컴포넌트의 공통 베이스입니다.
    /// </summary>
    public abstract class SFComponent
    {
        public SFGameObject gameObject { get; internal set; }
        public SFTransform transform => gameObject?.transform;
        public bool enabled { get; set; } = true;

        /// <summary>
        /// 컴포넌트 타입 이름을 반환합니다.
        /// </summary>
        public abstract string ComponentTypeName { get; }
    }

    #endregion

    #region UI Components

    /// <summary>
    /// UnityEngine.UI.Text를 래핑하는 컴포넌트
    /// </summary>
    public class SFText : SFComponent
    {
        public override string ComponentTypeName => "Text";

        public string text { get; set; } = "";
        public int fontSize { get; set; } = 14;
        public SFColor color { get; set; } = SFColor.black;
        public TextAnchor alignment { get; set; } = TextAnchor.UpperLeft;
        public FontStyle fontStyle { get; set; } = FontStyle.Normal;

        public override string ToString() => $"Text(\"{text}\", size={fontSize})";
    }

    /// <summary>
    /// UnityEngine.UI.Image를 래핑하는 컴포넌트
    /// </summary>
    public class SFImage : SFComponent
    {
        public override string ComponentTypeName => "Image";

        public SFColor color { get; set; } = SFColor.white;
        public float fillAmount { get; set; } = 1.0f;
        public string spriteName { get; set; }
        public ImageType type { get; set; } = ImageType.Simple;

        public override string ToString() => $"Image(color={color}, fill={fillAmount})";
    }

    /// <summary>
    /// UnityEngine.SpriteRenderer를 래핑하는 컴포넌트
    /// </summary>
    public class SFSpriteRenderer : SFComponent
    {
        public override string ComponentTypeName => "SpriteRenderer";

        public SFColor color { get; set; } = SFColor.white;
        public string spriteName { get; set; }
        public int sortingOrder { get; set; } = 0;
        public bool flipX { get; set; }
        public bool flipY { get; set; }

        public override string ToString() => $"SpriteRenderer(sprite={spriteName}, color={color})";
    }

    public enum TextAnchor
    {
        UpperLeft, UpperCenter, UpperRight,
        MiddleLeft, MiddleCenter, MiddleRight,
        LowerLeft, LowerCenter, LowerRight
    }

    public enum FontStyle
    {
        Normal, Bold, Italic, BoldAndItalic
    }

    public enum ImageType
    {
        Simple, Sliced, Tiled, Filled
    }

    #endregion

    #region Transform

    /// <summary>
    /// UnityEngine.Transform를 래핑하는 클래스
    /// </summary>
    public class SFTransform
    {
        private string _name;
        private SFVector3 _position;
        private SFQuaternion _rotation;
        private SFVector3 _scale;
        private SFGameObject? _gameObject;

        public SFTransform(string name = "Transform")
        {
            _name = name;
            _position = SFVector3.zero;
            _rotation = SFQuaternion.identity;
            _scale = SFVector3.one;
            // GameObject는 외부에서 설정하도록 변경
            _gameObject = null;
        }

        /// <summary>
        /// 내부용: GameObject를 설정합니다. (순환 참조 방지)
        /// </summary>
        internal void SetGameObject(SFGameObject gameObject)
        {
            _gameObject = gameObject;
        }

        public string name
        {
            get => _name;
            set => _name = value;
        }

        public SFVector3 position
        {
            get => _position;
            set => _position = value;
        }

        public SFQuaternion rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public SFVector3 eulerAngles
        {
            get => GetEulerAngles();
            set => _rotation = SFQuaternion.Euler(value.x, value.y, value.z);
        }

        public SFVector3 scale
        {
            get => _scale;
            set => _scale = value;
        }

        public SFGameObject? gameObject => _gameObject;

        public void Translate(float x, float y, float z)
        {
            _position.x += x;
            _position.y += y;
            _position.z += z;
        }

        public void Translate(SFVector3 translation)
        {
            Translate(translation.x, translation.y, translation.z);
        }

        public void Rotate(float x, float y, float z)
        {
            var currentEuler = GetEulerAngles();
            eulerAngles = new SFVector3(currentEuler.x + x, currentEuler.y + y, currentEuler.z + z);
        }

        public void Rotate(SFVector3 eulerAngles)
        {
            Rotate(eulerAngles.x, eulerAngles.y, eulerAngles.z);
        }

        public void LookAt(SFVector3 target)
        {
            var direction = (target - position).normalized;
            // 단순화된 LookAt (실제 구현은 더 복잡함)
        }

        private SFVector3 GetEulerAngles()
        {
            // 단순화된 Quaternion to Euler 변환
            float test = _rotation.x * _rotation.y + _rotation.z * _rotation.w;
            if (test > 0.499f)
            {
                return new SFVector3(0, 2 * (float)Math.Atan2(_rotation.x, _rotation.w) * 180 / (float)Math.PI, 90);
            }
            if (test < -0.499f)
            {
                return new SFVector3(0, -2 * (float)Math.Atan2(_rotation.x, _rotation.w) * 180 / (float)Math.PI, -90);
            }

            float sqx = _rotation.x * _rotation.x;
            float sqy = _rotation.y * _rotation.y;
            float sqz = _rotation.z * _rotation.z;

            float x = (float)Math.Atan2(2 * _rotation.y * _rotation.w - 2 * _rotation.x * _rotation.z, 1 - 2 * sqy - 2 * sqz) * 180 / (float)Math.PI;
            float y = (float)Math.Asin(2 * test) * 180 / (float)Math.PI;
            float z = (float)Math.Atan2(2 * _rotation.x * _rotation.w - 2 * _rotation.y * _rotation.z, 1 - 2 * sqx - 2 * sqz) * 180 / (float)Math.PI;

            return new SFVector3(x, y, z);
        }

        public override string ToString() => $"Transform ({_name}) - Pos: {_position}, Rot: {_rotation.ToString()}, Scale: {_scale}";
    }

    #endregion

    #region GameObject

    /// <summary>
    /// UnityEngine.GameObject를 래핑하는 클래스
    /// </summary>
    public class SFGameObject
    {
        private string _name;
        private bool _active;
        private SFTransform _transform;
        private readonly Dictionary<string, SFComponent> _components = new Dictionary<string, SFComponent>();

        public SFGameObject(string name = "GameObject")
        {
            _name = name;
            _active = true;
            _transform = new SFTransform(name);
            _transform.SetGameObject(this);
        }

        public string name
        {
            get => _name;
            set => _name = value;
        }

        public bool activeSelf
        {
            get => _active;
            set => _active = value;
        }

        public SFTransform transform => _transform;

        public void SetActive(bool value)
        {
            _active = value;
        }

        /// <summary>
        /// 컴포넌트를 추가합니다. Unity의 AddComponent&lt;T&gt;()에 해당합니다.
        /// </summary>
        public SFComponent AddComponent(string typeName)
        {
            SFComponent component;
            switch (typeName)
            {
                case "Text":
                    component = new SFText();
                    break;
                case "Image":
                    component = new SFImage();
                    break;
                case "SpriteRenderer":
                    component = new SFSpriteRenderer();
                    break;
                default:
                    throw new ArgumentException($"Unknown component type: {typeName}");
            }

            component.gameObject = this;
            _components[typeName] = component;
            return component;
        }

        /// <summary>
        /// 컴포넌트를 가져옵니다. Unity의 GetComponent&lt;T&gt;()에 해당합니다.
        /// </summary>
        public SFComponent GetComponent(string typeName)
        {
            return _components.ContainsKey(typeName) ? _components[typeName] : null;
        }

        /// <summary>
        /// 컴포넌트가 있는지 확인합니다.
        /// </summary>
        public bool HasComponent(string typeName)
        {
            return _components.ContainsKey(typeName);
        }

        /// <summary>
        /// 컴포넌트를 제거합니다. Unity의 Destroy(component)에 해당합니다.
        /// </summary>
        public bool RemoveComponent(string typeName)
        {
            return _components.Remove(typeName);
        }

        public override string ToString() => $"GameObject ({_name}) - Active: {_active}, Components: {_components.Count}";
    }

    #endregion
}
