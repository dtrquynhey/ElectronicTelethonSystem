using ETS_LIBRARY;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ListView = System.Windows.Forms.ListView;
using TextBox = System.Windows.Forms.TextBox;


namespace ETS_SYSTEM
{
    public partial class SystemForm : Form
    {
        Manager manager = new Manager();
        Manager.Message msg;
        LoginForm loginForm = new LoginForm();

        public string caption = "ETS Electronic Telethon System";
        private MessageBoxButtons buttonOk = MessageBoxButtons.OK;
        private MessageBoxButtons buttonYN = MessageBoxButtons.YesNo;
        private MessageBoxIcon iconWarning = MessageBoxIcon.Warning;
        private MessageBoxIcon iconQuestion = MessageBoxIcon.Question;
        private MessageBoxIcon iconInfo = MessageBoxIcon.Information;
        private MessageBoxIcon iconError = MessageBoxIcon.Error;

        public SystemForm()
        {
            InitializeComponent();
        }


        /// <summary>
        /// USER-FRIENDLY PURPOSE
        /// </summary>
        //to get username from LoginForm and display in SystemForm
        string username;
        public SystemForm(string username)
        {
            InitializeComponent();
            this.username = username;
        }

        private void SystemForm_Load(object sender, EventArgs e)
        {
            manager.readAllRecords();
            labelTab.Text = "SPONSOR";

            labelUsername.Text += username;
            listViewSponsor.BringToFront();

            buttonSaveSponsor.Enabled = false;
            buttonSavePrize.Enabled = false;
            buttonSaveDonor.Enabled = false;
            buttonSaveDonation.Enabled = false;
            buttonDeleteDonation.Enabled = false;
            buttonDeleteDonor.Enabled = false;
            buttonDeleteSponsor.Enabled = false;
            buttonDeletePrize.Enabled = false;
            comboBoxSearchOption2.Enabled = false;
            comboBoxSearch.Enabled = false;
        }

        //fetch sponsorID, donorID into comboBox when adding prize, donation
        private void SystemForm_Activated(object sender, EventArgs e)
        {
            comboBoxSponsorIDofPrize.Items.Clear();
            comboBoxDonorIDofDonation.Items.Clear();
            textBoxPrizeIDofDonation.Clear();
            List<string> listSponsorID = manager.fetchAnyID("sponsor");
            if (listSponsorID.Count == 0)
            {
                comboBoxSponsorIDofPrize.Text = "-- No options --";
                comboBoxSponsorIDofPrize.Enabled = false;
            }
            else
            {
                comboBoxSponsorIDofPrize.Enabled = true;
                foreach (var itemS in listSponsorID)
                {
                    comboBoxSponsorIDofPrize.Items.Add(itemS);
                }
            }
            List<string> listDonorID = manager.fetchAnyID("donor");
            if (listDonorID.Count == 0)
            {
                comboBoxDonorIDofDonation.Text = "-- No options --";
                comboBoxDonorIDofDonation.Enabled = false;
            }
            else
            {
                comboBoxDonorIDofDonation.Enabled = true;
                foreach (var itemDr in listDonorID)
                {
                    comboBoxDonorIDofDonation.Items.Add(itemDr);
                }
            }
            countItemOfEachEntity();
        }

        //Bring active tab to front
        private void tabControlSponPrize_Click(object sender, EventArgs e)
        {
            TabPage tabPage = tabControlSponPrize.SelectedTab;
            if (tabPage == tabPageSponsor)
            {
                labelTab.Text = "SPONSOR";
                listViewSponsor.BringToFront();
            }
            else if (tabPage == tabPagePrize)
            {
                labelTab.Text = "PRIZE";
                listViewPrize.BringToFront();
            }
        }

        private void tabControlDonorDon_Click(object sender, EventArgs e)
        {
            TabPage tabPage = tabControlDonorDon.SelectedTab;
            if (tabPage == tabPageDonor)
            {
                labelTab.Text = "DONOR";
                listViewDonor.BringToFront();
            }
            else if (tabPage == tabPageDonation)
            {
                labelTab.Text = "DONATION";
                listViewDonation.BringToFront();
            }
        }




        /// <summary>
        /// CLOSE BUTTON
        /// </summary>
        private void buttonLogOut_Click(object sender, EventArgs e)
        {
            string str = "";
            if (tabPageSponsor.Text.Contains("*"))
                str += tabPageSponsor.Text;
            if (tabPagePrize.Text.Contains("*"))
                str += tabPagePrize.Text;
            if (tabPageDonor.Text.Contains("*"))
                str += tabPageDonor.Text;
            if (tabPageDonation.Text.Contains("*"))
                str += tabPageDonation.Text;
            if (str == "")
            {
                //when user manually click saved changes on each entity
                if (MessageBox.Show("Do you really want to log out, " + username + "?", caption, buttonYN, iconQuestion) == DialogResult.Yes)
                {
                    Hide();
                    loginForm.Show();
                }
            }
            else
            {
                //when user didn't save changes
                DialogResult = MessageBox.Show("Save changes to the following items?\n" + str, caption, MessageBoxButtons.YesNoCancel, iconQuestion);
                switch (DialogResult)
                {
                    case DialogResult.Yes:
                        saveAll();
                        Hide();
                        loginForm.Show();
                        break;
                    case DialogResult.No:
                        Hide();
                        loginForm.Show();
                        break;
                    case DialogResult.Cancel:
                        break;
                }
            }
        }




        /// <summary>
        /// ADD BUTTON
        /// </summary>
        private void buttonAddSponsor_Click(object sender, EventArgs e)
        {
            try
            {
                msg = manager.addSponsor(textBoxSponsorID.Text, textBoxSponsorFN.Text, textBoxSponsorLN.Text, 0);

                if (!msg.valid)
                {
                    MessageBox.Show(msg.message, caption, buttonOk, iconError);
                    switch (msg.errorPointer)
                    {
                        case 0: textBoxSponsorID.Clear(); textBoxSponsorID.Focus(); break;
                        case 1: textBoxSponsorFN.Clear(); textBoxSponsorFN.Focus(); break;
                        case 2: textBoxSponsorLN.Clear(); textBoxSponsorLN.Focus(); break;
                    }
                }
                else
                {
                    MessageBox.Show(msg.message, caption, buttonOk, iconInfo);
                    textBoxSponsorID.Clear();
                    textBoxSponsorFN.Clear();
                    textBoxSponsorLN.Clear();
                    textBoxSponsorID.Focus();
                    autoRefreshList(sender, e);
                    buttonSaveSponsor.Enabled = true;
                    tabPageSponsor.Text = "SPONSOR*";
                }
            }
            catch (Exception ex)
            {
                ex = new ArgumentNullException("Required fields missing or not in valid format!", ex);
                MessageBox.Show(ex.Message, caption, buttonOk, iconWarning);
            }

        }

        private void buttonAddPrize_Click(object sender, EventArgs e)
        {
            try
            {
                msg = manager.addPrize(textBoxPrizeID.Text, comboBoxSponsorIDofPrize.Text, textBoxPrizeDesc.Text, reformatCurrencyTextBox(textBoxValuePerPrize),
            reformatCurrencyTextBox(textBoxDonationLimit), Convert.ToInt32(textBoxPrizeOriginalQuantity.Text), Convert.ToInt32(textBoxPrizeOriginalQuantity.Text));
                if (!msg.valid)
                {
                    MessageBox.Show(msg.message, caption, buttonOk, iconError);
                    switch (msg.errorPointer)
                    {
                        case 0: textBoxPrizeID.Clear(); textBoxPrizeID.Focus(); break;
                        case 1: comboBoxSponsorIDofPrize.Focus(); break;
                        case 2: textBoxPrizeDesc.Clear(); textBoxPrizeDesc.Focus(); break;
                        case 3: textBoxValuePerPrize.Clear(); textBoxValuePerPrize.Focus(); break;
                        case 4: textBoxPrizeOriginalQuantity.Clear(); textBoxPrizeOriginalQuantity.Focus(); break;
                    }
                }
                else
                {
                    MessageBox.Show(msg.message, caption, buttonOk, iconInfo);
                    textBoxPrizeID.Clear();
                    textBoxPrizeID.Focus();
                    comboBoxSponsorIDofPrize.Text = "-- Select --";
                    textBoxPrizeDesc.Clear();
                    textBoxValuePerPrize.Clear();
                    textBoxDonationLimit.Clear();
                    textBoxPrizeOriginalQuantity.Clear();
                    autoRefreshList(sender, e);
                    buttonSavePrize.Enabled = true;
                    tabPagePrize.Text = "PRIZE*";
                }
            }
            catch (Exception ex)
            {
                ex = new ArgumentNullException("Required fields missing or not in valid format!", ex);
                MessageBox.Show(ex.Message, caption, buttonOk, iconWarning);
            }
        }

        private void buttonAddDonor_Click(object sender, EventArgs e)
        {
            try
            {
                char cardType = ' ';
                if (radioButtonAmex.Checked) cardType = 'A';
                else if (radioButtonMC.Checked) cardType = 'M';
                else if (radioButtonVisa.Checked) cardType = 'V';

                msg = manager.addDonor(textBoxDonorID.Text, textBoxDonorFN.Text, textBoxDonorLN.Text, textBoxAddress.Text, maskedTextBoxDonorPhone.Text, cardType, textBoxCardNumber.Text, maskedTextBoxExpiredDate.Text);

                if (!msg.valid)
                {
                    MessageBox.Show(msg.message, caption, buttonOk, iconError);
                    switch (msg.errorPointer)
                    {
                        case 0: textBoxDonorID.Clear(); textBoxDonorID.Focus(); break;
                        case 1: textBoxDonorFN.Clear(); textBoxDonorFN.Focus(); break;
                        case 2: textBoxDonorLN.Clear(); textBoxDonorLN.Focus(); break;
                        case 3: textBoxAddress.Clear(); textBoxAddress.Focus(); break;
                        case 4: maskedTextBoxDonorPhone.Clear(); maskedTextBoxDonorPhone.Focus(); break;
                        case 5: textBoxCardNumber.Clear(); textBoxCardNumber.Focus(); break;
                        case 6: maskedTextBoxExpiredDate.Clear(); maskedTextBoxExpiredDate.Focus(); break;
                    }
                }
                else
                {
                    MessageBox.Show(msg.message, caption, buttonOk, iconInfo);
                    textBoxDonorID.Clear();
                    textBoxDonorFN.Clear();
                    textBoxDonorLN.Clear();
                    textBoxAddress.Clear();
                    maskedTextBoxExpiredDate.Clear();
                    maskedTextBoxDonorPhone.Clear();
                    textBoxCardNumber.Clear();
                    autoRefreshList(sender, e);
                    buttonSaveDonor.Enabled = true;
                    tabPageDonor.Text = "DONOR*";
                }
            }
            catch (Exception ex)
            {
                ex = new ArgumentNullException("Required fields missing or not in valid format!", ex);
                MessageBox.Show(ex.Message, caption, buttonOk, iconWarning);
            }

        }

        private void buttonAddDonation_Click(object sender, EventArgs e)
        {
            try
            {
                msg = manager.addDonation(textBoxDonationID.Text, comboBoxDonorIDofDonation.Text, dateTimePicker.Value.ToString("dd/MM/yyyy"),
                    reformatCurrencyTextBox(textBoxDonationAmount), textBoxPrizeIDofDonation.Text, int.Parse(textBoxQuantityToAward.Text));
                if (!msg.valid)
                {
                    MessageBox.Show(msg.message, caption, buttonOk, iconError);
                    switch (msg.errorPointer)
                    {
                        case 0: textBoxDonationID.Clear(); textBoxDonationID.Focus(); break;
                        case 1: comboBoxDonorIDofDonation.Focus(); break;
                        case 2: textBoxDonationAmount.Clear(); textBoxDonationAmount.Focus(); break;
                        case 3: textBoxPrizeIDofDonation.Clear(); textBoxPrizeIDofDonation.Focus(); break;
                        case 4: textBoxQuantityToAward.Clear(); textBoxQuantityToAward.Focus(); break;
                    }
                }
                else
                {
                    MessageBox.Show(msg.message, caption, buttonOk, iconInfo);
                    textBoxDonationID.Clear();
                    textBoxDonationID.Focus();
                    comboBoxDonorIDofDonation.Text = "-- Select --";
                    textBoxDonationAmount.Clear();
                    textBoxQuantityToAward.Clear();
                    autoRefreshList(sender, e);
                    buttonSaveDonation.Enabled = true;
                    tabPageDonation.Text = "DONATION*";
                }
            }
            catch (Exception ex)
            {
                ex = new ArgumentNullException("Required fields missing or not in valid format!", ex);
                MessageBox.Show(ex.Message, caption, buttonOk, iconWarning);
            }
        }

        //automatically list all qualified prizes (no button-clicked necessary)
        private void textBoxDonationAmount_TextChanged(object sender, EventArgs e)
        {
            formatCurrencyTextBox((TextBox)sender);
            listViewQualifiedPrizes.Items.Clear();
            try
            {
                List<string> allQualifiedPrizes = manager.getQualifiedPrize(reformatCurrencyTextBox(textBoxDonationAmount));
                foreach (var itemPr in allQualifiedPrizes)
                {
                    ListViewItem item = new ListViewItem(itemPr.Split(','));
                    listViewQualifiedPrizes.Items.Add(item);
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        //allow user to choose prize to award by select a row in listview
        private void listViewQualifiedPrizes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewQualifiedPrizes.SelectedItems.Count == 1)
            {
                string prizeID = listViewQualifiedPrizes.SelectedItems[0].Text;
                if (MessageBox.Show("You want to award the " + listViewQualifiedPrizes.SelectedItems[0].SubItems[1].Text + " to this new donation?", caption, buttonYN, iconQuestion) == DialogResult.Yes)
                {
                    textBoxPrizeIDofDonation.Text = prizeID;
                    textBoxQuantityToAward.Enabled = true;
                    textBoxQuantityToAward.Focus();
                }
            }
        }





        /// <summary>
        /// SAVE BUTTON
        /// </summary>
        private void buttonSaveSponsor_Click(object sender, EventArgs e)
        {
            msg = manager.saveSponsorRecords();
            MessageBox.Show(msg.message, caption, buttonOk, iconInfo);
            buttonSaveSponsor.Enabled = false;
            countItemOfEachEntity();
        }

        private void buttonSavePrize_Click(object sender, EventArgs e)
        {
            msg = manager.savePrizeRecords();
            MessageBox.Show(msg.message, caption, buttonOk, iconInfo);
            buttonSavePrize.Enabled = false;
            countItemOfEachEntity();
        }

        private void buttonSaveDonor_Click(object sender, EventArgs e)
        {
            msg = manager.saveDonorRecords();
            manager.saveDonationRecords();
            manager.savePrizeRecords();
            MessageBox.Show(msg.message, caption, buttonOk, iconInfo);
            buttonSaveDonor.Enabled = false;
            buttonSaveDonation.Enabled = false;
            buttonSavePrize.Enabled = false;
            countItemOfEachEntity();
        }

        private void buttonSaveDonation_Click(object sender, EventArgs e)
        {
            msg = manager.saveDonationRecords();
            manager.savePrizeRecords();
            MessageBox.Show(msg.message, caption, buttonOk, iconInfo);
            buttonSaveDonation.Enabled = false;
            buttonSavePrize.Enabled = false;
            countItemOfEachEntity();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveAll();
            countItemOfEachEntity();
        }

        private void saveAll()
        {
            manager.saveSponsorRecords(); buttonSaveSponsor.Enabled = false;
            manager.savePrizeRecords(); buttonSavePrize.Enabled = false;
            manager.saveDonorRecords(); buttonSaveDonor.Enabled = false;
            manager.saveDonationRecords(); buttonSaveDonation.Enabled = false;
            manager.saveDonationRecords(); buttonSaveDonation.Enabled = false;

        }





        /// <summary>
        /// LIST BUTTON
        /// </summary>
        //a common function that be used to load all records into listview
        private void LoadToList(string all, ListView listView)
        {
            listView.Items.Clear();
            string[] list = all.Split('\n');
            if (all.Length == 0)
                MessageBox.Show("Empty list!", caption, buttonOk, iconWarning);
            else
                foreach (var item in list)
                {
                    ListViewItem items = new ListViewItem(item.Split(','));
                    listView.Items.Add(items);
                }
        }

        private void buttonListSponsor_Click(object sender, EventArgs e)
        {
            string allSponsors = manager.getAllSponsors();
            LoadToList(allSponsors, listViewSponsor);
            formatCurrencyListView(listViewSponsor, 3);
            listViewSponsor.BringToFront();
        }

        private void buttonListPrize_Click(object sender, EventArgs e)
        {
            string allPrizes = manager.getAllPrizes();
            LoadToList(allPrizes, listViewPrize);
            formatCurrencyListView(listViewPrize, 3);
            formatCurrencyListView(listViewPrize, 4);
            listViewPrize.BringToFront();
        }

        private void buttonListDonor_Click(object sender, EventArgs e)
        {
            string allDonors = manager.getAllDonors();
            LoadToList(allDonors, listViewDonor);
            listViewDonor.BringToFront();
        }

        private void buttonListDonation_Click(object sender, EventArgs e)
        {
            string allDonations = manager.getAllDonations();
            LoadToList(allDonations, listViewDonation);
            formatCurrencyListView(listViewDonation, 3);
            listViewDonation.BringToFront();
        }





        /// <summary>
        /// SEARCH GROUP
        /// </summary>
        //Search among Sponsor, Prize, Donor, Donation
        private void comboBoxSearchOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearAllList();
            string[] sponsorOpt = { "Sponsor ID", "First name", "Last name" };
            string[] prizeOpt = { "Prize ID", "Description", "Sponsor ID" };
            string[] donorOpt = { "Donor ID", "First name", "Last name", "Card Type" };
            string[] donationOpt = { "Donation ID", "Donor ID", "Prize ID" };
            int option = comboBoxSearchOption1.SelectedIndex;
            comboBoxSearchOption2.Text = "-- Select --";
            comboBoxSearchOption2.Items.Clear();
            comboBoxSearchOption2.Focus();
            switch (option)
            {
                case 0: //search by sponsor
                    comboBoxSearchOption2.Items.AddRange(sponsorOpt);
                    listViewSponsor.BringToFront();
                    tabControlSponPrize.SelectedTab = tabControlSponPrize.TabPages[0];
                    break;
                case 1: //search by prize
                    comboBoxSearchOption2.Items.AddRange(prizeOpt);
                    listViewPrize.BringToFront();
                    tabControlSponPrize.SelectedTab = tabControlSponPrize.TabPages[1];
                    break;
                case 2: //search by donor
                    comboBoxSearchOption2.Items.AddRange(donorOpt);
                    listViewDonor.BringToFront();
                    tabControlDonorDon.SelectedTab = tabControlDonorDon.TabPages[0];
                    break;
                case 3: //search by donation
                    comboBoxSearchOption2.Items.AddRange(donationOpt);
                    listViewDonation.BringToFront();
                    tabControlDonorDon.SelectedTab = tabControlDonorDon.TabPages[1];
                    break;
            }
            comboBoxSearchOption2.Enabled = true;
            comboBoxSearch.Text = "-- Select --";
        }

        //Search among properties (id, fn, ln, desc...)
        private void comboBoxSearchOption2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int opt1 = comboBoxSearchOption1.SelectedIndex;
            int opt2 = comboBoxSearchOption2.SelectedIndex;
            List<string> list = manager.fetchAllProps(opt1, opt2);
            comboBoxSearch.Text = "-- Select --";
            comboBoxSearch.Items.Clear();
            comboBoxSearch.Items.AddRange(list.ToArray());
            comboBoxSearch.Enabled = true;
            comboBoxSearch.Focus();
        }

        private void comboBoxSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            int opt1 = comboBoxSearchOption1.SelectedIndex;
            int opt2 = comboBoxSearchOption2.SelectedIndex;
            string opt3 = comboBoxSearch.Text;
            clearAllList();

            List<string> list = manager.search(opt1, opt2, opt3);

            foreach (var line in list)
            {
                ListViewItem item = new ListViewItem(line.Split(','));
                item.SubItems.Add(line);
                switch (opt1)
                {
                    case 0:
                        listViewSponsor.Items.Add(item);
                        break;
                    case 1:
                        listViewPrize.Items.Add(item);
                        break;
                    case 2:
                        listViewDonor.Items.Add(item);
                        break;
                    case 3:
                        listViewDonation.Items.Add(item);
                        break;
                }
            }
            formatCurrencyListView(listViewSponsor, 3);
            formatCurrencyListView(listViewPrize, 3);
            formatCurrencyListView(listViewPrize, 4);
            formatCurrencyListView(listViewDonation, 3);
        }

        //Close drop-down list if user want to search by inputting
        private void comboBoxSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            comboBoxSearch.DroppedDown = false;
        }

        //if focus is not on groupBoxSearch, the value of 3 comboBoxSearch will be set to default
        private void groupBoxSearch_Leave(object sender, EventArgs e)
        {
            if (!listViewSponsor.Focused && !listViewPrize.Focused && !listViewDonor.Focused && !listViewDonation.Focused && !groupBoxSearch.Focused)
            {
                comboBoxSearchOption1.Text = "-- Select --";
                comboBoxSearchOption2.Text = "-- Select --";
                comboBoxSearch.Text = "-- Select --";
                comboBoxSearchOption2.Enabled = false;
                comboBoxSearch.Enabled = false;
            }
        }



        /// <summary>
        /// DELETE
        /// </summary>
        //fetch all columns into textbox --> only when there is a selected item in listview then buttonDelete is enabled
        private void listViewDonation_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listViewDonation.Items.Count; i++)
            {
                if (listViewDonation.Items[i].Selected)
                {
                    textBoxDonationID.Text = listViewDonation.Items[i].SubItems[0].Text;
                    comboBoxDonorIDofDonation.Text = listViewDonation.Items[i].SubItems[1].Text;
                    textBoxDonationAmount.Text = listViewDonation.Items[i].SubItems[3].Text;
                    textBoxPrizeIDofDonation.Text = listViewDonation.Items[i].SubItems[4].Text;
                    textBoxQuantityToAward.Text = listViewDonation.Items[i].SubItems[5].Text;
                }
            }
            buttonDeleteDonation.Enabled = true;
            buttonAddDonation.Enabled = false;
            textBoxDonationID.Enabled = false; comboBoxDonorIDofDonation.Enabled = false; textBoxDonationAmount.Enabled = false; textBoxQuantityToAward.Enabled = false; dateTimePicker.Enabled = false;
            listViewQualifiedPrizes.Items.Clear();
        }

        //if listview lost focus, only buttonDelete is enabled and valid to click
        private void listViewDonation_Leave(object sender, EventArgs e)
        {
            if (!buttonDeleteDonation.Focused)
            {
                buttonDeleteDonation.Enabled = false;
                clearDonation();
            }
        }

        //after deleting, buttonDelete will be disabled
        private void buttonDeleteDonation_Click(object sender, EventArgs e)
        {
            string donationID = listViewDonation.SelectedItems[0].Text;
            if (MessageBox.Show("Do you want to delete donation " + donationID + "?", caption, buttonYN, iconQuestion) == DialogResult.Yes)
            {
                msg = manager.deleteDonation(donationID);
                MessageBox.Show(msg.message, caption, buttonOk, iconInfo);
                tabPageDonation.Text = "DONATION*";
                tabPagePrize.Text = "PRIZE*";
                buttonSaveDonation.Enabled = true;
                buttonSavePrize.Enabled = true;
                clearAllField();
                autoRefreshList(sender, e);
            }
            buttonDeleteDonation.Enabled = false;
        }

        private void listViewDonor_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listViewDonor.Items.Count; i++)
            {
                if (listViewDonor.Items[i].Selected)
                {
                    textBoxDonorID.Text = listViewDonor.Items[i].SubItems[0].Text;
                    textBoxDonorFN.Text = listViewDonor.Items[i].SubItems[1].Text;
                    textBoxDonorLN.Text = listViewDonor.Items[i].SubItems[2].Text;
                    textBoxAddress.Text = listViewDonor.Items[i].SubItems[3].Text;
                    maskedTextBoxDonorPhone.Text = listViewDonor.Items[i].SubItems[4].Text;
                    if (listViewDonor.Items[i].SubItems[5].Text == "V")
                    {
                        radioButtonVisa.Checked = true;
                    }
                    else if (listViewDonor.Items[i].SubItems[5].Text == "M")
                    {
                        radioButtonMC.Checked = true;
                    }
                    else
                    {
                        radioButtonAmex.Checked = true;
                    }
                    textBoxCardNumber.Text = listViewDonor.Items[i].SubItems[6].Text;
                    maskedTextBoxExpiredDate.Text = listViewDonor.Items[i].SubItems[7].Text;
                }
            }
            buttonDeleteDonor.Enabled = true;
            buttonAddDonor.Enabled = false;
            textBoxDonorID.Enabled = false; textBoxDonorFN.Enabled = false; textBoxDonorLN.Enabled = false; textBoxAddress.Enabled = false; maskedTextBoxDonorPhone.Enabled = false; textBoxCardNumber.Enabled = false; maskedTextBoxExpiredDate.Enabled = false;

        }

        private void listViewDonor_Leave(object sender, EventArgs e)
        {
            if (!buttonDeleteDonor.Focused)
            {
                buttonDeleteDonor.Enabled = false;
                clearDonor();
            }
        }

        private void buttonDeleteDonor_Click(object sender, EventArgs e)
        {
            string donorID = listViewDonor.SelectedItems[0].Text;
            if (MessageBox.Show("All the donations made by donor " + donorID + " will also be deleted from file. Do you still want to delete this donor?", caption, buttonYN, iconQuestion) == DialogResult.Yes)
            {
                msg = manager.deleteDonor(donorID);
                MessageBox.Show(msg.message, caption, buttonOk, iconInfo);
                tabPageDonor.Text = "DONOR*";
                tabPageDonation.Text = "DONATION*";
                tabPagePrize.Text = "PRIZE*";
                buttonSaveDonor.Enabled = true;
                buttonSaveDonation.Enabled = true;
                buttonSavePrize.Enabled = true;
                clearAllField();
            }
            buttonDeleteDonor.Enabled = false;
        }

        private void listViewPrize_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listViewPrize.Items.Count; i++)
            {
                if (listViewPrize.Items[i].Selected)
                {
                    textBoxPrizeID.Text = listViewPrize.Items[i].SubItems[0].Text;
                    comboBoxSponsorIDofPrize.Text = listViewPrize.Items[i].SubItems[1].Text;
                    textBoxPrizeDesc.Text = listViewPrize.Items[i].SubItems[2].Text;
                    textBoxValuePerPrize.Text = listViewPrize.Items[i].SubItems[3].Text;
                    textBoxDonationLimit.Text = listViewPrize.Items[i].SubItems[4].Text;
                    textBoxPrizeOriginalQuantity.Text = listViewPrize.Items[i].SubItems[5].Text;
                }
            }
            buttonDeletePrize.Enabled = true;
            buttonAddPrize.Enabled = false;
            textBoxPrizeID.Enabled = false; textBoxPrizeDesc.Enabled = false; textBoxValuePerPrize.Enabled = false; textBoxPrizeOriginalQuantity.Enabled = false; textBoxDonationLimit.Enabled = false;

        }

        private void listViewPrize_Leave(object sender, EventArgs e)
        {
            if (!buttonDeletePrize.Focused)
            {
                buttonDeletePrize.Enabled = false;
                clearPrize();
            }
        }

        private void buttonDeletePrize_Click(object sender, EventArgs e)
        {
            string prizeID = listViewPrize.SelectedItems[0].Text;
            string desc = listViewPrize.SelectedItems[0].SubItems[2].Text;
            if (MessageBox.Show("All donations awarded with " + desc + " will also be deleted from file. Do you still want to delete this prize?", caption, buttonYN, iconQuestion) == DialogResult.Yes)
            {
                msg = manager.deletePrize(prizeID);
                MessageBox.Show(msg.message, caption, buttonOk, iconInfo);
                tabPagePrize.Text = "PRIZE*";
                tabPageSponsor.Text = "SPONSOR*";
                tabPageDonation.Text = "DONATION*";
                buttonSavePrize.Enabled = true;
                buttonSaveSponsor.Enabled = true;
                buttonSaveDonation.Enabled = true;
                clearAllField();
                autoRefreshList(sender, e);
            }
            buttonDeletePrize.Enabled = false;
        }

        private void listViewSponsor_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listViewSponsor.Items.Count; i++)
            {
                if (listViewSponsor.Items[i].Selected)
                {
                    textBoxSponsorID.Text = listViewSponsor.Items[i].SubItems[0].Text;
                    textBoxSponsorFN.Text = listViewSponsor.Items[i].SubItems[1].Text;
                    textBoxSponsorLN.Text = listViewSponsor.Items[i].SubItems[2].Text;
                    textBoxSponsorValue.Text = listViewSponsor.Items[i].SubItems[3].Text;
                }
            }
            buttonDeleteSponsor.Enabled = true;
            buttonAddSponsor.Enabled = false;
            textBoxSponsorID.Enabled = false; textBoxSponsorFN.Enabled = false; textBoxSponsorLN.Enabled = false;

        }

        private void listViewSponsor_Leave(object sender, EventArgs e)
        {
            if (!buttonDeleteSponsor.Focused)
            {
                buttonDeleteSponsor.Enabled = false;
                clearSponsor();
            }
        }

        private void buttonDeleteSponsor_Click(object sender, EventArgs e)
        {
            string sponsorID = listViewSponsor.SelectedItems[0].Text;
            if (MessageBox.Show("All prizes supplied by sponsor " + sponsorID + " will be also deleted. Do you still want to delete this sponsor?", caption, buttonYN, iconQuestion) == DialogResult.Yes)
            {
                msg = manager.deleteSponsor(sponsorID);
                MessageBox.Show(msg.message, caption, buttonOk, iconInfo);
                tabPageSponsor.Text = "SPONSOR*";
                tabPagePrize.Text = "PRIZE*";
                tabPageDonation.Text = "DONATION*";
                buttonSaveSponsor.Enabled = true;
                buttonSavePrize.Enabled = true;
                buttonSaveDonation.Enabled = true;
                clearAllField();
                autoRefreshList(sender, e);
            }
            buttonDeleteSponsor.Enabled = false;
        }





        /// <summary>
        /// FORMAT (user-friendly)
        /// </summary>
        //Set fixed string '$0.00' in textbox
        private bool isValidText(string text)
        {
            Regex money = new Regex(@"^\$(\d{1,3}(\,\d{3})*|(\d+))(\.\d{2})?$");
            return money.IsMatch(text);
        }

        //function to format any currency textbox 
        private void formatCurrencyTextBox(TextBox textBox)
        {
            string currency = textBox.Text.Replace(",", "").Replace("$", "").Replace(".", "").TrimStart('0');
            double result;

            if (double.TryParse(currency, out result))
            {
                result /= 100;
                //Eliminate event to format
                textBox.TextChanged -= textBoxDonationLimit_TextChanged;
                //Formatting
                textBox.Text = string.Format("{0:C2}", result);
                //Trigger event
                textBox.TextChanged += textBoxDonationLimit_TextChanged;
                textBox.Select(textBox.Text.Length, 0);
            }
            bool valid = isValidText(textBox.Text);
            //set fixed string '$0.00' if user try deleting it
            if (!valid)
            {
                textBox.Text = "$0.00";
                textBox.Select(textBox.Text.Length, 0);
            }
        }

        //format textBoxDonationLimit
        private void textBoxDonationLimit_TextChanged(object sender, EventArgs e)
        {
            formatCurrencyTextBox((TextBox)sender);
        }

        //format textBoxValuePerPrize
        private void textBoxValuePerPrize_TextChanged(object sender, EventArgs e)
        {
            formatCurrencyTextBox((TextBox)sender);
        }

        //Format currency value for listviewcontrol when listing
        private void formatCurrencyListView(ListView listView, int index)
        {
            if (listView.Items.Count > 0)
            {
                foreach (ListViewItem value in listView.Items)
                {
                    value.SubItems[index].Text = double.Parse(value.SubItems[index].Text).ToString("C");
                }
            }
        }

        //Reformat textbox.text before adding (remove "$,.")
        private double reformatCurrencyTextBox(TextBox textBox)
        {
            double value;
            value = double.Parse(textBox.Text.Replace(",", "").Replace("$", "").TrimStart('0'));
            return value;
        }



        //Refresh or clear all lists
        private void autoRefreshList(object sender, EventArgs e)
        {
            if (listViewSponsor.Items.Count != 0) buttonListSponsor_Click(sender, e);
            if (listViewPrize.Items.Count != 0) buttonListPrize_Click(sender, e);
            if (listViewDonor.Items.Count != 0) buttonListDonor_Click(sender, e);
            if (listViewDonation.Items.Count != 0) buttonListDonation_Click(sender, e);
        }

        private void clearAllList()
        {
            listViewSponsor.Items.Clear();
            listViewPrize.Items.Clear();
            listViewDonor.Items.Clear();
            listViewDonation.Items.Clear();
        }

        //Refresh application and clear all fields
        private void clearAllField()
        {
            clearSponsor();
            clearPrize();
            clearDonor();
            clearDonation();
            clearSearch();
            buttonAddSponsor.Enabled = true;
            buttonAddPrize.Enabled = true;
            buttonAddDonor.Enabled = true;
            buttonAddDonation.Enabled = true;
        }

        private void clearSearch()
        {
            comboBoxSearchOption1.Text = "-- Select --";
            comboBoxSearchOption2.Text = "-- Select --";
            comboBoxSearch.Text = "-- Select --";
            comboBoxSearchOption2.Enabled = false;
            comboBoxSearch.Enabled = false;
        }

        private void clearDonation()
        {
            textBoxDonationID.Clear();
            comboBoxDonorIDofDonation.Text = "-- Select --";
            textBoxDonationAmount.Clear();
            textBoxPrizeIDofDonation.Clear();
            textBoxQuantityToAward.Clear();
            textBoxDonationID.Enabled = true;
            comboBoxDonorIDofDonation.Enabled = true;
            textBoxDonationAmount.Enabled = true;
            textBoxQuantityToAward.Enabled = true;
            dateTimePicker.Enabled = true;
        }

        private void clearDonor()
        {
            textBoxDonorID.Clear();
            textBoxDonorFN.Clear();
            textBoxDonorLN.Clear();
            textBoxAddress.Clear();
            maskedTextBoxDonorPhone.Clear();
            textBoxCardNumber.Clear();
            maskedTextBoxExpiredDate.Clear();
            textBoxDonorID.Enabled = true;
            textBoxDonorFN.Enabled = true;
            textBoxDonorLN.Enabled = true;
            textBoxAddress.Enabled = true;
            maskedTextBoxDonorPhone.Enabled = true;
            textBoxCardNumber.Enabled = true;
            maskedTextBoxExpiredDate.Enabled = true;
        }

        private void clearPrize()
        {
            textBoxPrizeID.Clear();
            textBoxPrizeDesc.Clear();
            comboBoxSponsorIDofPrize.Text = "-- Select --";
            textBoxValuePerPrize.Clear();
            textBoxPrizeOriginalQuantity.Clear();
            textBoxDonationLimit.Clear();
            textBoxPrizeID.Enabled = true;
            textBoxPrizeDesc.Enabled = true;
            textBoxValuePerPrize.Enabled = true;
            textBoxPrizeOriginalQuantity.Enabled = true;
            textBoxDonationLimit.Enabled = true;
        }

        private void clearSponsor()
        {
            textBoxSponsorID.Clear();
            textBoxSponsorFN.Clear();
            textBoxSponsorLN.Clear();
            textBoxSponsorValue.Text = "$0.00";
            textBoxSponsorID.Enabled = true;
            textBoxSponsorFN.Enabled = true;
            textBoxSponsorLN.Enabled = true;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            clearAllField();
            clearAllList();
        }

        //Count each entity's number of items to display on each tabpage
        private void countItemOfEachEntity()
        {
            if (!buttonSaveSponsor.Enabled) tabPageSponsor.Text = "SPONSOR  " + manager.fetchAnyID("sponsor").Count.ToString();
            if (!buttonSavePrize.Enabled) tabPagePrize.Text = "PRIZE  " + manager.fetchAnyID("prize").Count.ToString();
            if (!buttonSaveDonor.Enabled) tabPageDonor.Text = "DONOR  " + manager.fetchAnyID("donor").Count.ToString();
            if (!buttonSaveDonation.Enabled) tabPageDonation.Text = "DONATION  " + manager.fetchAnyID("donation").Count.ToString();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonLogOut_Click(sender, e);
        }

        private void aboutToolStripMenuAbout_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(@"..\Debug\about.txt"))
            {
                MessageBox.Show(sr.ReadToEnd(), caption);
            }
        }
    }
}
