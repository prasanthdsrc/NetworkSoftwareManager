   M S B A M L   `   `   `  ����Q  MNetworkSoftwareManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:NetworkSoftwareManager.ViewsNetworkSoftwareManager.Views  5      ,    &NetworkSoftwareManager.Views.LoginView   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      <local*clr-namespace:NetworkSoftwareManager.Views     5      .��5
      �   � RowDefinitions  5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   ���#2962FF �� Padding$ 15q�63   ._�5      �� $&�� Network Software Manager - Login��6   
�������$��20(�5      $
��Bold"�5      $��Center=�5      5      �� 5   
   $��1��6   
������� �� CornerRadius$ 5��5      $��50,30q�5      $ 30q�5      ���#DDDDDD$��1q�5!      $��Center=�5"      $��Center=�5#      $	��400��5$      ._�5&      �   5'      �� 5(      $
��Auto �6$   �� 5)      $
��Auto �6$   �� 5*      $
��Auto �6$   �� 5+      $
��Auto �6$   �� 5,      $
��Auto �6$   �� 5-      $
��Auto �6$   .+�51      �� $��0��6   $��Login��52      $��24(�53      $��0,0,0,20q�54      $��Center=�55      �� 58      $��1��6   $��0,0,0,10q�6*   .+�59      �� $��	Username:��6   $��SemiBold"�60   �� 5:      $��8q�5;      $��1q�5<      ���#CCCCCC��5:      �� *
Username+ �� UpdateSourceTrigger$ PropertyChanged=�5=      5>      �� 5A      $��2��6   $��0,0,0,15q�6*   .+�5B      �� $��	Password:��6   $��SemiBold"�60   G� 5C      -   5         G�Name$ PasswordBox��5C   "   $��8q�5D   "   $��1q�5E   "   ���#CCCCCC5F   "   5G      �� 5J      $��3��6   $��Remember credentials��5K      $��0,0,0,15q�5M       d� 	IsChecked 5L      �� *RememberCredentials+5M      �� 5P      $��4��6   $��Login��5Q      $
��10,8q�5T      ���#2962FF
�������$��0q�5W      $��SemiBold"�5X      $��0,0,0,15q�5Y      ��5R      �� *LoginCommand+��5S      ��  �� ElementName$ PasswordBox��5Y      �� 5\      $��5��6   $
��Wrap=�5^      $��Center=�5`      ��5]      �� *StatusMessage+��5_      �� *StatusMessage+   MessageColorConverter �� 	Converter# [  5`      5a      5b      �� 5e   
   $��2��6   ���#F5F5F5$ 15q�63   ._�5f      �� $&�� © 2025 Network Software Manager��6   ���#555555$��Center=�5h      5i      5j      5k      