   M S B A M L   `   `   `  ����J  FAutoCAD-Project, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:AutoCAD_ProjectAutoCAD_Project  5   	        AutoCAD_Project.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:AutoCAD_Project     5   	     � Title$  AutoCAD Drawing Analyzer��5   	   $	��450��6*   $	��800��67   $��Segoe UI)�5	   	   .��5      ]� ���#e8eef5$
��True��6%   .+�5   
   � $��10q�6    � ColumnDefinitions 5      �� 5      $	��220 �6#   �� 5      $��* �6#   .+�5      �� $
��Auto=�6   $��Disabled=�6>   .��5      �� ���#1f1f2e �� Padding$ 15q�6.    �� CornerRadius$ 8��6;   ._�5      ��  �� Orientation$ Vertical=�6!   $
��Left=�68   .+�5      �� $��
Navigation��6$   ���#E0E0E0$��18(�5      $
��Bold"�5      $��0 0 0 15q�5      $
��Left=�5      �� 5!      -   5        $��BlockCounterBtn��5!   !   $��🧱  Block Counter��5"      ���#3a3d5c
�������$��SemiBold"�5%      $��14(�5&      $��32��5'      $��Stretch=�5(      $��4 0 60 0q�5)       � Cursor$
 Hand��5*      ���#505070$��1q�5,       � ToolTip$5 /Displays the count of each block in model space��5-      $��0 4 0 0q�5/      $
��Left=�50       c� ButtonStyleKey#��[" 51       � 	Resources 52      	 �� 
TargetType
 �� Property �� Value��%^   '��       �� 53   "   "	 ��6(   .�54   &   �� 
 �@��CornerRadius$ 6��6E   55   #   56      57      �� 5=      -   5        $��LayerCleanerBtn��5=   !   $��🧹  Layer Cleaner��5>   	   ���#3a3d5c
�������$��SemiBold"�5A   	   $��14(�5B   	   $��32��5C   	   $��Stretch=�5D   	   $��4 0 60 0q�5E   	   $
 Hand��5F   	   ���#505070$��1q�5H   	   $+ %Displays all unused and frozen layers��5I   	   $��0 10 0 0q�5K   	   $
��Left=�5L   	   #��[" 5M   	    5N      ��%^   '��       �� 5O   "   "	 ��6(   .�5P   &   �� 
 �@��CornerRadius$ 6��6E   5Q   #   5R      5S      �� 5V      -   5        $��EntityConverterBtn��5V   !   $��🧱  Entity Converter��5W   	   ���#3a3d5c
�������$��SemiBold"�5Z   	   $��14(�5[   	   $��32��5\   	   $��Stretch=�5]   	   $��4 0 50 0q�5^   	   $
 Hand��5_   	   ���#505070$��1q�5a   	   $= 7Convert between Text, MText, Line, Polyline, and Blocks��5b   	   $��0 10 0 0q�5d   	   $
��Left=�5e   	   #��[" 5f   	    5g      ��%^   '��       �� 5h   "   "	 ��6(   .�5i   &   �� 
 �@��CornerRadius$ 6��6E   5j   #   5k      5l      �� 5r      -   5        $��OptimizeDrawingBtn��5r   !   $��🛠️  Optimize Drawing��5s   	   ���#3a3d5c
�������$��SemiBold"�5v   	   $��14(�5w   	   $��32��5x   	   $��Stretch=�5y   	   $��4 0 40 0q�5z   	   $
 Hand��5{   	   ���#505070$��1q�5}   	   $: 4Remove all unused entities like blocks, layers, etc.��5~   	   $��0 10 0 0q�5�   	   $
��Left=�5�   	   #��[" 5�   	    5�      ��%^   '��       �� 5�   "   "	 ��6(   .�5�   &   �� 
 �@��CornerRadius$ 6��6E   5�   #   5�      5�      5�      5�      5�      �� 5�      -   5        $��OutputScroll��5�   +   $��1��6   $
��Auto=�6?   $
��Auto=�6b   .��5�      �� $ 20q�6   
�������$ 10��69   ._�5�      �� -   5        $��
RightPanel��5�   !   5�      5�      5�      5�      5�      5�      