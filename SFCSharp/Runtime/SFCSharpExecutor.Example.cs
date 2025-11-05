// ============================================================================
// SFCSharpExecutor - MonoBehaviour 컴포넌트 예시
// ============================================================================
//
// 이 파일은 예시 코드입니다.
// Unity 프로젝트에서 사용하려면 복사하여 .cs 파일로 저장하세요.
//
// ============================================================================
// 사용 방법:
// ============================================================================
// 1. 이 코드를 Unity 프로젝트에 복사 (Assets/Scripts/Runtime/ 등)
// 2. Prefab의 GameObject에 SFCSharpExecutor 컴포넌트 추가
// 3. Inspector에서 "Script Text Asset" 필드에 텍스트 스크립트 할당
// 4. 게임 실행 시 자동으로 스크립트 로드 및 실행
// ============================================================================

/*
using SFCSharp.Context;
using SFCSharp.Runtime;
using UnityEngine;

public class SFCSharpExecutor : MonoBehaviour
{
    /// <summary>
    /// 실행할 스크립트 텍스트 (TextAsset)
    /// 빌드 시 C# 스크립트가 변환되어 저장됨
    /// </summary>
    [SerializeField]
    private TextAsset scriptTextAsset;

    /// <summary>
    /// 스크립트 실행 엔진
    /// </summary>
    private ScriptExecutor _scriptExecutor;

    /// <summary>
    /// 스크립트 컨텍스트
    /// </summary>
    private SFContext _context;

    /// <summary>
    /// 스크립트 로드 및 실행 완료 여부
    /// </summary>
    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        if (scriptTextAsset == null)
        {
            Debug.LogError("[SFCSharpExecutor] Script TextAsset is not assigned!");
            return;
        }

        InitializeScript();
    }

    /// <summary>
    /// 스크립트를 초기화합니다.
    /// </summary>
    private void InitializeScript()
    {
        try
        {
            // 1. 스크립트 텍스트 가져오기
            string scriptText = scriptTextAsset.text;

            if (string.IsNullOrWhiteSpace(scriptText))
            {
                Debug.LogError("[SFCSharpExecutor] Script text is empty!");
                return;
            }

            // 2. 컨텍스트 생성
            _context = new SFContext(gameObject.name + ".Script", gameObject.name);

            // 3. ScriptExecutor 생성
            _scriptExecutor = new ScriptExecutor(_context);

            // 4. OnStart 메서드 호출 (있으면)
            ExecuteLifecycleMethod("OnStart");

            IsInitialized = true;
            Debug.Log($"[SFCSharpExecutor] Script initialized for GameObject: {gameObject.name}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SFCSharpExecutor] Failed to initialize script: {ex.Message}\n{ex.StackTrace}");
            IsInitialized = false;
        }
    }

    private void Start()
    {
        // OnStart 메서드는 Awake에서 호출됨
    }

    private void Update()
    {
        if (!IsInitialized || _scriptExecutor == null)
            return;

        // OnUpdate 메서드 호출 (있으면)
        ExecuteLifecycleMethod("OnUpdate");
    }

    private void OnDestroy()
    {
        if (!IsInitialized || _scriptExecutor == null)
            return;

        // OnDestroy 메서드 호출 (있으면)
        ExecuteLifecycleMethod("OnDestroy");
    }

    /// <summary>
    /// 명령어를 실행합니다.
    /// </summary>
    /// <param name="command">실행할 명령어</param>
    /// <returns>실행 결과</returns>
    public ExecutionResult Execute(string command)
    {
        if (!IsInitialized || _scriptExecutor == null)
        {
            return new ExecutionResult
            {
                Success = false,
                ErrorMessage = "ScriptExecutor is not initialized"
            };
        }

        try
        {
            return _scriptExecutor.Execute(command);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SFCSharpExecutor] Execution error: {ex.Message}");
            return new ExecutionResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                Error = ex
            };
        }
    }

    /// <summary>
    /// 변수를 설정합니다.
    /// </summary>
    public void SetVariable(string name, object value)
    {
        if (!IsInitialized || _scriptExecutor == null)
        {
            Debug.LogWarning("[SFCSharpExecutor] ScriptExecutor is not initialized");
            return;
        }

        _scriptExecutor.SetVariable(name, value);
    }

    /// <summary>
    /// 변수를 조회합니다.
    /// </summary>
    public object GetVariable(string name)
    {
        if (!IsInitialized || _scriptExecutor == null)
        {
            Debug.LogWarning("[SFCSharpExecutor] ScriptExecutor is not initialized");
            return null;
        }

        return _scriptExecutor.GetVariable(name);
    }

    /// <summary>
    /// 생명주기 메서드를 실행합니다.
    /// OnStart, OnUpdate, OnDestroy 등
    /// </summary>
    private void ExecuteLifecycleMethod(string methodName)
    {
        if (_scriptExecutor == null)
            return;

        try
        {
            // 생명주기 메서드는 인자 없이 호출됨
            // 나중에 필요하면 구현 가능
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SFCSharpExecutor] Lifecycle method error: {ex.Message}");
        }
    }

    /// <summary>
    /// 스크립트 컨텍스트를 반환합니다.
    /// </summary>
    public SFContext GetContext()
    {
        return _context;
    }

    /// <summary>
    /// ScriptExecutor를 반환합니다.
    /// </summary>
    public ScriptExecutor GetScriptExecutor()
    {
        return _scriptExecutor;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Editor에서 스크립트 정보를 표시합니다.
    /// </summary>
    public void OnDrawGizmos()
    {
        if (!IsInitialized)
            return;

        // Editor에서 스크립트 상태 표시
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.2f);
    }
#endif
}
*/
