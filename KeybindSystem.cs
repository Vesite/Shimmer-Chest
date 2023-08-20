
using Terraria.ModLoader;

namespace ShimmerChest
{
	// Acts as a container for keybinds registered by this mod.
	// See Common/Players/ExampleKeybindPlayer for usage.
	public class KeybindSystem : ModSystem
	{
		public static ModKeybind QuickStackShimmerChest { get; private set; }

		public override void Load() {
			// Registers a new keybind
			// We localize keybinds by adding a Mods.{ModName}.Keybind.{KeybindName} entry to our localization files. The actual text displayed to english users is in en-US.hjson
			QuickStackShimmerChest = KeybindLoader.RegisterKeybind(Mod, "QuickStackShimmerChest", "Q");
		}

		// Please see ExampleMod.cs' Unload() method for a detailed explanation of the unloading process.
		public override void Unload() {
			// Not required if your AssemblyLoadContext is unloading properly, but nulling out static fields can help you figure out what's keeping it loaded.
			QuickStackShimmerChest = null;
		}
	}
}
