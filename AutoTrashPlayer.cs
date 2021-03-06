﻿using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AutoTrash
{
	internal class AutoTrashPlayer : ModPlayer
	{
		public bool AutoTrashEnabled = false;
		public List<Item> AutoTrashItems;
		public Item LastAutoTrashItem;

		public override void Initialize()
		{
			AutoTrashItems = new List<Item>();
			LastAutoTrashItem = new Item();
			LastAutoTrashItem.SetDefaults(0, true);
			AutoTrashEnabled = false;
		}

		public override TagCompound Save()
		{
			return new TagCompound
			{
				["AutoTrashItems"] = AutoTrashItems.Select(ItemIO.Save).ToList(),
				["AutoTrashEnabled"] = AutoTrashEnabled
			};
		}

		public override void Load(TagCompound tag)
		{
			AutoTrashItems = tag.GetList<TagCompound>("AutoTrashItems").Select(ItemIO.Load).ToList();
			AutoTrashEnabled = tag.GetBool("AutoTrashEnabled");

			AutoTrash.instance.autoTrashListUI.UpdateNeeded();
		}

		internal static bool IsModItem(Item item)
		{
			return item.type >= ItemID.Count;
		}

		public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
		{
			if (context == Terraria.UI.ItemSlot.Context.InventoryItem || context == Terraria.UI.ItemSlot.Context.InventoryCoin || context == Terraria.UI.ItemSlot.Context.InventoryAmmo)
			{
				if (Main.keyState.IsKeyDown(Keys.LeftControl) || Main.keyState.IsKeyDown(Keys.RightControl))
				{
					var autoTrashPlayer = Main.LocalPlayer.GetModPlayer<AutoTrashPlayer>();
					if (autoTrashPlayer.AutoTrashEnabled && !autoTrashPlayer.AutoTrashItems.Any(x => x.type == inventory[slot].type))
					{
						Main.PlaySound(7, -1, -1, 1, 1f, 0f);

						Item newItem = new Item();
						newItem.SetDefaults(inventory[slot].type);
						autoTrashPlayer.AutoTrashItems.Add(newItem);

						autoTrashPlayer.LastAutoTrashItem = inventory[slot].Clone();

						inventory[slot].SetDefaults();

						AutoTrash.instance.autoTrashListUI.UpdateNeeded();
						return true;
					}
				}
			}
			return false; // do default behavior.
		}
	}
}
