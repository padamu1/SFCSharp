# SFCSharp 작업 목록

Lua 기반 런타임 스크립팅을 대체하여 **편한 모딩**과 **런타임 다운로드/로드**를 달성하기 위한 작업 목록입니다.

## 현재 상태 요약

| 영역 | 상태 | 비고 |
|------|------|------|
| 명령어 파싱 및 실행 | O | 메서드 호출 기반 명령어 처리 가능 |
| 타입 시스템 (인터페이스/상속) | O | 커스텀 타입, 인터페이스, 폴리모픽 조회 |
| MOD 로드/언로드 | O | SFModManager로 관리 |
| MOD 업로드/다운로드 | O | HTTP 기반 저장소 |
| 빌드 시스템 | O | C# → SFCSharp 자동 변환, AssetBundle 패킹 |
| Unity 기본 타입 | O | GameObject, Transform, Vector3, Color, UI 등 |

## 부족한 부분 (우선순위별)

---

### P0 - 핵심 언어 기능 (Lua 대체를 위한 필수)

현재 SFCSharp는 명령어 디스패처로, 스크립팅 언어의 기본 요소가 없습니다.
이 항목들이 없으면 모드에서 게임 로직을 구현할 수 없습니다.

- [ ] **변수 선언/할당** - 스크립트 내에서 변수를 선언하고 값을 할당하는 문법
  - `var player = UnityEngine.GameObject.Create('Player')`
  - `var hp = 100`
  - 현재는 `$변수명` 참조만 가능하고, 스크립트 내 할당 불가

- [ ] **제어 흐름 (if/else)** - 조건 분기
  - `if ($hp > 0) { ... } else { ... }`
  - 현재 모든 명령어가 무조건 순차 실행됨

- [ ] **반복문 (for/while)** - 루프
  - `for (var i = 0; i < 10; i++) { ... }`
  - `while ($isAlive) { ... }`

- [ ] **연산자 및 표현식** - 산술, 비교, 논리 연산
  - 산술: `+`, `-`, `*`, `/`, `%`
  - 비교: `==`, `!=`, `<`, `>`, `<=`, `>=`
  - 논리: `&&`, `||`, `!`
  - 현재는 `UnityEngine.Mathf.Add()` 같은 메서드 호출로만 가능

- [ ] **함수 정의** - 스크립트 내 재사용 가능한 함수 작성
  - `function SpawnEnemy(name, x, y) { ... }`
  - 현재는 함수를 정의할 수 없어 코드 재사용 불가

> **구현 방향**: CommandParser를 Regex 기반에서 재귀 하향 파서(Recursive Descent Parser)로 교체하여 AST를 생성하고, ScriptExecutor가 AST를 순회하며 실행하도록 변경. IL2CPP 안전을 위해 인터프리터 방식 유지 (VM이나 코드 생성 사용 금지).

---

### P1 - Unity 라이프사이클 연동 (실시간 모드 동작)

모드가 게임 루프에 참여하지 못하면 실시간 게임 로직을 구현할 수 없습니다.

- [ ] **라이프사이클 콜백** - Unity MonoBehaviour 이벤트 연동
  - `Start()`, `Update()`, `LateUpdate()`, `FixedUpdate()`
  - `OnEnable()`, `OnDisable()`, `OnDestroy()`
  - SFCSharpExecutor MonoBehaviour가 스크립트의 라이프사이클 함수를 호출하도록 구현

- [ ] **코루틴 지원** - 지연 실행 및 비동기 흐름
  - `yield WaitForSeconds(2.0)`
  - `yield WaitUntil($condition)`
  - Unity 코루틴을 스크립트에서 사용할 수 있도록 래핑

- [ ] **이벤트 시스템** - 이벤트 구독 및 발행
  - `Event.Subscribe('OnPlayerDamaged', $callback)`
  - `Event.Emit('OnPlayerDamaged', $damage)`
  - 모드 간 통신에도 활용 가능

---

### P2 - Unity 핵심 시스템 지원

게임 모드에서 가장 자주 사용하는 Unity 시스템들입니다.

- [ ] **Input 지원** - 플레이어 입력 처리
  - `UnityEngine.Input.GetKey('Space')`
  - `UnityEngine.Input.GetAxis('Horizontal')`
  - `UnityEngine.Input.GetMouseButtonDown(0)`

- [ ] **Physics 지원** - 물리 시스템
  - Rigidbody: AddForce, velocity, mass
  - Collider: OnCollisionEnter, OnTriggerEnter 콜백
  - Physics.Raycast

- [ ] **GameObject.Destroy** - 오브젝트 파괴
  - `UnityEngine.GameObject.Destroy($obj)`
  - `UnityEngine.GameObject.Destroy($obj, 2.0)` (딜레이)

- [ ] **Instantiate** - 프리팹 인스턴스화
  - `UnityEngine.Object.Instantiate($prefab, $position, $rotation)`

- [ ] **Audio 지원** - 사운드 제어
  - AudioSource: Play, Stop, Pause, volume, pitch
  - `UnityEngine.AudioSource.Play($source)`

- [ ] **Animator 지원** - 애니메이션 제어
  - `UnityEngine.Animator.SetTrigger($anim, 'Attack')`
  - `UnityEngine.Animator.SetBool($anim, 'IsRunning', true)`

- [ ] **Camera 지원** - 카메라 제어
  - 위치, 회전, FOV, 뷰포트, 렌더링 설정

- [ ] **DoTween 지원** - 트윈 애니메이션
  - Move, Rotate, Scale, Fade, Sequence 등

---

### P3 - 데이터 구조 및 스크립트 기능

복잡한 모드 로직을 위한 데이터 처리 기능입니다.

- [ ] **배열/리스트** - 컬렉션 지원
  - `var enemies = [enemy1, enemy2, enemy3]`
  - `enemies.Add($newEnemy)`
  - `enemies.Count()`

- [ ] **딕셔너리** - 키-값 저장소
  - `var stats = { 'hp': 100, 'mp': 50 }`
  - `stats.Get('hp')`

- [ ] **문자열 연산** - 문자열 처리
  - 연결: `'Hello' + ' ' + 'World'`
  - 포맷: `String.Format('HP: {0}', $hp)`
  - 기본 메서드: Length, Contains, Replace, Split

- [ ] **스크립트 간 호출** - 모듈 시스템
  - `import 'Utils'` 또는 `require('Utils')`
  - 함수/변수 공유

- [ ] **에러 처리** - 스크립트 내 예외 처리
  - `try { ... } catch ($err) { ... }`
  - 현재는 C# 레벨에서만 예외 처리됨

---

### P4 - MOD 배포 고도화

프로덕션 수준의 모드 배포 시스템을 위한 기능입니다.

- [ ] **MOD 의존성 관리** - 모드 간 의존 관계
  - SFModInfo에 Dependencies 필드 추가
  - 로드 순서 자동 결정
  - 누락 의존성 경고

- [ ] **버전 호환성 검사** - SemVer 기반 호환성
  - 게임 버전 ↔ MOD 버전 호환성 매트릭스
  - 자동 호환성 경고

- [ ] **핫 리로드** - 개발 중 즉시 반영
  - 모드 언로드 → 재로드 시 상태 보존 옵션
  - 개발 모드에서 파일 변경 감지 자동 리로드

- [ ] **MOD 검증/보안** - 안전한 모드 실행
  - 모드 서명 검증
  - 샌드박스 실행 (위험 API 제한)
  - 권한 시스템

- [ ] **리소스 로딩** - 모드 에셋 지원
  - 텍스처, 오디오, 프리팹 등 에셋 번들에서 로드
  - `Resources.Load` 래핑

---

### P5 - 개발 편의

모더의 개발 경험 개선을 위한 기능입니다.

- [ ] **디버깅 지원** - 스크립트 디버깅
  - 스크립트 줄 번호 포함 에러 메시지
  - 변수 상태 덤프
  - 실행 추적 로그

- [ ] **PlayerPrefs 지원** - 모드 데이터 영속화
  - `UnityEngine.PlayerPrefs.SetInt('HighScore', $score)`
  - `UnityEngine.PlayerPrefs.GetString('PlayerName')`

- [ ] **Scene 관리** - 씬 전환
  - `UnityEngine.SceneManager.LoadScene('Level2')`

---

## 권장 작업 순서

```
Phase 1: 언어 기능 (P0)
  변수 할당 → 연산자/표현식 → 제어 흐름 → 함수 정의
  (CommandParser를 재귀 하향 파서로 교체)

Phase 2: 게임 루프 (P1)
  라이프사이클 콜백 → 코루틴 → 이벤트 시스템

Phase 3: Unity 시스템 (P2)
  Input → Physics → Destroy/Instantiate → Audio → Animator → Camera → DoTween

Phase 4: 데이터 + 배포 (P3 + P4)
  배열/딕셔너리 → 에러 처리 → 의존성 관리 → 핫 리로드
```

Phase 1이 완료되면 Lua 대비 기본적인 스크립팅이 가능해지고,
Phase 2까지 완료되면 실시간 게임 모드 제작이 가능해집니다.
