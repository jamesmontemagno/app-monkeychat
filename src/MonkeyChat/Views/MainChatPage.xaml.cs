using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MonkeyChat
{
    public partial class MainChatPage : ContentPage
    {
        MainChatViewModel vm;
        public MainChatPage()
        {
            InitializeComponent();
            Title = "#general";
            BindingContext = vm =  new MainChatViewModel();


            vm.Messages.CollectionChanged += (sender, e) =>
            {
                var target = vm.Messages[vm.Messages.Count - 1];
                MessagesListView.ScrollTo(target, ScrollToPosition.End, true);
            };


            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Location",
                Command = vm.LocationCommand
            });
           
        }

        private void MyListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            MessagesListView.SelectedItem = null;
        }

        private void MyListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            MessagesListView.SelectedItem = null;

        }
    }
}
