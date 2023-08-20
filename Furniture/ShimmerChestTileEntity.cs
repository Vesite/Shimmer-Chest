
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
         
        // These are field initialised values
        public int storedItemID = 0; // If we are not empty this one will have a value, otherwise it will not be defined?
		public int storedItemAmount = 0;
		public bool isEmpty = true; // If we take out all items some later time i will leave "storedItemID" and set isEmpty to true

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

                //Main.NewText($"Net");
                
                //Replaced with this from "SendTileRange" (error)
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
            tag["isEmpty"] = GetIsEmpty();
			tag["storedItemID"] = GetStoredItemID();
			tag["storedItemAmount"] = GetStoredItemAmount();
        }

        // Load data for items stored
		public override void LoadData(TagCompound tag) {
            SetIsEmpty(tag.GetBool("isEmpty"));
			SetstoredItemID(tag.GetInt("storedItemID"));
			SetstoredItemAmount(tag.GetInt("storedItemAmount"));
		}





        // Im using "Get / Set" functions for these
        public bool GetIsEmpty() {
			return isEmpty;
		}

		public void SetIsEmpty(bool _val) {
            
            /* Thought this could be a way to make the chest unbreakable
            if (_val == true) {
                chest.MinPick = 0;
            } else {
                chest.MinPick = 1000; // When we have items in the chest this makes it unminable?
            }
            */
            
			isEmpty = _val;
		}

		public int GetStoredItemID() {
			return storedItemID;
		}

		public void SetstoredItemID(int _val) {
			storedItemID = _val;
		}

		public int GetStoredItemAmount() {
			return storedItemAmount;
		}

		public void SetstoredItemAmount(int _val) {
			storedItemAmount = _val;
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
                // Only deposit if there already is an item there
                if (!isEmpty && storedItemID == toDeposit.type) {
                    DepositItemFinal(toDeposit);
                    toDeposit.TurnToAir();
                }
            } else {
                // If our shimmer chest is empty or if the stored_item_id is the same as the item we are depositing
                if (isEmpty || storedItemID == toDeposit.type) {
                    DepositItemFinal(toDeposit);
                    toDeposit.TurnToAir();
                }
            }


            

		}

		public void DepositItemFinal(Item toDeposit)
		{

			// Make the stored item type correct
			storedItemID = toDeposit.type;
			// Add the correct amount of items
			storedItemAmount += toDeposit.stack;
			if (toDeposit.stack > 0) {
				isEmpty = false;
			}

		}




    }
}

