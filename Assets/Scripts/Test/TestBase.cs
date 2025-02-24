using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestBase : MonoBehaviour
{
    TestInputActions testInputActions;
    void Awake()
    {
        testInputActions = new TestInputActions();
    }

    private void OnEnable()
    {
        testInputActions.Enable();
        testInputActions.Test.test1.performed += Test1;
        testInputActions.Test.test2.performed += Test2;
        testInputActions.Test.test3.performed += Test3;
        testInputActions.Test.test4.performed += Test4;
        testInputActions.Test.test5.performed += Test5;
        testInputActions.Test.test6.performed += Test6;
        testInputActions.Test.test7.performed += Test7;
        testInputActions.Test.test8.performed += Test8;
    }


    private void OnDisable()
    {
        testInputActions.Test.test8.performed -= Test8;
        testInputActions.Test.test7.performed -= Test7;
        testInputActions.Test.test6.performed -= Test6;
        testInputActions.Test.test5.performed -= Test5;
        testInputActions.Test.test4.performed -= Test4;
        testInputActions.Test.test3.performed -= Test3;
        testInputActions.Test.test2.performed -= Test2;
        testInputActions.Test.test1.performed -= Test1;
        testInputActions.Disable(); 
    }

    public virtual void Test1(InputAction.CallbackContext context) { }
    public virtual void Test2(InputAction.CallbackContext context) { }
    public virtual void Test3(InputAction.CallbackContext context) { }
    public virtual void Test4(InputAction.CallbackContext context) { }
    public virtual void Test5(InputAction.CallbackContext context) { }
    public virtual void Test6(InputAction.CallbackContext context) { }
    public virtual void Test7(InputAction.CallbackContext context) { }
    public virtual void Test8(InputAction.CallbackContext context) { }
}
