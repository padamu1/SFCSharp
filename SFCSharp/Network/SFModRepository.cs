using SFCSharp.Runtime.ModLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace SFCSharp.Network
{
    /// <summary>
    /// HTTP 기반 MOD 저장소 기본 구현
    /// HttpClient를 사용하며, IL2CPP 환경에서도 동작합니다.
    /// Unity 환경에서는 ISFModRepository를 UnityWebRequest로 구현하는 것을 권장합니다.
    /// </summary>
    public class SFModRepository : ISFModRepository
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        /// <param name="baseUrl">MOD 서버 기본 URL (예: "https://mods.example.com/api")</param>
        public SFModRepository(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("Base URL cannot be null or empty");

            _baseUrl = baseUrl.TrimEnd('/');
            _httpClient = new HttpClient();
        }

        public void Upload(SFModInfo info, byte[] bundleData, Action<bool, string> callback)
        {
            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(info.ModId), "modId");
                    content.Add(new StringContent(info.ModName), "modName");
                    content.Add(new StringContent(info.Version), "version");
                    content.Add(new StringContent(info.Author ?? ""), "author");
                    content.Add(new StringContent(info.Description ?? ""), "description");
                    content.Add(new ByteArrayContent(bundleData), "bundle", $"{info.ModId}.sfmod");

                    var response = _httpClient.PostAsync($"{_baseUrl}/mods/upload", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        callback?.Invoke(true, null);
                    }
                    else
                    {
                        callback?.Invoke(false, $"Upload failed: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                callback?.Invoke(false, ex.Message);
            }
        }

        public void Download(string modId, Action<byte[], string> callback)
        {
            try
            {
                var response = _httpClient.GetAsync($"{_baseUrl}/mods/{modId}/download").Result;

                if (response.IsSuccessStatusCode)
                {
                    byte[] data = response.Content.ReadAsByteArrayAsync().Result;
                    callback?.Invoke(data, null);
                }
                else
                {
                    callback?.Invoke(null, $"Download failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                callback?.Invoke(null, ex.Message);
            }
        }

        public void GetModList(Action<List<SFModInfo>, string> callback)
        {
            try
            {
                var response = _httpClient.GetAsync($"{_baseUrl}/mods").Result;

                if (response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().Result;
                    var mods = ParseModListJson(json);
                    callback?.Invoke(mods, null);
                }
                else
                {
                    callback?.Invoke(null, $"GetModList failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                callback?.Invoke(null, ex.Message);
            }
        }

        public void Search(string query, Action<List<SFModInfo>, string> callback)
        {
            try
            {
                var response = _httpClient.GetAsync($"{_baseUrl}/mods/search?q={Uri.EscapeDataString(query)}").Result;

                if (response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().Result;
                    var mods = ParseModListJson(json);
                    callback?.Invoke(mods, null);
                }
                else
                {
                    callback?.Invoke(null, $"Search failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                callback?.Invoke(null, ex.Message);
            }
        }

        public void Delete(string modId, Action<bool, string> callback)
        {
            try
            {
                var response = _httpClient.DeleteAsync($"{_baseUrl}/mods/{modId}").Result;

                if (response.IsSuccessStatusCode)
                {
                    callback?.Invoke(true, null);
                }
                else
                {
                    callback?.Invoke(false, $"Delete failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                callback?.Invoke(false, ex.Message);
            }
        }

        /// <summary>
        /// 간단한 JSON 배열 파싱 (외부 라이브러리 의존 없이)
        /// 서버 응답 형식:
        /// [{"modId":"...","modName":"...","version":"...","author":"...","description":"..."}]
        /// </summary>
        private List<SFModInfo> ParseModListJson(string json)
        {
            var mods = new List<SFModInfo>();

            if (string.IsNullOrWhiteSpace(json))
                return mods;

            // 각 객체 블록을 분리
            int index = 0;
            while (index < json.Length)
            {
                int objStart = json.IndexOf('{', index);
                if (objStart < 0) break;

                int objEnd = json.IndexOf('}', objStart);
                if (objEnd < 0) break;

                string objStr = json.Substring(objStart + 1, objEnd - objStart - 1);
                var mod = ParseModInfoFromJson(objStr);
                if (mod != null)
                {
                    mods.Add(mod);
                }

                index = objEnd + 1;
            }

            return mods;
        }

        private SFModInfo ParseModInfoFromJson(string objStr)
        {
            var mod = new SFModInfo();

            mod.ModId = ExtractJsonStringValue(objStr, "modId");
            mod.ModName = ExtractJsonStringValue(objStr, "modName");
            mod.Version = ExtractJsonStringValue(objStr, "version");
            mod.Author = ExtractJsonStringValue(objStr, "author");
            mod.Description = ExtractJsonStringValue(objStr, "description");

            if (string.IsNullOrEmpty(mod.ModId))
                return null;

            return mod;
        }

        private string ExtractJsonStringValue(string json, string key)
        {
            string pattern = $"\"{key}\"";
            int keyIndex = json.IndexOf(pattern);
            if (keyIndex < 0) return null;

            int colonIndex = json.IndexOf(':', keyIndex + pattern.Length);
            if (colonIndex < 0) return null;

            int quoteStart = json.IndexOf('"', colonIndex + 1);
            if (quoteStart < 0) return null;

            int quoteEnd = json.IndexOf('"', quoteStart + 1);
            if (quoteEnd < 0) return null;

            return json.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);
        }
    }
}
