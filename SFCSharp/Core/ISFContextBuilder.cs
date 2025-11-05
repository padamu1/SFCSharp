using SFCSharp.Context;

namespace SFCSharp.Core
{
    public interface ISFContextBuilder
    {
        /// <summary>
        /// 스크립트를 빌드합니다.
        /// </summary>
        /// <param name="script">C# 스크립트 소스 코드</param>
        /// <returns>빌드된 SFContext</returns>
        public SFContext Build(string script);
    }
}
