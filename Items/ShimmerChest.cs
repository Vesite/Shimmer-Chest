
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShimmerChest.Items
{
	public class ShimmerChest : ModItem
	{
		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Furniture.ShimmerChest>());
			Item.width = 32;
			Item.height = 26;
			Item.value = 150;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.WorkBenches);
			recipe.AddIngredient(ItemID.ShimmerBrick, 8);
			recipe.AddRecipeGroup("IronBar", 2);
			recipe.Register();

		}
	}
}

