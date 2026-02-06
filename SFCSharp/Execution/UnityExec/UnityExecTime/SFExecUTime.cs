using SFCSharp.Execution.Base;
using System;
using System.Collections.Generic;

namespace SFCSharp.Execution.UnityExec.UnityExecTime
{
    /// <summary>
    /// UnityEngine.Time 메서드 핸들러
    /// Unity 환경에서는 ValueProvider를 통해 실제 Time 값을 주입할 수 있습니다.
    /// </summary>
    public class SFExecUTime : SFMethodHandlerBase
    {
        /// <summary>
        /// 외부에서 Time 값을 주입하기 위한 프로바이더
        /// Unity 환경에서 실제 Time.deltaTime 등을 공급할 수 있습니다.
        /// </summary>
        public static ISFTimeProvider TimeProvider { get; set; } = new DefaultTimeProvider();

        protected override void InitMethodHandler(ref Dictionary<string, IMethodHandler> _methodHandlerDic)
        {
            _methodHandlerDic = new Dictionary<string, IMethodHandler>()
            {
                {"deltaTime", new DeltaTimeHandler() },
                {"time", new TimeHandler() },
                {"timeScale", new TimeScaleHandler() },
                {"fixedDeltaTime", new FixedDeltaTimeHandler() },
                {"frameCount", new FrameCountHandler() },
            };
        }

        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = null;
        }

        private class DeltaTimeHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                execCallback?.Invoke(TimeProvider.DeltaTime);
            }
        }

        private class TimeHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                execCallback?.Invoke(TimeProvider.Time);
            }
        }

        private class TimeScaleHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                if (args.Length > 0)
                {
                    TimeProvider.TimeScale = Convert.ToSingle(args[0]);
                }
                execCallback?.Invoke(TimeProvider.TimeScale);
            }
        }

        private class FixedDeltaTimeHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                execCallback?.Invoke(TimeProvider.FixedDeltaTime);
            }
        }

        private class FrameCountHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                execCallback?.Invoke(TimeProvider.FrameCount);
            }
        }
    }

    /// <summary>
    /// Time 값 공급 인터페이스
    /// Unity에서는 이를 구현하여 실제 Time 값을 공급합니다.
    /// </summary>
    public interface ISFTimeProvider
    {
        float DeltaTime { get; }
        float Time { get; }
        float TimeScale { get; set; }
        float FixedDeltaTime { get; }
        int FrameCount { get; }
    }

    /// <summary>
    /// 기본 Time 프로바이더 (시스템 시간 기반)
    /// </summary>
    public class DefaultTimeProvider : ISFTimeProvider
    {
        private readonly DateTime _startTime = DateTime.Now;
        private float _timeScale = 1.0f;
        private int _frameCount;

        public float DeltaTime => 0.016f; // 60fps 기준
        public float Time => (float)(DateTime.Now - _startTime).TotalSeconds;
        public float TimeScale
        {
            get => _timeScale;
            set => _timeScale = value;
        }
        public float FixedDeltaTime => 0.02f;
        public int FrameCount => _frameCount++;
    }
}
