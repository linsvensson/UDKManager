   M S B A M L   `   `   `  ���� U  QMahApps.Metro, Version=0.10.0.0, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4fV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls   PinkPoo Editor   ZerO.CMessageBox   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     PControls;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     -   $��0,3,0,3q�$	��152��  � ShowInTaskbar  .   J� SnapsToDevicePixels . $��Stretch=� � Topmost .  � WindowStartupLocation$ CenterOwner=�$	��455�� � 
ResizeMode$ NoResize=� � WindowStyle$
 None=�(   "MahApps.Metro.Controls.MetroWindow   ShowCloseButton .     	TitleCaps .  {�.   � 	Resources 	 �� MergedDictionaries
 �� Source�� 	 �� $H
 Bpack://application:,,,/MahApps.Metro;component/Styles/Colours.xaml?��� $F
 @pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml?��� $L
 Fpack://application:,,,/MahApps.Metro;component/Styles/Accents/Red.xaml?��� $Q
 Kpack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml?��� $I
 Cpack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml?�%     J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect  .���� $	��455��
���   �$��Center=�$��0,0,-6,0q�._�� $��
-10,0,10,0q� � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� -    ��Name$ title��$��16(�$
��Bold"�$��
90,10,0,44q�$��2���� -   $ 	tbMessage��$��1��$
��Wrap=�$��
95,41,10,0q��� $��2��$��Right=� �� Orientation$ 
Horizontal=�.+��� -    ��Name$ btnYes��$��70�� �� 	IsDefault . $��10q�   SquareButtonStyle#���   $	��Yes���� -   $ btnNo��$��70�� �� IsCancel . $��10q�#���   $��No���� -   $ btnOk��$��70�� . $��10q�#���   $��OK���� -   $ 	btnCancel��$��70��$��10q� . #���   $��Cancel���� -    ��Name$ 	iconImage��$
��Left=�$��73��$��	17,10,0,0q�$	��Top=�$��73�� J� RenderTransformOrigin$ 0.521,0.354&�$��2��$
��Fill=�