   M S B A M L   `   `   `  ���� I  EPinkPoo Editor, Version=1.2.3.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ZerOZerO  U QMahApps.Metro, Version=0.10.0.0, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4fV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls +clr-namespace:ZerO.HelpersZerO.Helpers      ZerO.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     $localclr-namespace:ZerO     Pcontrols;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     .helpersclr-namespace:ZerO.Helpers     [ WXceed.Wpf.AvalonDock, Version=2.0.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f47xcad,http://schemas.xceed.com/wpf/xaml/avalondock  -   (  "MahApps.Metro.Controls.MetroWindow   Name$
  Main�� � Title$ PinkPoo Editor��$
��1300��$	��700�� � Icon$ Resources/icon.ico��$
��1300��$	��700�� � 
ResizeMode$ CanResizeWithGrip=�   SaveWindowPosition .    	TitleCaps .  ��. |�.  .��� .+��� -    ��Name$ logRichTextBox��
���   �" �� VerticalScrollBarVisibility$
 Auto=�$��Stretch=� �� 
LineHeight$ 6��$��
275,0,5,10q�	 �� 
IsReadOnly	 . 
 �� AutoWordSelection
 .   �� IsUndoEnabled .  $��Infinity�� J� RenderTransformOrigin$ 0.5,0.5&�$	��999��$	��137��$��Stretch=�$��Bottom=�$	��119����.  ���	#FFABADB3 � FocusVisualStyle Z�  �� SelectionBrush Z�  � LayoutTransform V� .~��� �� �� T� z�V� .~��� �� �� T� �� 
���   �$��1q�$��275,5,5,152q�$
��1012��$	��512���� -    ��Name$ 	newButton��$�� ��$
��Left=�$��
292,10,0,0q�$	��Top=�$��34��
���   �$��2q�$��30�� � ToolTip$ New tab      Ctrl+N������  �� 
TargetType" ��.R���  ��Name$ bdr_main��$��0,0,4,4q�$��0q�
���   �����  �� ImageSource$  Resources/ImageFileNew.png�� j� Stretch$ Uniform=�._��� $��Center=�$��Center=�$��8,6,8,6q�$��Content�� �� Triggers P� 
border�� P� Property � �� P� Value$	 3.0q�P�  �@J�IsMouseOver . . ���  �� 
TargetName$ bdr_main�� �� Property � �� �� Value �� $  Resources/ImageFileNew.png�� �� Opacity$	 0.7Q�$ Uniform=�P�  � �� . . ��� $ bdr_main�� � �� �� $  Resources/ImageFileNew.png��$	 0.4Q��� -   $ 
openButton��$�� ��$
��Left=�$��
331,10,0,0q�$	��Top=�$��34��
���   �$��2q�$��30��$ Open file      Ctrl+O������ " ��.R��� $ bdr_main��$��0,0,4,4q�$��0q�
���   ����� $! Resources/ImageFileOpen.png��$ Uniform=�._��� $��Center=�$��Center=�$��8,6,8,6q�$��Content�� P� 
border�� � ��$	 3.0q�P�  �@J�IsMouseOver . . ��� $ bdr_main�� � �� �� $! Resources/ImageFileOpen.png��$	 0.7Q�$ Uniform=�P�  � �� . . ��� $ bdr_main�� � �� �� $! Resources/ImageFileOpen.png��$	 0.4Q�$ Uniform=��� -   $ 
saveButton��$�� ��$
��Left=�$��
370,10,0,0q�$	��Top=�$��34��
���   �$��2q�$��30��$# Save current file      Ctrl+S������ " ��.R��� $ bdr_main��$��0,0,4,4q�$��0q�
���   ����� $! Resources/ImageFileSave.png��$ Uniform=�._��� $��Center=�$��Center=�$��8,6,8,6q�$��Content�� P� 
border�� � ��$	 3.0q�P�  �@J�IsMouseOver . . ��� $ bdr_main�� � �� �� $! Resources/ImageFileSave.png��$	 0.7Q�$ Uniform=�P�  � �� . . ��� $ bdr_main�� � �� �� $! Resources/ImageFileSave.png��$	 0.4Q�$ Uniform=��� -   $ saveAllButton��$�� ��$
��Left=�$��
409,10,0,0q�$	��Top=�$��34��
���   �$��2q�$��30��$$ Save all open files      Alt+S������ " ��.R��� $ bdr_main��$��0,0,4,4q�$��0q�
���   ����� $$ Resources/ImageFileSaveAll.png��$ Uniform=�._��� $��Center=�$��Center=�$��8,6,8,6q�$��Content�� P� 
border�� � ��$	 3.0q�P�  �@J�IsMouseOver . . ��� $ bdr_main�� � �� �� $$ Resources/ImageFileSaveAll.png��$	 0.7Q�$ Uniform=�P�  � �� . . ��� $ bdr_main�� � �� �� $$ Resources/ImageFileSaveAll.png��$	 0.4Q�$ Uniform=��� -   $ closeButton��$�� ��$
��Left=�$��
476,10,0,0q�$	��Top=�$��34��
���   �$��2q�$��30��$% Close selected tab       Ctrl+W������ " ��.R��� $ bdr_main��$��0,0,4,4q�$��0q�
���   ����� $" Resources/ImageFileClose.png��$ Uniform=�._��� $��Center=�$��Center=�$��8,6,8,6q�$��Content�� P� 
border�� � ��$	 3.0q�P�  �@J�IsMouseOver . . ��� $ bdr_main�� � �� �� $" Resources/ImageFileClose.png��$	 0.7Q�$ Uniform=�P�  � �� . . ��� $ bdr_main�� � �� �� $" Resources/ImageFileClose.png��$	 0.4Q�$ Uniform=��� -   $ closeAllButton��$�� ��$
��Left=�$��
515,10,0,0q�$	��Top=�$��34��
���   �$��2q�$��30��$&  Close all open tabs        Alt+W������ " ��.R��� $ bdr_main��$��0,0,4,4q�$��0q�
���   ����� $% Resources/ImageFileCloseAll.png��$ Uniform=�._��� $��Center=�$��Center=�$��8,6,8,6q�$��Content�� P� 
border�� � ��$	 3.0q�P�  �@J�IsMouseOver . . ��� $ bdr_main�� � �� �� $% Resources/ImageFileCloseAll.png��$	 0.7Q�$ Uniform=�P�  � �� . . ��� $ bdr_main�� � �� �� $% Resources/ImageFileCloseAll.png��$	 0.4Q�$ Uniform=��� -	   $ 	cutButton��$�� ��$
��Left=�$��
581,10,0,0q�$	��Top=�$��34��
���   �$��2q�$��30��$ Cut      Ctrl+X������ " ��.R��� $ bdr_main��$��0,0,4,4q�$��0q�
���   ����� $  Resources/ImageEditCut.png��$ Uniform=�._��� $��Center=�$��Center=�$��8,6,8,6q�$��Content�� P� 
border�� � ��$	 3.0q�P�  �@J�IsMouseOver . . ��� $ bdr_main�� � �� �� $  Resources/ImageEditCut.png��$	 0.7Q�$ Uniform=�P�  � �� . . ��� $ bdr_main�� � �� �� $  Resources/ImageEditCut.png��$	 0.4Q�$ Uniform=��� -
   $ 
copyButton��$�� ��$
��Left=�$��
620,10,0,0q�$	��Top=�$��34��
���   �$��2q�$��30��$ Copy       Ctrl+C������ " ��.R��� $ bdr_main��$��0,0,4,4q�$��0q�
���   ����� $! Resources/ImageEditCopy.png��$ Uniform=�._��� $��Center=�$��Center=�$��8,6,8,6q�$��Content�� P� 
border�� � ��$	 3.0q�P�  �@J�IsMouseOver . . ��� $ bdr_main�� � �� �� $! Resources/ImageEditCopy.png��$	 0.7Q�$ Uniform=�P�  � �� . . ��� $ bdr_main�� � �� �� $! Resources/ImageEditCopy.png��$	 0.4Q�$ Uniform=��� -   $ pasteButton��$�� ��$
��Left=�$��
659,10,0,0q�$	��Top=�$��34��
���   �$��2q�$��30��$ Paste        Ctrl+V������ " ��.R��� $ bdr_main��$��0,0,4,4q�$��0q�
���   ����� $" Resources/ImageEditPaste.png��$ Uniform=�._��� $��Center=�$��Center=�$��8,6,8,6q�$��Content�� P� 
border�� � ��$	 3.0q�P�  �@J�IsMouseOver . . ��� $ bdr_main�� � �� �� $" Resources/ImageEditPaste.png��$	 0.7Q�$ Uniform=�P�  � �� . . ��� $ bdr_main�� � �� �� $" Resources/ImageEditPaste.png��$	 0.4Q�$ Uniform=��� -   $ 
undoButton��$�� ��$
��Left=�$��
735,10,0,0q�$	��Top=�$��34��
���   �$��2q�$��30��$ Undo       Ctrl+Z������ " ��.R��� $ bdr_main��$��0,0,4,4q�$��0q�
���   ����� $! Resources/ImageEditUndo.png��$ Uniform=�._��� $��Center=�$��Center=�$��8,6,8,6q�$��Content�� P� 
border�� � ��$	 3.0q�P�  �@J�IsMouseOver . . ��� $ bdr_main�� � �� �� $! Resources/ImageEditUndo.png��$	 0.7Q�$ Uniform=�P�  � �� . . ��� $ bdr_main�� � �� �� $! Resources/ImageEditUndo.png��$	 0.4Q�$ Uniform=��� -   $ 
redoButton��$�� ��$
��Left=�$��
774,10,0,0q�$	��Top=�$��34��
���   �$��2q�$��30��$ Redo       Ctrl+Y������ " ��.R��� $ bdr_main��$��0,0,4,4q�$��0q�
���   ����� $! Resources/ImageEditRedo.png��$ Uniform=�._��� $��Center=�$��Center=�$��8,6,8,6q�$��Content�� P� 
border�� � ��$	 3.0q�P�  �@J�IsMouseOver . . ��� $ bdr_main�� � �� �� $! Resources/ImageEditRedo.png��$	 0.7Q�$ Uniform=�P�  � �� . . ��� $ bdr_main�� � �� �� $! Resources/ImageEditRedo.png��$	 0.4Q�$ Uniform=��� -   $ 
findButton��$�� ��$
��Left=�$��
844,10,0,0q�$	��Top=�$��34��
���   �$��2q�$��30��$ Find       Ctrl+F������ " ��.R��� $ bdr_main��$��0,0,4,4q�$��0q�
���   ����� $! Resources/ImageEditFind.png��$ Uniform=�._��� $��Center=�$��Center=�$��8,6,8,6q�$��Content�� P� 
border�� � ��$	 3.0q�P�  �@J�IsMouseOver . . ��� $ bdr_main�� � �� �� $! Resources/ImageEditFind.png��$	 0.7Q�$ Uniform=�P�  � �� . . ��� $ bdr_main�� � �� �� $! Resources/ImageEditFind.png��$	 0.4Q�$ Uniform=��� -   $ replaceButton��$�� ��$
��Left=�$��
883,10,0,0q�$	��Top=�$��34��
���   �$��2q�$��30��$ Replace      Ctrl+H������ " ��.R��� $ bdr_main��$��0,0,4,4q�$��0q�
���   ����� $$ Resources/ImageEditReplace.png��$ Uniform=�._��� $��Center=�$��Center=�$��8,6,8,6q�$��Content�� P� 
border�� � ��$	 3.0q�P�  �@J�IsMouseOver . . ��� $ bdr_main�� � �� �� $$ Resources/ImageEditReplace.png��$	 0.7Q�$ Uniform=�P�  � �� . . ��� $ bdr_main�� � �� �� $$ Resources/ImageEditReplace.png��$	 0.4Q�$ Uniform=��� -   $ commentButton��$�� ��$
��Left=�$��
961,10,0,0q�$	��Top=�$��34��
���   �$��2q�$��30��$0 *Comment out the selected lines      Ctrl+K������ " ��.R��� $ bdr_main��$��0,0,4,4q�$��0q�
���   ����� $  Resources/ImageComment.png��$ Uniform=�._��� $��Center=�$��Center=�$��8,6,8,6q�$��Content�� P� 
border�� � ��$	 3.0q�P�  �@J�IsMouseOver . . ��� $ bdr_main�� � �� �� $  Resources/ImageComment.png��$	 0.7Q�$ Uniform=�P�  � �� . . ��� $ bdr_main�� � �� �� $  Resources/ImageComment.png��$	 0.4Q�$ Uniform=��� -   $ noCommentButton��$�� ��$
��Left=�$��1000,10,0,0q�$	��Top=�$��34��
���   �$��2q�$��30��$0 *Uncomment the selected lines        Ctrl+K������ " ��.R��� $ bdr_main��$��0,0,4,4q�$��0q�
���   ����� $" Resources/ImageNoComment.png��$ Uniform=�._��� $��Center=�$��Center=�$��8,6,8,6q�$��Content�� P� 
border�� � ��$	 3.0q�P�  �@J�IsMouseOver . . ��� $ bdr_main�� � �� �� $" Resources/ImageNoComment.png��$	 0.7Q�$ Uniform=�P�  � �� . . ��� $ bdr_main�� � �� �� $" Resources/ImageNoComment.png��$	 0.4Q�$ Uniform=�)  #Xceed.Wpf.AvalonDock.DockingManager  -   $��dockManager��$��281,40,10,177q�
���  ��   ActiveContent �� * DocumentManager.ActiveDocument+ �� Mode$ TwoWay=�   DocumentHeaderTemplate �� .R���   �� Orientation$  
Horizontal=�.+��� $��0,0,4,0q�$
��Fill=�$��15��$��15������ *
IconSource+�� $��CharacterEllipsis=����� *Title+!   Layout.! ,  &Xceed.Wpf.AvalonDock.Layout.LayoutRoot  "   	RootPanel." -  'Xceed.Wpf.AvalonDock.Layout.LayoutPanel  �  �Xceed.Wpf.AvalonDock.Layout.LayoutGroup`1[[Xceed.Wpf.AvalonDock.Layout.ILayoutPanelElement, Xceed.Wpf.AvalonDock, Version=2.0.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4]]#   Children.# 4  .Xceed.Wpf.AvalonDock.Layout.LayoutDocumentPane  -   documentPane���� -   $ ��Name$$ typeTabControl��$��10,40,0,156q�% �� TabStripPlacement$% Bottom=�
���   �$	��261��$	��441��$
��Left=�$	��Top=�$ 0.5,0.5&�$
��Auto=�$
��Auto=�$
��Left=�$	��261��$	��261�� V� .~��� �� �� T� z�V� .~��� �� �� T� .@��� -   & ��Name$& browseTabItem��$��Browse��$��0,0,10,0q�$��59��$��31��$��31����Z� ' � 	Resources' ( �� 
TargetType) ��Name   AccentColor* �� Color+ P� 
SourceName��%8  '��       �� "( ��.���  �@�FocusVisualStyle Z� ��  � �� .  ��  � ��$ 1q���  � ��$ 6,2,6,2q���  � ��$ Stretch=���  � ��$ Stretch=���  � ��$ 5����  � ��$ 5����  � �� �� " ��.R��� $
) root��$��12(�$��31��.���� $��Header����.  P�  �@��
IsSelected . . ��� $
 root�� � �� �� #* �   P�  �@��
IsSelected .  . ��� $
 root�� � �� �� $
* Gray��P� $
+ root�� �@J�IsMouseOver . . ��� $
 root�� � �� �� $* DarkGray��.��� .+�R� -   $��browseTreeView��, %� IsVirtualizing, . - %� VirtualizationMode$- 	Recycling=�$	��255��$	��402��$	��Top=�$
��Left=���Z� '   onClassItem. �� 	StaysOpen  onProjectItem  offItem   ZerO.CustomItem/ ��Name0 �� RelativeSource1 �� AncestorType2 �� Mode3 �� Triggers4 ]� 
Conditions5 �� Property6 �� Value��%�  &	       &	 �     &	 G    (�� �    * SystemColors.HighlightBrushKey+)(�� �    *1/SystemColors.InactiveSelectionHighlightBrushKey+)'  �    �� . . .@�m� $��	openClass��-   $
��Open��m� $��renameClass��-   $��Rename��m� $��deleteClass��-   $��Delete���� . . .@�m� $��newClass��-   $��Create New Class���� m� $��openProject��-   $
��Open��m� $��renameProject��-   $��Rename��m� $��deleteProject��-   $��Delete���� . . .@�m� $��
newProject��-   $��Create New Project���� #* �   �� #* �   �� "(  .���  � �� �� .R��� $  
Horizontal=�.+��� $	/ img��$��15��$��15��$
��Fill=����� *Image+0 �� "1  $2 FindAncestor=��� $	��5,0q����� ��  � ��$	 1.5q�3 P�  �@J�IsMouseOver . . ���  � ��
 �   �P�  �@Q�
IsSelected . . ���  � ��
 �   ���  � �� �	#FF65A4FF]� 4 �� 5 �@Q�
IsSelected6 . �� 5 �@Q�IsSelectionActive6 . �� -   $& explorerTabItem��$��Script Explorer��$��47��$��31��$��31��$��	-8,0,11,0q���Z� ' ��%8  '��       �� "( ��.���  �@�FocusVisualStyle Z� ��  � �� .  ��  � ��$ 1q���  � ��$ 6,2,6,2q���  � ��$ Stretch=���  � ��$ Stretch=���  � ��$ 5����  � ��$ 5����  � �� �� " ��.R��� $
) root��$��12(�$��31��.���� $��Header����.  P�  �@��
IsSelected . . ��� $
 root�� � �� �� #* �   P�  �@��
IsSelected .  . ��� $
 root�� � �� �� $
* Gray��P� $
+ root�� �@J�IsMouseOver . . ��� $
 root�� � �� �� $* DarkGray��.��� .+�R� -    7 R�Name$7 exploreTreeView��, . $- 	Recycling=�$	��Top=�$��0,0,1,0q���Z� '    ZerO.Helpers.Function&	    ZerO.Helpers.TagToImageConverter8 	  Instance9 �� 	Converter��%c  (��       * SystemColors.HighlightBrushKey+)(��      *1/SystemColors.InactiveSelectionHighlightBrushKey+)'       �� #* �   �� #* �   �� "(  .���  � �� �� .R��� $  
Horizontal=�$��20��.+��� $	/ img��$��15��$��15��$	��0,0q�$	��Top=����� *Tag+#9 Z8 0 �� "1  $2 FindAncestor=��� $	��5,1q�$	��Top=����� ��  � ��$	 1.5q�3 P�  �@J�IsMouseOver . . ���  � ��
 �   �P�  �@Q�
IsSelected . . ���  � ��
 �   ���  � �� �	#FF65A4FF]� 4 �� 5 �@Q�
IsSelected6 . �� 5 �@Q�IsSelectionActive6 .  �� -!   $& testMapTabItem��$��	Test Maps��$��-6,0,6,0q�$��47��$��31��$��31����Z� '   
GrayNormal  	GrayHover��%,  '��       �� "( ��.���  �@�FocusVisualStyle Z� ��  � �� .  ��  � ��$ 1q���  � ��$ 6,2,6,2q���  � ��$ Stretch=���  � ��$ Stretch=���  � ��$ 5����  � ��$ 5����  � �� �� " ��.R��� $
) root��$��12(�$��31��.���� $��Header����.  P�  �@��
IsSelected . . ��� $
 root�� � �� �� #* �   P�  �@��
IsSelected .  . ��� $
 root�� � �� �� #* �  P� $
+ root�� �@J�IsMouseOver . . ��� $
 root�� � �� �� #* �  .��� .+�R� -"   $7 testMapTreeView��$
��Left=�, . $- 	Recycling=�$	��Top=�$	��254����Z� '   
offMapItem  	onMapItem��%�  &	       &	 D     (�� z     * SystemColors.HighlightBrushKey+)(�� �     *1/SystemColors.InactiveSelectionHighlightBrushKey+)'  �     �� . . .@�m� $��
newtestMap��-#   $��Create New Test Map���� . . .@�m� $��	deleteMap��-$   $��Delete���� #* �   �� #* �   �� "(  .���  � �� �� .R��� $  
Horizontal=�.+��� $	/ img��$��15��$��15��$
��Fill=����� *Image+0 �� "1  $2 FindAncestor=��� $	��5,0q����� ��  � ��$	 1.5q�3 P�  �@J�IsMouseOver . . ���  � ��
 �   �P�  �@Q�
IsSelected . . ���  � ��
 �   ���  � �� �	#FF65A4FF]� 4 �� 5 �@Q�
IsSelected6 . �� 5 �@Q�IsSelectionActive6 .  �� -%   $& searchTabItem��$��Search��$��47��$��47��$��31��$��31��$��-1,0,1,0q���Z� ' ��%8  '��       �� "( ��.���  �@�FocusVisualStyle Z� ��  � �� .  ��  � ��$ 1q���  � ��$ 6,2,6,2q���  � ��$ Stretch=���  � ��$ Stretch=���  � ��$ 5����  � ��$ 5����  � �� �� " ��.R��� $
) root��$��12(�$��31��.���� $��Header����.  P�  �@��
IsSelected . . ��� $
 root�� � �� �� #* �   P�  �@��
IsSelected .  . ��� $
 root�� � �� �� $
* Gray��P� $
+ root�� �@J�IsMouseOver . . ��� $
 root�� � �� �� $* DarkGray��.��� .+�R� -&   $7 searchTreeView��$
��Left=�, . $- 	Recycling=�$ Alt+F to search for files��$	��Top=�$	��254����Z� ' ��%B  (��       * SystemColors.HighlightBrushKey+)(��      *1/SystemColors.InactiveSelectionHighlightBrushKey+)'       �� #* �   �� #* �   �� "(  .���  � �� �� .R��� $  
Horizontal=�.+��� $	/ img��$��15��$��15��$
��Fill=����� *Image+0 �� "1  $2 FindAncestor=��� $	��5,0q����� ��  � ��$	 1.5q�3 P�  �@J�IsMouseOver . . ���  � ��
 �   �P�  �@Q�
IsSelected . . ���  � ��
 �   ���  � �� �	#FF65A4FF]� 4 �� 5 �@Q�
IsSelected6 . �� 5 �@Q�IsSelectionActive6 .  �� -'   $ compileButton��  SquareButtonStyle#���  $��Compile Scripts��$��	43,0,0,27q�$��Bottom=�$
��Left=�$	��190��$��64��$��Center=�$��Center=�$��22��$	��172���� -(   $ editorButton��#���  $��Start UDK Editor��$��	43,0,0,96q�$��Bottom=�$
��Left=�$	��190��$��34��$��Center=�$��Center=�$��33��$	��173���� -)   $ cheerUpButton��$��:(��$��	238,5,0,0q�$	��Top=�$
��Left=�$��10��$��27��$��Center=�$��Center=�$��25��$��32��$ Cheer me up!����Z� �� -*   : ��Name$: totalTextBlock��$��290,0,0,156q�$
��Wrap=�$��TOTAL��$
��Bold"�$
��Left=�$��Bottom=�����  	 
BlackColor#* � 	 �� -+   $: lengthTextBlock��$��433,0,0,156q�$
��Wrap=�$��	length: 0��$
��Left=�$��Bottom=�$��77���� -,   $: linesTextBlock��$��356,0,0,156q�$
��Wrap=�$��lines: 0��$
��Left=�$��Bottom=�$��72���� $��527,0,0,152q�$ 0.5,0.5&�$��18��$��26��$
��Left=�$��Bottom=�z�V� .~��� �� �� ; �� Angle$	; -90Q�T� �� --   $: currentTextBlock��$��596,0,0,156q�$
��Wrap=�$��CURRENT��$
��Bold"�$
��Left=�$��Bottom=��� -.   $: lnTextBlock��$��684,0,0,156q�$
��Wrap=�$��line: 1��$
��Left=�$��Bottom=�$��46���� -/   $: colTextBlock��$��735,0,0,156q�$
��Wrap=�$��col: 1��$
��Left=�$��Bottom=�$��49���� $��832,0,0,152q�$ 0.5,0.5&�$��18��$��26��$
��Left=�$��Bottom=�z�V� .~��� �� �� $	; -90Q�T� �� -0   $: selectedTextBlock��$��912,0,0,156q�$
��Wrap=�$��SELECTED��$
��Bold"�$
��Left=�$��Bottom=��� -1   $: selTextBlock��$��1000,0,0,156q�$
��Wrap=�$��0��$
��Left=�$��Bottom=�$��46��o� -2   < o�Name$< mainMenu��$��24��$
��Bold"�$��13(�$
��Left=�$	��Top=�$	��216����Z� .@�m� $
��_New����.  .@�m� -3   = m�Name$= newProjectMenuItem��$��_Project��m� -4   $= newClassMenuItem��$��_Class��m� -5   $= newTestMapMenuItem��$��_TestMap��m� $��_Open����.  .@�m� -6   $= udkRootMenuItem��$��_UDK Root Directory��m� -7   $= defaultEngMenuItem��$��_DefaultEngine.ini���� m� -8   $= runEditorMenuItem��$��_UDK Editor��m� -9   $= runGameMenuItem��$��	_UDK Game��m� -:   $= runFrontendMenuItem��$��_Unreal Frontend���� m� -;   $= lastLogMenuItem��$��	_Last Log��m� $��_Reload����.  .@�m� -<   $= reloadAllMenuItem��$
��_All���� m� -=   $= reloadProjectsMenuItem��$��_Projects Folder��m� ->   $= reloadTestMapsMenuItem��$��_TestMaps Folder���� m� -?   $= reloadScintillaMenuItem��$��_UnrealScript.xml���� -@   > ��Name$> zoomCB��$��Right=�$��
0,0,11,156q�$��Bottom=�$��53��$��19��? �� 
IsReadOnly? . @ �� 
IsEditable@ . $��10���� $��Zoom factor:��$��Right=�$��
0,0,67,152q�$��Bottom=�$��71��$��26��$��11(�$% Ctrl+Mouse Wheel to zoom in/out��A   WindowCommandsA +
  %MahApps.Metro.Controls.WindowCommands
  .@��� -A   $ settingsButton��$��settings���� -B   $ aboutButton��$��about��' B �� MergedDictionariesC �� Source�� B �� $HC Bpack://application:,,,/MahApps.Metro;component/Styles/Colours.xaml?��� $FC @pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml?��� $LC Fpack://application:,,,/MahApps.Metro;component/Styles/Accents/Red.xaml?��� $QC Kpack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml?��� $IC Cpack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml?��� $\C Vpack://application:,,,/MahApps.Metro;component/Styles/Controls.AnimatedTabControl.xaml?�%    