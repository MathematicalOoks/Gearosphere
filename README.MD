# Readme

Hello and welcome to my project! This project is a 3D simulation Unity game, named Gearosphere.

## Overview
In this game you can drive 3 amazing cars on a free roam map. You can also launch a SpaceX falcon 9 rocket and watch the rocket head for space while its rocket boosters land themselves back on Earth.

## Gaming Experience
### Playing Standalone
The game can be downloaded [here](https://1drv.ms/u/s!ApRqJhdoSlGBjY1dmYGa8yHyMfZdYQ?e=EFyhao "here") as a standalone packaged application. To run the game, unzip in a folder then launch Gearosphere.exe in the folder where the zip was uncompressed. Then enjoy!

If you notice that the FPS of the game is too small, you can modify the resolution using the UI from the settings icon in game, which will reduce the graphics for improved performance and FPS.
By default (unless running on a gaming computer) when launching the rocket, the resolution should be set to LOW for the game to function as intended.
### Watching Sample Gameplay
If the application is inaccessible for various reasons, I have provided a video showcasing the main functionalities [here](https://www.youtube.com/watch?v=-ngDfMpZ3Sg "here")

### Playing from within Unity
Or if you wish to run the game in the Unity scene, you can download the Unity Hub (if not installed already) and install the 2021.3f version of Unity. Then clone this GitHub repo into the location of the newly created project and open the SampleScene in the scenes folder of the project to load the game scene, then run it.

## Controls 
When the Unity game is loaded, there will be a settings icon at the top right corner, which when clicked will display a UI for the controls of the car and rocket, as well as buttons to set the resolution. 
#### Default Controls
##### Car Controls
- A - Turn Left
- D - Turn Right
- S - Move Backward
- W - Move Forward
- V - Change view
- R - Reset
- F - Flip Car
- P - Park car
- Keys 1,2,3 - Change car

##### Rocket Controls
- L - Launch rocket
- V - Change view
- E - Reset rocket

## Project Structure
The project is essentially composed of scripts and resources.
### Scripts
Scripts are the code which brings together all the resources in the game, organises the gameplay, calculates the game physics and takes care of the user input.
In this repo, they are located in Assets -> Scripts.
All scripts I created are written in C#, which is the language used by the Unity Engine. You may notice other assets and prefabs in the scripts folder, which are the outcome of the asset import activity. These are additional files accompanying the various resources and are only located in the folder to be easily accessed by the main scripts.

The main gameplay scripts I created are:

    CarSpawner.cs
    CarController.cs
    AntiRollBar.cs
    Rocket.cs
    RocketBooster.cs
    CameraFollow.cs
    StateManager.cs
    OptionsManager.cs


### Resources
#### Credits

For this project, I have imported assets from users online to use in my game, which I will give appropriate credit to in the section below (Imported assets that are not mentioned below are free assets imported from the Unity asset store and are viewable in the GitHub repo).

##### Rocket

User : https://sketchfab.com/artemycz

##### Rocket Launchpad

Link : https://www.cgtrader.com/free-3d-models/space/spaceship/rocket-launch-pad

##### House

User : https://sketchfab.com/gulo.arch

##### Cars

User : https://sketchfab.com/samsidparty

##### Music
All music in this game is original and was created by me on GarageBand.
