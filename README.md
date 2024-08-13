# Live2D-dotnet

This application showcases all functionalities of a Live2D model. This README only has an introduction on how to run and use the app. To see detailed workflow of the application, please see [Live2DCSharpSDK](https://github.com/chikin030611/Live2DCSharpSDK). 

![demo](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/demo.png)

The repository was forked from [ColorMC](https://github.com/Coloryr/ColorMC/tree/master). Most of the original code is removed to keep the repository simple.

## Getting Started

### Prerequisites

Download the zip file of Cubism SDK from the [website](https://www.live2d.com/en/sdk/download/native/). Note that the zip file does not need to be extracted.

### Setup

    # Clone the repository
    git clone https://github.com/chikin030611/Live2D-dotnet.git

    # Initialize, update, and clone all submodules.
    git submodule update --init --recursive

### Running on Terminal

    # Go into the Launcher folder
    cd src/Live2DDotNet.Launcher

    # Run the application
    dotnet run

### Running on Visual Studio

1. Open `Live2D-dotnet.sln` in `\Live2D-dotnet\src\`.
2. Configure Startup Project as **Live2DDotNet.Launcher**.

## How To Use

### 1. Import Cubism Core

To import the Cubism Core, follow the steps below.

Click the gear icon to open up setting.

![import1](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/import1.png)

Click "Import Core".

![import2](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/import2.png)

Select the Cubism SDK zip file.

![import3](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/import3.png)

The launcher should restart after importing the core.

### 2. Setup Live2D Model

To set up a Live2D model, follow the steps below.

Click the gear icon to open up setting.

![model1](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/model1.png)

Enable Live2D Avatar Model.

![model2](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/model2.png)

Click "Select File".

![model3](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/model3.png)

Select `.model3.json`. Default models can be found in `\Live2D-dotnet\res\live2d-model\`.

![model4](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/model4.png)

Now the model should appear in the main window.

![model5](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/model5.png)

### 3. Using the functionalities

To use the functionalities, right-click on the model. A flyout menu should pop up.

![use1](https://github.com/chikin030611/Live2D-dotnet/blob/master/image/use1.png)

## Modules

- **Live2DCSharpSDK.Framework**: Live2D Cubism Framework
- **Live2DCSharpSDK.App**: Live2D model renderer
- **Live2DDotNet**: The main application
- **Live2DDotNet.Launcher**: Launcher of the main application

## How it works

The working principles is same as [Live2DCSharpSDK](https://github.com/chikin030611/Live2DCSharpSDK). Please see the README in the repository.

### Main Componenets

- `Live2DDotNet.UI.Controls.Main.Live2dRender.cs`: Defines the UI layout and elements for the **Live2dControl** user control.
- `Live2DDotNet.UI.Controls.Main.Live2dControl.axaml`: Contains the code-behind logic for the `Live2dControl.axaml`.
- `Live2DDotNet.UI.Controls.Main.Live2dControl.axaml.cs`: Responsible for the overall rendering process within the Avalonia application.
- `Live2DDotNet.UI.Controls.Main.MainControl.axaml`: Main window component.
- `Live2DCSharpSDK.App.LAppDelegate.cs`: Central component for managing Live2D interactions and rendering.
- `Live2DDotNet.Manager.QnaAudioManager.cs`: Manages the playback of audio files associated with Q&A objects.
- `Live2DDotNet.UI.Flyouts.Live2DFlyout`: The flyout control (right-click menu) for Live2D.

### Resources

Live2D models files are stored in `\Live2D-dotnet\res\live2d-model`. Audio files are stored in `\Live2D-dotnet\src\Live2DDotNet\Resource\Audio`.

## Related

- [Live2DCSharpSDK](https://github.com/chikin030611/Live2DCSharpSDK): Live2D models in .NET application with less functionalities
- [ColorMC](https://github.com/Coloryr/ColorMC/tree/master): the original repository of Live2D-dotnet

