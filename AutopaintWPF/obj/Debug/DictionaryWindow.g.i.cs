﻿#pragma checksum "..\..\DictionaryWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "E46AD951EA3654336CC985FEEFD907B3A64C2817874FDD7BC052696AF30B15A2"
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
    /// DictionaryWindow
    /// </summary>
    public partial class DictionaryWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\DictionaryWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Grid_dictionary;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\DictionaryWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label Label_field_name;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\DictionaryWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Textbox_item_value;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\DictionaryWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Button_action;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\DictionaryWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Button_cancel;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\DictionaryWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Button_exit;
        
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
            System.Uri resourceLocater = new System.Uri("/AutopaintWPF;component/dictionarywindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\DictionaryWindow.xaml"
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
            this.Grid_dictionary = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.Label_field_name = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.Textbox_item_value = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.Button_action = ((System.Windows.Controls.Button)(target));
            
            #line 45 "..\..\DictionaryWindow.xaml"
            this.Button_action.Click += new System.Windows.RoutedEventHandler(this.Button_action_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.Button_cancel = ((System.Windows.Controls.Button)(target));
            
            #line 49 "..\..\DictionaryWindow.xaml"
            this.Button_cancel.Click += new System.Windows.RoutedEventHandler(this.Button_cancel_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.Button_exit = ((System.Windows.Controls.Button)(target));
            
            #line 53 "..\..\DictionaryWindow.xaml"
            this.Button_exit.Click += new System.Windows.RoutedEventHandler(this.Button_exit_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

