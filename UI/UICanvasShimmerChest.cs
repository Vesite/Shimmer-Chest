
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
        public UIText textUI_1;
        public UIHoverImageButton textUI_2;
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
            SetRectangle(playButton, left: ui_width/2 - 40 - 11, top: 10f, width: 26f, height: 26f);
            playButton.OnLeftClick += new MouseEvent(PlayButtonClicked);
            panel.Append(playButton);

            // Adding a "Take All" Button
            Asset<Texture2D> textureArrowUp2 = ModContent.Request<Texture2D>("ShimmerChest/ArrowUp2");
			UIHoverImageButton takeAllButton = new UIHoverImageButton(textureArrowUp2, "Take All");
			SetRectangle(takeAllButton, left: ui_width/2 - 11, top: 10f, width: 26f, height: 26f);
			takeAllButton.OnLeftClick += new MouseEvent(TakeAllButtonClicked);
			panel.Append(takeAllButton);

            //Adding a "Take 1 Stack" Button
            Asset<Texture2D> textureArrowUp = ModContent.Request<Texture2D>("ShimmerChest/ArrowUp");
			UIHoverImageButton takeOneButton = new UIHoverImageButton(textureArrowUp, "Take 1 Stack");
			SetRectangle(takeOneButton, left: ui_width/2 + 40 - 11, top: 10f, width: 26f, height: 26f);
			takeOneButton.OnLeftClick += new MouseEvent(TakeOneButtonClicked);
			panel.Append(takeOneButton);

            // Debug Button
            /*
            Asset<Texture2D> tempDebugTex = ModContent.Request<Texture2D>("ShimmerChest/ArrowUp");
			UIHoverImageButton tempDebug = new UIHoverImageButton(tempDebugTex, "Take 1 Stack");
			SetRectangle(tempDebug, left: ui_width/2 + 120 - 11, top: 10f, width: 26f, height: 26f);
			tempDebug.OnLeftClick += new MouseEvent(DebugClicked);
			panel.Append(tempDebug);
            */

            // Info Hover
			textUI_1 = new UIText("1"); 
			SetRectangle(textUI_1, left: ui_width - 30, top: 10f, width: 26f, height: 26f);
			panel.Append(textUI_1);
            
            Asset<Texture2D> textTexture = ModContent.Request<Texture2D>("ShimmerChest/SmallFrame");
            UIHoverImageButton textUI_2 = new UIHoverImageButton(textTexture, "The amount of normal chests required to store these items");
            SetRectangle(textUI_2, left: ui_width - 30, top: 4f, width: 26f, height: 26f);
            panel.Append(textUI_2);

            
            // Item Text
            itemText = new UIText("Empty"); // I edit this "Empty" value later
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
			
            //This tries to give all the items in the chest, wont give more than the player has space for
            DoWithdraw(false);

		}

        // Prints the data in the chest
        private void DebugClicked(UIMouseEvent evt, UIElement listeningElement) {
            var print_text = "";
            ShimmerChestTileEntity chest_object = GetChestEntity();
            foreach (Item item_temp in chest_object.chestInventoryList) {
                print_text += "|" + item_temp.type + " - " + item_temp.stack + "\n";
            }

            Main.NewText(print_text);
        }

        public void MyUpdateUIState() {
            // My Update
            MyUpdateText();
        }

        public void MyUpdateText() {
        
            ShimmerChestTileEntity chest_object = GetChestEntity();

            if (chest_object.AnyItemsStored()) {
                var temp_id_int = chest_object.chestInventoryList[0].type;
                string itemName = Language.GetTextValue(Lang.GetItemNameValue(temp_id_int));
                itemText.SetText(itemName + " - " + chest_object.ItemCount());

                // Calculate how many "normal chests" we need and update the text
                var chests_required = ((chest_object.chestInventoryList.Count - 1)/40) + 1; 
                textUI_1.SetText(chests_required.ToString());

            } else {
                itemText.SetText("Empty");
                textUI_1.SetText("0");
            }

            
        }

        public override void Draw(SpriteBatch spriteBatch) {
            
            base.Draw(spriteBatch);

            ShimmerChestTileEntity chest_object = GetChestEntity();
            Texture2D itemSprite;
            
            // Draws the sprite of the item in the chest
            if (chest_object.AnyItemsStored()) {

                var type_id = chest_object.chestInventoryList[0].type;
                // This is how i get a sprite from a int-value
                itemSprite = TextureAssets.Item[type_id].Value;
                var _w = itemSprite.Width;
                var _h = itemSprite.Height;
                var _x = start_x + ui_width*0.5f                  - _w/2;
                var _y = start_y + ui_height*0.6f + frame_width/2 - _h/2;
                Vector2 vector = new Vector2(_x, _y);
                spriteBatch.Draw(itemSprite, vector, Color.White);

            }

        }

        // This needs to return the object-variable for the chest we are currently interacting with while this UI is open
        public static ShimmerChestTileEntity GetChestEntity() {
            return StoragePlayer.LocalPlayer.currentShimmerChestTileEntity;
        }





        // Finds out how many items the player has space for and withdraws as many as it can from the chest
        internal static void DoWithdraw(bool takeOneStack) {
            
            ShimmerChestTileEntity chest_object = GetChestEntity();
            
            if (chest_object.AnyItemsStored()) {
                
                // Init
                int item_int = chest_object.chestInventoryList[0].type;
                int item_amount_to_grab = chest_object.ItemCount();
                int stored_item_max_stack = chest_object.chestInventoryList[0].maxStack;
                Player player = Main.LocalPlayer;
                
                //Check how many items the player has space for
                int empty_spaces = 0;
                int items_we_have_space_for = 0;
                //Loop though all 40 relevant inventory slots
                for (int k = 10; k < 50; k++)
			    {
				    Item item = player.inventory[k];

                    if (item.IsAir) {
                        empty_spaces += 1;
                        items_we_have_space_for += stored_item_max_stack;
                    }
                    if (item.type == item_int) {
                        items_we_have_space_for += stored_item_max_stack - item.stack;
                    }
                }

                // Limit the amount we are taking to 1 stack
                if (takeOneStack) {
                    if (items_we_have_space_for > stored_item_max_stack) {
                        items_we_have_space_for = stored_item_max_stack;
                    }
                }

                // Don't grab more items than we have space for
                if (item_amount_to_grab > items_we_have_space_for) {
                    item_amount_to_grab = items_we_have_space_for;
                }

                // Add the items to the player's inventory while removing them from the list at the same time
                while (item_amount_to_grab > stored_item_max_stack) {
                    
                    player.QuickSpawnItem(new EntitySource_OverfullInventory(player), chest_object.chestInventoryList[0], stored_item_max_stack);
                    chest_object.RemoveItems(0, stored_item_max_stack);
                    
                    item_amount_to_grab -= stored_item_max_stack;
                }
                player.QuickSpawnItem(new EntitySource_OverfullInventory(player), chest_object.chestInventoryList[0], item_amount_to_grab);
                chest_object.RemoveItems(0, item_amount_to_grab);
            
            }

        }

        internal static void TryDepositAll()
		{
			Player player = Main.LocalPlayer;
			ShimmerChestTileEntity chest_object = GetChestEntity();

            if (chest_object == null) {
                return;
            }

            // Make a new empty list of items
			var items = new List<Item>();

            // Loop through the 40 relevant inventory slots for the player and add them to the "items" list
			for (int k = 10; k < 50; k++) {
				Item item = player.inventory[k];
				if (MyFilter(item))
					items.Add(item);
			}

            chest_object.TryDeposit(items, false);

		}

        // Returns true if the item is valid to be deposited
        public static bool MyFilter(Item item) {
            
            if (!item.IsAir && !item.favorited) {
                return true;
            }

            return false;
        }

        // Lets us Shift-Click the item into our chest
        private void HandleShiftClick()
        {
            if (Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                // Not sure what this does
                if (Main.LocalPlayer.mouseInterface)
                {   
                    // Current mouse hovering item
                    Item item = Main.mouseItem;

                    if (MyFilter(item)) {
                        
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



