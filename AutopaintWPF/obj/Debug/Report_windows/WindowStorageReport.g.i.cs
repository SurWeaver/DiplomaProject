﻿#pragma checksum "..\..\..\Report_windows\WindowStorageReport.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "2F7380E0CC4A8B1B23B3F7640726651BF15A9CDCCC929E828C97F90CBC227D2A"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using AutopaintWPF.Report_windows;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
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


namespace AutopaintWPF.Report_windows {
    
    
    /// <summary>
    /// WindowStorageReport
    /// </summary>
    public partial class WindowStorageReport : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 34 "..\..\..\Report_windows\WindowStorageReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox combobox_paint_type;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\Report_windows\WindowStorageReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox combobox_paint_name;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\Report_windows\WindowStorageReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox combobox_supplier;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\Report_windows\WindowStorageReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_make_report;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\Report_windows\WindowStorageReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_cancel;
        
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
            System.Uri resourceLocater = new System.Uri("/AutopaintWPF;component/report_windows/windowstoragereport.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Report_windows\WindowStorageReport.xaml"
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
            this.combobox_paint_type = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 2:
            this.combobox_paint_name = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.combobox_supplier = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 4:
            this.button_make_report = ((System.Windows.Controls.Button)(target));
            
            #line 67 "..\..\..\Report_windows\WindowStorageReport.xaml"
            this.button_make_report.Click += new System.Windows.RoutedEventHandler(this.button_make_report_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.button_cancel = ((System.Windows.Controls.Button)(target));
            
            #line 74 "..\..\..\Report_windows\WindowStorageReport.xaml"
            this.button_cancel.Click += new System.Windows.RoutedEventHandler(this.button_cancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
