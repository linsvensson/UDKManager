   M S B A M L   `   `   `  ���� U  QMahApps.Metro, Version=0.10.0.0, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4fV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls   PinkPoo Editor!   ZerO.Windows.SettingsWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     PControls;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro       � Title$  Settings��$	��373��$	��398�� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterOwner=�(   "MahApps.Metro.Controls.MetroWindow   ShowCloseButton .     ShowIconOnTitleBar .     ShowMinButton .     ShowMaxRestoreButton .   � ShowInTaskbar .     	TitleCaps .  {�.  .��� $
��Left=�$	��418��.+��� -   	 ��Name$	 settingsTabControl��$	��292��$��
10,10,36,0q�$	��Top=�
 �� TabStripPlacement$	
 Top=�
���   �.@��� -    ��Name$ generalTabItem��$��General��$��0,0,-3,0q� � 	Resources  �� 
TargetType �� Property �� Value �� 
TargetType ��Name �� Triggers P� Property P� Value �� 
TargetName   AccentColor �� Color  
GrayNormal P� 
SourceName  	GrayHover��%/  '��       �� " ��.���  �@�FocusVisualStyle Z� ��  � �� .  ��  � ��$ 1q���  � ��$ 6,2,6,2q���  � ��$ Stretch=���  � ��$ Stretch=���  � ��$ 5����  � ��$ 5����  � �� �� " ��.R��� $
 root��$��13.67(�$��31��.���� $��Header����.  P�  �@��
IsSelected . . ��� $
 root�� � �� �� # �   P�  �@��
IsSelected .  . ��� $
 root�� � �� �� # �  P� $
 root�� �@J�IsMouseOver . . ��� $
 root�� � �� �� # �  .��� .+��� -   $ 	rootLabel��$��	UDK root:��$
��Left=�$��10,0,0,0q�$	��Top=�$	��117��$
��Bold"��� -    ��Name$ rootTextBox��$
��Left=�$��23��$��	10,26,0,0q� �� TextWrapping$
 Wrap=�$	��Top=�$	��181��{�.   � ToolTip$5 /This is the main folder where you installed UDK���� -    ��Name$ changeRootFolder��$��Set Root folder��  SquareButtonStyle#���  $
��Left=�$��
196,20,0,0q�$	��Top=�$��94��$��36���� -   $ exeLabel��$��UDK exe:��$
��Left=�$��
10,124,0,0q�$	��Top=�$��70��$
��Bold"��� -   $ 
exeTextBox��$
��Left=�$��23��$��
10,150,0,0q�$
 Wrap=�$	��Top=�$	��117��{�.  $% This is your main .exe to start���� -   $ 
setExeFile��$��	Set .exe ��#���  $
��Left=�$��135,146,0,0q�$	��Top=�$��94��$��35���� -	   $ 	mapsLabel��$��Maps folder:��$
��Left=�$��	10,57,0,0q�$	��Top=�$��84��$
��Bold"��� -
   $ mapsTextBox��$
��Left=�$��23��$��	10,83,0,0q�$
 Wrap=�$	��Top=�$	��234��{�.  $9 3This is your folder containing all of your UDK maps���� -   $ setMapsFolder��$��Set Maps folder��#���  $
��Left=�$��
249,78,0,0q�$	��Top=�$	��107��$��35���� -    ��Name$ trayCheckBox��$��Minimize to system tray��$
��Left=�$��
13,217,0,0q�$	��Top=�$��26��$	��169���� -   $ forceUpdate��$��Check for updates��#���  $
��Left=�$��239,212,0,0q�$	��Top=�$	��117��$��35���� -   $ textEditorTabItem��$��
TextEditor��$��0,0,-3,0q� ��%/  '��       �� " ��.���  �@�FocusVisualStyle Z� ��  � �� .  ��  � ��$ 1q���  � ��$ 6,2,6,2q���  � ��$ Stretch=���  � ��$ Stretch=���  � ��$ 5����  � ��$ 5����  � �� �� " ��.R��� $
 root��$��13.67(�$��31��.���� $��Header����.  P�  �@��
IsSelected . . ��� $
 root�� � �� �� # �   P�  �@��
IsSelected .  . ��� $
 root�� � �� �� # �  P� $
 root�� �@J�IsMouseOver . . ��� $
 root�� � �� �� # �  .��� .+��� -   $ rememberCheckBox��$.��(Remember current session for next launch��$
��Left=�$��	37,39,0,0q�$	��Top=�$��26��$	��277���� -   $ autoCompleteCheckBox��$��Use AutoComplete��$
��Left=�$��	37,70,0,0q�$	��Top=�$	��185���� $��Multiline Selection:��$
��Left=�$��
37,114,0,0q�$	��Top=�$	��125��$
��Bold"�	� -    	�Name$ 	defaultRB��$��Default - //��$
��Left=�$��
37,145,0,0q�$	��Top=�$	��103��	� -   $ otherRB��$��Other - /* */��$
��Left=�$��180,145,0,0q�$	��Top=�$	��115���� -   $ themeTabItem��$��Theme��$��10(�$��
10,0,-10,0q� ��%/  '��       �� " ��.���  �@�FocusVisualStyle Z� ��  � �� .  ��  � ��$ 1q���  � ��$ 6,2,6,2q���  � ��$ Stretch=���  � ��$ Stretch=���  � ��$ 5����  � ��$ 5����  � �� �� " ��.R��� $
 root��$��13.67(�$��31��.���� $��Header����.  P�  �@��
IsSelected . . ��� $
 root�� � �� �� # �   P�  �@��
IsSelected .  . ��� $
 root�� � �� �� # �  P� $
 root�� �@J�IsMouseOver . . ��� $
 root�� � �� �� # �  .��� .+��� -    ��Name$ themeComboBox��$
��Left=�$��
23,142,0,0q�$	��Top=�$	��105��$��Segoe UI)��� -   $ 
themeLabel��$��Theme:��$
��Left=�$��
23,118,0,0q�$	��Top=�$��90��$��12(�$
��Bold"��� -   $ accentLabel��$��Accent:��$
��Left=�$��23,7,0,0q�$	��Top=�$��90��$��12(�$
��Bold"��� -   $ greenButton��$�� ��$
��Left=�$��	23,38,0,0q�$	��Top=�$��42��$��34��
���2�2���Z�   ��Name��%:  '��       �� " ��.���  � ��$	 5.0q���  � �� �� " ��.R��� $ Border��$��3q����	#FF32CD32._��� $��2q�$��Center=�$��Center=���.  P�  �@J�IsKeyboardFocused . . ��� $ Border�� � �� �	#FF4F4F4FP�  �@��IsDefaulted . . ��� $ Border�� � �� �#202020P�  �@J�IsMouseOver . . ��� $ Border�� � �� �	#FF4F4F4FP�  � �� . . ��� $ Border�� � �� �	#FF4F4F4FP�  � {� .  . ��� $ Border�� � �� �	#FF4F4F4F��  � �� �#888888�� -   $ 
blueButton��$�� ��$
��Left=�$��	86,38,0,0q�$	��Top=�$��42��$��34�����	#FF4AA4F0��Z�  ��%:  '��       �� " ��.���  � ��$	 3.0q���  � �� �� " ��.R��� $ Border��$��2q����	#FF4AA4F0._��� $��2q�$��Center=�$��Center=���.  P�  �@J�IsKeyboardFocused . . ��� $ Border�� � �� �	#FF4F4F4FP�  �@��IsDefaulted . . ��� $ Border�� � �� �#202020P�  �@J�IsMouseOver . . ��� $ Border�� � �� �	#FF4F4F4FP�  � �� . . ��� $ Border�� � �� �	#FF4F4F4FP�  � {� .  . ��� $ Border�� � �� �	#FF4F4F4F��  � �� �#888888�� -   $ purpleButton��$�� ��$
��Left=�$��
147,38,0,0q�$	��Top=�$��42��$��34�����	#FFC751C7��Z�  ��%:  '��       �� " ��.���  � ��$	 3.0q���  � �� �� " ��.R��� $ Border��$��2q����	#FFC751C7._��� $��2q�$��Center=�$��Center=���.  P�  �@J�IsKeyboardFocused . . ��� $ Border�� � �� �	#FF4F4F4FP�  �@��IsDefaulted . . ��� $ Border�� � �� �#202020P�  �@J�IsMouseOver . . ��� $ Border�� � �� �	#FF4F4F4FP�  � �� . . ��� $ Border�� � �� �	#FF4F4F4FP�  � {� .  . ��� $ Border�� � �� �	#FF4F4F4F��  � �� �#888888�� -   $ orangeButton��$�� ��$
��Left=�$��
208,38,0,0q�$	��Top=�$��42��$��34�����	#FFFBB875��Z�  ��%:  '��       �� " ��.���  � ��$	 3.0q���  � �� �� " ��.R��� $ Border��$��2q����	#FFFBB875._��� $��2q�$��Center=�$��Center=���.  P�  �@J�IsKeyboardFocused . . ��� $ Border�� � �� �	#FF4F4F4FP�  �@��IsDefaulted . . ��� $ Border�� � �� �#202020P�  �@J�IsMouseOver . . ��� $ Border�� � �� �	#FF4F4F4FP�  � �� . . ��� $ Border�� � �� �	#FF4F4F4FP�  � {� .  . ��� $ Border�� � �� �	#FF4F4F4F��  � �� �#888888�� -   $ 	redButton��$�� ��$
��Left=�$��
268,38,0,0q�$	��Top=�$��42��$��34�����	#FFF34C4C��Z�  ��%:  '��       �� " ��.���  � ��$	 3.0q���  � �� �� " ��.R��� $ Border��$��2q����	#FFF34C4C._��� $��2q�$��Center=�$��Center=���.  P�  �@J�IsKeyboardFocused . . ��� $ Border�� � �� �	#FF4F4F4FP�  �@��IsDefaulted . . ��� $ Border�� � �� �#202020P�  �@J�IsMouseOver . . ��� $ Border�� � �� �	#FF4F4F4FP�  � �� . . ��� $ Border�� � �� �	#FF4F4F4FP�  � {� .  . ��� $ Border�� � �� �	#FF4F4F4F��  � �� �#888888�� -   $ 
backButton��$
��Back��#���  $
��Left=�$��
10,307,0,0q�$	��Top=�$	��118��$��32��   �� MergedDictionaries! �� Source��   �� $H! Bpack://application:,,,/MahApps.Metro;component/Styles/Colours.xaml?��� $F! @pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml?��� $M! Gpack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml?��� $L! Fpack://application:,,,/MahApps.Metro;component/Styles/Accents/Red.xaml?��� $Q! Kpack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml?��� $I! Cpack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml?��� $\! Vpack://application:,,,/MahApps.Metro;component/Styles/Controls.AnimatedTabControl.xaml?�%    