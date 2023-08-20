
using Terraria;
using Terraria.ID;
using Terraria.GameInput;
using Terraria.ModLoader;
using System.Collections.Generic;
using ShimmerChest.Furniture;
using System.Linq;
using Terraria.DataStructures;
using Terraria.Audio;

namespace ShimmerChest
{
	// See Common/Systems/KeybindSystem for keybind registration.
	public class KeybindPlayer : ModPlayer
	{

		public override void ProcessTriggers(TriggersSet triggersSet) {
			if (KeybindSystem.QuickStackShimmerChest.JustPressed) {
                
                bool deposited_anything = false;
                bool deposited_something_now = false;

                // Find all Shimmer Chests in a 40x40 Area
                List<ShimmerChestTileEntity> all_chests_list = FindNearbyShimmerChests(Player);

                // Run "Quick Stack" on each one, one after another.
                foreach (var chest_entity in all_chests_list) {
                    Item[] player_items_list = Player.inventory;
                    deposited_something_now = chest_entity.TryDeposit(player_items_list.ToList(), true);

                    if (deposited_something_now) {
                        deposited_anything = true;
                    }
                }

                if (deposited_anything) {
                    SoundEngine.PlaySound(SoundID.Grab);
                } else {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                }

			}
		}




        public static bool TryGetTileEntityAs<T>(int i, int j, out T entity) where T : TileEntity
        {
            Point16 origin = TileUtils.GetTopLeftTileInMultitile(i, j);
            if (TileEntity.ByPosition.TryGetValue(origin, out TileEntity existing) && existing is T existingAsT)
            {
                entity = existingAsT;
                return true;
            }
            entity = null;
            return false;
        }


        public static List<ShimmerChestTileEntity> FindNearbyShimmerChests(Player player) {
            
            List<ShimmerChestTileEntity> list_of_chests = new List<ShimmerChestTileEntity>();

            int startX = (int)(player.position.X / 16f); /* starting X coordinate of the area */
            int startY = (int)(player.position.Y / 16f); /* starting Y coordinate of the area */

            for (int x = startX - 20; x < startX + 20; x++)
            {
                for (int y = startY - 20; y < startY + 20; y++)
                {   
                    // Check if there is a TileEntity at the current location
                    if (TryGetTileEntityAs(x, y, out ShimmerChestTileEntity entity))
                    {
                        // Do something with the TileEntity instance
                        // For example, you can access its properties or call its methods
                        list_of_chests.Add(entity);
                    }
                }
            }

            return list_of_chests;
            
        }








	}
}

