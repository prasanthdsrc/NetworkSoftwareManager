   M S B A M L   `   `   `  ����Q  MNetworkSoftwareManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:NetworkSoftwareManager.ViewsNetworkSoftwareManager.Views  5      U/clr-namespace:NetworkSoftwareManager.ViewModels!NetworkSoftwareManager.ViewModels  5      8    2NetworkSoftwareManager.Views.SoftwareInventoryView   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      <local*clr-namespace:NetworkSoftwareManager.Views     5      _ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      @
viewmodels/clr-namespace:NetworkSoftwareManager.ViewModels  5        N� Title$  Software Inventory��5       � DataContext 5      B   <NetworkSoftwareManager.ViewModels.SoftwareInventoryViewModel  5   
   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��Software Inventory��5       $   MaterialDesignHeadline4TextBlock#��[  5      $��	16,16,0,8q�5      #  MaterialDesignThemes.Wpf.Card  5    
   $��1��6   $��
16,0,16,16q�6+   .��5!      � $��16q�6    5"      �� 5#      $
��Auto �6$   �� 5$      $
��Auto �6$    � ColumnDefinitions 5&      �� 5'      $��* �6'   �� 5(      $
��Auto �6'   .+�5,      � $��0��6   $��0��6$    5-      �� 5.      $
��Auto �6+   �� 5/      $��* �6+   .+�52      �� $��0��6    $��Select Machine:��60   $��Center=�6G   $��0,0,8,0q�6b   �� 53      $��1��6    �� DisplayMemberPath$ Hostname��56       � ToolTip$5 /Select a machine to view its software inventory��57      ��54      �� *
Machines+ �� SelectedItem 55      �� *SelectedMachine+57      58      �� 5;      $��0��6   $��1��6*    �� Orientation$ 
Horizontal=�6:   $��Center=�6S   .+�5<      ��   MaterialDesignRaisedButton#��[ 5>      $( "Scan Software for Selected Machine��5?      $��0,0,8,0q�5@      ��5<      �� *ScanSoftwareCommand+��5=      �� *SelectedMachine+{�5A      �� *IsInventoryRunning+  InverseBooleanConverter �� 	Converter# [ .��5B      �� $ 
Horizontal=�6%   .+�5C      '  !MaterialDesignThemes.Wpf.PackIcon  	   Kind$
	 Scan=�66   $��0,0,8,0q�6B   �� 5D      $��Scan Software��6(   5E      5F      �� 5H         MaterialDesignOutlinedButton#��[ 5I      $$ Scan Software for All Machines��5J      $��0,0,8,0q�5K      ��5H      �� *ScanAllSoftwareCommand+{�5L      �� *IsInventoryRunning+# [ .��5M      �� $ 
Horizontal=�6%   .+�5N        $
	 Sync=�66   $��0,0,8,0q�6B   �� 5O      $��Scan All��6(   5P      5Q      �� 5S      #��[ 5T      $ Cancel Current Scan��5U      ��5S      �� *CancelScanCommand+y�5V      �� *IsInventoryRunning+   BooleanToVisibilityConverter# [ .��5W      �� $ 
Horizontal=�6%   .+�5X        $	 Cancel=�66   $��0,0,8,0q�6D   �� 5Y      $��Cancel��6(   5Z      5[      5\      �� 5_      $��1��6   $��0��6*   $��2��6:   $��0,8,0,0q�6N   .+�5`      �  5a      �� 5b      $��* �6/   �� 5c      $
��Auto �6/   .+�5f      � $��0��6&   $��10��6Z   
 � Maximum$	
 100Q�6f    � Value 66   �� *InventoryProgress+y�5g   %   �� *IsInventoryRunning+# [ �� 5i      $��1��6$   $��8,0,0,0q�64   ��5j   #   �� *InventoryProgress+ �� StringFormat$ {0:0}%��y�5k   #   �� *IsInventoryRunning+# [ 5l      �� 5n      $��0,8,0,0q�6?   ��6    �� *StatusMessage+6?   5o      5p      5q      � 5t   
   $��2��6   $��
16,0,16,16q�6    5u      �� 5v      $
��Auto �6    �� 5w      $��* �6    .+�5{      � $��0��6   $��0,0,0,8q�6     5|      �� 5}      $��* �6'   �� 5~      $
��Auto �6'   .+�5�      �� $��0��6   )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ Search for software...��5�      .  (MaterialDesignThemes.Wpf.TextFieldAssist   HasClearButton$
 True��5�      ��5�      �� *
SearchText+ �� UpdateSourceTrigger$ PropertyChanged=�.��5�   O    J� InputBindings 5�      �� 5�      
 �� Key$ Enter��6%    �� Command 61   �� *SearchCommand+.��5�   -   5�      �� 5�      $��1��6   $ 
Horizontal=�6-   $��16,0,0,0q�6F   .+�5�      �� $��Show only updatable��6   $��Center=�5�      $��0,0,16,0q�5�       d� 	IsChecked 5�      �� *ShowOnlyUpdatable+5�      �� 5�        MaterialDesignIconButton#��[ 5�      $ Search��5�      ��5�      �� *SearchCommand+.��5�        $	 Magnify=�62   $��24��6A   $��24��6L   5�      �� 5�      #��[ 5�      $ 
Select All��5�      ��5�      �� *SelectAllSoftwareCommand+.��5�        $	 	SelectAll=�62   $��24��6C   $��24��6N   5�      �� 5�      #��[ 5�      $ Deselect All��5�      ��5�      �� *DeselectAllSoftwareCommand+.��5�        $	 SelectionOff=�62   $��24��6F   $��24��6Q   5�      �� 5�      #��[ 5�      $ Refresh Latest Versions��5�      ��5�      �� *RefreshLatestVersionsCommand+.��5�        $	 Update=�62   $��24��6@   $��24��6K   5�      5�      5�      &   System.Windows.Controls.DataGrid  5�      $��1��6      AutoGenerateColumns$ False��5�         CanUserAddRows$ False��5�         CanUserDeleteRows$ False��5�         
IsReadOnly$ False��5�      ��5�      �� *SoftwareList+ 5�      �� *SelectedSoftware+   Columns 5�      4  .System.Windows.Controls.DataGridCheckBoxColumn  5�      ,  &System.Windows.Controls.DataGridColumn   Header$ Select��6-      Width5	  /System.Windows.Controls.DataGridLengthConverter$
 Auto	 6�   1
  +System.Windows.Controls.DataGridBoundColumn 
  Binding 6=   �� *
IsSelected+$ PropertyChanged=�6�   0  *System.Windows.Controls.DataGridTextColumn  5�      $
 Name��6)   $ *	 6P      
IsReadOnly$
 True��6Z    67   �� *Name+6Z     5�      $ 	Publisher��6)   $ *	 6Z   $
 True��6d    6<   �� *	Publisher+6d     5�      $ Installed Version��6)   $
 Auto	 6i   $
 True��6v    6D   �� *InstalledVersion+6v     5�      $ Latest Version��6)   $
 Auto	 6c    6A   �� *LatestVersion+6c     5�      $ 
Use Latest��6-   $
 Auto	 6f    6A   �� *UseLatestVersion+6f     5�      $ Target Version��6)   $
 Auto	 6c    6A   �� *TargetVersion+6c   4  .System.Windows.Controls.DataGridTemplateColumn  5�      $ Update Available��6-   $
 Auto	 6G   $
 True��6T      CellTemplate 5�      �� 5�      .R�5�   "     $	 CheckCircle=�6:   
��� � �$��Center=�5�   9   y�5�   9   �� *UpdateAvailable+# [ 5�      5�        5�      $ Install Location��6)   $ *	 6g   $
 True��6q    6C   �� *InstallLocation+6q     5�      $ Install Date��6)   $
 Auto	 6~   $
 True��6�    6?   �� *InstallDate+$ {0:yyyy-MM-dd}��6�     5�      $
 Auto	 6-    5�      �� 5�      .R�5�   "   �� #��[ 5�   )   $ Set Latest Version��5�   )   ��5�   )   �� *%#DataContext.SetLatestVersionCommand+ �� RelativeSource ��  �� AncestorType DataGrid��5�   )   �� .��5�   &     $		 Tag=�6>   5�   #   5�      5�      5�      5�      �� 5�   
   $��3��6   .@�5�      �� .��5�      �� �� 5�      $�Software Count:��6    �� 5�      �6   �� *SoftwareCount+5�      5�      �� 5�        MaterialDesignSeparator#��[ 6   �� 5�      .��5�      �� �� 5�      $�
Updatable:��6    �� 5�      �6   �� *UpdatableSoftwareCount+5�      5�      �� 5�      #��[ 6   �� 5�      .��5�      �� ��6   �� *StatusMessage+5�      5�      5�      5�      