//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.13.0
//     from Assets/Samples/Input System/TestInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

/// <summary>
/// Provides programmatic access to <see cref="InputActionAsset" />, <see cref="InputActionMap" />, <see cref="InputAction" /> and <see cref="InputControlScheme" /> instances defined in asset "Assets/Samples/Input System/TestInputActions.inputactions".
/// </summary>
/// <remarks>
/// This class is source generated and any manual edits will be discarded if the associated asset is reimported or modified.
/// </remarks>
/// <example>
/// <code>
/// using namespace UnityEngine;
/// using UnityEngine.InputSystem;
///
/// // Example of using an InputActionMap named "Player" from a UnityEngine.MonoBehaviour implementing callback interface.
/// public class Example : MonoBehaviour, MyActions.IPlayerActions
/// {
///     private MyActions_Actions m_Actions;                  // Source code representation of asset.
///     private MyActions_Actions.PlayerActions m_Player;     // Source code representation of action map.
///
///     void Awake()
///     {
///         m_Actions = new MyActions_Actions();              // Create asset object.
///         m_Player = m_Actions.Player;                      // Extract action map object.
///         m_Player.AddCallbacks(this);                      // Register callback interface IPlayerActions.
///     }
///
///     void OnDestroy()
///     {
///         m_Actions.Dispose();                              // Destroy asset object.
///     }
///
///     void OnEnable()
///     {
///         m_Player.Enable();                                // Enable all actions within map.
///     }
///
///     void OnDisable()
///     {
///         m_Player.Disable();                               // Disable all actions within map.
///     }
///
///     #region Interface implementation of MyActions.IPlayerActions
///
///     // Invoked when "Move" action is either started, performed or canceled.
///     public void OnMove(InputAction.CallbackContext context)
///     {
///         Debug.Log($"OnMove: {context.ReadValue&lt;Vector2&gt;()}");
///     }
///
///     // Invoked when "Attack" action is either started, performed or canceled.
///     public void OnAttack(InputAction.CallbackContext context)
///     {
///         Debug.Log($"OnAttack: {context.ReadValue&lt;float&gt;()}");
///     }
///
///     #endregion
/// }
/// </code>
/// </example>
public partial class @TestInputActions: IInputActionCollection2, IDisposable
{
    /// <summary>
    /// Provides access to the underlying asset instance.
    /// </summary>
    public InputActionAsset asset { get; }

    /// <summary>
    /// Constructs a new instance.
    /// </summary>
    public @TestInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""TestInputActions"",
    ""maps"": [
        {
            ""name"": ""Test"",
            ""id"": ""331092d8-fdc8-496b-b155-bfa7f482735d"",
            ""actions"": [
                {
                    ""name"": ""test1"",
                    ""type"": ""Button"",
                    ""id"": ""124663f4-ef54-4da4-ac0e-88b9cca27040"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""test2"",
                    ""type"": ""Button"",
                    ""id"": ""13432397-39cf-4999-9b9c-4f896b710a8e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""test3"",
                    ""type"": ""Button"",
                    ""id"": ""7d0a840a-391c-4271-b984-a270353c18a2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""test4"",
                    ""type"": ""Button"",
                    ""id"": ""969c56f3-6fa2-442f-85fe-7839aba28c62"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""test5"",
                    ""type"": ""Button"",
                    ""id"": ""cc2f671c-6a04-4b4d-af5f-b1ccce3feee2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""test6"",
                    ""type"": ""Button"",
                    ""id"": ""3cfa423c-7cb8-45c4-99dd-881324935d56"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""test7"",
                    ""type"": ""Button"",
                    ""id"": ""fc45874b-8746-4ad8-8cee-7dc20f7c92c3"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""test8"",
                    ""type"": ""Button"",
                    ""id"": ""d69cc9fe-89c1-45ec-b6d0-5bffc70be463"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7fbd824e-7d3f-4240-b23e-96bc7430645b"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""test1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3deb7858-1ffc-4fcc-acea-f7fc65d7a999"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""test2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3c15fb2a-3ef5-491f-ba62-57a20daee42a"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""test3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""658a3498-55c2-4b85-b11f-771d331ee383"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""test4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c1a3587e-2ce6-4192-a5d9-eb31024526fa"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""test5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""630a3b81-b61b-4d12-9c4b-5d56712f6c2b"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""test6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""58de8f9a-0742-410f-8794-767675000a5c"",
                    ""path"": ""<Keyboard>/7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""test7"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c889e35b-07f4-4194-8b03-02ce32392948"",
                    ""path"": ""<Keyboard>/8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""test8"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KM"",
            ""bindingGroup"": ""KM"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Test
        m_Test = asset.FindActionMap("Test", throwIfNotFound: true);
        m_Test_test1 = m_Test.FindAction("test1", throwIfNotFound: true);
        m_Test_test2 = m_Test.FindAction("test2", throwIfNotFound: true);
        m_Test_test3 = m_Test.FindAction("test3", throwIfNotFound: true);
        m_Test_test4 = m_Test.FindAction("test4", throwIfNotFound: true);
        m_Test_test5 = m_Test.FindAction("test5", throwIfNotFound: true);
        m_Test_test6 = m_Test.FindAction("test6", throwIfNotFound: true);
        m_Test_test7 = m_Test.FindAction("test7", throwIfNotFound: true);
        m_Test_test8 = m_Test.FindAction("test8", throwIfNotFound: true);
    }

    ~@TestInputActions()
    {
        UnityEngine.Debug.Assert(!m_Test.enabled, "This will cause a leak and performance issues, TestInputActions.Test.Disable() has not been called.");
    }

    /// <summary>
    /// Destroys this asset and all associated <see cref="InputAction"/> instances.
    /// </summary>
    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    /// <inheritdoc cref="UnityEngine.InputSystem.InputActionAsset.bindingMask" />
    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    /// <inheritdoc cref="UnityEngine.InputSystem.InputActionAsset.devices" />
    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    /// <inheritdoc cref="UnityEngine.InputSystem.InputActionAsset.controlSchemes" />
    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    /// <inheritdoc cref="UnityEngine.InputSystem.InputActionAsset.Contains(InputAction)" />
    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    /// <inheritdoc cref="UnityEngine.InputSystem.InputActionAsset.GetEnumerator()" />
    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    /// <inheritdoc cref="IEnumerable.GetEnumerator()" />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc cref="UnityEngine.InputSystem.InputActionAsset.Enable()" />
    public void Enable()
    {
        asset.Enable();
    }

    /// <inheritdoc cref="UnityEngine.InputSystem.InputActionAsset.Disable()" />
    public void Disable()
    {
        asset.Disable();
    }

    /// <inheritdoc cref="UnityEngine.InputSystem.InputActionAsset.bindings" />
    public IEnumerable<InputBinding> bindings => asset.bindings;

    /// <inheritdoc cref="UnityEngine.InputSystem.InputActionAsset.FindAction(string, bool)" />
    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    /// <inheritdoc cref="UnityEngine.InputSystem.InputActionAsset.FindBinding(InputBinding, out InputAction)" />
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Test
    private readonly InputActionMap m_Test;
    private List<ITestActions> m_TestActionsCallbackInterfaces = new List<ITestActions>();
    private readonly InputAction m_Test_test1;
    private readonly InputAction m_Test_test2;
    private readonly InputAction m_Test_test3;
    private readonly InputAction m_Test_test4;
    private readonly InputAction m_Test_test5;
    private readonly InputAction m_Test_test6;
    private readonly InputAction m_Test_test7;
    private readonly InputAction m_Test_test8;
    /// <summary>
    /// Provides access to input actions defined in input action map "Test".
    /// </summary>
    public struct TestActions
    {
        private @TestInputActions m_Wrapper;

        /// <summary>
        /// Construct a new instance of the input action map wrapper class.
        /// </summary>
        public TestActions(@TestInputActions wrapper) { m_Wrapper = wrapper; }
        /// <summary>
        /// Provides access to the underlying input action "Test/test1".
        /// </summary>
        public InputAction @test1 => m_Wrapper.m_Test_test1;
        /// <summary>
        /// Provides access to the underlying input action "Test/test2".
        /// </summary>
        public InputAction @test2 => m_Wrapper.m_Test_test2;
        /// <summary>
        /// Provides access to the underlying input action "Test/test3".
        /// </summary>
        public InputAction @test3 => m_Wrapper.m_Test_test3;
        /// <summary>
        /// Provides access to the underlying input action "Test/test4".
        /// </summary>
        public InputAction @test4 => m_Wrapper.m_Test_test4;
        /// <summary>
        /// Provides access to the underlying input action "Test/test5".
        /// </summary>
        public InputAction @test5 => m_Wrapper.m_Test_test5;
        /// <summary>
        /// Provides access to the underlying input action "Test/test6".
        /// </summary>
        public InputAction @test6 => m_Wrapper.m_Test_test6;
        /// <summary>
        /// Provides access to the underlying input action "Test/test7".
        /// </summary>
        public InputAction @test7 => m_Wrapper.m_Test_test7;
        /// <summary>
        /// Provides access to the underlying input action "Test/test8".
        /// </summary>
        public InputAction @test8 => m_Wrapper.m_Test_test8;
        /// <summary>
        /// Provides access to the underlying input action map instance.
        /// </summary>
        public InputActionMap Get() { return m_Wrapper.m_Test; }
        /// <inheritdoc cref="UnityEngine.InputSystem.InputActionMap.Enable()" />
        public void Enable() { Get().Enable(); }
        /// <inheritdoc cref="UnityEngine.InputSystem.InputActionMap.Disable()" />
        public void Disable() { Get().Disable(); }
        /// <inheritdoc cref="UnityEngine.InputSystem.InputActionMap.enabled" />
        public bool enabled => Get().enabled;
        /// <summary>
        /// Implicitly converts an <see ref="TestActions" /> to an <see ref="InputActionMap" /> instance.
        /// </summary>
        public static implicit operator InputActionMap(TestActions set) { return set.Get(); }
        /// <summary>
        /// Adds <see cref="InputAction.started"/>, <see cref="InputAction.performed"/> and <see cref="InputAction.canceled"/> callbacks provided via <param cref="instance" /> on all input actions contained in this map.
        /// </summary>
        /// <param name="instance">Callback instance.</param>
        /// <remarks>
        /// If <paramref name="instance" /> is <c>null</c> or <paramref name="instance"/> have already been added this method does nothing.
        /// </remarks>
        /// <seealso cref="TestActions" />
        public void AddCallbacks(ITestActions instance)
        {
            if (instance == null || m_Wrapper.m_TestActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_TestActionsCallbackInterfaces.Add(instance);
            @test1.started += instance.OnTest1;
            @test1.performed += instance.OnTest1;
            @test1.canceled += instance.OnTest1;
            @test2.started += instance.OnTest2;
            @test2.performed += instance.OnTest2;
            @test2.canceled += instance.OnTest2;
            @test3.started += instance.OnTest3;
            @test3.performed += instance.OnTest3;
            @test3.canceled += instance.OnTest3;
            @test4.started += instance.OnTest4;
            @test4.performed += instance.OnTest4;
            @test4.canceled += instance.OnTest4;
            @test5.started += instance.OnTest5;
            @test5.performed += instance.OnTest5;
            @test5.canceled += instance.OnTest5;
            @test6.started += instance.OnTest6;
            @test6.performed += instance.OnTest6;
            @test6.canceled += instance.OnTest6;
            @test7.started += instance.OnTest7;
            @test7.performed += instance.OnTest7;
            @test7.canceled += instance.OnTest7;
            @test8.started += instance.OnTest8;
            @test8.performed += instance.OnTest8;
            @test8.canceled += instance.OnTest8;
        }

        /// <summary>
        /// Removes <see cref="InputAction.started"/>, <see cref="InputAction.performed"/> and <see cref="InputAction.canceled"/> callbacks provided via <param cref="instance" /> on all input actions contained in this map.
        /// </summary>
        /// <remarks>
        /// Calling this method when <paramref name="instance" /> have not previously been registered has no side-effects.
        /// </remarks>
        /// <seealso cref="TestActions" />
        private void UnregisterCallbacks(ITestActions instance)
        {
            @test1.started -= instance.OnTest1;
            @test1.performed -= instance.OnTest1;
            @test1.canceled -= instance.OnTest1;
            @test2.started -= instance.OnTest2;
            @test2.performed -= instance.OnTest2;
            @test2.canceled -= instance.OnTest2;
            @test3.started -= instance.OnTest3;
            @test3.performed -= instance.OnTest3;
            @test3.canceled -= instance.OnTest3;
            @test4.started -= instance.OnTest4;
            @test4.performed -= instance.OnTest4;
            @test4.canceled -= instance.OnTest4;
            @test5.started -= instance.OnTest5;
            @test5.performed -= instance.OnTest5;
            @test5.canceled -= instance.OnTest5;
            @test6.started -= instance.OnTest6;
            @test6.performed -= instance.OnTest6;
            @test6.canceled -= instance.OnTest6;
            @test7.started -= instance.OnTest7;
            @test7.performed -= instance.OnTest7;
            @test7.canceled -= instance.OnTest7;
            @test8.started -= instance.OnTest8;
            @test8.performed -= instance.OnTest8;
            @test8.canceled -= instance.OnTest8;
        }

        /// <summary>
        /// Unregisters <param cref="instance" /> and unregisters all input action callbacks via <see cref="TestActions.UnregisterCallbacks(ITestActions)" />.
        /// </summary>
        /// <seealso cref="TestActions.UnregisterCallbacks(ITestActions)" />
        public void RemoveCallbacks(ITestActions instance)
        {
            if (m_Wrapper.m_TestActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        /// <summary>
        /// Replaces all existing callback instances and previously registered input action callbacks associated with them with callbacks provided via <param cref="instance" />.
        /// </summary>
        /// <remarks>
        /// If <paramref name="instance" /> is <c>null</c>, calling this method will only unregister all existing callbacks but not register any new callbacks.
        /// </remarks>
        /// <seealso cref="TestActions.AddCallbacks(ITestActions)" />
        /// <seealso cref="TestActions.RemoveCallbacks(ITestActions)" />
        /// <seealso cref="TestActions.UnregisterCallbacks(ITestActions)" />
        public void SetCallbacks(ITestActions instance)
        {
            foreach (var item in m_Wrapper.m_TestActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_TestActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    /// <summary>
    /// Provides a new <see cref="TestActions" /> instance referencing this action map.
    /// </summary>
    public TestActions @Test => new TestActions(this);
    private int m_KMSchemeIndex = -1;
    /// <summary>
    /// Provides access to the input control scheme.
    /// </summary>
    /// <seealso cref="UnityEngine.InputSystem.InputControlScheme" />
    public InputControlScheme KMScheme
    {
        get
        {
            if (m_KMSchemeIndex == -1) m_KMSchemeIndex = asset.FindControlSchemeIndex("KM");
            return asset.controlSchemes[m_KMSchemeIndex];
        }
    }
    /// <summary>
    /// Interface to implement callback methods for all input action callbacks associated with input actions defined by "Test" which allows adding and removing callbacks.
    /// </summary>
    /// <seealso cref="TestActions.AddCallbacks(ITestActions)" />
    /// <seealso cref="TestActions.RemoveCallbacks(ITestActions)" />
    public interface ITestActions
    {
        /// <summary>
        /// Method invoked when associated input action "test1" is either <see cref="UnityEngine.InputSystem.InputAction.started" />, <see cref="UnityEngine.InputSystem.InputAction.performed" /> or <see cref="UnityEngine.InputSystem.InputAction.canceled" />.
        /// </summary>
        /// <seealso cref="UnityEngine.InputSystem.InputAction.started" />
        /// <seealso cref="UnityEngine.InputSystem.InputAction.performed" />
        /// <seealso cref="UnityEngine.InputSystem.InputAction.canceled" />
        void OnTest1(InputAction.CallbackContext context);
        /// <summary>
        /// Method invoked when associated input action "test2" is either <see cref="UnityEngine.InputSystem.InputAction.started" />, <see cref="UnityEngine.InputSystem.InputAction.performed" /> or <see cref="UnityEngine.InputSystem.InputAction.canceled" />.
        /// </summary>
        /// <seealso cref="UnityEngine.InputSystem.InputAction.started" />
        /// <seealso cref="UnityEngine.InputSystem.InputAction.performed" />
        /// <seealso cref="UnityEngine.InputSystem.InputAction.canceled" />
        void OnTest2(InputAction.CallbackContext context);
        /// <summary>
        /// Method invoked when associated input action "test3" is either <see cref="UnityEngine.InputSystem.InputAction.started" />, <see cref="UnityEngine.InputSystem.InputAction.performed" /> or <see cref="UnityEngine.InputSystem.InputAction.canceled" />.
        /// </summary>
        /// <seealso cref="UnityEngine.InputSystem.InputAction.started" />
        /// <seealso cref="UnityEngine.InputSystem.InputAction.performed" />
        /// <seealso cref="UnityEngine.InputSystem.InputAction.canceled" />
        void OnTest3(InputAction.CallbackContext context);
        /// <summary>
        /// Method invoked when associated input action "test4" is either <see cref="UnityEngine.InputSystem.InputAction.started" />, <see cref="UnityEngine.InputSystem.InputAction.performed" /> or <see cref="UnityEngine.InputSystem.InputAction.canceled" />.
        /// </summary>
        /// <seealso cref="UnityEngine.InputSystem.InputAction.started" />
        /// <seealso cref="UnityEngine.InputSystem.InputAction.performed" />
        /// <seealso cref="UnityEngine.InputSystem.InputAction.canceled" />
        void OnTest4(InputAction.CallbackContext context);
        /// <summary>
        /// Method invoked when associated input action "test5" is either <see cref="UnityEngine.InputSystem.InputAction.started" />, <see cref="UnityEngine.InputSystem.InputAction.performed" /> or <see cref="UnityEngine.InputSystem.InputAction.canceled" />.
        /// </summary>
        /// <seealso cref="UnityEngine.InputSystem.InputAction.started" />
        /// <seealso cref="UnityEngine.InputSystem.InputAction.performed" />
        /// <seealso cref="UnityEngine.InputSystem.InputAction.canceled" />
        void OnTest5(InputAction.CallbackContext context);
        /// <summary>
        /// Method invoked when associated input action "test6" is either <see cref="UnityEngine.InputSystem.InputAction.started" />, <see cref="UnityEngine.InputSystem.InputAction.performed" /> or <see cref="UnityEngine.InputSystem.InputAction.canceled" />.
        /// </summary>
        /// <seealso cref="UnityEngine.InputSystem.InputAction.started" />
        /// <seealso cref="UnityEngine.InputSystem.InputAction.performed" />
        /// <seealso cref="UnityEngine.InputSystem.InputAction.canceled" />
        void OnTest6(InputAction.CallbackContext context);
        /// <summary>
        /// Method invoked when associated input action "test7" is either <see cref="UnityEngine.InputSystem.InputAction.started" />, <see cref="UnityEngine.InputSystem.InputAction.performed" /> or <see cref="UnityEngine.InputSystem.InputAction.canceled" />.
        /// </summary>
        /// <seealso cref="UnityEngine.InputSystem.InputAction.started" />
        /// <seealso cref="UnityEngine.InputSystem.InputAction.performed" />
        /// <seealso cref="UnityEngine.InputSystem.InputAction.canceled" />
        void OnTest7(InputAction.CallbackContext context);
        /// <summary>
        /// Method invoked when associated input action "test8" is either <see cref="UnityEngine.InputSystem.InputAction.started" />, <see cref="UnityEngine.InputSystem.InputAction.performed" /> or <see cref="UnityEngine.InputSystem.InputAction.canceled" />.
        /// </summary>
        /// <seealso cref="UnityEngine.InputSystem.InputAction.started" />
        /// <seealso cref="UnityEngine.InputSystem.InputAction.performed" />
        /// <seealso cref="UnityEngine.InputSystem.InputAction.canceled" />
        void OnTest8(InputAction.CallbackContext context);
    }
}
