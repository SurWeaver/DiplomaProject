﻿#pragma checksum "..\..\..\Interaction_windows\WindowProducts.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "67C34309645B9CAEB64B4B1428678BDDC63677F7F715CCFE36A59279467CF9E6"
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
    /// WindowProducts
    /// </summary>
    public partial class WindowProducts : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 27 "..\..\..\Interaction_windows\WindowProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextBox_name;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\Interaction_windows\WindowProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ComboBox_paint_type;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\Interaction_windows\WindowProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ComboBox_color_code;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\..\Interaction_windows\WindowProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Button_accept;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\Interaction_windows\WindowProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Button_reset;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\..\Interaction_windows\WindowProducts.xaml"
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
            System.Uri resourceLocater = new System.Uri("/AutopaintWPF;component/interaction_windows/windowproducts.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Interaction_windows\WindowProducts.xaml"
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
            
            #line 35 "..\..\..\Interaction_windows\WindowProducts.xaml"
            this.TextBox_name.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TextBox_ru_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ComboBox_paint_type = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.ComboBox_color_code = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 4:
            this.Button_accept = ((System.Windows.Controls.Button)(target));
            
            #line 75 "..\..\..\Interaction_windows\WindowProducts.xaml"
            this.Button_accept.Click += new System.Windows.RoutedEventHandler(this.Button_accept_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.Button_reset = ((System.Windows.Controls.Button)(target));
            
            #line 80 "..\..\..\Interaction_windows\WindowProducts.xaml"
            this.Button_reset.Click += new System.Windows.RoutedEventHandler(this.Button_reset_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.Button_cancel = ((System.Windows.Controls.Button)(target));
            
            #line 85 "..\..\..\Interaction_windows\WindowProducts.xaml"
            this.Button_cancel.Click += new System.Windows.RoutedEventHandler(this.Button_cancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

