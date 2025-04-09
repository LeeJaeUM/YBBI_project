using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_06_Chating : TestBase
{
    [SerializeField] private TMP_InputField _loginEmailInputField;        //로그인 이메일
    [SerializeField] private TMP_InputField _loginPasswordInputField;     //로그인 비밀번호

    public string TestID = " ";
    public string TestPASS = " ";

    public override void Test4(InputAction.CallbackContext context)
    {
        _loginEmailInputField.text = TestID;
        _loginPasswordInputField.text = TestPASS;
    }
}
