   M S B A M L   `   `   `  ����Q  MNetworkSoftwareManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:NetworkSoftwareManager.ViewsNetworkSoftwareManager.Views  5      U/clr-namespace:NetworkSoftwareManager.ViewModels!NetworkSoftwareManager.ViewModels  5      2    ,NetworkSoftwareManager.Views.NetworkScanView   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      <local*clr-namespace:NetworkSoftwareManager.Views     5      _ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      @
viewmodels/clr-namespace:NetworkSoftwareManager.ViewModels  5        N� Title$  Network Scan��5       � DataContext 5      <   6NetworkSoftwareManager.ViewModels.NetworkScanViewModel  5   
   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��Network Scan��5       $   MaterialDesignHeadline4TextBlock#��[  5      $��	16,16,0,8q�5      #  MaterialDesignThemes.Wpf.Card  5    
   $��1��6   $��
16,0,16,16q�6+   .��5!      � $��16q�6    5"      �� 5#      $
��Auto �6$   �� 5$      $
��Auto �6$    � ColumnDefinitions 5&      �� 5'      $��* �6'   �� 5(      $
��Auto �6'   .+�5,      � $��0��6   $��0��6$    5-      �� 5.      $
��Auto �6(   �� 5/      $
��Auto �6(   �� 50      $
��Auto �6(    52      �� 53      $
��Auto �6+   �� 54      $��* �6+   �� 55      $
��Auto �6+   �� 56      $��* �6+   .+�59      �� $��0��6    $��0��6-   $��	IP Range:��6=   $��Center=�6N   $��0,0,8,0q�6i   �� 5:      $��0��6   $��1��6+   $��0,0,16,0q�6Z    � ToolTip$9 3Enter IP range in format: 192.168.1.1-192.168.1.254��5;      ��5:   ;   �� *IPRangeToScan+5;      �� 5=      $��0��6    $��2��6-   $��Excluded IPs:��6=   $��Center=�6R   $��0,0,8,0q�6m   �� 5>      $��0��6   $��3��6+   $3 -Enter comma separated IP addresses to exclude��5?      ��5>   ;   �� *ExcludedIPs+5?      �� 5A      $��1��6    $��0��6-   $��Scan Timeout (ms):��6=   $��Center=�6W   $��0,8,8,0q�6r   �� 5B      $��1��6   $��1��6+   $��0,8,16,0q�6X   $0 *Timeout in milliseconds for ping responses��5C      ��5B   ;   �� *ScanTimeout+5C      �� 5E      $��1��6    $��2��6-   $��Thread Count:��6=   $��Center=�6R   $��0,8,8,0q�6m   �� 5F      $��1��6   $��3��6+   $��0,8,0,0q�6X   $) #Number of parallel scanning threads��5G      ��5F   ;   �� *ThreadCount+5G      5H      �� 5K      $��0��6   $��1��6*    �� Orientation$ 
Horizontal=�6:   $��Bottom=�6S   .+�5L      ��   MaterialDesignRaisedButton#��[ 5M      $ Start Network Scan��5N      $��0,0,8,0q�5O      ��5L      �� *ScanNetworkCommand+{�5P      �� *IsNetworkScanRunning+  InverseBooleanConverter �� 	Converter# [ .��5Q      �� $ 
Horizontal=�6%   .+�5R      '  !MaterialDesignThemes.Wpf.PackIcon     Kind$	 Lan=�66   $��0,0,8,0q�6A   �� 5S      $��Scan Network��6(   5T      5U      �� 5W         MaterialDesignOutlinedButton#��[ 5X      $ Cancel Current Scan��5Y      ��5W      �� *CancelScanCommand+y�5Z      �� *IsNetworkScanRunning+   BooleanToVisibilityConverter# [ .��5[      �� $ 
Horizontal=�6%   .+�5\        $ Cancel=�66   $��0,0,8,0q�6D   �� 5]      $��Cancel��6(   5^      5_      5`      �� 5c      $��1��6   $��0��6*   $��2��6:   $��0,8,0,0q�6N   .+�5d      �  5e      �� 5f      $��* �6/   �� 5g      $
��Auto �6/   .+�5j      � $��0��6&   $��10��6U    � Maximum$	 100Q�6a   	 � Value	 66   �� *ScanProgress+y�5k   %   �� *IsNetworkScanRunning+# [ �� 5m      $��1��6$   $��8,0,0,0q�64   ��5n   #   �� *ScanProgress+
 �� StringFormat$
 {0:0}%��y�5o   #   �� *IsNetworkScanRunning+# [ 5p      �� 5r      $��0,8,0,0q�6?   ��6    �� *StatusMessage+6?   5s      5t      5u      � 5x   
   $��2��6   $��
16,0,16,16q�6    5y      �� 5z      $
��Auto �6    �� 5{      $��* �6    .+�5      �� $��0��6   $ 
Horizontal=�6&   $��0,0,0,8q�6?   .+�5�      �� #��[ 5�      $ Select All Machines��5�      $��0,0,8,0q�5�      ��5�      �� *SelectAllCommand+.��5�      �� $��
Select All��6    5�      �� 5�      #��[ 5�      $ Deselect All Machines��5�      $��0,0,8,0q�5�      ��5�      �� *DeselectAllCommand+.��5�      �� $��Deselect All��6    5�      �� 5�      #��[ 5�      $ Save Selected Machines��5�      $��0,0,8,0q�5�      ��5�      �� *SaveSelectedCommand+.��5�      �� $ 
Horizontal=�6!   .+�5�        $ ContentSave=�62   $��0,0,8,0q�6E   �� 5�      $��Save Selected��6$   5�      5�      �� 5�      #��[ 5�      $ Remove Selected Machines��5�      $��0,0,8,0q�5�      ��5�      �� *RemoveSelectedCommand+.��5�      �� $ 
Horizontal=�6!   .+�5�        $ Delete=�62   $��0,0,8,0q�6@   �� 5�      $��Remove Selected��6$   5�      5�      �� 5�      #��[ 5�      $ Import Machines from File��5�      $��0,0,8,0q�5�      ��5�      �� *ImportMachinesCommand+.��5�      �� $ 
Horizontal=�6!   .+�5�        $ Import=�62   $��0,0,8,0q�6@   �� 5�      $��Import��6$   5�      5�      �� 5�      #��[ 5�      $ Export Machines to File��5�      ��5�      �� *ExportMachinesCommand+.��5�      �� $ 
Horizontal=�6!   .+�5�        $ Export=�62   $��0,0,8,0q�6@   �� 5�      $��Export��6$   5�      5�      5�      &   System.Windows.Controls.DataGrid  5�      $��1��6      AutoGenerateColumns$ False��5�         CanUserAddRows$ False��5�         CanUserDeleteRows$ False��5�         
IsReadOnly$ False��5�      ��5�      �� *
Machines+ �� SelectedItem 5�      �� *SelectedMachine+   Columns 5�      4  .System.Windows.Controls.DataGridCheckBoxColumn  5�      ,  &System.Windows.Controls.DataGridColumn   Header$ Select��6-      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6�   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6=   �� *
IsSelected+ �� UpdateSourceTrigger$ PropertyChanged=�6�   0	  *System.Windows.Controls.DataGridTextColumn	  5�      $ Hostname��6)   $ * 6X      
IsReadOnly$
 True��6b    6;   �� *
Hostname+6b   	  5�      $ 
IP Address��6)   $
 Auto 6[   $
 True��6h    6=   �� *	IPAddress+6h   	  5�      $ OS��6)   $ * 6Y   $
 True��6c    65   �� *OperatingSystem+6c   4
  .System.Windows.Controls.DataGridTemplateColumn
  5�      $ Status��6-   $
 Auto 6=   $
 True��6J    
  CellTemplate 5�      �� 5�      .R�5�   "   �� $ 
Horizontal=�6-   .+�5�   &     $ CheckCircle=�6>   
��� � �$��Center=�5�   =   y�5�   =   �� *
IsOnline+# [   5�   &   $ CloseCircle=�6>   
���  ��$��Center=�5�   =   y�5�   =   �� *
IsOnline+ ' #InverseBooleanToVisibilityConverter# [ �� 5�   &   $��4,0,0,0q�5�   /   $��Center=�5�   /   ��5�   0   �� *
IsOnline+ " BooleanToOnlineStatusConverter# [ 5�   /   5�   #   5�      5�      	  5�      $ Last Scanned��6)   $
 Auto 6�   $
 True��6�    6?   �� *LastScanned+$
 {0:yyyy-MM-dd HH:mm}��6�   	  5�      $ Software��6)   $
 Auto 6]   $
 True��6j    6;   �� *SoftwareCount+6j   	  5�      $ Updates��6)   $
 Auto 6]   $
 True��6j    6:   �� *PendingUpdates+6j   	  5�      $ Domain��6)   $
 Auto 6T    69   �� *Domain+6T   	  5�      $ Username��6)   $
 Auto 6X    6;   �� *
Username+6X   
  5�      $ Password��6-   $
 Auto 6?    5�      �� 5�      .R�5�   "   G�  G�Name$ MachinePasswordBox��6.   -   5        )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ Machine Password��5�   -   5�   -   5�      5�      	  5�      $ Status Message��6)   $ * 6\   $
 True��6f    6A   �� *Status+6f   5�      5�      �� 5�   
   $��3��6   .@�5�      �� .��5�      �� �� 5�      $�Machines Found:��6    �� 5�      �6   �� *MachinesFound+5�      5�      �� 5�        MaterialDesignSeparator#��[ 6   �� 5�      .��5�      �� �� 5�      $�Online:��6    �� 5�      �6   �� *MachinesOnline+5�      5�      �� 5�      #��[ 6   �� 5�      .��5�      �� ��6   �� *StatusMessage+5�      5�      5�      5�      