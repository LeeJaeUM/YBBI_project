using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class AuthTestLogin : TestBase
{
    [SerializeField] private TMP_InputField _loginEmailInputField;        //로그인 이메일
    [SerializeField] private TMP_InputField _loginPasswordInputField;     //로그인 비밀번호

    public override void Test1(InputAction.CallbackContext context)
    {
        _loginEmailInputField.text = "tester1@c.com";
        _loginPasswordInputField.text = "tester1";
    }
    public override void Test2(InputAction.CallbackContext context)
    {
        _loginEmailInputField.text = "tester2@c.com";
        _loginPasswordInputField.text = "tester2";
    }
    public override void Test3(InputAction.CallbackContext context)
    {
        _loginEmailInputField.text = "qwerzxcv@gmail.com";
        _loginPasswordInputField.text = "qwerzxcv";
    }
    public override void Test4(InputAction.CallbackContext context)
    {
        _loginEmailInputField.text = "rama@gmail.com";
        _loginPasswordInputField.text = "rama12";
    }
}
