using SFCSharp.Context;
using System;
using System.Collections.Generic;

namespace SFCSharp.Runtime.ModLoader
{
    /// <summary>
    /// 로드된 MOD 인스턴스
    /// 스크립트 텍스트를 파싱하고 실행할 수 있는 상태를 관리합니다.
    /// </summary>
    public class SFLoadedMod
    {
        private readonly SFModInfo _info;
        private readonly SFContext _context;
        private readonly ScriptExecutor _executor;
        private readonly Dictionary<string, List<string>> _scriptCommands;
        private bool _isLoaded;

        public SFModInfo Info => _info;
        public bool IsLoaded => _isLoaded;

        internal SFLoadedMod(SFModInfo info, Dictionary<string, string> scripts)
        {
            _info = info ?? throw new ArgumentNullException(nameof(info));
            _context = new SFContext(info.ModId, info.ModName);
            _executor = new ScriptExecutor(_context);
            _scriptCommands = new Dictionary<string, List<string>>();

            ParseScripts(scripts);
            InjectSerializedFields(scripts);
            _isLoaded = true;
        }

        /// <summary>
        /// 특정 스크립트의 모든 명령어를 실행합니다.
        /// </summary>
        public List<ExecutionResult> ExecuteScript(string scriptName)
        {
            if (!_isLoaded)
                throw new InvalidOperationException("Mod is not loaded");

            if (!_scriptCommands.ContainsKey(scriptName))
                throw new ArgumentException($"Script not found: {scriptName}");

            var results = new List<ExecutionResult>();
            foreach (string command in _scriptCommands[scriptName])
            {
                results.Add(_executor.Execute(command));
            }
            return results;
        }

        /// <summary>
        /// 모든 스크립트의 모든 명령어를 순차 실행합니다.
        /// </summary>
        public List<ExecutionResult> ExecuteAll()
        {
            if (!_isLoaded)
                throw new InvalidOperationException("Mod is not loaded");

            var results = new List<ExecutionResult>();
            foreach (var commands in _scriptCommands.Values)
            {
                foreach (string command in commands)
                {
                    results.Add(_executor.Execute(command));
                }
            }
            return results;
        }

        /// <summary>
        /// 단일 명령어를 직접 실행합니다.
        /// </summary>
        public ExecutionResult Execute(string command)
        {
            if (!_isLoaded)
                throw new InvalidOperationException("Mod is not loaded");

            return _executor.Execute(command);
        }

        /// <summary>
        /// MOD 컨텍스트에 변수를 설정합니다.
        /// </summary>
        public void SetVariable(string name, object value)
        {
            _executor.SetVariable(name, value);
        }

        /// <summary>
        /// MOD 컨텍스트에서 변수를 조회합니다.
        /// </summary>
        public object GetVariable(string name)
        {
            return _executor.GetVariable(name);
        }

        /// <summary>
        /// 로드된 스크립트 이름 목록을 반환합니다.
        /// </summary>
        public IReadOnlyList<string> GetScriptNames()
        {
            return new List<string>(_scriptCommands.Keys);
        }

        /// <summary>
        /// MOD를 언로드합니다.
        /// </summary>
        internal void Unload()
        {
            _isLoaded = false;
            _scriptCommands.Clear();
            _context.ClearVariables();
        }

        /// <summary>
        /// 스크립트에 포함된 SerializeField 메타데이터를 파싱하여 변수로 주입합니다.
        /// 빌드 시 인스펙터에서 설정한 값이 // @field:타입 이름 = 값 형식으로 저장됩니다.
        /// </summary>
        private void InjectSerializedFields(Dictionary<string, string> scripts)
        {
            foreach (var entry in scripts)
            {
                var fields = SFSerializedFieldParser.Parse(entry.Value);
                var variables = SFSerializedFieldParser.ConvertAll(fields);

                foreach (var variable in variables)
                {
                    _context.SetVariable(variable.Key, variable.Value);
                }
            }
        }

        /// <summary>
        /// 스크립트 텍스트를 파싱하여 명령어 목록으로 변환합니다.
        /// </summary>
        private void ParseScripts(Dictionary<string, string> scripts)
        {
            foreach (var entry in scripts)
            {
                var commands = new List<string>();
                string[] lines = entry.Value.Split('\n');

                foreach (string line in lines)
                {
                    string trimmed = line.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmed) &&
                        !trimmed.StartsWith("//") &&
                        !trimmed.StartsWith("/*"))
                    {
                        commands.Add(trimmed);
                    }
                }

                _scriptCommands[entry.Key] = commands;
            }
        }
    }
}
