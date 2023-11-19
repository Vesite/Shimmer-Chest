
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ShimmerChest.UI;
using ShimmerChest.Furniture;

namespace ShimmerChest
{
	public class StoragePlayer : ModPlayer
	{
		
		public ShimmerChestTileEntity currentShimmerChestTileEntity;

        public static StoragePlayer LocalPlayer => Main.LocalPlayer.GetModPlayer<StoragePlayer>();

        // Point16 is a 2D point in the world, These are tile coordiantes, they are whole numbers.
        private Point16 storageAccess2D = Point16.NegativeOne;
        
        public int timeSinceOpen = 1;
        public Point16 Storage { get; private set; }

        public void OpenStorage(Point16 point)
		{
            // ?
			storageAccess2D = point;

            // Open inventory like chests do
			Main.playerInventory = true;

            ModContent.GetInstance<UISystemShimmerChest>().ShowMyUI();

		}








        // ?
        public Point16 ViewingStorage() => storageAccess2D;

        public bool StorageEnvironment() {
			if (storageAccess2D.X < 0 || storageAccess2D.Y < 0)
				return false;
			Tile tile = Main.tile[storageAccess2D.X, storageAccess2D.Y];
			return tile.HasTile;
		}

        public static bool IsStorageEnvironment() => StoragePlayer.LocalPlayer.StorageEnvironment();


		// Returns true if we need to close the storage
        public bool CloseStorage() {
			
			var is_ui_open = ModContent.GetInstance<UISystemShimmerChest>().CheckMyUI();

            if (storageAccess2D != Point16.NegativeOne) {
                storageAccess2D = Point16.NegativeOne;
            }
            if (Storage != Point16.NegativeOne) {
                Storage = Point16.NegativeOne;
            }
            ModContent.GetInstance<UISystemShimmerChest>().HideMyUI();

			return is_ui_open;
        }

        public override void UpdateDead()
		{
			if (Player.whoAmI == Main.myPlayer)
				CloseStorage();
		}

        // Copied most from magic storage
        // This closes the chest when we walk too far away, when we click ESC, and when we die
        public override void ResetEffects()
		{
			if (Player.whoAmI != Main.myPlayer)
				return;

			if (timeSinceOpen < 1 && !Main.autoPause && storageAccess2D.X >= 0 && storageAccess2D.Y >= 0)
			{
				Player.SetTalkNPC(-1);
				Main.playerInventory = true;
				timeSinceOpen++;
			}

			// Does this close the storage if we are far away?
			if (storageAccess2D.X >= 0 && storageAccess2D.Y >= 0 && (Player.chest != -1 || !Main.playerInventory || Player.sign > -1 || Player.talkNPC > -1))
			{
				CloseStorage();
				Recipe.FindRecipes();
			}
			else if (storageAccess2D.X >= 0 && storageAccess2D.Y >= 0)
			{
				int playerX = (int)(Player.Center.X / 16f);
				int playerY = (int)(Player.Center.Y / 16f);
				var modTile = TileLoader.GetTile(Main.tile[storageAccess2D.X, storageAccess2D.Y].TileType);

				if  (playerX < storageAccess2D.X - Player.lastTileRangeX     ||
					 playerX > storageAccess2D.X + Player.lastTileRangeX + 1 ||
					 playerY < storageAccess2D.Y - Player.lastTileRangeY     ||
					 playerY > storageAccess2D.Y + Player.lastTileRangeY + 1)
				{
					SoundEngine.PlaySound(SoundID.MenuClose);
					CloseStorage();
					Recipe.FindRecipes();
				}
				else if (modTile is not ModTile)
				{
					SoundEngine.PlaySound(SoundID.MenuClose);
					CloseStorage();
					Recipe.FindRecipes();
				}
			}
        }





    }
}



