   M S B A M L   `   `   `  ����  WPF-FORM    WPF_FORM.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  AutoCAD Entity Drawer��5   	   $	��650��5   	   $	��480��6    � WindowStartupLocation$ CenterScreen=�5   	   ���#FAFAFA.��5	      � $��20q�6    � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��Select Entity to Draw��6!   $��22(�5      $
��Bold"�6"   ���#333$��Center=�5      $��0 0 0 20q�61   �� 5   
   -   5         ��Name$ EntitySelector��5       $��1��6   $��35��5      $��0 0 0 20q�6   $��14(�61   $	��Top=�5      .@�5      �� $
��Line��6   �� 5      $��Polyline��6   �� 5      $��Circle��6   �� 5      $��	Rectangle��6   �� 5      $	��Arc��6   5       �� 5#   
   $��2��6   $
��Auto=�6$   .��5$      ��  �� Orientation$ Vertical=�6   $	��Top=�60   .+�5'      ��  �� 
TargetType" ��6   .�5(      ��  �� Property � �� �� Value$ 0 0 0 15q�6/   �� 5)       � ��$ 10q�60   �� 5*       � ��$
 Bold"�63   5+      �� 5.      -   5         ��Name$ 	LineGroup��5.   )   $
��Line��6   $y�	Collapsed=�6<   .��5/      �� .+�50      �� $��Start Point (x,y):��6$   �� 51      -   5        	 ��Name$	 LineLengthInput��51   "   $��30��6;   $��0 5 0 10q�6G   �� 52      $��End Point (x,y):��6$   �� 53      -   5        $	 LineEndInput��53   "   $��30��68   $��0 5 0 10q�6D   �� 54      -   5        $��Create Line��54   :   $��35��55   !   ���#1976D2
�������6B   56      57      �� 5:      -   5        $ PolylineGroup��5:   -   $��Polyline��6   $y�	Collapsed=�6D   .��5;      �� .+�5<      �� $��Start Point (x,y):��6$   �� 5=      -   5        $	 PolylineStartInput��5=   "   $��30��6>   $��0 5 0 10q�6J   �� 5>      $��Mid Point (x,y):��6$   �� 5?      -   5        $	 PolylineMidInput��5?   "   $��30��6<   $��0 5 0 10q�6H   �� 5@      $��End Point (x,y):��6$   �� 5A      -	   5        $	 PolylineEndInput��5A   "   $��30��6<   $��0 5 0 10q�6H   �� 5B      -
   5        $��Create Polyline��5B   >   $��35��5C   !   ���#1976D2
�������6B   5D      5E      �� 5H      -   5        $ CircleGroup��5H   +   $��Circle��6   $y�	Collapsed=�6@   .��5I      �� .+�5J      �� $��Center Point (x,y):��6$   �� 5K      -   5        $	 CircleStartInput��5K   "   $��30��6<   $��0 5 0 10q�6H   �� 5L      $��Radius:��6$   �� 5M      -   5        $	 CircleRadiusInput��5M   "   $��30��6=   $��0 5 0 10q�6I   �� 5N      -   5        $��Create Circle��5N   <   $��35��5O   !   ���#1976D2
�������6B   5P      5Q      �� 5T      -   5        $ RectangleGroup��5T   .   $��	Rectangle��6   $y�	Collapsed=�6F   .��5U      �� .+�5V      �� $��Height:��6$   �� 5W      -   5        $	 RectHeightInput��5W   "   $��30��6;   $��0 5 0 10q�6G   �� 5X      $��Width:��6$   �� 5Y      -   5        $	 RectWidthInput��5Y   "   $��30��6:   $��0 5 0 10q�6F   �� 5Z      -   5        $��Create Rectangle��5Z   ?   $��35��5[   !   ���#1976D2
�������6B   5\      5]      �� 5`      -   5        $ ArcGroup��5`   (   $	��Arc��6   $y�	Collapsed=�6:   .��5a      �� .+�5b      �� $��Center Point (x,y):��6$   �� 5c      -   5        $	 ArcCenterInput��5c   "   $��30��6:   $��0 5 0 10q�6F   �� 5d      $��Radius:��6$   �� 5e      -   5        $	 ArcRadiusInput��5e   "   $��30��6:   $��0 5 0 10q�6F   �� 5f      $��Start Angle (°):��6$   �� 5g      -   5        $	 ArcStartAngleInput��5g   "   $��30��6>   $��0 5 0 10q�6J   �� 5h      $��End Angle (°):��6$   �� 5i      -   5        $	 ArcEndAngleInput��5i   "   $��30��6<   $��0 5 0 10q�6H   �� 5j      -   5        $��
Create Arc��5j   9   $��35��5k   !   ���#1976D2
�������6B   5l      5m      5n      5o      �� 5r   
   $��3��6   $ Vertical=�6"   $��0 20 0 0q�69   .+�5s      �� -   5        $��Show Layer Creator��5s      $��40��5t      $��0 0 0 10q�6!   ���#333
�������$��SemiBold"�5v      6+   �� 5x      -   5        $��Add Dimension��5x      $��40��5y      $��0 0 0 10q�6!   ���#388E3C
�������$��SemiBold"�5{      6+   �� 5}      -   5        $$��Remove All Layers (Reset to 0)��5}      $��40��5~      ���#C62828
�������$��SemiBold"�5�      
 J� Effect
 5�      3  -System.Windows.Media.Effects.DropShadowEffect  5�         
BlurRadius$ 4Q�6'      ShadowDepth$ 2Q�66      Opacity$	 0.3Q�6F   5�      �� 5�      -   5         ��Name$ LayerInputPanel��5�      $y�	Collapsed=�62   $��0 20 0 0q�6I   .+�5�      �� $��Entity Type:��6   $
��Bold"�60   �� 5�      -   5        $ LayerEntitySelector��5�      $��30��5�      $��0 5 0 10q�6'   .@�5�      �� $
��Line��6#   �� 5�      $��Polyline��6#   �� 5�      $��Circle��6#   �� 5�      $��	Rectangle��6#   �� 5�      $	��Arc��6#   5�      �� 5�      $��Layer Name:��6   $
��Bold"�6/   �� 5�      -   5        $	 LayerNameInput��5�      $��30��62   $��0 5 0 10q�6>   �� 5�      $��Color Index (1-255):��6   $
��Bold"�68   �� 5�      -   5        $	 LayerColorInput��5�      $��30��63   $��0 5 0 10q�6?   �� 5�      -    5        $��Create Layer��5�      $��35��5�      ���#1976D2
�������$��SemiBold"�5�      6/   5�      5�      5�      5�      