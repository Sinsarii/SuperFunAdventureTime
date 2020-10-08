using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;

namespace SuperFunAdventureTime
{
    public partial class SuperFunAdventureTime : Form
    {
        private Player _player;
        public SuperFunAdventureTime()
        {
            InitializeComponent();

            _player = new Player(10,10,20,0,1);

            Location location = new Location(1, "Home", "This is your house.");

            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();
        }

        private void btnNorth_Click(object sender, EventArgs e)
        {

        }

        private void btnWest_Click(object sender, EventArgs e)
        {

        }

        private void btnEast_Click(object sender, EventArgs e)
        {

        }

        private void btnSouth_Click(object sender, EventArgs e)
        {

        }

        private void btnUseWeapon_Click(object sender, EventArgs e)
        {

        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {

        }
        
        private void MoveTo(Location newLocation)
        {
            //Does the location have an required items before entering
            if(newLocation.ItemRequiredToEnter != null)
            {
                //see if the player has the required item in their inventory
                bool playerHasRequiredItem = false;

                foreach(InventoryItem ii in _player.Inventory)
                {
                    if(ii.Details.ID == newLocation.ItemRequiredToEnter.ID)
                    {
                        //We found the required Item
                        playerHasRequiredItem = true;
                        break; //exit our of foreach loop
                    }
                }

                if(!playerHasRequiredItem)
                {
                    //The required item was not found in the players inventory, display message and halt movement
                    rtbMessages.Text += "You must have a" + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                    return;
                }
            }

            //Update the player's current location
            _player.CurrentLocation = newLocation;

            //Show/Hide available movement buttons
            btnNorth.Visible = (newLocation != null);
            btnSouth.Visible = (newLocation != null);
            btnEast.Visible = (newLocation != null);
            btnWest.Visible = (newLocation != null);

            //display current location name and description
            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;

            //completely heal the player
            _player.CurrentHitPoints = _player.MaximumHitPoints;

            //update hit points in UI
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();

            //does the location have a quest?
            if(newLocation.QuestAvailableHere != null)
            {
                //see if the player already has the quest, and if they've completed it
                bool playerAlreadyHasQuest = false;
                bool playerAlreadyCompletedQuest = false;

                foreach(PlayerQuest playerQuest in _player.Quests)
                {
                    if(playerQuest.Details.ID == newLocation.QuestAvailableHere.ID)
                    {
                        playerAlreadyHasQuest = true;

                        if(playerQuest.IsCompleted)
                        {
                            playerAlreadyCompletedQuest = true;
                        }

                    }
                }
                //see if the player already has the quest
                if (playerAlreadyHasQuest)
                {
                    //if the player has not completed the quest yet
                    if(!playerAlreadyCompletedQuest)
                    {
                        //see if the player has all the items needed to complete the quest
                        bool playerHasAllItemsToCompleteQuest = true;

                        foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                        {
                            bool foundItemInPlayersInventory = false;

                            //check each item in the players inventory too see: if the have it and if they have enough of it
                            foreach(InventoryItem ii in _player.Inventory)
                            {
                                //the player has this item in their inventory
                                if(ii.Details.ID == qci.Details.ID)
                                {
                                    foundItemInPlayersInventory = true;

                                    if(ii.Quantity < qci.Quantity)
                                    {
                                        //the player does not have enough of this item to complete the quest
                                        playerHasAllItemsToCompleteQuest = false;

                                        //there is no reason to continue checking for the other quest completion items
                                        break;
                                    }
                                    //we found the item, so don't check the rest of the players inventory
                                    break;
                                }
                            }
                            //if we didn't find the required item, set our variable to stop looking for other items
                            if(!foundItemInPlayersInventory)
                            {
                                //the player does not have this item in their inventory
                                playerHasAllItemsToCompleteQuest = false;

                                //there is no reason to continue checking for the other quest completion items
                                break;
                            }
                        }
                        //the player has all the items required to complete the quest
                        if(playerHasAllItemsToCompleteQuest)
                        {
                            //display message
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You complete the  '" + newLocation.QuestAvailableHere.Name + "'quest." + Environment.NewLine;

                            //remove quest items from inventory
                            foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                            {
                                foreach (InventoryItem ii in _player.Inventory)
                                {
                                    if (ii.Details.ID == qci.Details.ID)
                                    {
                                        //subtract the quantity from the player's inventory that was needed to complete the quest
                                        ii.Quantity -= qci.Quantity;
                                        break;
                                    }
                                }
                            }

                            //give quest rewards!
                            rtbMessages.Text += "You recieve: " + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " experience points." + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + " gold." + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                            rtbMessages.Text += Environment.NewLine;

                            _player.ExperiencePoints += newLocation.QuestAvailableHere.RewardExperiencePoints;
                            _player.Gold += newLocation.QuestAvailableHere.RewardGold;

                            //add the reward item to the player's inventory
                            bool addItemToPlayerInventory = false;

                            foreach(InventoryItem ii in _player.Inventory)
                            {
                                if(ii.Details.ID == newLocation.QuestAvailableHere.RewardItem.ID)
                                {
                                    //the have the item in their inventory, so increase the quantity by 1
                                    ii.Quantity++;

                                    addItemToPlayerInventory = true;

                                    break;
                                }
                            }

                            //the didn't have the item, so add it to their inventory with a quantity of 1
                            if(!addItemToPlayerInventory)
                            {
                                _player.Inventory.Add(new InventoryItem(newLocation.QuestAvailableHere.RewardItem, 1));
                            }

                            //mark the quest as completed
                            //find the quest in the players quest list
                            foreach(PlayerQuest pq  in _player.Quests)
                            {
                                if (pq.Details.ID == newLocation.QuestAvailableHere.ID)
                                {
                                    //mark it as completed
                                    pq.IsCompleted = true;

                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //the player does not already have the quest

                    //display the messages

                }
            }
        }
    }
 }
