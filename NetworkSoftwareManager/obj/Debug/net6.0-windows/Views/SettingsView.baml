   M S B A M L   `   `   `  ����Q  MNetworkSoftwareManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:NetworkSoftwareManager.ViewsNetworkSoftwareManager.Views  5      U/clr-namespace:NetworkSoftwareManager.ViewModels!NetworkSoftwareManager.ViewModels  5      /    )NetworkSoftwareManager.Views.SettingsView   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      <local*clr-namespace:NetworkSoftwareManager.Views     5      _ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      @
viewmodels/clr-namespace:NetworkSoftwareManager.ViewModels  5        N� Title$  Settings��5       � DataContext 5      9   3NetworkSoftwareManager.ViewModels.SettingsViewModel  5   
   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��Settings��5       $   MaterialDesignHeadline4TextBlock#��[  5      $��	16,16,0,8q�5      �� 5   
   $��1��6   $
��Auto=�6$   .��5       �� $��
16,0,16,16q�6   .+�5"      #  MaterialDesignThemes.Wpf.Card  $��0,0,0,16q�6&   .��5#      � $��16q�6    5$      �� 5%      $
��Auto �6,   �� 5&      $
��Auto �6,   .+�5)      �� $��0��6$   $��General Settings��5*   $    $  MaterialDesignHeadline6TextBlock#��[ 5+   $   $��0,0,0,16q�5,   $   � 5.      $��1��6    5/      �� 50   "   $
��Auto �60   �� 51   "   $
��Auto �60   �� 52   "   $
��Auto �60    � ColumnDefinitions 54      �� 55   "   $
��Auto �63   �� 56   "   $��* �63   �� 57   "   $
��Auto �63   �� 58   "   $��* �63   .+�5;      �� $��0��6(   $��0��65   $��Update Staging Directory:��5<   (   $��Center=�5=   (   $��0,0,8,0q�5>   (   � 5?      $��0��6#   $��1��60   $��3��6@    5@   "   �� 5A   &   $��* �67   �� 5B   &   $
��Auto �67   .+�5D   "   �� $��0��6*    �� 
IsReadOnly$
 True��5F   *   ��5E   *   �� *!Settings.UpdateStagingDirectory+5F   *   �� 5G   "   $��1��6)     MaterialDesignIconButton#��[ 5I   )   $��4,0,0,0q�5J   )    � ToolTip$ Browse for Directory��5K   )   ��5H   )   �� *BrowseDirectoryCommand+.��5L   &   '  !MaterialDesignThemes.Wpf.PackIcon     Kind$ 
FolderOpen=�6>   5M   #   5N      �� 5P      $��1��6'   $��1��64   $��Scan Network on Startup��5Q   '   $��0,16,0,0q�5S   '    d� 	IsChecked 5R   '   �� *Settings.ScanOnStartup+5S   '   �� 5U      $��1��6'   $��3��64   $��Enable Log File��5V   '   $��0,16,0,0q�5X   '    5W   '   �� *Settings.LogFileEnabled+5X   '   �� 5Z      $��2��6'   $��1��64   $��Use WMI��5[   '   $��0,8,0,0q�5]   '    5\   '   �� *Settings.UseWMI+5]   '   �� 5_      $��2��6'   $��3��64   $��Use PowerShell��5`   '   $��0,8,0,0q�5b   '    5a   '   �� *Settings.UsePowerShell+5b   '   5c      5d      5e        5h      $��0,0,0,16q�6&   .��5i      � $��16q�6    5j      �� 5k      $
��Auto �6,   �� 5l      $
��Auto �6,   .+�5o      �� $��0��6$   $��Network Scan Settings��5p   $   #��[ 5q   $   $��0,0,0,16q�5r   $   � 5t      $��1��6    5u      �� 5v   "   $
��Auto �60   �� 5w   "   $
��Auto �60   �� 5x   "   $
��Auto �60    5z      �� 5{   "   $
��Auto �63   �� 5|   "   $��* �63   �� 5}   "   $
��Auto �63   �� 5~   "   $��* �63   .+�5�      �� $��0��6(   $��0��65   $��Default IP Ranges:��5�   (   $��Center=�5�   (   $��0,0,8,0q�5�   (   �� 5�      $��0��6&   $��1��63   $��0,0,16,0q�5�   &   $' !Format: 192.168.1.1-192.168.1.254��5�   &   ��5�   &   �� *Settings.IPRangesToScan+5�   &   �� 5�      $��0��6(   $��2��65   $��Default Excluded IPs:��5�   (   $��Center=�5�   (   $��0,0,8,0q�5�   (   �� 5�      $��0��6&   $��3��63   $" Comma separated IP addresses��5�   &   ��5�   &   �� *ExcludedIPsText+5�   &   �� 5�      $��1��6(   $��0��65   $��Scan Timeout (ms):��5�   (   $��Center=�5�   (   $��0,16,8,0q�5�   (   �� 5�      $��1��6&   $��1��63   $��	0,16,16,0q�5�   &   ��5�   &   �� *Settings.ScanTimeout+5�   &   �� 5�      $��1��6(   $��2��65   $��Connection Timeout (ms):��5�   (   $��Center=�5�   (   $��0,16,8,0q�5�   (   �� 5�      $��1��6&   $��3��63   $��0,16,0,0q�5�   &   ��5�   &   �� *Settings.ConnectionTimeout+5�   &   �� 5�      $��2��6(   $��0��65   $��Thread Count:��5�   (   $��Center=�5�   (   $��0,16,8,0q�5�   (   �� 5�      $��2��6&   $��1��63   $��	0,16,16,0q�5�   &   ��5�   &   �� *Settings.ThreadCount+5�   &   5�      5�      5�        5�      .��5�      � $��16q�6    5�      �� 5�      $
��Auto �6,   �� 5�      $
��Auto �6,   �� 5�      $
��Auto �6,   .+�5�      �� $��0��6$   $��Global Credentials��5�   $   #��[ 5�   $   $��0,0,0,16q�5�   $   � 5�      $��1��6    5�      �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60    5�      �� 5�   "   $
��Auto �63   �� 5�   "   $��* �63   .+�5�      �� $��0��6(   $��0��65   $��Domain:��5�   (   $��Center=�5�   (   $��0,0,8,0q�5�   (   �� 5�      $��0��6&   $��1��63   ��5�   &   �� *#!Settings.GlobalCredentials.Domain+�� 5�      $��1��6(   $��0��65   $��	Username:��5�   (   $��Center=�5�   (   $��0,16,8,0q�5�   (   �� 5�      $��1��6&   $��1��63   $��0,16,0,0q�5�   &   ��5�   &   �� *%#Settings.GlobalCredentials.Username+5�   &   �� 5�      $��2��6(   $��0��65   $��	Password:��5�   (   $��Center=�5�   (   $��0,16,8,0q�5�   (   G� 5�      -   5         G�Name$ GlobalPasswordBox��5�   *   $��2��5�   *   $��1��67   $��0,16,0,0q�5�   *   5�   *   5�      �� 5�      $��2��6%   	 �� Orientation$	 
Horizontal=�62   $��0,16,0,0q�6K   .+�5�      �� $��Remember Credentials��6'   $��0,0,16,0q�5�   '    5�   '   �� *Settings.RememberCredentials+5�   '   �� 5�         MaterialDesignOutlinedButton#��[ 5�   %   $ Test Credentials��5�   %   ��5�   %   �� *TestCredentialsCommand+{�5�   %   �� *IsTestingCredentials+  InverseBooleanConverter
 �� 	Converter#
 [ .��5�   "   �� $	 
Horizontal=�6-   .+�5�   &     $ ShieldCheck=�6>   $��0,0,8,0q�6Q   �� 5�   &   $��Test Credentials��60   5�   #   5�      5�      5�      5�      5�      5�      � 5�   
   $��2��6   $��16q�6    5�      �� 5�      $��* �6#   �� 5�      $
��Auto �6#   �� 5�      $
��Auto �6#   .+�5�      �� $��Center=�67   ��6   �� *StatusMessage+67   �� 5�      $��1��6   #��[ 5      $# Reset All Settings to Default��5     $��0,0,16,0q�5     ��5�      �� *ResetSettingsCommand+.��5     �� $	 
Horizontal=�6   .+�5       $ Restore=�6.   $��0,0,8,0q�6=   �� 5     $��Reset to Default��6    5     5     �� 5	     $��2��6     MaterialDesignRaisedButton#��[ 5     $ Save Settings��5     ��5
     �� *SaveSettingsCommand+.��5     �� $	 
Horizontal=�6   .+�5       $ ContentSave=�6.   $��0,0,8,0q�6A   �� 5     $��Save Settings��6    5     5     5     5     5     