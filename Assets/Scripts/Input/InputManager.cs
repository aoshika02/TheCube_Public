using R3;

public class InputManager
{
    PlayerAction _playerAction;
    #region AtoM
    public ReadOnlyReactiveProperty<float> KeyA => _keyA;
    private readonly ReactiveProperty<float> _keyA = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyB => _keyB;
    private readonly ReactiveProperty<float> _keyB = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyC => _keyC;
    private readonly ReactiveProperty<float> _keyC = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyD => _keyD;
    private readonly ReactiveProperty<float> _keyD = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyE => _keyE;
    private readonly ReactiveProperty<float> _keyE = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyF => _keyF;
    private readonly ReactiveProperty<float> _keyF = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyG => _keyG;
    private readonly ReactiveProperty<float> _keyG = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyH => _keyH;
    private readonly ReactiveProperty<float> _keyH = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyI => _keyI;
    private readonly ReactiveProperty<float> _keyI = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyJ => _keyJ;
    private readonly ReactiveProperty<float> _keyJ = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyK => _keyK;
    private readonly ReactiveProperty<float> _keyK = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyL => _keyL;
    private readonly ReactiveProperty<float> _keyL = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyM => _keyM;
    private readonly ReactiveProperty<float> _keyM = new ReactiveProperty<float>();
    #endregion
    #region NtoZ
    public ReadOnlyReactiveProperty<float> KeyN => _keyN;
    private readonly ReactiveProperty<float> _keyN = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyO => _keyO;
    private readonly ReactiveProperty<float> _keyO = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyP => _keyP;
    private readonly ReactiveProperty<float> _keyP = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyQ => _keyQ;
    private readonly ReactiveProperty<float> _keyQ = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyR => _keyR;
    private readonly ReactiveProperty<float> _keyR = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyS => _keyS;
    private readonly ReactiveProperty<float> _keyS = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyT => _keyT;
    private readonly ReactiveProperty<float> _keyT = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyU => _keyU;
    private readonly ReactiveProperty<float> _keyU = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyV => _keyV;
    private readonly ReactiveProperty<float> _keyV = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyW => _keyW;
    private readonly ReactiveProperty<float> _keyW = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyX => _keyX;
    private readonly ReactiveProperty<float> _keyX = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyY => _keyY;
    private readonly ReactiveProperty<float> _keyY = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> KeyZ => _keyZ;
    private readonly ReactiveProperty<float> _keyZ = new ReactiveProperty<float>();
    #endregion

    public ReadOnlyReactiveProperty<float> Any => _any;
    private readonly ReactiveProperty<float> _any = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> Space => _space;
    private readonly ReactiveProperty<float> _space = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> Enter => _enter;
    private readonly ReactiveProperty<float> _enter = new ReactiveProperty<float>();

    public InputManager()
    {
        _playerAction = new PlayerAction();
        _playerAction.Enable();
        #region AtoM
        _playerAction.KeyInput.Key_A.started += context => { _keyA.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_A.performed += context => { _keyA.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_A.canceled += context => { _keyA.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_B.started += context => { _keyB.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_B.performed += context => { _keyB.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_B.canceled += context => { _keyB.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_C.started += context => { _keyC.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_C.performed += context => { _keyC.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_C.canceled += context => { _keyC.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_D.started += context => { _keyD.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_D.performed += context => { _keyD.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_D.canceled += context => { _keyD.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_E.started += context => { _keyE.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_E.performed += context => { _keyE.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_E.canceled += context => { _keyE.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_F.started += context => { _keyF.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_F.performed += context => { _keyF.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_F.canceled += context => { _keyF.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_G.started += context => { _keyG.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_G.performed += context => { _keyG.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_G.canceled += context => { _keyG.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_H.started += context => { _keyH.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_H.performed += context => { _keyH.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_H.canceled += context => { _keyH.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_I.started += context => { _keyI.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_I.performed += context => { _keyI.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_I.canceled += context => { _keyI.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_J.started += context => { _keyJ.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_J.performed += context => { _keyJ.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_J.canceled += context => { _keyJ.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_K.started += context => { _keyK.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_K.performed += context => { _keyK.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_K.canceled += context => { _keyK.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_L.started += context => { _keyL.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_L.performed += context => { _keyL.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_L.canceled += context => { _keyL.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_M.started += context => { _keyM.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_M.performed += context => { _keyM.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_M.canceled += context => { _keyM.Value = context.ReadValue<float>(); };
        #endregion
        #region NtoZ
        _playerAction.KeyInput.Key_N.started += context => { _keyN.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_N.performed += context => { _keyN.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_N.canceled += context => { _keyN.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_O.started += context => { _keyO.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_O.performed += context => { _keyO.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_O.canceled += context => { _keyO.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_P.started += context => { _keyP.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_P.performed += context => { _keyP.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_P.canceled += context => { _keyP.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_Q.started += context => { _keyQ.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_Q.performed += context => { _keyQ.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_Q.canceled += context => { _keyQ.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_R.started += context => { _keyR.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_R.performed += context => { _keyR.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_R.canceled += context => { _keyR.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_S.started += context => { _keyS.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_S.performed += context => { _keyS.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_S.canceled += context => { _keyS.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_T.started += context => { _keyT.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_T.performed += context => { _keyT.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_T.canceled += context => { _keyT.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_U.started += context => { _keyU.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_U.performed += context => { _keyU.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_U.canceled += context => { _keyU.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_V.started += context => { _keyV.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_V.performed += context => { _keyV.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_V.canceled += context => { _keyV.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_W.started += context => { _keyW.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_W.performed += context => { _keyW.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_W.canceled += context => { _keyW.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_X.started += context => { _keyX.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_X.performed += context => { _keyX.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_X.canceled += context => { _keyX.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_Y.started += context => { _keyY.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_Y.performed += context => { _keyY.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_Y.canceled += context => { _keyY.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Key_Z.started += context => { _keyZ.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_Z.performed += context => { _keyZ.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Key_Z.canceled += context => { _keyZ.Value = context.ReadValue<float>(); };

        #endregion

        _playerAction.KeyInput.Any.started += context => { _any.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Any.performed += context => { _any.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Any.canceled += context => { _any.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Space.started += context => { _space.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Space.performed += context => { _space.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Space.canceled += context => { _space.Value = context.ReadValue<float>(); };

        _playerAction.KeyInput.Enter.started += context => { _enter.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Enter.performed += context => { _enter.Value = context.ReadValue<float>(); };
        _playerAction.KeyInput.Enter.canceled += context => { _enter.Value = context.ReadValue<float>(); };
    }
}
