
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ShimmerChest.Furniture;
using System.Linq;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Input;
using Terraria.GameContent;

// This is a "Canvas"/UIState - UI Object 

namespace ShimmerChest.UI
{
    class UICanvasShimmerChest : UIState
    {
        public UIPanel panel;
        public int numberOfItems = 0;
        public UIText itemText;
        public UIHoverImageButton itemSlot;

        bool canItemClickAway = true; // So that shift-click event happens once 

        public float frame_width = 50;
        public float ui_width = 350;
        public float ui_height = 160;
        public float start_x = 200;
        public float start_y = 340;

        public override void OnInitialize() {
            
            

            // Make Background Panel
            UIPanel panel = new UIPanel();
            panel.SetPadding(0);
            SetRectangle(panel, left: start_x, top: start_y, width: ui_width, height: ui_height);
            panel.BackgroundColor = new Color(73, 94, 171);
 
            // Adding a "Deposit All" Button
            Asset<Texture2D> textureArrowDown = ModContent.Request<Texture2D>("ShimmerChest/ArrowDown");
            UIHoverImageButton playButton = new UIHoverImageButton(textureArrowDown, "Deposit All");
            SetRectangle(playButton, left: ui_width/2 - 40 - 26, top: 10f, width: 26f, height: 26f); // Size of the button
            playButton.OnLeftClick += new MouseEvent(PlayButtonClicked);
            panel.Append(playButton);

            // Adding a "Take All" Button
            Asset<Texture2D> textureArrowUp2 = ModContent.Request<Texture2D>("ShimmerChest/ArrowUp2");
			UIHoverImageButton takeAllButton = new UIHoverImageButton(textureArrowUp2, "Take All"); // Localized text for "Close"
			SetRectangle(takeAllButton, left: ui_width/2 - 26, top: 10f, width: 26f, height: 26f);
			takeAllButton.OnLeftClick += new MouseEvent(TakeAllButtonClicked);
			panel.Append(takeAllButton);

            //Adding a "Take 1 Stack" Button
            Asset<Texture2D> textureArrowUp = ModContent.Request<Texture2D>("ShimmerChest/ArrowUp");
			UIHoverImageButton takeOneButton = new UIHoverImageButton(textureArrowUp, "Take 1 Stack"); // Localized text for "Close"
			SetRectangle(takeOneButton, left: ui_width/2 + 40 - 26, top: 10f, width: 26f, height: 26f);
			takeOneButton.OnLeftClick += new MouseEvent(TakeOneButtonClicked);
			panel.Append(takeOneButton);

            


            // Item Text
            itemText = new UIText("Empty"); // This UI text information stays all the time?
            itemText.HAlign = 0.5f;
            itemText.Top.Set(ui_height*0.35f, 0f);
            panel.Append(itemText);

            //Then draw a empty square to put an item into
            UIPanel itemFrame = new UIPanel();
            
            Asset<Texture2D> itemInFrame = ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonPlay");
            SetRectangle(itemFrame, left: ui_width*0.5f - frame_width/2, top: ui_height*0.6f, width: frame_width, height: frame_width);
            panel.Append(itemFrame);

            Append(panel);

        }


        public override void Update(GameTime gameTime)
        {
            HandleShiftClick();
            MyUpdateUIState();
        }

        private void SetRectangle(UIElement uiElement, float left, float top, float width, float height) {
			uiElement.Left.Set(left, 0f);
			uiElement.Top.Set(top, 0f);
			uiElement.Width.Set(width, 0f);
			uiElement.Height.Set(height, 0f);
		}


        private void PlayButtonClicked(UIMouseEvent evt, UIElement listeningElement) {

			SoundEngine.PlaySound(SoundID.Shimmer2);

            // Deposit all items
            TryDepositAll();

		}

        private void TakeOneButtonClicked(UIMouseEvent evt, UIElement listeningElement) {

			SoundEngine.PlaySound(SoundID.ShimmerWeak2);
			
            //Tries to take 1 stack
            DoWithdraw(true);

		}

        private void TakeAllButtonClicked(UIMouseEvent evt, UIElement listeningElement) {

			SoundEngine.PlaySound(SoundID.Shimmer1);
			
            //This tries to give all the items in the chest to the player as long as the player has space for them
            DoWithdraw(false);

		}

        public void MyUpdateUIState() {
            // My Update
            MyUpdateText();
            //MyUpdateSprites(spriteBatch);
        }

        public void MyUpdateText() {
        
            ShimmerChestTileEntity chest_object = GetChestEntity();

            if (chest_object.GetIsEmpty()) {
                itemText.SetText("Empty");
            } else {
                var temp_id_int = chest_object.GetStoredItemID();
                string itemName = Language.GetTextValue(Lang.GetItemNameValue(temp_id_int));
                itemText.SetText(itemName + " - " + chest_object.GetStoredItemAmount());
            }
            
        }

        // Testing this
        public override void Draw(SpriteBatch spriteBatch) {
            
            base.Draw(spriteBatch);

            ShimmerChestTileEntity chest_object = GetChestEntity();
            //Rectangle item_frame;
            Texture2D itemSprite;
            
            // This is how i get a sprite from a int i think
            itemSprite = TextureAssets.Item[chest_object.GetStoredItemID()].Value;
            var _w = itemSprite.Width;
            var _h = itemSprite.Height;
            var _x = start_x + ui_width*0.5f                  - _w/2;
            var _y = start_y + ui_height*0.6f + frame_width/2 - _h/2;
            Vector2 vector = new Vector2(_x, _y);
            spriteBatch.Draw(itemSprite, vector, Color.White);

        }

        // This needs to return the object-variable for the chest we are currently interacting with while this UI is open
        public static ShimmerChestTileEntity GetChestEntity() {
            return StoragePlayer.LocalPlayer.currentShimmerChestTileEntity;
        }














        internal static void DoWithdraw(bool takeOneStack) {
            
            ShimmerChestTileEntity chest_object = GetChestEntity();
            
            if (!chest_object.GetIsEmpty()) {
                
                int item_int = chest_object.GetStoredItemID();
                int item_amount = chest_object.GetStoredItemAmount();
                Player player = Main.LocalPlayer;

                // Create a new instance of the item to add
                Item newItem = new Item();
                newItem.SetDefaults(item_int);
                int stored_item_max_stack = newItem.maxStack;

                //Check how many items the player has space for
                int empty_spaces = 0;
                int items_we_have_space_for = 0;
                //Loop though all 40 relevant inventory slots
                for (int k = 10; k < 50; k++)
			    {
				    Item item = player.inventory[k];

                    // Assuming the item we are dealing with is stackable to 9999
                    if (item.IsAir) {
                        empty_spaces += 1;
                        items_we_have_space_for += stored_item_max_stack;
                    }
                    if (item.type == item_int) {
                        items_we_have_space_for += stored_item_max_stack - item.stack;
                    }
                }

                // Print the remaining space
                //Main.NewText($"items we have space for: {items_we_have_space_for}");
                //Main.NewText($"empty_spaces: {empty_spaces}");

                // Limit the amount we are taking to 1 stack
                if (takeOneStack) {
                    if (items_we_have_space_for > stored_item_max_stack) {
                        items_we_have_space_for = stored_item_max_stack;
                    }
                }

                // Don't grab more items than we have space for
                int items_left_in_chest = 0;
                if (item_amount > items_we_have_space_for) {
                    items_left_in_chest = item_amount - items_we_have_space_for;

                    // This is now how many we will grab
                    item_amount = items_we_have_space_for;

                }

                

                // Add the item to the player's inventory
                // Only spawn these in the max stack size i think is the solution
                while (item_amount > stored_item_max_stack) {
                    player.QuickSpawnItem(new EntitySource_OverfullInventory(player), newItem, stored_item_max_stack);
                    item_amount -= stored_item_max_stack;
                }
                player.QuickSpawnItem(new EntitySource_OverfullInventory(player), newItem, item_amount);

                //Update chest variables
                if (items_left_in_chest > 0) {
                    chest_object.SetstoredItemAmount(items_left_in_chest);
                    chest_object.SetIsEmpty(false);
                } else {
                    chest_object.SetstoredItemID(0);
                    chest_object.SetstoredItemAmount(0);
                    chest_object.SetIsEmpty(true);
                }
                
            }

        }

        internal static void TryDepositAll()
		{
			Player player = Main.LocalPlayer;
			ShimmerChestTileEntity chest_object = GetChestEntity();

            if (chest_object == null)
            {
                return;
            }

            // Make a new empty list of items
			var items = new List<Item>();

            // Loop through the 40 relevant inventory slots for the player and add them to the "items" list
			for (int k = 10; k < 50; k++)
			{
				Item item = player.inventory[k];
				if (MyFilter(item))
					items.Add(item);
			}

            // Make a new array of item types instead of item objects?
			int[] types = items.Select(static i => i.type).ToArray();

            chest_object.TryDeposit(items, false);

		}

        // Returns true if the item is valid to be deposited
        public static bool MyFilter(Item item) {
            
            if (!item.IsAir && !item.favorited) {
                return true;
            }

            return false;
        }




        // Lets us Shift-Click the item into our chest!
        private void HandleShiftClick()
        {
            if (Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                // Not sure what this does
                if (Main.LocalPlayer.mouseInterface)
                {   
                    // Current mouse hovering item
                    Item item = Main.mouseItem;

                    if (!item.IsAir) {
                        
                        if (Main.mouseLeft && canItemClickAway) {
                            canItemClickAway = false;

                            ShimmerChestTileEntity chest_object = GetChestEntity();
                            chest_object.TryDeposit(item, false);

                        }
                        
                    }

                    if (!Main.mouseLeft) {
                        canItemClickAway = true;
                    }
                }
            }
        }
    }

}



