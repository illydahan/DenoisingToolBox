   M S B A M L   `   `   `  ����F  BDenoiseUtil, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:DenoiseUtilDenoiseUtil  5   	       DenoiseUtil.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:DenoiseUtil     5   	     � Title$  Image Denoising ToolBox��5   	   $	��450��6)   $	��850��66   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $
��653* �6   �� 5      $
��192* �6   .+�5   
   �� -   5        $��originalIamge��5      $	��175��5      $��204,12,274,235.5q�6   5      �� 5   
   -   5        $��
noisyImage��5      $��425,12,53,235.5q�5      $	��175��6)   5      �� 5   
   -   5        $��calculatedDenoisedImage��5      $	��175��5      $��633,12,37,235.5q�6   $��2��5   /   �� 5   
   -   5        $��Browse 
Images��5      $
��Left=�63   $��	10,10,0,0q�6N   $	��Top=�6a   $	��125��5      $��38��68   �� 5   
   -   5        $��VarianceSlider��5   g   $
��Left=�5      $��
10,101,0,0q�6,   $	��Top=�6@   $��22��5      $	��125��6    � Maximum$	 450Q�6)    � Minimum$ 50Q�67   6g   �� 5   
   -   5        $��VarianceSliderLabel��5   X   $��Noise Variance:��5      $
��Left=�6*   $��	10,70,0,0q�6E   $	��Top=�5       J� RenderTransformOrigin$ 0.741,2.521&�6(   $	��125��6L   6X   �� 5   
   -   5        $��
DenoiseBTN��5       $��Denoise��5      $
��Left=�6#   $��
10,151,0,0q�6>   $	��Top=�6R   $	��125��6j   $��31��6v   {�.  5    =   � 5#   
   -   5        $��popup1��5#      $
��Left=�6   $��10,437,0,-189.5q�69    � AllowsTransparency . $	��Top=�5$   *   $	��175��6B   $	��300��6O   $��Center=�6[   .��5%      � 
�������$��10,10,-10,-10q�6&   $	��300��6=   $	��175��6I   .+�5'      �� $��	Progress:��6   $	��100��6,   $��20(�69   $��90,0,90,100q�6G   � 5(      -	   5        $��progressBar1��5)      $
��Left=�5(      $��25��5)   *   $ 0Q�5*      $	 100Q�6"   $��Center=�5+      $	��275��61   ���	#FF3EA3EA � ForceCursor .  $��
10,32,0,10q�5.   *    � Value 5-      ��  �� Path$ progress�5.   *   �� 5/      -
   5        $��PopupBTN��5/      $��OK��6*   $	��100��67   $��40��6C   $��40,115,60,10q�6O   6e   50      51      �� 52   
   -   5        $��DenoisingSchemeComboBox��52      $
��Left=�62   $	��Top=�6M   $	��120��6e   $��
10,233,0,0q�6q   .@�54      �� 	 �� 
IsSelected	 . .��6-   Non Local Means6>   �� 55      .��6   External Denoising6/   56      �� 57   
   $��Denoising Scheme��6   $
��Left=�6+   $��
10,202,0,0q�6F   $	��Top=�6Z   $ -0.337,-0.397&�58      $	��125��66   59      5:      