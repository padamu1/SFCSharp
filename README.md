# SFCSharp

Unity 기반 프로젝트를 위한 C# 스크립팅/모딩 라이브러리입니다.

## 프로젝트 소개

SFCSharp는 Unity 프로젝트에서 **모드(MOD) 제작과 배포**를 지원하는 프레임워크입니다.

모드 제작자는 기존 Unity 개발 방식 그대로 C# 스크립트와 프리팹을 작성하고, `[SFCSharp]` 어트리뷰트만 추가하면 빌드 시스템이 자동으로 MOD 패키지로 변환합니다. 완성된 MOD는 중앙 저장소에 업로드하여 다른 유저들이 다운로드하고 런타임에 로드하여 사용할 수 있습니다.

### 핵심 특징

- **Unity 네이티브 경험** - C# + Unity를 사용하는 환경과 동일한 API를 제공하여 별도 학습 없이 MOD를 제작할 수 있습니다.
- **자동 코드 변환** - `[SFCSharp]` 어트리뷰트가 붙은 MonoBehaviour를 자동으로 SFCSharp 스크립트로 변환하고 Prefab의 컴포넌트도 자동 교체합니다.
- **인터페이스 및 상속** - 커스텀 타입 정의, 인터페이스 구현, 상속 기반 폴리모픽 컴포넌트 조회를 지원합니다.
- **MOD 배포 시스템** - HTTP 기반 MOD 저장소를 통해 업로드, 다운로드, 검색, 삭제가 가능합니다.
- **IL2CPP / 모바일 완전 지원** - Reflection.Emit이나 런타임 코드 생성을 사용하지 않아 iOS, Android IL2CPP 빌드에서도 안전하게 동작합니다.

### 동작 흐름

```
[모드 제작자]                              [모드 사용자]
C# 스크립트 + 프리팹 작성                    MOD 검색/다운로드
        │                                       │
  [SFCSharp] 어트리뷰트 추가                 SFModManager로 로드
        │                                       │
  빌드 → SFCSharp 스크립트 자동 변환          스크립트 파싱 및 실행
        │                                       │
  AssetBundle 패킹                          런타임에 MOD 동작
        │                                       │
  저장소에 업로드  ──────────────────────>  저장소에서 다운로드
```

## 지원 환경

| 환경 | 지원 여부 | 비고 |
|------|-----------|------|
| Unity Mono (Desktop/Editor) | O | |
| Unity IL2CPP (Android/iOS) | O | AOT 안전 설계 |
| .NET Standard 2.1 | O | 타겟 프레임워크 |

### IL2CPP / AOT 호환성

- `System.Reflection.Emit` 및 런타임 코드 생성을 사용하지 않습니다.
- `RegexOptions.Compiled`를 사용하지 않습니다.
- 동적 어셈블리 로딩을 사용하지 않습니다.
- 외부 라이브러리 의존 없이 `System.Net.Http`만 사용합니다.

## 지원 기능

### 스크립트 실행 엔진

텍스트 기반 명령어를 파싱하고 실행하는 엔진을 제공합니다.

**지원하는 인자 타입:**

| 타입 | 예시 |
|------|------|
| 문자열 | `'hello'`, `"hello"` |
| 정수 | `123` |
| 실수 | `45.67` |
| Boolean | `true`, `false` |
| Vector3 | `Vector3(1, 2, 3)` |
| null | `null` |
| 변수 참조 | `$myVariable` |

### UnityEngine 네임스페이스

#### GameObject

| 메서드 | 설명 |
|--------|------|
| `UnityEngine.GameObject.Create('name')` | GameObject 생성 |
| `UnityEngine.GameObject.GetName($obj)` | 이름 조회 |
| `UnityEngine.GameObject.SetName($obj, 'name')` | 이름 설정 |
| `UnityEngine.GameObject.SetActive($obj, true)` | 활성화/비활성화 |
| `UnityEngine.GameObject.GetActive($obj)` | 활성 상태 조회 |
| `UnityEngine.GameObject.GetTransform($obj)` | Transform 조회 |
| `UnityEngine.GameObject.AddComponent($obj, 'Text')` | 컴포넌트 추가 |
| `UnityEngine.GameObject.GetComponent($obj, 'Text')` | 컴포넌트 조회 (인터페이스/상속 지원) |
| `UnityEngine.GameObject.GetComponents($obj, 'IMovable')` | 타입/인터페이스로 다중 컴포넌트 조회 |
| `UnityEngine.GameObject.HasComponent($obj, 'Text')` | 컴포넌트 존재 여부 확인 |
| `UnityEngine.GameObject.RemoveComponent($obj, 'Text')` | 컴포넌트 제거 |

#### Transform

| 메서드 | 설명 |
|--------|------|
| `UnityEngine.Transform.Translate($t, 1, 2, 3)` | 이동 |
| `UnityEngine.Transform.Rotate($t, 0, 90, 0)` | 회전 |
| `UnityEngine.Transform.LookAt($t, $target)` | 대상을 바라봄 |
| `UnityEngine.Transform.GetPosition($t)` | 위치 조회 |
| `UnityEngine.Transform.SetPosition($t, Vector3(1, 2, 3))` | 위치 설정 |

#### Vector3

| 메서드 | 설명 |
|--------|------|
| `UnityEngine.Vector3.Create(x, y, z)` | Vector3 생성 |
| `UnityEngine.Vector3.Zero()` | (0, 0, 0) |
| `UnityEngine.Vector3.One()` | (1, 1, 1) |
| `UnityEngine.Vector3.Distance($a, $b)` | 거리 계산 |
| `UnityEngine.Vector3.Magnitude($v)` | 크기 계산 |
| `UnityEngine.Vector3.Normalized($v)` | 정규화 |
| `UnityEngine.Vector3.Dot($a, $b)` | 내적 |
| `UnityEngine.Vector3.Cross($a, $b)` | 외적 |

#### Quaternion

| 메서드 | 설명 |
|--------|------|
| `UnityEngine.Quaternion.Euler(x, y, z)` | 오일러 각도로 회전 생성 |
| `UnityEngine.Quaternion.identity()` | 기본 회전 |

#### Color

| 메서드 | 설명 |
|--------|------|
| `UnityEngine.Color.Create(r, g, b, a)` | 색상 생성 |
| `UnityEngine.Color.red()` | 프리셋 색상 |
| `UnityEngine.Color.green()` | 프리셋 색상 |
| `UnityEngine.Color.blue()` | 프리셋 색상 |
| `UnityEngine.Color.white()` | 프리셋 색상 |
| `UnityEngine.Color.black()` | 프리셋 색상 |
| `UnityEngine.Color.yellow()` | 프리셋 색상 |
| `UnityEngine.Color.cyan()` | 프리셋 색상 |
| `UnityEngine.Color.magenta()` | 프리셋 색상 |
| `UnityEngine.Color.gray()` | 프리셋 색상 |
| `UnityEngine.Color.clear()` | 프리셋 색상 |

#### Debug

| 메서드 | 설명 |
|--------|------|
| `UnityEngine.Debug.Log('message')` | 로그 출력 |
| `UnityEngine.Debug.LogWarning('message')` | 경고 출력 |
| `UnityEngine.Debug.LogError('message')` | 에러 출력 |

#### Mathf

| 메서드 | 설명 |
|--------|------|
| `UnityEngine.Mathf.Abs(x)` | 절대값 |
| `UnityEngine.Mathf.Clamp(value, min, max)` | 범위 제한 |
| `UnityEngine.Mathf.Lerp(a, b, t)` | 선형 보간 |
| `UnityEngine.Mathf.Sin(x)` / `Cos(x)` / `Tan(x)` | 삼각함수 |
| `UnityEngine.Mathf.Sqrt(x)` | 제곱근 |
| `UnityEngine.Mathf.Floor(x)` / `Ceil(x)` / `Round(x)` | 반올림 |
| `UnityEngine.Mathf.Min(a, b)` / `Max(a, b)` | 최솟값/최댓값 |
| `UnityEngine.Mathf.Pow(base, exp)` | 거듭제곱 |
| `UnityEngine.Mathf.Atan2(y, x)` | 아크탄젠트 |
| `UnityEngine.Mathf.PI()` | 원주율 |
| `UnityEngine.Mathf.Deg2Rad()` / `Rad2Deg()` | 각도 변환 상수 |
| `UnityEngine.Mathf.Infinity()` | 무한대 |

#### Time

| 메서드 | 설명 |
|--------|------|
| `UnityEngine.Time.deltaTime()` | 프레임 간 시간 |
| `UnityEngine.Time.time()` | 시작 후 경과 시간 |
| `UnityEngine.Time.timeScale()` | 시간 스케일 |
| `UnityEngine.Time.fixedDeltaTime()` | 고정 프레임 간 시간 |
| `UnityEngine.Time.frameCount()` | 프레임 카운트 |

#### UI

| 메서드 | 설명 |
|--------|------|
| `UnityEngine.UI.Text.SetText($t, 'text')` | 텍스트 설정 |
| `UnityEngine.UI.Text.GetText($t)` | 텍스트 조회 |
| `UnityEngine.UI.Text.SetFontSize($t, 24)` | 폰트 크기 설정 |
| `UnityEngine.UI.Text.GetFontSize($t)` | 폰트 크기 조회 |
| `UnityEngine.UI.Text.SetColor($t, $color)` | 텍스트 색상 설정 |
| `UnityEngine.UI.Text.GetColor($t)` | 텍스트 색상 조회 |
| `UnityEngine.UI.Text.SetAlignment($t, 'center')` | 정렬 설정 |
| `UnityEngine.UI.Text.SetFontStyle($t, 'bold')` | 폰트 스타일 설정 |
| `UnityEngine.UI.Image.SetColor($img, $color)` | 이미지 색상 설정 |
| `UnityEngine.UI.Image.GetColor($img)` | 이미지 색상 조회 |
| `UnityEngine.UI.Image.SetFillAmount($img, 0.5)` | Fill Amount 설정 |
| `UnityEngine.UI.Image.GetFillAmount($img)` | Fill Amount 조회 |
| `UnityEngine.UI.Image.SetEnabled($img, true)` | 활성화/비활성화 |

### SFCSharp 네임스페이스 (타입 시스템)

#### Interface - 인터페이스 정의

| 메서드 | 설명 |
|--------|------|
| `SFCSharp.Interface.Define('IMovable')` | 인터페이스 정의 |
| `SFCSharp.Interface.Define('IFlyable', 'IMovable')` | 부모 인터페이스를 상속하는 인터페이스 정의 |
| `SFCSharp.Interface.IsDefined('IMovable')` | 인터페이스 존재 여부 확인 |

#### Type - 타입 정의 및 조회

| 메서드 | 설명 |
|--------|------|
| `SFCSharp.Type.Define('Enemy', 'Component', 'IMovable')` | 커스텀 타입 정의 (부모 타입 + 인터페이스) |
| `SFCSharp.Type.DefineProperty('Enemy', 'health', 100)` | 타입에 기본 프로퍼티 추가 |
| `SFCSharp.Type.Implement('Text', 'IUIElement')` | 기존 타입에 인터페이스 추가 |
| `SFCSharp.Type.Is($comp, 'IMovable')` | 타입/인터페이스 할당 가능 여부 확인 |
| `SFCSharp.Type.IsSubclassOf('Enemy', 'Component')` | 상속 관계 확인 |
| `SFCSharp.Type.ImplementsInterface('Enemy', 'IMovable')` | 인터페이스 구현 여부 확인 |
| `SFCSharp.Type.GetTypeName($comp)` | 컴포넌트 타입 이름 조회 |
| `SFCSharp.Type.SetProperty($comp, 'health', 80)` | 커스텀 컴포넌트 프로퍼티 설정 |
| `SFCSharp.Type.GetProperty($comp, 'health')` | 커스텀 컴포넌트 프로퍼티 조회 |

### System 네임스페이스

| 메서드 | 설명 |
|--------|------|
| `System.Console.WriteLine('message')` | 콘솔 출력 (줄바꿈 포함) |
| `System.Console.Write('message')` | 콘솔 출력 |

## 사용 방법

### 1. MOD 로드 및 실행

```csharp
// SFModManager 생성
var manager = new SFModManager();

// 단일 스크립트로 MOD 로드
var mod = manager.LoadMod("MyScript", @"
    UnityEngine.GameObject.Create('Player')
    UnityEngine.Debug.Log('Player created!')
");

// 모든 스크립트 실행
var results = mod.ExecuteAll();

// 단일 명령어 직접 실행
var result = mod.Execute("UnityEngine.GameObject.Create('Enemy')");
if (result.Success)
{
    // result.Result 에서 반환값 사용
}
```

### 2. MOD 메타데이터와 함께 로드

```csharp
var info = new SFModInfo
{
    ModId = "my-mod-001",
    ModName = "My First Mod",
    Version = "1.0.0",
    Author = "작성자",
    Description = "설명"
};

var scripts = new Dictionary<string, string>
{
    { "init", "UnityEngine.GameObject.Create('Player')" },
    { "setup", "UnityEngine.Debug.Log('Setup complete')" }
};

var mod = manager.LoadMod(info, scripts);

// 특정 스크립트만 실행
var results = mod.ExecuteScript("init");

// 모든 스크립트 실행
mod.ExecuteAll();
```

### 3. 변수 사용

```csharp
// 변수를 설정하고 명령어에서 $변수명으로 참조
mod.Execute("UnityEngine.GameObject.Create('Player')");
mod.SetVariable("player", someGameObject);
mod.Execute("UnityEngine.GameObject.SetName($player, 'Hero')");
mod.Execute("UnityEngine.Transform.Translate($player, 1, 0, 0)");

// 변수 조회
var obj = mod.GetVariable("player");
```

### 4. MOD 관리

```csharp
// 로드된 MOD 확인
bool loaded = manager.IsModLoaded("my-mod-001");

// 로드된 모든 MOD 조회
var mods = manager.GetLoadedMods();

// MOD 수 확인
int count = manager.LoadedModCount;

// 특정 MOD 언로드
manager.UnloadMod("my-mod-001");

// 모든 MOD 언로드
manager.UnloadAll();
```

### 5. MOD 업로드 / 다운로드

```csharp
var repo = new SFModRepository("https://mods.example.com/api");

// MOD 업로드
repo.Upload(info, bundleData, (success, error) =>
{
    if (success) { /* 업로드 성공 */ }
    else { /* error 메시지 확인 */ }
});

// MOD 다운로드
repo.Download("mod-id", (data, error) =>
{
    if (data != null) { /* data 바이트 배열로 번들 사용 */ }
});

// MOD 목록 조회
repo.GetModList((mods, error) =>
{
    foreach (var mod in mods) { /* mod.ModName, mod.Author 등 */ }
});

// MOD 검색
repo.Search("keyword", (results, error) =>
{
    foreach (var mod in results) { /* 검색 결과 처리 */ }
});
```

`ISFModRepository` 인터페이스를 구현하여 `UnityWebRequest` 기반의 커스텀 저장소를 만들 수도 있습니다.

### 6. 인터페이스 및 상속

```csharp
// 인터페이스 정의
mod.Execute("SFCSharp.Interface.Define('IMovable')");
mod.Execute("SFCSharp.Interface.Define('IDamageable')");
mod.Execute("SFCSharp.Interface.Define('IFlyable', 'IMovable')");  // 인터페이스 상속

// 커스텀 타입 정의 (상속 + 인터페이스 구현)
mod.Execute("SFCSharp.Type.Define('Enemy', 'Component', 'IMovable', 'IDamageable')");
mod.Execute("SFCSharp.Type.DefineProperty('Enemy', 'health', 100)");
mod.Execute("SFCSharp.Type.DefineProperty('Enemy', 'speed', 5)");

// 커스텀 컴포넌트 추가
mod.Execute("UnityEngine.GameObject.AddComponent($go, 'Enemy')");

// 프로퍼티 접근
mod.Execute("SFCSharp.Type.SetProperty($enemy, 'health', 80)");
mod.Execute("SFCSharp.Type.GetProperty($enemy, 'health')");

// 타입 체크
mod.Execute("SFCSharp.Type.Is($enemy, 'IMovable')");           // true
mod.Execute("SFCSharp.Type.Is($enemy, 'IDamageable')");        // true
mod.Execute("SFCSharp.Type.IsSubclassOf('Enemy', 'Component')"); // true

// 인터페이스 기반 컴포넌트 조회
mod.Execute("UnityEngine.GameObject.GetComponent($go, 'IMovable')");
mod.Execute("UnityEngine.GameObject.GetComponents($go, 'IDamageable')");

// 기존 빌트인 타입에도 인터페이스 적용 가능
mod.Execute("SFCSharp.Interface.Define('IUIElement')");
mod.Execute("SFCSharp.Type.Implement('Text', 'IUIElement')");
mod.Execute("SFCSharp.Type.Implement('Image', 'IUIElement')");
mod.Execute("UnityEngine.GameObject.GetComponents($go, 'IUIElement')"); // Text + Image 모두 반환
```

### 7. MOD 빌드 (C# → SFCSharp 스크립트 변환)

`[SFCSharp]` 어트리뷰트를 MonoBehaviour에 추가하면, 빌드 시 자동으로 SFCSharp 스크립트로 변환됩니다.

```csharp
[SFCSharp]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private string playerName = "Hero";

    public void Start()
    {
        Debug.Log("Player started");
        var go = new GameObject("Weapon");
    }
}
```

빌드 설정:

```csharp
var config = new SFCSharpBuildConfig
{
    SourceScriptDirectory = "Assets/Scripts",
    SourcePrefabDirectory = "Assets/Prefabs",
    OutputDirectory = "Build/SFCSharp",
    BundleName = "MyMod",
    BundleVersion = "1.0.0"
};

var processor = new SFCSharpBuildProcessor(config);
var result = processor.Build();

if (result.Status == BuildStatus.Success)
{
    // result.ProcessedScripts - 변환된 스크립트 목록
    // result.ProcessedPrefabs - 수정된 프리팹 목록
}
```

**빌드 과정:**

1. `[SFCSharp]` 어트리뷰트가 있는 C# 스크립트를 탐지
2. C# 코드를 SFCSharp 스크립트 문법으로 변환
3. `[SerializeField]` 필드를 메타데이터(`// @field:타입 이름 = 값`)로 보존
4. 해당 스크립트를 사용하는 Prefab에서 MonoBehaviour를 SFCSharpExecutor로 교체
5. AssetBundle로 패킹

**자동 변환 예시:**

| C# 코드 | SFCSharp 스크립트 |
|----------|-------------------|
| `new GameObject("name")` | `UnityEngine.GameObject.Create('name')` |
| `obj.GetComponent<Text>()` | `UnityEngine.GameObject.GetComponent($obj, 'Text')` |
| `obj.AddComponent<Image>()` | `UnityEngine.GameObject.AddComponent($obj, 'Image')` |
| `text.text = "value"` | `UnityEngine.UI.Text.SetText($text, 'value')` |
| `text.fontSize = 24` | `UnityEngine.UI.Text.SetFontSize($text, 24)` |
| `Debug.Log("msg")` | `UnityEngine.Debug.Log('msg')` |

## 프로젝트 구조

```
SFCSharp/
├── Attributes/          # [SFCSharp] 어트리뷰트
├── Context/             # MOD별 변수 저장소 (SFContext)
├── Execution/           # 명령어 실행 핸들러
│   ├── Base/            # 핸들러 추상 클래스
│   ├── SFCSharpExec/    # SFCSharp 네임스페이스 (Type, Interface)
│   ├── SystemExec/      # System 네임스페이스 (Console)
│   └── UnityExec/       # UnityEngine 네임스페이스
│       └── UnityExecUI/ # UI 컴포넌트 (Text, Image)
├── TypeSystem/          # 인터페이스/상속 타입 시스템
├── Runtime/             # 명령어 파서, 스크립트 실행기
│   └── ModLoader/       # MOD 로드/관리
├── Build/               # 빌드 프로세서
└── Network/             # MOD 저장소 (업로드/다운로드)
```

## 작업 목록

- [ ] DoTween 지원 - 트윈 애니메이션 (Move, Rotate, Scale, Fade, Sequence 등)
- [ ] Camera 지원 - 카메라 제어 (위치, 회전, FOV, 뷰포트, 렌더링 설정 등)

## 라이선스

이 프로젝트의 라이선스는 저장소 라이선스 파일을 참고하세요.
