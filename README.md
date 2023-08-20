# ShimmerChest
 Terraria 1.4.4 Mod. Shimmer Chest can store an infinite amount of a single item type


The use cases are for late game worlds where you want to collect/hoard a bunch of items  
For example with Shimmer Chests its possible to store all items from a pumpkin moon farm or a dungeon farm by using only a few chests  
I wanted to make it mostly for personal use in late game vanilla worlds  
I posted this concept as a [suggestion on the Terraria forums](https://forums.terraria.org/index.php?threads/infinite-chests-for-single-items.115468/)  
This mod currently very janky as it is the first mod I have made and I dont know how to fix most of the issues listed below.  



- [ ] Does not work in multiplayer. Have not learned how to code for multiplayer yet.
- [ ] I think i should save the item as a list (array?) of actuall item objects instead of int values?
- [ ] Make the chest unbrekable when there are items inside. Don't know how to make it work. "KillMultiTile" is ran after the tile is destroyed, I need to check before it is destroyed. Blocks below the chest are unbreakable as intended (I don't think it works to set Modtile.MinPick, it would change all shimmer chests).
- [ ] Bug, breaking chest items spawn on the player insted of on the chest.
- [ ] Bug, the UI Stays if we dont close it and relog the world, can crash.
- [ ] Make the buttons disable using items when clicking them. 
- [ ] Ideally non-stackable items would keep their modifiers when taken out if the chest, I think it would work if i store data as a list of Item-objects.
- [ ] UI would change if we added any item with a modifier, so that it would look like a normal chest / look like magic storage UI with a scroll-bar. This would allow us to grab the modifier we want. Or it could be 1 slot for each modifier (would be neat for completionist?, collect one of each modifier for the stored item). 



- [X] The text of the UI is wrong / its global stored.
- [X] Data does not get saved on relogs.
- [X] Ability to shift click items into the chest.
- [X] Ability to quick-stack items into the chest (Q), seems this is way too hard to do properly, i think i will make my own hotkey instead (Q).
- [X] Does not work to open chest most of the time.
- [X] There is a sound of UI closing whenever i break a chest.
- [X] Make the chest display the item sprite when it is stored.
- [X] Shift Clicking items into the chest is buggy, sometimes it picks up the item instead.
- [X] I made the chest drop all items inside when broken. (annoying solution, have to split stacks to 9999, and can in some situations go above item limit and lose items).
