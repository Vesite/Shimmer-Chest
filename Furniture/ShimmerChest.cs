
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.Audio;
using Terraria.GameInput;

namespace ShimmerChest.Furniture
{
	public class ShimmerChest : ModTile
	{
		
		public override void SetStaticDefaults()
		{
			
			// Properties
			Main.tileSpelunker[Type] = true;
			Main.tileContainer[Type] = true;
			Main.tileShine2[Type] = true;
			Main.tileShine[Type] = 1200;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileOreFinderPriority[Type] = 500;

			Main.tileSolid[Type] = false; // Can walk through
			Main.tileNoFail[Type] = true; // Makes in unbreakable to explosives

			
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.BasicChest[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.AvoidedByNPCs[Type] = true;
			//TileID.Sets.InteractibleByNPCs[Type] = true;
			TileID.Sets.IsAContainer[Type] = true;
			TileID.Sets.FriendlyFairyCanLureTo[Type] = true;
			
			// ?
			AdjTiles = new int[] { TileID.Containers };

			AddMapEntry(new Color(200, 200, 200), this.GetLocalization("MapEntry0"), MapChestName);

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };

			// This is to make the furniture an actual TileEntity i think
			// MyTileEntity refers to the tile entity mentioned in the previous section
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<ShimmerChestTileEntity>().Hook_AfterPlacement, -1, 0, false);
			// This is required so the hook is actually called.
			TileObjectData.newTile.UsesCustomCanPlace = true;

			//TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
			//TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
			TileObjectData.newTile.AnchorInvalidTiles = new int[] {
				TileID.MagicalIceBlock,
				TileID.Boulder,
				TileID.BouncyBoulder,
				TileID.LifeCrystalBoulder,
				TileID.RollingCactus
			};
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			

			TileObjectData.addTile(Type);

		}


		// Decides what "MapEntry0" variable to use to display on the map
		public override ushort GetMapOption(int i, int j) {
			return 0;
		}

		public override LocalizedText DefaultContainerName(int frameX, int frameY) {
			//int option = frameX / 36;
			//return this.GetLocalization("MapEntry" + option);
			return CreateMapEntryName();
		}

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
			return true;
		}

		// Some functuion for the chest name, changed some stuff here so it is only in english and removed some code.
		public static string MapChestName(string name, int i, int j) {
			return "Shimmer Chest";

			//int left = i;
			//int top = j;
			//Tile tile = Main.tile[i, j];
			//if (tile.TileFrameX % 36 != 0) {
			//	left--;
			//}

			//if (tile.TileFrameY != 0) {
			//	top--;
			//}

			//int chest = Chest.FindChest(left, top);
   //         return "Shimmer Chest"


   //         if (Main.chest[chest].name == "") {
			//	return "Shimmer Chest";
			//}

			//return "Shimmer Chest"// + ": " + Main.chest[chest].name;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			
			// From the tileEntity guide
			ShimmerChestTileEntity shimmerChestTileEntity;
			TileUtils.TryGetTileEntityAs(i, j, out shimmerChestTileEntity);

			if (shimmerChestTileEntity.AnyItemsStored()) {
				
				foreach (Item item_temp in shimmerChestTileEntity.chestInventoryList)
        		{
					Main.LocalPlayer.QuickSpawnItem(new EntitySource_TileEntity(shimmerChestTileEntity), item_temp, item_temp.stack);
				}

			}

			shimmerChestTileEntity.Kill(i, j);

			// We override KillMultiTile to handle additional logic other than the item drop. In this case, unregistering the Chest from the world
			Chest.DestroyChest(i, j);

			// But also close the UI
			ChestCloseStorage(Main.LocalPlayer);

		}

		public override void MouseOver(int i, int j) {

			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;
			if (tile.TileFrameX % 36 != 0) {
				left--;
			}
			if (tile.TileFrameY != 0) {
				top--;
			}

			int chest = Chest.FindChest(left, top);
			player.cursorItemIconID = -1;
			if (chest < 0) {
				player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
			}
			else {
				string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY); // This gets the ContainerName text for the currently selected language
				player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
				if (player.cursorItemIconText == defaultName) {
					player.cursorItemIconID = ModContent.ItemType<Items.ShimmerChest>();

					player.cursorItemIconText = "";
				}
			}

			player.noThrow = 2;
			player.cursorItemIconEnabled = true;

		}

		
		

		// I made this for closing the chest in this script
		internal static void ChestCloseStorage(Player player) {
			
			StoragePlayer modPlayer = player.GetModPlayer<StoragePlayer>();

			// Close Storage
			if (modPlayer.CloseStorage()) {
				// Only play sound if the UI was open
				SoundEngine.PlaySound(SoundID.MenuClose);
			}

			Recipe.FindRecipes(); // I guess change recipies to not use chest contents

		}

		//This code is what the storage-furniture does when we open it.
		internal static void OpenStorageUI(Player player, int i, int j, ShimmerChestTileEntity tile_entity) {

			StoragePlayer modPlayer = player.GetModPlayer<StoragePlayer>();
			Main.mouseRightRelease = false;


			bool hadChestOpen = (player.chest != -1);
			player.chest = -1;
			Main.stackSplit = 600;
			Point16 toOpen2D = new(i, j); // 2D point
			Point16 prevOpen2D = modPlayer.ViewingStorage();
			
			if (prevOpen2D == toOpen2D)
			{
				// Close Storage
				ChestCloseStorage(player);
			}
			else
			{
				//Here we actually tell the player / "modPlayer" to open the storage
				bool hadOtherOpen = prevOpen2D.X >= 0 && prevOpen2D.Y >= 0;
				if (hadOtherOpen)
					modPlayer.CloseStorage();

				modPlayer.currentShimmerChestTileEntity = tile_entity; // This is where i store it
				modPlayer.timeSinceOpen = 0;
				modPlayer.OpenStorage(toOpen2D);


				if (PlayerInput.GrappleAndInteractAreShared)
					PlayerInput.Triggers.JustPressed.Grapple = false;
				Main.recBigList = false;
				SoundEngine.PlaySound(hadChestOpen || hadOtherOpen ? SoundID.MenuTick : SoundID.MenuOpen);
				Recipe.FindRecipes();

			}
		}

		public override void MouseOverFar(int i, int j) {
			MouseOver(i, j);

			Player player = Main.LocalPlayer;
			if (player.cursorItemIconText == "") {
				player.cursorItemIconEnabled = false;
				player.cursorItemIconID = 0;
			}
		}





















		



		













		// This hook goes in your ModTile
        public override bool RightClick(int i, int j)
        {
			
			// ???
			if (Main.tile[i, j].TileFrameX % 36 == 18)
				i--;
			if (Main.tile[i, j].TileFrameY % 36 == 18)
				j--;

            Player player = Main.LocalPlayer;

            // Should your tile entity bring up a UI, this line is useful to prevent item slots from misbehaving
            Main.mouseRightRelease = false;

            // The following four (4) if-blocks are recommended to be used if your multitile opens a UI when right clicked:
            if (player.sign > -1)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = string.Empty;
            }
            if (Main.editChest)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = string.Empty;
            }
            if (player.editedChestName)
            {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
                player.editedChestName = false;
            }
            if (player.talkNPC > -1)
            {
                player.SetTalkNPC(-1);
                Main.npcChatCornerItem = 0;
                Main.npcChatText = string.Empty;
            }


			//This went through only once with one chest? when replacing it it didnt work
            if (TileUtils.TryGetTileEntityAs(i, j, out ShimmerChestTileEntity entity))
            {	
				// Do things to your entity here
				OpenStorageUI(Main.LocalPlayer, i, j, entity);
                
            } else {
				Main.NewText($"Failed the TryGetTileEntityAs Function");
			}

			return true;
        }




	}
}

