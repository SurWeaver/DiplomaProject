   M S B A M L   `   `   `  ���� G  CAutopaintWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:AutopaintWPFAutopaintWPF  &     AutopaintWPF.SupplyManagerWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     V RMaterialDesignThemes.Wpf, Version=3.1.2.1465, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  &localclr-namespace:AutopaintWPF  -      MaterialDesignBody#���   $��Regular"�$��13(�&   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�   TextRenderingMode$
 Auto=�  MaterialDesignPaper#���    MaterialDesignFont#���   � Title$ Рабочее окно��$	��450��$	��810�� � 
ResizeMode$ CanMinimize=� � WindowStartupLocation$ CenterScreen=�.���  � ColumnDefinitions �� $
��0.4* ��� $��3* ���  � RowDefinitions �� $��6* ��� .+��� -   $��	MainPanel��$��0��$��1��$��3�� �� Orientation$ 
Horizontal=�.+�&   System.Windows.Controls.DataGrid  -   $��DataGrid��$	��300��$	��450��$��10q�   ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$
 Auto $
��Left=�$	��Top=�	   
IsReadOnly	 . 
   AutoGenerateColumns
 .     AreRowDetailsFrozen .    CanUserResizeRows .     CanUserResizeColumns .     SelectionMode$ Single=�   CanUserReorderColumns .  �� $��0��$��4��.+��� $&�� Выберите таблицу:���� -   $��ComboBoxTables��$��0��$��4��$��5q�$��30��$	��Top=�.@��� $��
Склад���� $��Поставщики���� $��Поставки���� -   $��button_delivery_supply��$)��#Совершить поставку��$��5q��� -   $��button_supply_report��$&�� Отчёт о поставках��$��5q��� -   $��button_storage_report��$&�� Отчёт об остатках��$��5q��� -   $��Border_color��
���   �$��1q�$	��168��$	��300��$��Center=�$��Center=�$��6,0,7,0q��� $��Images/Logo.png��$��0��$��1��$
��Left=��� $��0��$��0��$
��Left=�.+��� -	   $��
Button_add�� � ToolTip$ Добавить��$��0, 10q�$��50��.���� $��Images/Add.png���� -
   $��Button_change��$ Изменить��$��0, 10q�$��50��.���� $��Images/Change.png���� -   $��Button_delete��$ Удалить��$��0, 10q�$��50��.���� $��Images/Delete.png���� -   $��Button_update��$ Обновить��$��0, 10q�$��50��.���� $��Images/Update.png���� $
��Left=�$��1��$��1��$ 
Horizontal=�.+��� $��Поиск:��$��Center=��� -   $��TextBox_search��$��Center=�$	��200��$��15q��� $��Center=�$��2��$��1��$ 
Horizontal=�.+��� -   $��Button_exit_account��$&  Выйти из аккаунта��$��0, 0, 20, 0q�$��50��.���� $��Images/Exit_account.png���� -   $��Button_exit_program��$( "Выйти из программы��$��50��.���� $��Images/Exit_program.png��