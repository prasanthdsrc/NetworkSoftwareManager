   M S B A M L   `   `   `  ����Q  MNetworkSoftwareManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:NetworkSoftwareManager.ViewsNetworkSoftwareManager.Views  5      U QLiveCharts.Wpf, Version=0.9.8.0, Culture=neutral, PublicKeyToken=0bc1f845d1ebb8dfG4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.WpfLiveCharts.Wpf 5      U/clr-namespace:NetworkSoftwareManager.ViewModels!NetworkSoftwareManager.ViewModels  5	      0    *NetworkSoftwareManager.Views.DashboardView   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      <local*clr-namespace:NetworkSoftwareManager.Views     5      _ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      >lvc4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.Wpf  5      @
viewmodels/clr-namespace:NetworkSoftwareManager.ViewModels  5	        N� Title$  	Dashboard��5       � DataContext 5      :   4NetworkSoftwareManager.ViewModels.DashboardViewModel  5   
   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��	Dashboard��5       $   MaterialDesignHeadline4TextBlock#��[  5      $��	16,16,0,8q�5      �� 5!   
   $��1��6    �� Orientation$ 
Horizontal=�6"   $��
16,0,16,16q�6;   .+�5"      ��    MaterialDesignOutlinedButton#��[ 5#       � ToolTip$ Refresh Dashboard��5$      $��0,0,8,0q�5%      ��5"      �� *RefreshCommand+.��5&      �� $ 
Horizontal=�6   .+�5'      '  !MaterialDesignThemes.Wpf.PackIcon     Kind$ Refresh=�6.   $��0,0,8,0q�6=   �� 5(      $��Refresh��6    5)      5*      �� 5,      #��[ 5-      $ Scan Network��5.      ��5,      �� *ScanNetworkCommand+.��5/      �� $ 
Horizontal=�6   .+�50        $ NetworkOutline=�6.   $��0,0,8,0q�6D   �� 51      $��Scan Network��6    52      53      �� 55      $��Center=�56      $��16,0,0,0q�57        MaterialDesignBodyLight#���  58      ��55      �� *StatusMessage+58      59      �� 5<   
   $��2��6   $
��Auto=�6$   .��5=      �  5>      �� 5?      $
��Auto �6$   �� 5@      $
��Auto �6$   �� 5A      $��* �6$   .+�5E      � $��0��6    � Orientation$ 
Horizontal=�6)   $��8q�6B   .+�5G      #  MaterialDesignThemes.Wpf.Card  $��8q�6*   $	��220��65   $	��120��6A   .��5H      � $��8q�6    5I      �� 5J   "   $
��Auto �60   �� 5K   "   $��* �60   .+�5N      �� $��0��6(   $��Machine Status��5O   (    $  MaterialDesignHeadline6TextBlock#��[ 5P   (   �� 5R      $��1��6)   $��Center=�66   .+�5S   "   �� $ 
Horizontal=�6-   $��0,4,0,0q�6F   .+�5T   &     $ ServerNetwork=�6>   $��24��5U   =   $��24��5V   =   $��Center=�5W   =   $��0,0,8,0q�5X   =   �� 5Y   &   $��Total Machines:��60   $��Center=�6G   �� 5Z   &   $
��Bold"�5[   /   $��Center=�5\   /   $��8,0,0,0q�5]   /   ��5Z   0   �� *TotalMachines+5]   /   5^   #   �� 5`   "   $ 
Horizontal=�6-   $��0,4,0,0q�6F   .+�5a   &     $ 
LanConnect=�6>   $��24��5b   =   $��24��5c   =   $��Center=�5d   =   
��� � �$��0,0,8,0q�5f   =   �� 5g   &   $��Online:��60   $��Center=�6?   �� 5h   &   $
��Bold"�5i   /   $��Center=�5j   /   
��� � �$��8,0,0,0q�5l   /   ��5h   0   �� *OnlineMachines+5l   /   5m   #   �� 5o   "   $ 
Horizontal=�6-   $��0,4,0,0q�6F   .+�5p   &     $ LanDisconnect=�6>   $��24��5q   =   $��24��5r   =   $��Center=�5s   =   
���  ��$��0,0,8,0q�5u   =   �� 5v   &   $��Offline:��60   $��Center=�6@   �� 5w   &   $
��Bold"�5x   /   $��Center=�5y   /   
���  ��$��8,0,0,0q�5{   /   ��5w   0   �� *OfflineMachines+5{   /   5|   #   5}      5~      5        5�      $��8q�6*   $	��220��65   $	��120��6A   .��5�      � $��8q�6    5�      �� 5�   "   $
��Auto �60   �� 5�   "   $��* �60   .+�5�      �� $��0��6(   $��Software Status��5�   (   #��[ 5�   (   �� 5�      $��1��6)   $��Center=�66   .+�5�   "   �� $ 
Horizontal=�6-   $��0,4,0,0q�6F   .+�5�   &     $
 Apps=�6>   $��24��5�   =   $��24��5�   =   $��Center=�5�   =   $��0,0,8,0q�5�   =   �� 5�   &   $��Total Software:��60   $��Center=�6G   �� 5�   &   $
��Bold"�5�   /   $��Center=�5�   /   $��8,0,0,0q�5�   /   ��5�   0   �� *TotalSoftware+5�   /   5�   #   �� 5�   "   $ 
Horizontal=�6-   $��0,4,0,0q�6F   .+�5�   &     $ Update=�6>   $��24��5�   =   $��24��5�   =   $��Center=�5�   =   
��� ���$��0,0,8,0q�5�   =   �� 5�   &   $��Needs Updates:��60   $��Center=�6F   �� 5�   &   $
��Bold"�5�   /   $��Center=�5�   /   
��� ���$��8,0,0,0q�5�   /   ��5�   0   �� *SoftwareWithUpdates+5�   /   5�   #   �� 5�   "   $ 
Horizontal=�6-   $��0,4,0,0q�6F   .+�5�   &     $ CloudDownload=�6>   $��24��5�   =   $��24��5�   =   $��Center=�5�   =   
����  �$��0,0,8,0q�5�   =   �� 5�   &   $��Pending Deployments:��60   $��Center=�6L   �� 5�   &   $
��Bold"�5�   /   $��Center=�5�   /   
����  �$��8,0,0,0q�5�   /   ��5�   0   �� *PendingDeployments+5�   /   5�   #   5�      5�      5�        5�      $��8q�6*   $	��250��65   $	��120��6A   .��5�      � $��8q�6    5�      �� 5�   "   $
��Auto �60   �� 5�   "   $��* �60   .+�5�      �� $��0��6(   $��Network Overview��5�   (   #��[ 5�   (   � 5�      $��1��6#   .+�5�   "     LiveCharts.Wpf.PieChart  &   LiveCharts.Wpf.Charts.Base.Chart   LegendLocation$ Right=�5�   .      	Hoverable$ False��5�   .   	   Series	 5�   /   �� *ChartSeries+
   DataTooltip
 5�   .   Z�    ChartLegend 5�   &   "  LiveCharts.Wpf.DefaultLegend  5�   *      
BulletSize$ 10Q�6<   5�   #   �� 5�   "   $��No data available��6,   $��Center=�5�   +   $��Center=�5�   +   
�������y�5�   +   �� *HasNoChartData+   BooleanToVisibilityConverter �� 	Converter# [ 5�      5�      5�      5�        5�      $��1��6&   $��	16,8,16,8q�63   .��5�      � $��16q�6    5�      �� 5�      $
��Auto �6,   �� 5�      $��* �6,   .+�5�      �� $��0��6$   $��Recently Scanned Machines��5�   $   #��[ 5�   $   $��0,0,0,8q�5�   $   &   System.Windows.Controls.DataGrid  5�      $��1��6#      AutoGenerateColumns$ False��5�   #      CanUserAddRows$ False��5�   #      CanUserDeleteRows$ False��5�   #      
IsReadOnly$
 True��5�   #   $	��200��5�   #   ��5�   #   �� *RecentlyScannedMachines+   Columns 5�      0  *System.Windows.Controls.DataGridTextColumn  5�   "   ,	  &System.Windows.Controls.DataGridColumn 	  Header$ Hostname��65    	  Width5
  /System.Windows.Controls.DataGridLengthConverter$ *
 6d   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6G   �� *
Hostname+6d     5�   "   $ 
IP Address��65   $
 Auto
 6g    6I   �� *	IPAddress+6g     5�   "   $ OS��65   $ 2*
 6e    6A   �� *OperatingSystem+6e   4  .System.Windows.Controls.DataGridTemplateColumn  5�   "   $ Status��69   $
 Auto
 6I      CellTemplate 5�   &   �� 5�   *   .R�5�   .   �� $ 
Horizontal=�69   .+�5�   2     $ CheckCircle=�6J   
��� � �$��Center=�5�   I   y�5�   I   �� *
IsOnline+# [   5�   2   $ CloseCircle=�6J   
���  ��$��Center=�5�   I   y�5   I   �� *
IsOnline+ ' #InverseBooleanToVisibilityConverter# [ �� 5  2   $��4,0,0,0q�5  <   $��Center=�5  <   ��5  <   �� *
IsOnline+ " BooleanToOnlineStatusConverter# [ 5  <   5  /   5  +   5  #     5  "   $ Last Scanned��65   $
 Auto
 6�    6K   �� *LastScanned+ �� StringFormat$ {0:yyyy-MM-dd HH:mm}��6�     5	  "   $ Software Count��65   $
 Auto
 6o    6M   �� *SoftwareCount+6o     5
  "   $ Updates��65   $
 Auto
 6i    6F   �� *PendingUpdates+6i     5  "   $
 Auto
 69    5  &   �� 5  *   .R�5  .   ��   MaterialDesignIconButton#��[ 5  5   $ View Details��5  5   ��5  5   �� *'%DataContext.ViewMachineDetailsCommand+ �� RelativeSource ��  �� AncestorType DataGrid��5  5   �� .��5  2     $ Information=�6J   5  /   5  +   5  #   5     5     5     � 5     $��2��6   $��8q�6$    � ColumnDefinitions 5     �� 5     $��* �6+   �� 5      $��* �6+   .+�5$       $��0��6*   $��8q�6:   .��5%     � $��16q�6    5&     �� 5'  "   $
��Auto �60   �� 5(  "   $��* �60   .+�5+     �� $��0��6(   $#��Top Software by Install Count��5,  (   #��[ 5-  (   $��0,0,0,8q�5.  (     50     $��1��6'   $ False��52  '   $ False��53  '   $ False��54  '   $
 True��55  '   ��51  '   �� *TopSoftwareByInstallCount+ 56  "     57  &   $
 Name��69   $ *
 6`    6G   �� *Name+6`     58  &   $ 	Publisher��69   $ *
 6j    6L   �� *	Publisher+6j     59  &   $ Version��69   $
 Auto
 6o    6J   �� *InstalledVersion+6o     5:  &   $ Installations��69   $
 Auto
 6q    6P   �� *InstallCount+6q     5;  &   $ Updates��6=   $
 Auto
 6N    5<  *   �� 5=  .   .R�5>  2     $ Alert=�6J   
��� ���$��Center=�5@  I   y�5A  I   �� *UpdateAvailable+# [ 5B  /   5D  '     5E  &   $
 Auto
 6=    5F  *   �� 5G  .   .R�5H  2   �� #��[ 5J  9   $ View Details��5K  9   ��5H  9   �� *(&DataContext.ViewSoftwareDetailsCommand+ ��  DataGrid��5I  9   �� .��5L  6     $ Information=�6N   5M  3   5N  /   5P  '   5R     5S     5T       5W     $��1��6*   $��8q�6:   .��5X     � $��16q�6    5Y     �� 5Z  "   $
��Auto �60   �� 5[  "   $��* �60   .+�5^     �� $��0��6(   $��Recent Deployments��5_  (   #��[ 5`  (   $��0,0,0,8q�5a  (     5c     $��1��6'   $ False��5e  '   $ False��5f  '   $ False��5g  '   $
 True��5h  '   ��5d  '   �� *RecentDeployments+ 5i  "     5j  &   $ Software��69   $ *
 6l    6K   �� *SoftwareName+6l     5k  &   $ Version��69   $
 Auto
 6l    6J   �� *TargetVersion+6l     5l  &   $ Created��69   $
 Auto
 6�    6J   �� *CreatedDate+$ {0:yyyy-MM-dd}��6�     5m  &   $ Status��6=   $
 Auto
 6M    5n  *   �� 5o  .   .R�5p  2   �� $��SemiBold"�6T   ��6<   �� *Status+��5q  6   �� 5r  :    �� 
TargetType" ��6@     MaterialDesignTextBlock �� BasedOn# [ 6W    �� Triggers 5s  >   �� 5t  B    �� Binding 6N   �� *Status+ �� Value 	Completed6i   .a�5u  F   ��   �� Property  � ��! �� Value
! � � �6c   5v  C   �� 5w  B    6N   �� *Status+ In Progress6i   .a�5x  F   ��   � ��
! ��  �6c   5y  C   �� 5z  B    6N   �� *Status+ Pending6i   .a�5{  F   ��   � ��
! � ���6c   5|  C   �� 5}  B    6N   �� *Status+
 Failed6i   .a�5~  F   ��   � ��
! �  ��6c   5  C   5�  ;   5�  3   5�  /   5�  '     5�  &   $ Progress��6=   $
 Auto
 6O    5�  *   �� 5�  .   .R�5�  2   � $��80��5�  >   $��12��5�  >   " � Minimum$" 0Q�5�  >   # � Maximum$	# 100Q�5�  >   $ � Value$ 5�  >   �� *ProgressPercentage+5�  >   5�  /   5�  '     5�  &   $
 Auto
 6=    5�  *   �� 5�  .   .R�5�  2   �� #��[ 5�  9   $ View Details��5�  9   ��5�  9   �� *&$DataContext.ViewUpdateDetailsCommand+ ��  DataGrid��5�  9   �� .��5�  6     $ Information=�6N   5�  3   5�  /   5�  '   5�     5�     5�     5�     5�     5�     � 5�  
   $��3��6   $��5��5�     $��0q�5�     % � IsIndeterminate% 5�     �� *IsBusy+y�5�     �� *IsBusy+# [ 5�     5�     5�     