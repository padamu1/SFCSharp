using System;

namespace SFCSharp.Execution
{
    /// <summary>
    /// SFCSharp 명령어 실행 관리자 (싱글톤)
    /// 메서드 호출을 라우팅하고 실행합니다.
    /// </summary>
    public class SFExecManager
    {
        private static SFExecManager _instance;
        private static readonly object _lock = new object();
        private readonly INamespaceHandler _rootHandler;

        public static SFExecManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SFExecManager();
                        }
                    }
                }
                return _instance;
            }
        }

        private SFExecManager()
        {
            _rootHandler = new SFExecHandler();
        }

        /// <summary>
        /// 메서드를 실행합니다 (콜백 기반)
        /// </summary>
        /// <param name="methodPath">메서드 경로 (예: "UnityEngine.GameObject.Create")</param>
        /// <param name="execCallback">실행 결과 콜백</param>
        /// <param name="args">메서드 인자</param>
        public static void ExecWrapper(string methodPath, Action<object> execCallback, params object[] args)
        {
            if (string.IsNullOrEmpty(methodPath))
                throw new ArgumentException("Method path cannot be null or empty");

            try
            {
                string[] methodNames = methodPath.Split('.');
                Instance._rootHandler.Exec(methodNames, execCallback, 0, args);
            }
            catch (Exception ex)
            {
                execCallback?.Invoke(ex);
            }
        }

        /// <summary>
        /// 메서드를 실행합니다 (동기 방식)
        /// </summary>
        public static object Execute(string methodPath, params object[] args)
        {
            if (string.IsNullOrEmpty(methodPath))
                throw new ArgumentException("Method path cannot be null or empty");

            object result = null;

            ExecWrapper(methodPath, (obj) =>
            {
                result = obj;
            }, args);

            return result;
        }
    }
}
