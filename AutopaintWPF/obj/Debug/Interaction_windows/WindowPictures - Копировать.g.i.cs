﻿#pragma checksum "..\..\..\Interaction_windows\WindowPictures - Копировать.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "B6EF5BB836D7A29F8E1211168958B7D5D54BFC9DCE974338533F4507E9ADC2FA"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using AutopaintWPF;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace AutopaintWPF {
    
    
    /// <summary>
    /// WindowPictures
    /// </summary>
    public partial class WindowPictures : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 27 "..\..\..\Interaction_windows\WindowPictures - Копировать.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextBox_name;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\Interaction_windows\WindowPictures - Копировать.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Button_choose_image;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\Interaction_windows\WindowPictures - Копировать.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image Image;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\Interaction_windows\WindowPictures - Копировать.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Button_accept;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\Interaction_windows\WindowPictures - Копировать.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Button_reset;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\Interaction_windows\WindowPictures - Копировать.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Button_cancel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/AutopaintWPF;component/interaction_windows/windowpictures%20-%20%d0%9a%d0%be%d0%" +
                    "bf%d0%b8%d1%80%d0%be%d0%b2%d0%b0%d1%82%d1%8c.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Interaction_windows\WindowPictures - Копировать.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.TextBox_name = ((System.Windows.Controls.TextBox)(target));
            
            #line 34 "..\..\..\Interaction_windows\WindowPictures - Копировать.xaml"
            this.TextBox_name.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TextBox_ru_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Button_choose_image = ((System.Windows.Controls.Button)(target));
            
            #line 43 "..\..\..\Interaction_windows\WindowPictures - Копировать.xaml"
            this.Button_choose_image.Click += new System.Windows.RoutedEventHandler(this.Button_choose_image_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Image = ((System.Windows.Controls.Image)(target));
            return;
            case 4:
            this.Button_accept = ((System.Windows.Controls.Button)(target));
            
            #line 61 "..\..\..\Interaction_windows\WindowPictures - Копировать.xaml"
            this.Button_accept.Click += new System.Windows.RoutedEventHandler(this.Button_accept_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.Button_reset = ((System.Windows.Controls.Button)(target));
            
            #line 66 "..\..\..\Interaction_windows\WindowPictures - Копировать.xaml"
            this.Button_reset.Click += new System.Windows.RoutedEventHandler(this.Button_reset_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.Button_cancel = ((System.Windows.Controls.Button)(target));
            
            #line 71 "..\..\..\Interaction_windows\WindowPictures - Копировать.xaml"
            this.Button_cancel.Click += new System.Windows.RoutedEventHandler(this.Button_cancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

