using System;
using Extension;
using R3;

public class InputUIModel
{
    public Observable<InputUIActiveData> OnChangeActiveData => _onChangeActiveData;
    private Subject<InputUIActiveData> _onChangeActiveData = new Subject<InputUIActiveData>();

    public void UpdateActive(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.TitleInit:

                _onChangeActiveData.OnNext(new InputUIActiveData(
                    UpActive: false,
                    LeftActive: false,
                    DownActive: false,
                    RightActive: false));
                break;
            case GameState.StageSelectInit:

                _onChangeActiveData.OnNext(new InputUIActiveData(
                    UpActive: true,
                    LeftActive: true,
                    DownActive: false,
                    RightActive: true));
                break;
            case GameState.InGameInit:

                _onChangeActiveData.OnNext(new InputUIActiveData(
                    UpActive: true,
                    LeftActive: true,
                    DownActive: true,
                    RightActive: true));
                break;
        }
    }

    
}

[Serializable]
public class ArrowObjData
{
    public ArrowType ArrowType;
    public ArrowObj ArrowObj;
}

public record InputUIActiveData
{
    public bool UpActive;
    public bool LeftActive;
    public bool DownActive;
    public bool RightActive;
    public InputUIActiveData(
        bool UpActive,
        bool LeftActive,
        bool DownActive,
        bool RightActive)
    {
        this.UpActive = UpActive;
        this.LeftActive = LeftActive;
        this.DownActive = DownActive;
        this.RightActive = RightActive;
    }
}
public record InputUIMovableData
{
    public bool UpMovable;
    public bool LeftMovable;
    public bool DownMovable;
    public bool RightMovable;
    public InputUIMovableData(
        bool UpMovable,
        bool LeftMovable,
        bool DownMovable,
        bool RightMovable)
    {
        this.UpMovable = UpMovable;
        this.LeftMovable = LeftMovable;
        this.DownMovable = DownMovable;
        this.RightMovable = RightMovable;
    }
}
