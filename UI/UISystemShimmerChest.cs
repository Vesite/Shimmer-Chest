
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

// This is a UI "System"
// Most of the code will be in "UICanvas"

namespace ShimmerChest.UI {

    public class UISystemShimmerChest : ModSystem
	{

        internal UICanvasShimmerChest canvasShimmerChest;
        private UserInterface _shimmerChestCanvasUserInterface;
        private GameTime _lastUpdateUiGameTime;

        public override void Load()
        {
            _shimmerChestCanvasUserInterface = new UserInterface();

            canvasShimmerChest = new UICanvasShimmerChest(); // My own practice UI canvas
            canvasShimmerChest.Activate();

        }

        
        
        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (_shimmerChestCanvasUserInterface?.CurrentState != null) {
                _shimmerChestCanvasUserInterface.Update(gameTime);
            }
        }
        
        // Adding a custom layer to the vanilla layer list that will call .Draw on your interface if it has a state
		// Setting the InterfaceScaleType to UI for appropriate UI scaling
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {   
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Practice mod vesite thing",
                    delegate
                    {
                        if ( _lastUpdateUiGameTime != null && _shimmerChestCanvasUserInterface?.CurrentState != null) {
                            _shimmerChestCanvasUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

        }

        
    

        internal void ShowMyUI() {
            _shimmerChestCanvasUserInterface?.SetState(canvasShimmerChest);

            // My Update
            canvasShimmerChest.MyUpdateUIState();
        }

        internal void HideMyUI() {
            _shimmerChestCanvasUserInterface?.SetState(null);
        }

        // Returns true if the UI is open (i think)
        internal bool CheckMyUI() {
            if (_shimmerChestCanvasUserInterface?.CurrentState == null) {
                return false;
            } else {
                return true;
            }
        }

    }

}

