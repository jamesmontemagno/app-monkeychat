using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MonkeyChat
{
    public partial class RoomsPage : ContentPage
    {
        public RoomsPage()
        {
            InitializeComponent();
            BindingContext = new RoomsViewModel(this);
        }
    }
}

