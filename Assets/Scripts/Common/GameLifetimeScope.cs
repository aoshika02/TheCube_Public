using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private TileInfoDatas _tileInfoDatas;

    protected override void Configure(IContainerBuilder builder)
    {
        //model
        builder.RegisterInstance(new DialogModel());
        builder.Register(resolver =>
        {
            var loadManager = resolver.Resolve<LoadManager>();

            TileInfoModel tileInfoModel = null;

            if (loadManager.GetGameSaveData(out var saveData, true))
            {
                var verifiedTypes = saveData.VerifiedTypes;
                tileInfoModel = new TileInfoModel(_tileInfoDatas, verifiedTypes != null ? verifiedTypes.ToList() : null);
            }
            else
            {
                tileInfoModel = new TileInfoModel(_tileInfoDatas, null);
            }
            return tileInfoModel;
        }, Lifetime.Singleton);
        builder.Register(resolver =>
        {
            var dataSetLoader = resolver.Resolve<DataSetLoader>();
            var maxStageID = dataSetLoader.GetTotalStageCount() - 1;
            var stageSaveManager = resolver.Resolve<StageSaveManager>();
            int stageID = 0;
            if (resolver.Resolve<LoadManager>().GetGameSaveData(out var saveData, true))
            {
                stageID = saveData.LastStageID;
            }
            var stageSelectModel = new StageSelectModel(stageSaveManager, maxStageID, stageID);
            return stageSelectModel;
        }, Lifetime.Singleton);

        //save/load
        var savePath = new SavePath();
        builder.RegisterInstance(savePath);
        builder.RegisterInstance(new LoadManager(savePath));
        builder.Register<SaveManager>(Lifetime.Singleton);
        builder.Register(resolver =>
        {
            var loadManager = resolver.Resolve<LoadManager>();
            StageSaveManager stageSaveManager = null;
            var datasetLoader = resolver.Resolve<DataSetLoader>();
            var totalStageCount = datasetLoader.GetTotalStageCount();

            if (loadManager.GetGameSaveData(out var gameSaveData, true))
            {
                stageSaveManager = new StageSaveManager(gameSaveData.StageSaveDatas.ToList(), totalStageCount);
            }
            else
            {
                stageSaveManager = new StageSaveManager(0, totalStageCount);
            }
            return stageSaveManager;
        }, Lifetime.Singleton);

        //data
        builder.RegisterComponentInHierarchy<DataSetLoader>();
        builder.RegisterComponentInHierarchy<TextDataset>();

        //input
        builder.Register<InputManager>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<MouseInputManager>();
        builder.RegisterComponentInHierarchy<PlayerMoveInput>();

        //state
        builder.Register<GameStateManager>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<FadeManager>();

        //tile
        builder.Register<TitleModel>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<TileManager>();
        builder.RegisterComponentInHierarchy<TilePool>();
        builder.RegisterComponentInHierarchy<InfoViewPool>();

        //ui
        builder.RegisterComponentInHierarchy<IconUIPresenter>();
        builder.RegisterComponentInHierarchy<InputUIView>();
        builder.RegisterComponentInHierarchy<InputUIPresenter>();
        builder.RegisterComponentInHierarchy<TileInfoSystem>();
        builder.RegisterComponentInHierarchy<DialogPresenter>();

        //player
        builder.RegisterComponentInHierarchy<PlayerCube>();
        builder.RegisterComponentInHierarchy<PlayerCamera>();
        builder.Register<PlayerMoveProcessor>(Lifetime.Singleton);

        //ingame
        builder.Register<InGameModel>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<InGamePresenter>();
        builder.RegisterComponentInHierarchy<InGameView>();
        builder.Register<TutorialManager>(Lifetime.Singleton);

        //title 
        builder.RegisterComponentInHierarchy<TitlePresenter>();
        builder.RegisterComponentInHierarchy<TitleEffectPresenter>();

        //stage select
        builder.RegisterComponentInHierarchy<StageSelectPresenter>();

        //initialize
        builder.RegisterComponentInHierarchy<GameInitialize>();

        RegisterTileEvent(builder);
        RegisterMoveCommand(builder);
        RegisterInputUIDataCommand(builder);
        RegisterTileAnim(builder);
    }

    private void RegisterTileAnim(IContainerBuilder builder)
    {
        builder.Register(_ =>
        {
            var locator = new TileAnimLocator(new EmptyTileAnim());
            locator.Register(TileType.Goal, new ScaleAnim());
            locator.Register(TileType.Reset, new RotateAnim());
            locator.Register(TileType.Warp, new ReverseScaleAnim());
            locator.Register(TileType.UpperArrow, new ArrowAnim(TileType.UpperArrow));
            locator.Register(TileType.LeftArrow, new ArrowAnim(TileType.LeftArrow));
            locator.Register(TileType.DownArrow, new ArrowAnim(TileType.DownArrow));
            locator.Register(TileType.RightArrow, new ArrowAnim(TileType.RightArrow));
            locator.Register(TileType.JumpUp, new BounceAnim());
            locator.Register(TileType.JumpDown, new BounceAnim());
            locator.Register(TileType.JumpLeft, new BounceAnim());
            locator.Register(TileType.JumpRight, new BounceAnim());
            return locator;
        }, Lifetime.Singleton);
    }

    private void RegisterTileEvent(IContainerBuilder builder)
    {
        builder.Register(resolver =>
        {
            var tileManager = resolver.Resolve<TileManager>();
            var playerCube = resolver.Resolve<PlayerCube>();
            var playerCameraMove = resolver.Resolve<PlayerCamera>();
            var jumpDistance = 2;

            var locator = new TileEventLocator(new EmptyTileEvent(playerCube, tileManager));
            locator.Register(TileType.Normal, new NormalTileEvent(playerCube, tileManager));
            locator.Register(TileType.Goal, new GoalTileEvent(playerCube, tileManager));
            locator.Register(TileType.Skip, new SkipTileEvent(playerCube, tileManager));
            locator.Register(TileType.Reset, new ResetTileEvent(playerCube, tileManager));
            locator.Register(TileType.Warp, new WarpTileEvent(playerCube, tileManager));
            locator.Register(TileType.UpperArrow, new AccelTileEvent(playerCube, tileManager));
            locator.Register(TileType.LeftArrow, new AccelTileEvent(playerCube, tileManager));
            locator.Register(TileType.DownArrow, new AccelTileEvent(playerCube, tileManager));
            locator.Register(TileType.RightArrow, new AccelTileEvent(playerCube, tileManager));
            locator.Register(TileType.JumpUp, new JumpTileEvent(playerCube, tileManager, playerCameraMove, jumpDistance));
            locator.Register(TileType.JumpDown, new JumpTileEvent(playerCube, tileManager, playerCameraMove, jumpDistance));
            locator.Register(TileType.JumpLeft, new JumpTileEvent(playerCube, tileManager, playerCameraMove, jumpDistance));
            locator.Register(TileType.JumpRight, new JumpTileEvent(playerCube, tileManager, playerCameraMove, jumpDistance));
            return locator;
        }, Lifetime.Singleton);
    }

    private void RegisterMoveCommand(IContainerBuilder builder)
    {
        builder.Register(resolver =>
        {
            var locator = new MoveCommandLocator(new EmptyMoveCommand());
            locator.Register(TileType.Normal, new NormalMoveCommand());
            locator.Register(TileType.Goal, new NormalMoveCommand());
            locator.Register(TileType.Skip, new NormalMoveCommand());
            locator.Register(TileType.Reset, new NormalMoveCommand());
            locator.Register(TileType.Warp, new WarpMoveCommand());
            locator.Register(TileType.UpperArrow, new AccelMoveCommand());
            locator.Register(TileType.LeftArrow, new AccelMoveCommand());
            locator.Register(TileType.DownArrow, new AccelMoveCommand());
            locator.Register(TileType.RightArrow, new AccelMoveCommand());
            locator.Register(TileType.JumpUp, new JumpMoveCommand());
            locator.Register(TileType.JumpDown, new JumpMoveCommand());
            locator.Register(TileType.JumpLeft, new JumpMoveCommand());
            locator.Register(TileType.JumpRight, new JumpMoveCommand());
            return locator;
        }, Lifetime.Singleton);
    }

    private void RegisterInputUIDataCommand(IContainerBuilder builder)
    {
        builder.Register(resolver =>
        {
            var playerCube = resolver.Resolve<PlayerCube>();
            var tileManager = resolver.Resolve<TileManager>();
            var stageSelectModel = resolver.Resolve<StageSelectModel>();
            var playerMoveProcessor = resolver.Resolve<PlayerMoveProcessor>();
            var locator = new InputUIDataLocator(new EmptyInputUICommand());
            locator.Register(GameState.StageSelectIdle, new StageSelectInputUICommand(stageSelectModel));
            locator.Register(GameState.InGameIdle, new InGameInputUICommand(playerCube, tileManager, playerMoveProcessor));
            locator.Register(GameState.StageSelectInit, new OtherInputData());
            locator.Register(GameState.InGameInit, new OtherInputData());
            return locator;
        }, Lifetime.Singleton);
    }
}
