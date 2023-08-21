
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace ShimmerChest.Furniture
{
	public class ShimmerChestTileEntity : ModTileEntity
	{
        
        // The list that stores all the data in the chest
        public List<Item> chestInventoryList = new List<Item>();

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<ShimmerChest>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {   
            // This gave some errors
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // Sync the entire multitile's area.  Modify "width" and "height" to the size of your multitile in tiles
                int width = 2;
                int height = 2;
                
                // This was "SendTileRange" but i gave an error?
                NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

                // Sync the placement of the tile entity with other clients
                // The "type" parameter refers to the tile type which placed the tile entity, so "Type" (the type of the tile entity) needs to be used here instead
                NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
            }
            
            // ModTileEntity.Place() handles checking if the entity can be placed, then places it for you
            // Set "tileOrigin" to the same value you set TileObjectData.newTile.Origin to in the ModTile
            Point16 tileOrigin = new Point16(0, 1);
            int placedEntity = Place(i - tileOrigin.X, j - tileOrigin.Y);
            return placedEntity;

        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
        }

        // Save data for items stored
        public override void SaveData(TagCompound tag) { 
            tag["chestInventoryList"] = chestInventoryList;
        }

        // Load data for items stored
		public override void LoadData(TagCompound tag) {
            chestInventoryList = (List<Item>)tag.GetList<Item>("chestInventoryList");
		}


        public bool AnyItemsStored() {
            if (chestInventoryList.Count > 0) {
                return true;
            } else {
                return false;
            }
		}

        public int ItemCount() {
            var counter = 0;
            foreach (Item item_temp in chestInventoryList) {
                counter += item_temp.stack;
            }
            return counter;
        }


        // Tries to deposit the item functions
        public void TryDeposit(Item item, bool quick_stack)
		{
			DepositItem(item, quick_stack);
		}

        // Tries to deposit all items in the list of items
        // Return true of our list of items changed because of depositing
        public bool TryDeposit(List<Item> items, bool quick_stack)
		{

            bool changed = false;
			
            foreach (Item item in items)
            {
                int oldStack = item.stack;
                DepositItem(item, quick_stack);
                if (oldStack != item.stack)
                    changed = true;
            }

			return changed;
        }

        public void DepositItem(Item toDeposit, bool quick_stack)
		{   

            if (toDeposit.IsAir)
                return;
            
            // Here we deposit the item
            if (quick_stack) {

                // Only deposit if there already is an item there with correct type
                if (AnyItemsStored() && chestInventoryList[0].type == toDeposit.type) {
                    DepositItemFinal(toDeposit);
                }

            } else {
                // Only deposit if the chest is empty or if the type is correct
                if (!AnyItemsStored() || chestInventoryList[0].type == toDeposit.type) {
                    DepositItemFinal(toDeposit);
                }
            }

		}

        public void DepositItemFinal(Item toDeposit)
		{
            var item_shallow_clone = (Item)toDeposit.Clone();
            chestInventoryList.Add(item_shallow_clone);
            chestInventoryList = UpdateAndCombineStacksNew();
            toDeposit.TurnToAir();
		}

        // This will update the "chestInventoryList" so that all stacks that can combine will combine
        // Making the list as short as possible O(2n)
        public List<Item> UpdateAndCombineStacksNew() {

            if (AnyItemsStored() && chestInventoryList[0].maxStack > 1) {
                
                var stack_limit = chestInventoryList[0].maxStack;
                var type_id = chestInventoryList[0].type;
                List<Item> newList = new List<Item>();

                var total_items_stored = 0;
                foreach (Item item in chestInventoryList) {
                    total_items_stored += item.stack;
                }

                while (total_items_stored >= stack_limit) {

                    Item newItem = new Item();
                    newItem.SetDefaults(type_id);
                    newItem.stack = stack_limit;
                    newList.Add(newItem);

                    total_items_stored -= stack_limit;
                }
                if (total_items_stored > 0) {

                    Item newItem = new Item();
                    newItem.SetDefaults(type_id);
                    newItem.stack = total_items_stored;
                    newList.Add(newItem);

                }

                return newList;

            } else {
                return chestInventoryList;
            }
        }





        // Will remove the "_stack" amount of items from the item at index "_index"
        public void RemoveItems(int _index, int _stack) {
            
            if (_stack >= chestInventoryList[_index].stack) {
                chestInventoryList.RemoveAt(_index);
            } else {
                chestInventoryList[_index].stack -= _stack;
            }

            UpdateAndCombineStacksNew();
            
        }



    }
}

