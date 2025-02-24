# Death's Door Archipelago Randomizer

This is a work in progress Archipelago randomizer mod for Death's Door.

> [!IMPORTANT]
> This is in very early alpha stage and is not yet playable.

## Contributing

If you're interested in helping out, join the discussion in the [Archipelago Discord server](https://discord.com/invite/8Z65BR2).
We talk about the development of this in the Death's Door thread in the `future-game-design` forum!

### Building
1. Clone this repository.
2. Navigate to the project's root directory in a terminal and run `dotnet restore` to install dependencies.
3. Run `dotnet build` to build the project.

### Automatically Copy Build Files to Plugin Directory
1. Add a `config.txt` to the project's root directory.
2. In the `config.txt`, add the following line and replace the value with the path to where you want this plugin to be stored:
    ```plaintext
    PLUGIN_PATH=Path\To\Your\Plugin\Path\For\This\Mod
    ```
    Note: Do not add quotes around the path, the script handles spaces in path automatically.
3. Now, when you build, it will automatically copy the build files to the path the `PLUGIN_PATH`.

## Acknowledgements

Thanks to [dpinela](https://github.com/dpinela) for their work on the
[Multiworld Randomizer](https://github.com/dpinela/DeathsDoor.Randomizer)
(of which I'm using to figure out how to make this mod) and for the dependencies this mod uses:
[Alternative Game Modes](https://github.com/dpinela/DeathsDoor.AlternativeGameModes) and
[Item Changer](https://github.com/dpinela/DeathsDoor.ItemChanger)!

Thanks to [lunix33](https://github.com/lunix33) for starting this project with their work on the
[APWorld](https://github.com/lunix33/Archipelago_DeathsDoor/tree/deaths-door/worlds/deaths_door) side of things!