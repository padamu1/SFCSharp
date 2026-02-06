using SFCSharp.Runtime.ModLoader;
using System;
using System.Collections.Generic;

namespace SFCSharp.Network
{
    /// <summary>
    /// MOD 저장소 인터페이스
    /// 업로드/다운로드를 위한 추상화 레이어입니다.
    /// Unity 환경에서는 UnityWebRequest 기반으로 구현할 수 있습니다.
    /// </summary>
    public interface ISFModRepository
    {
        /// <summary>
        /// MOD를 업로드합니다.
        /// </summary>
        /// <param name="info">MOD 메타데이터</param>
        /// <param name="bundleData">번들 바이너리 데이터</param>
        /// <param name="callback">완료 콜백 (성공 여부, 에러 메시지)</param>
        void Upload(SFModInfo info, byte[] bundleData, Action<bool, string> callback);

        /// <summary>
        /// MOD를 다운로드합니다.
        /// </summary>
        /// <param name="modId">MOD 식별자</param>
        /// <param name="callback">완료 콜백 (바이너리 데이터, 에러 메시지)</param>
        void Download(string modId, Action<byte[], string> callback);

        /// <summary>
        /// 사용 가능한 MOD 목록을 조회합니다.
        /// </summary>
        /// <param name="callback">완료 콜백 (MOD 목록, 에러 메시지)</param>
        void GetModList(Action<List<SFModInfo>, string> callback);

        /// <summary>
        /// MOD를 검색합니다.
        /// </summary>
        /// <param name="query">검색어</param>
        /// <param name="callback">완료 콜백 (검색 결과, 에러 메시지)</param>
        void Search(string query, Action<List<SFModInfo>, string> callback);

        /// <summary>
        /// MOD를 삭제합니다.
        /// </summary>
        /// <param name="modId">MOD 식별자</param>
        /// <param name="callback">완료 콜백 (성공 여부, 에러 메시지)</param>
        void Delete(string modId, Action<bool, string> callback);
    }
}
