using SFCSharp.Attributes;

namespace SFCSharp.Scripts
{
    /// <summary>
    /// UnityEngine Transform 기능을 테스트하는 스크립트
    /// SFCSharp 시스템을 통해 동적으로 로드되고 실행됩니다.
    /// </summary>
    [SFCSharp]
    public class UnityTestScript
    {
        private float testValue = 0;

        /// <summary>
        /// Transform 위치 변경 테스트
        /// </summary>
        public void ChangePosition()
        {
            testValue = 10.5f;
            System.Console.WriteLine($"Position changed to: {testValue}");
        }

        /// <summary>
        /// Transform 회전 테스트
        /// </summary>
        public void RotateObject()
        {
            testValue = 45.0f;
            System.Console.WriteLine($"Rotated to: {testValue} degrees");
        }

        /// <summary>
        /// Transform 스케일 변경 테스트
        /// </summary>
        public void ScaleObject()
        {
            testValue = 2.0f;
            System.Console.WriteLine($"Scaled to: {testValue}x");
        }

        /// <summary>
        /// 현재 테스트 값 반환
        /// </summary>
        public float GetTestValue()
        {
            return testValue;
        }

        /// <summary>
        /// 테스트 값 설정
        /// </summary>
        public void SetTestValue(float value)
        {
            testValue = value;
        }
    }
}
