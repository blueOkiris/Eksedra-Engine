# Εξέδρα Engine

## Description

A 2D game engine written in C# for use in my games that I'm releasing to the public. (Runs in dotnet core)

It is inspired by the style of game development shown in Gamemaker 7, 8, and 8.1, but is only code, not a GUI.

<img src="https://github.com/blueOkiris/Eksedra-Engine/blob/master/docs/example-image.png" width="640" />

## Run

### Windows

To run the test game, run test.exe

To build the test game from source, run `dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true`. This will create an executable in ".\\test\\bin\\Release\\netcoreapp3.1\\win-x64\\publish". Simply copy over the old test.exe, or run it from that folder

To build the library, open a terminal in the EksedraEngine folder and run `dotnet build`

## Credits

For fun I use Link from the Legend of Zelda Minish Cap. That sprite as well as the character itself are owned by Nintendo

I want to make clear that I DO NOT use these sprites in a professional capacity, instead using them for the sake of example

Same thing for overworld audio which comes from the game the Legend of Zelda: Link's Awakening

Again, same thing for the jump sound, it's Mario's
