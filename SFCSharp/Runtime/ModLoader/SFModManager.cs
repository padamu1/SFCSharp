using System;
using System.Collections.Generic;

namespace SFCSharp.Runtime.ModLoader
{
    /// <summary>
    /// MOD 라이프사이클 관리자
    /// 로드된 MOD들의 등록, 조회, 언로드를 관리합니다.
    /// </summary>
    public class SFModManager
    {
        private readonly SFModLoader _loader;
        private readonly Dictionary<string, SFLoadedMod> _loadedMods;

        public SFModManager()
        {
            _loader = new SFModLoader();
            _loadedMods = new Dictionary<string, SFLoadedMod>();
        }

        /// <summary>
        /// 단일 스크립트로 MOD를 로드하고 등록합니다.
        /// </summary>
        public SFLoadedMod LoadMod(string scriptName, string scriptText)
        {
            var mod = _loader.LoadFromText(scriptName, scriptText);
            RegisterMod(mod);
            return mod;
        }

        /// <summary>
        /// MOD 정보와 스크립트들로 MOD를 로드하고 등록합니다.
        /// </summary>
        public SFLoadedMod LoadMod(SFModInfo info, Dictionary<string, string> scripts)
        {
            if (_loadedMods.ContainsKey(info.ModId))
                throw new InvalidOperationException($"Mod '{info.ModId}' is already loaded. Unload it first.");

            var mod = _loader.Load(info, scripts);
            RegisterMod(mod);
            return mod;
        }

        /// <summary>
        /// MOD를 언로드합니다.
        /// </summary>
        public void UnloadMod(string modId)
        {
            if (!_loadedMods.ContainsKey(modId))
                throw new ArgumentException($"Mod not found: {modId}");

            _loadedMods[modId].Unload();
            _loadedMods.Remove(modId);
        }

        /// <summary>
        /// 모든 MOD를 언로드합니다.
        /// </summary>
        public void UnloadAll()
        {
            foreach (var mod in _loadedMods.Values)
            {
                mod.Unload();
            }
            _loadedMods.Clear();
        }

        /// <summary>
        /// 특정 MOD를 조회합니다.
        /// </summary>
        public SFLoadedMod GetMod(string modId)
        {
            if (_loadedMods.ContainsKey(modId))
                return _loadedMods[modId];

            return null;
        }

        /// <summary>
        /// MOD가 로드되어 있는지 확인합니다.
        /// </summary>
        public bool IsModLoaded(string modId)
        {
            return _loadedMods.ContainsKey(modId);
        }

        /// <summary>
        /// 로드된 모든 MOD 목록을 반환합니다.
        /// </summary>
        public IReadOnlyList<SFLoadedMod> GetLoadedMods()
        {
            return new List<SFLoadedMod>(_loadedMods.Values);
        }

        /// <summary>
        /// 로드된 MOD 수를 반환합니다.
        /// </summary>
        public int LoadedModCount => _loadedMods.Count;

        private void RegisterMod(SFLoadedMod mod)
        {
            _loadedMods[mod.Info.ModId] = mod;
        }
    }
}
