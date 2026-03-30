# TheCube

キューブを転がして終着点へ向かわせるパズルゲームです。

このリポジトリには以下に記載している再配布不可能なアセットを含めていません。

# 開発期間

基本機能の開発
2025/12/03 ~ 2026/1/22

ギミックの新規追加(ジャンプ・テレポート)
2026/2/9~2026/2/20

技術構成・設計変更  
2026/3/21~2026/3/29

# 開発環境
|  Tools  |  Version  |
| ---- | ---- |
|  Unity  |  2022.3.56f1 |
|  Visual Studio |  2022  |

|  PakageName  |  Version  |
| ---- | ---- |
|  Universal RP  |  14.0.11   |
|  UniRx  |  7.1.0  |
|  UniTask  |  2.5.10  |
| [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)   |

### V2.0.0以降 　
|  PakageName  |  Version  |
| ---- | ---- |
| R3 | 1.3.0 |
| NuGetForUnity | 4.5.0 |
| VContainer | 1.8.0 |
- リアクティブライブラリを UniRx から R3 へ移行
- DIコンテナとして VContainer を新規導入
- 非同期処理は UniTask を継続使用

# 使用アセット
|  AssetName  |  Version  |
| ---- | ---- |
|  [DOTween (HOTween v2)](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676)  |  1.2.765  |

### フォント
|  AssetName  |
| ---- |
|[DotGothic16](https://fonts.google.com/specimen/DotGothic16)|
|[Orbitron](https://fonts.google.com/specimen/Orbitron)|


# 命名規則
|public|private|local|function|
|:--:|:--:|:--:|:--:|
|Upper|_+lower|lower|Upper|

