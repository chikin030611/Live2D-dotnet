# Live2D-dotnet

\* 此README使用了AI由英文翻譯為中文。如有任何句子不通順的問題，建議參考英文版。

該應用程式展示了 Live2D 模型的所有功能。本 README 僅介紹如何運作和使用該應用程式。若要查看應用程式的詳細工作流程，請參閱[Live2DCSharpSDK](https://github.com/chikin030611/Live2DCSharpSDK)。

![demo](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/demo.png)

這個儲存庫是從 [ColorMC](https://github.com/Coloryr/ColorMC/tree/master) 分叉的。大部分原始程式碼已被刪除，以保持儲存庫簡單。

## 開始使用

### 前提條件

從[官網](https://www.live2d.com/en/sdk/download/native/)下載Cubism SDK的zip檔。請注意，不需要解壓縮 zip 檔案。

### 設定

    # 複製儲存庫
    git clone https://github.com/chikin030611/Live2D-dotnet.git

    # 轉至儲存庫
    cd Live2D-dotnet

    # 初始化、更新和複製所有子模組。
    git submodule update --init --recursive

### 使用Terminal啟動項目

    # 轉至Launcher
    cd src/Live2DDotNet.Launcher

    # 執行應用程式
    dotnet run

### 使用 Visual Studio 啟動項目

1. 在 `\Live2D-dotnet\src\` 開啟 `Live2D-dotnet.sln` 。
2. 將啟動項目配置為 **Live2DDotNet.Launcher**。

## 如何使用

### 1. 導入Cubism核心

若要匯入 Cubism Core，請依照下列步驟操作。

點擊齒輪圖示打開設定。

![import1](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/import1.png)

按一下 “Import Core”。

![import2](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/import2.png)

選擇 Cubism SDK zip 檔。

![import3](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/import3.png)

導入核心後，啟動器應重新啟動。

### 2. 設定 Live2D 模型

若要設定 Live2D 模型，請依照下列步驟操作。

點擊齒輪圖示打開設定。

![model1](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/model1.png)

啟用 Live2D 頭像模型。

![model2](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/model2.png)

按一下 "Select File"。

![model3](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/model3.png)

選擇 `.model3.json` 。預設模型可以在 `\Live2D-dotnet\res\live2d-model\`中找到。
![model4](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/model4.png)

現在模型應該會出現在主視窗中。

![model5](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/model5.png)

### 3. 使用功能

若要使用這些功能，請以滑鼠右鍵按一下模型。應該會彈出一個彈出式選單。

![use1](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/use1.png)

## 模組

- **Live2DCSharpSDK.Framework**：Live2D Cubism 框架
- **Live2DCSharpSDK.App**：Live2D 模型繪製器
- **Live2DDotNet**：主要應用程式
- **Live2DDotNet.Launcher**：主應用程式的啟動器

## 工作原理

工作原理與[Live2DCSharpSDK](https://github.com/chikin030611/Live2DCSharpSDK)相同。請參閱儲存庫中README中的「工作原理」。

### 主要元件

- `Live2DDotNet.UI.Controls.Main.Live2dRender.cs`：負責Avalonia應用程式整個繪製過程。
- `Live2DDotNet.UI.Controls.Main.Live2dControl.axaml`：定義 **Live2dControl** 使用者控制項的UI佈局和元素。
- `Live2DDotNet.UI.Controls.Main.Live2dControl.axaml.cs`：包含 `Live2dControl.axaml` 程式碼後端邏輯，管理交互、繪製和資料綁定。
- `Live2DDotNet.UI.Controls.Main.MainControl.axaml`：主視窗組件。
- `Live2DCSharpSDK.App.LAppDelegate.cs`：處理Live2D模型的詳細管理，包括初始化、繪製和資源管理。 
- `Live2DDotNet.Manager.QnaAudioManager.cs`：管理與Q&A物件相關聯的音頻文件播放。
- `Live2DDotNet.UI.Flyouts.Live2DFlyout`：Live2D 的彈出控制（右鍵選單）。

### 資源

Live2D模型儲存在 `\Live2D-dotnet\res\live2d-model` 。答案音頻檔案儲存在 `\Live2D-dotnet\src\Live2DDotNet\Resource\Audio` 。

##  相關

- [Live2DCSharpSDK](https://github.com/chikin030611/Live2DCSharpSDK)：有功能較少的使用 Live2D 模型的 .NET 應用程式
- [ColorMC](https://github.com/Coloryr/ColorMC/tree/master)：Live2D-dotnet 原始Repo



