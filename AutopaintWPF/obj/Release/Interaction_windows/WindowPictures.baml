   M S B A M L   `   `   `  ���� G  CAutopaintWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:AutopaintWPFAutopaintWPF  !    AutopaintWPF.WindowPictures   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:AutopaintWPF       � Title$  Изображения��$	��322��$	��400�� � 
ResizeMode$ CanMinimize=�.���  J� RenderTransformOrigin$ 0.514,0.264&� � RowDefinitions �� $
��0.4* ��� $
��0.4* ��� �� $
��0.4* � � ColumnDefinitions �� �� �� .+��� $��Название:��$��Center=�$��Right=�$��0��$��0��$��0,3,58,3q�$��64���� -   $��TextBox_name�� �� 	MaxLength$ 10��$��74,7,0,3q�$
��Left=�$��Center=�$	��100��$��0��$��2���� -   $��Button_choose_image��$'��!Выбор изображения��$��Center=�$��Center=�$��0��$��1��$��2��$��
52,7,5.5,3q�$	��206���� $��	Цена:��$��Center=�$
��Left=�$��1��$��0���� -   $��TextBox_price��$ 10��$
��Left=�$��Center=�$	��100��$��1��$��2��$��74,0,0,0q�$	��100���� -   $��Image��$��2��$��0��$��2��$��5q��� $��3��$��0��$��3�� �� Orientation$ 
Horizontal=�$��Center=�.+��� -   $��Button_accept��$��5q�$��Принять���� -   $��Button_reset��$��5q�$��
Сброс���� -   $��Button_cancel��$��5q�$��Отмена��