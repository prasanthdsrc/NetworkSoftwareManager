   M S B A M L   `   `   `  ����Q  MNetworkSoftwareManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:NetworkSoftwareManager.ViewsNetworkSoftwareManager.Views  5      U/clr-namespace:NetworkSoftwareManager.ViewModels!NetworkSoftwareManager.ViewModels  5      5    /NetworkSoftwareManager.Views.SoftwareUpdateView   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      <local*clr-namespace:NetworkSoftwareManager.Views     5      _ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      @
viewmodels/clr-namespace:NetworkSoftwareManager.ViewModels  5        N� Title$  Software Update��5       � DataContext 5      ?   9NetworkSoftwareManager.ViewModels.SoftwareUpdateViewModel  5   
   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��Software Updates��5       $   MaterialDesignHeadline4TextBlock#��[  5      $��	16,16,0,8q�5      �� 5   
   $��1��6   $��
16,0,16,16q�6"   .@�5!      �� $��Create Update��6   .��5"      �  5#      �� 5$      $
��Auto �6(   �� 5%      $��* �6(   �� 5&      $
��Auto �6(    � ColumnDefinitions 5(      �� 5)      $��* �6+   �� 5*      $��* �6+   .+�5.      #  MaterialDesignThemes.Wpf.Card  $��0��6*   $��0��67   $��2��6G   $��0,0,0,16q�6[   .��5/      � $��16q�6    50      �� 51   "   $
��Auto �60   �� 52   "   $
��Auto �60   �� 53   "   $
��Auto �60   �� 54   "   $
��Auto �60    56      �� 57   "   $
��Auto �63   �� 58   "   $��* �63   �� 59   "   $
��Auto �63   �� 5:   "   $��* �63   .+�5=      �� $��0��6(   $��0��65   $��Update Name:��5>   (   $��Center=�5?   (   $��0,0,8,0q�5@   (   �� 5A      $��0��6&   $��1��63   $��0,0,16,0q�5C   &   ��5B   &   �� *
UpdateName+5C   &   �� 5E      $��0��6(   $��2��65   $��Target Version:��5F   (   $��Center=�5G   (   $��0,0,8,0q�5H   (   �� 5I      $��0��6&   $��3��63   ��5J   &   �� *TargetVersion+{�5K   &   �� *UseLatestVersion+  InverseBooleanConverter �� 	Converter# [ �� 5M      $��1��6'   $��1��64   $��Use Latest Version��5N   '   $��0,8,0,0q�5P   '    d� 	IsChecked 5O   '   �� *UseLatestVersion+5P   '   �� 5R      $��1��6'   $��3��64   $��Force Reinstall��5S   '   $��0,8,0,0q�5U   '    5T   '   �� *ForceReinstall+5U   '   �� 5W      $��2��6(   $��0��65   $��Install Command:��5X   (   $��Center=�5Y   (   $��0,8,8,0q�5Z   (   �� 5[      $��2��6&   $��1��63   $��3��6C   $��0,8,0,0q�5]   &   ��5\   &   �� *InstallCommandTemplate+5]   &   �� 5_      $��3��6(   $��0��65   $��Uninstall Command:��5`   (   $��Center=�5a   (   $��0,8,8,0q�5b   (   �� 5c      $��3��6&   $��1��63   $��0,8,16,0q�5e   &   ��5d   &   �� *UninstallCommandTemplate+5e   &   �� 5g      $��3��6(   $��2��65   $��Update File:��5h   (   $��Center=�5i   (   $��0,8,8,0q�5j   (   � 5k      $��3��6#   $��3��60   $��0,8,0,0q�6@    5l   "   �� 5m   &   $��* �67   �� 5n   &   $
��Auto �67   .+�5p   "   �� $��0��6*    �� 
IsReadOnly$
 True��5r   *   ��5q   *   �� *UpdateFilePath+5r   *   �� 5s   "   $��1��6)     MaterialDesignIconButton#��[ 5u   )   $��4,0,0,0q�5v   )    � ToolTip$ Browse for Update File��5w   )   ��5t   )   �� *BrowseUpdateFileCommand+.��5x   &   '  !MaterialDesignThemes.Wpf.PackIcon     Kind$ 
FolderOpen=�6>   5y   #   5z      5{      5|        5      $��1��6*   $��0��67   $��0,0,8,0q�6G   .��5�      � $��16q�6    5�      �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60   �� 5�   "   $��* �60   .+�5�      �� $��0��6(   $��Target Machines��5�   (    $  MaterialDesignHeadline6TextBlock#��[ 5�   (   $��0,0,0,8q�5�   (   �� 5�      $��1��6)   	 �� Orientation$	 
Horizontal=�66   $��0,0,0,8q�6O   .+�5�   "   ��    MaterialDesignOutlinedButton#��[ 5�   )   $ Select All Machines��5�   )   $��0,0,8,0q�5�   )   ��5�   )   �� *SelectAllMachinesCommand+.��5�   &   �� $��
Select All��60   5�   #   �� 5�   "   #��[ 5�   )   $ Deselect All Machines��5�   )   ��5�   )   �� *DeselectAllMachinesCommand+.��5�   &   �� $��Deselect All��60   5�   #   5�      &   System.Windows.Controls.DataGrid  5�      $��2��6'   
   AutoGenerateColumns$
 False��5�   '      CanUserAddRows$ False��5�   '      CanUserDeleteRows$ False��5�   '      
IsReadOnly$ False��5�   '   ��5�   '   �� *TargetMachines+ �� SelectedItem 5�   '   �� *SelectedMachine+   Columns 5�   "   4  .System.Windows.Controls.DataGridCheckBoxColumn  5�   &   ,  &System.Windows.Controls.DataGridColumn   Header$ Select��6=      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6�   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6M   �� *
IsSelected+ �� UpdateSourceTrigger$ PropertyChanged=�6�   0	  *System.Windows.Controls.DataGridTextColumn	  5�   &   $ Hostname��69   $ * 6h      
IsReadOnly$
 True��6r    6K   �� *
Hostname+6r   	  5�   &   $ 
IP Address��69   $
 Auto 6k   $
 True��6x    6M   �� *	IPAddress+6x   4
  .System.Windows.Controls.DataGridTemplateColumn
  5�   &   $ Status��6=   $
 Auto 6M   $
 True��6Z    
  CellTemplate 5�   *   �� 5�   .   .R�5�   2   �� $	 
Horizontal=�6=   .+�5�   6     $ CheckCircle=�6N   
��� � �$��Center=�5�   M   y�5�   M   �� *
IsOnline+   BooleanToVisibilityConverter# [   5�   6   $ CloseCircle=�6N   
���  ��$��Center=�5�   M   y�5�   M   �� *
IsOnline+ ' #InverseBooleanToVisibilityConverter# [ 5�   3   5�   /   5�   '   5�      5�      5�        5�      $��1��6*   $��1��67   $��8,0,0,0q�6G   .��5�      � $��16q�6    5�      �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60   �� 5�   "   $��* �60   .+�5�      �� $��0��6(   $��Software to Update��5�   (   #��[ 5�   (   $��0,0,0,8q�5�   (   �� 5�      $��1��6)   $	 
Horizontal=�66   $��0,0,0,8q�6O   .+�5�   "   �� #��[ 5�   )   $ Select All Software��5�   )   $��0,0,8,0q�5�   )   ��5�   )   �� *SelectAllSoftwareCommand+.��5�   &   �� $��
Select All��60   5�   #   �� 5�   "   #��[ 5�   )   $ Deselect All Software��5�   )   ��5�   )   �� *DeselectAllSoftwareCommand+.��5�   &   �� $��Deselect All��60   5�   #   5�        5�      $��2��6'   $
 False��5�   '   $ False��5�   '   $ False��5�   '   $ False��5�   '   ��5�   '   �� *SoftwareToUpdate+ 5�   '   �� *SelectedSoftware+ 5�   "     5�   &   $ Select��6=   $
 Auto 6�    6M   �� *
IsSelected+$ PropertyChanged=�6�   	  5�   &   $
 Name��69   $ * 6`   $
 True��6j    6G   �� *Name+6j   	  5�   &   $ 	Publisher��69   $ * 6j   $
 True��6t    6L   �� *	Publisher+6t   	  5�   &   $ Version��69   $
 Auto 6o   $
 True��6|    6J   �� *InstalledVersion+6|   	  5�   &   $ Latest��69   $
 Auto 6k   $
 True��6x    6I   �� *LatestVersion+6x   
  5�   &   $ Update��6=   $
 Auto 6M   $
 True��6Z    5�   *   �� 5�   .   .R�5�   2     $ Update=�6J   
��� ���$��Center=�5�   I   y�5�   I   �� *UpdateAvailable+# [ 5�   /   5�   '   5�      5�      5�      �� 5�      $��2��6   $��0��6*   $��2��6:     MaterialDesignRaisedButton#��[ 5�      $��Center=�5�      $��0,8,0,0q�5�      ��5�      �� *CreateUpdateTaskCommand+.��5�      �� $	 
Horizontal=�6%   .+�5�        $ PlaylistPlus=�66   $��0,0,8,0q�6J   �� 5�      $��Create Update Task��6(   5�      5�      5�      5      �� 5     $��Pending Updates��6   .��5     �  5     �� 5     $
��Auto �6(   �� 5     $��* �6(   .+�5     � $��0��6   $��0,8,0,16q�6(   y�5     �� *IsUpdateRunning+# [  5     �� 5     $
��Auto �6,   �� 5     $
��Auto �6,    5     �� 5     $��* �6/   �� 5     $
��Auto �6/   .+�5     �� $��0��6$   $��0��61   $��2��6A   $��Deploying Update...��5  $   $��0,0,0,4q�5  $   � 5     $��1��6&   $��0��63   $��10��5  &    � Maximum$	 100Q�5  &    � Value 5  &   �� *UpdateProgress+5  &   �� 5     $��1��6$   $��1��61   $��8,0,0,0q�5!  $   ��5   $   �� *UpdateProgress+ �� StringFormat$ {0:0}%��5!  $   5"       5%     $��1��6   $
 False��5(     $ False��5)     $ False��5*     $
 True��5+     ��5&     �� *PendingUpdates+ 5'     �� *SelectedUpdate+ 5,     	  5-     $ Software��61   $ * 6d    6C   �� *SoftwareName+6d   	  5.     $ 	Publisher��61   $ * 6b    6D   �� *	Publisher+6b   	  5/     $ Target Version��61   $
 Auto 6I    50  "   `� 51  &   $ 	{0} ({1})��63   .1�52  *   ��  �� Path$ TargetVersion�62   �� 53  *   $ UseLatestVersion�62    # BooleanToLatestVersionConverter# [ 6J   54  '   56     	  57     $ Machines��61   $
 Auto 6j    6C   �� *TargetMachineCount+6j   	  58     $ Created��61   $
 Auto 6�    6B   �� *CreatedDate+$ {0:yyyy-MM-dd HH:mm}��6�   
  59     $ Status��65   $
 Auto 6E    5:  "   �� 5;  &   .R�5<  *   �� $��SemiBold"�6L   ��64   �� *Status+��5=  .   �� 5>  2    �� 
TargetType" ��68    	 MaterialDesignTextBlock �� BasedOn# [	 6O    �� Triggers 5?  6   �� 5@  :    �� Binding 6F   �� *Status+ �� Value 	Completed6a   .a�5A  >   ��  �� Property � ��  �� Value
  � � �6[   5B  ;   �� 5C  :    6F   �� *Status+ In Progress6a   .a�5D  >   ��  � ��
  ��  �6[   5E  ;   �� 5F  :    6F   �� *Status+ Pending6a   .a�5G  >   ��  � ��
  � ���6[   5H  ;   �� 5I  :    6F   �� *Status+
 Failed6a   .a�5J  >   ��  � ��
  �  ��6[   5K  ;   5M  3   5O  +   5P  '   5R     
  5S     $ Progress��65   $
 Auto 6G    5T  "   �� 5U  &   .R�5V  *   � $��80��5W  6   $��12��5X  6   ! � Minimum$! 0Q�5Y  6   $	 100Q�5Z  6    5V  6   �� *ProgressPercentage+5Z  6   5[  '   5]     
  5^     $
 Auto 65    5_  "   �� 5`  &   .R�5a  *   �� $	 
Horizontal=�65   .+�5b  .   �� #��[ 5d  5   $ Deploy Update��5e  5   ��5b  5   �� *!DataContext.DeployUpdateCommand+" �� RelativeSource" �� # �� AncestorType# DataGrid��5c  5   �� y�5f  5   �� *Status+ &
 "StringToPendingVisibilityConverter# [
 .��5g  2     $
 Play=�6J   5h  /   �� 5i  .   #��[ 5k  5   $ Remove Update��5l  5   ��5i  5   �� *!DataContext.RemoveUpdateCommand+" �� # DataGrid��5j  5   �� .��5m  2     $ Delete=�6J   5n  /   �� 5o  .   #��[ 5q  5   $ View Details��5r  5   ��5o  5   �� *&$DataContext.ViewUpdateDetailsCommand+" �� # DataGrid��5p  5   �� .��5s  2     $ Information=�6J   5t  /   5u  +   5v  '   5x     5z     5{     5|     �� 5     $��Completed Updates��6   .��5�       $
 False��5�     $ False��5�     $ False��5�     $
 True��5�     ��5�     �� *CompletedUpdates+ 5�     �� *SelectedUpdate+ 5�     	  5�     $ Software��6-   $ * 6`    6?   �� *SoftwareName+6`   	  5�     $ 	Publisher��6-   $ * 6^    6@   �� *	Publisher+6^   	  5�     $ Target Version��6-   $
 Auto 6E    5�     `� 5�  "   $ 	{0} ({1})��6/   .1�5�  &   �� $ TargetVersion�6.   �� 5�  &   $ UseLatestVersion�6.   # [ 6F   5�  #   5�     	  5�     $ Machines��6-   $
 Auto 6f    6?   �� *TargetMachineCount+6f   	  5�     $ Created��6-   $
 Auto 6}    6>   �� *CreatedDate+$ {0:yyyy-MM-dd}��6}   	  5�     $ 	Completed��6-   $
 Auto 6�    6@   �� *CompletedDate+$ {0:yyyy-MM-dd HH:mm}��6�   	  5�     $ Success��6-   $
 Auto 6_    6>   �� *SuccessCount+$   	CellStyle$ 5�     �� 5�  "   *  $System.Windows.Controls.DataGridCell"  6(     MaterialDesignDataGridCell# [ 6B   .�5�  &   ��  � ��
  � � �6C   5�  #   5�     	  5�     $ Failed��6-   $
 Auto 6^    6=   �� *FailureCount+$ 5�     �� 5�  "   "  6(   # [ 6B   .�5�  &   ��  � ��
  �  ��6C   5�  #   5�     
  5�     $
 Auto 61    5�     �� 5�  "   .R�5�  &   �� $	 
Horizontal=�61   .+�5�  *   �� #��[ 5�  1   $ Remove Update��5�  1   ��5�  1   �� *!DataContext.RemoveUpdateCommand+" �� # DataGrid��5�  1   �� .��5�  .     $ Delete=�6F   5�  +   �� 5�  *   #��[ 5�  1   $ View Details��5�  1   ��5�  1   �� *&$DataContext.ViewUpdateDetailsCommand+" �� # DataGrid��5�  1   �� .��5�  .     $ Information=�6F   5�  +   5�  '   5�  #   5�     5�     5�     5�     �� 5�  
   $��2��6   .@�5�     �� .��5�     �� $	 
Horizontal=�6   .+�5�     �� #��[ 5�     $ Refresh Updates��5�     ��5�     �� *RefreshCommand+.��5�       $ Refresh=�62   5�     �� 5�     $��8,0,0,0q�6?   ��6    �� *StatusMessage+6?   5�     5�     �� 5�     $��Right=�6   .��5�     �� $	 
Horizontal=�6   .+�5�     �� #��[ 5�     $ Cancel Update��5�     ��5�     �� *CancelUpdateCommand+y�5�     �� *IsUpdateRunning+# [ .��5�       $ Cancel=�62   5�     5�     5�     5�     5�     5�     