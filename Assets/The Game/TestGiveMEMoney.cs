using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestGiveMeMoney : TestBase
{
    [SerializeField] private TMP_InputField _loginEmailInputField;        //로그인 이메일
    [SerializeField] private TMP_InputField _loginPasswordInputField;     //로그인 비밀번호

    public override void Test1(InputAction.CallbackContext context)
    {
        _loginEmailInputField.text = "tester1@c.com";
        _loginPasswordInputField.text = "tester1";
    }
}
