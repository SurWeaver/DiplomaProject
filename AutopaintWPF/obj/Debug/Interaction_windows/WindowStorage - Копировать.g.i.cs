﻿#pragma checksum "..\..\..\Interaction_windows\WindowStorage - Копировать.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "486FB3118B9886D912595B9C0B9E16365A27C8A36A7AB0B4DA3803C24D2EA8B5"
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
    /// WindowStorage
    /// </summary>
    public partial class WindowStorage : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 29 "..\..\..\Interaction_windows\WindowStorage - Копировать.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ComboBox_product_name;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\Interaction_windows\WindowStorage - Копировать.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextBox_product_amount;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\Interaction_windows\WindowStorage - Копировать.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ComboBox_supplier;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\Interaction_windows\WindowStorage - Копировать.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextBox_average_purchase_price;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\..\Interaction_windows\WindowStorage - Копировать.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Button_accept;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\..\Interaction_windows\WindowStorage - Копировать.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Button_reset;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\..\Interaction_windows\WindowStorage - Копировать.xaml"
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
            System.Uri resourceLocater = new System.Uri("/AutopaintWPF;component/interaction_windows/windowstorage%20-%20%d0%9a%d0%be%d0%b" +
                    "f%d0%b8%d1%80%d0%be%d0%b2%d0%b0%d1%82%d1%8c.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Interaction_windows\WindowStorage - Копировать.xaml"
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
            this.ComboBox_product_name = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 2:
            this.TextBox_product_amount = ((System.Windows.Controls.TextBox)(target));
            
            #line 50 "..\..\..\Interaction_windows\WindowStorage - Копировать.xaml"
            this.TextBox_product_amount.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TextBox_amount_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 57 "..\..\..\Interaction_windows\WindowStorage - Копировать.xaml"
            ((System.Windows.Controls.Label)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TextBox_amount_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ComboBox_supplier = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 5:
            this.TextBox_average_purchase_price = ((System.Windows.Controls.TextBox)(target));
            
            #line 79 "..\..\..\Interaction_windows\WindowStorage - Копировать.xaml"
            this.TextBox_average_purchase_price.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TextBox_amount_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 6:
            this.Button_accept = ((System.Windows.Controls.Button)(target));
            
            #line 91 "..\..\..\Interaction_windows\WindowStorage - Копировать.xaml"
            this.Button_accept.Click += new System.Windows.RoutedEventHandler(this.Button_accept_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.Button_reset = ((System.Windows.Controls.Button)(target));
            
            #line 96 "..\..\..\Interaction_windows\WindowStorage - Копировать.xaml"
            this.Button_reset.Click += new System.Windows.RoutedEventHandler(this.Button_reset_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.Button_cancel = ((System.Windows.Controls.Button)(target));
            
            #line 101 "..\..\..\Interaction_windows\WindowStorage - Копировать.xaml"
            this.Button_cancel.Click += new System.Windows.RoutedEventHandler(this.Button_cancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

