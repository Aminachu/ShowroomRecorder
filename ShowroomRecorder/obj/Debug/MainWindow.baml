   M S B A M L   `   `   `  ����K  GShowroomRecorder, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:ShowroomRecorderShowroomRecorder  5   	   !    ShowroomRecorder.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:ShowroomRecorder     5   	   -   5          � Title$  
MainWindow��5   	   $	��600��6   $	��800��6)    � 
ResizeMode$ NoResize=�65   .��5	      � .+�5
   
   �� -   5        $��Button��5
      $
��Left=�6"   $��
700,10,0,0q�6=   $	��Top=�6Q   $��75��6i   $��24��6�   �� 5   
   -   5         ��Name$ roomID��5      $
��Left=�6"   $��23��6=   $��
561,11,0,0q�6I    �� TextWrapping$
 Wrap=�6]   $��TextBox��6q   $	��Top=�6�   $	��120��6�   �� 5   
   -   5         ��Name$ htmlTxt��5      $
��Left=�6%   $��	10,14,0,0q�6@   $
��Wrap=�6S   $��	TextBlock��6g   $	��Top=�6x   $��20��6�   $	��511��6�   �� 5   
   -   5         ��Name$ proclist��5      $
��Left=�6$   $	��103��6?   $��
561,39,0,0q�6L   $	��Top=�6`   $	��214��6x   �� 5   
   -   5        $ rooms��5      $
��Left=�6!   $	��277��6<   $��561,156,0,0q�6I   $	��Top=�6^   $	��214��6v   �� 5   
   -   5         ��Name$ sv��5      $
��Left=�6#   $��	10,39,0,0q�6>   $	��Top=�6Q   $	��489��6i   $	��546��6v   .��5      �� -   5         ��Name$ roomlist��5      5      � 5   
   -	   5         �Name$
 prog��5      $
��Left=�6$   $��24��6?   $��
10,534,0,0q�6K   $	��Top=�6_   $	��214��6w   �� 5   
   -
   5        	 ��Name$	 progtext��5      $1��+Checking for updates. X of Y Rooms updated.��6"   $
��Left=�6X   $��229,534,0,0q�6s   $	��Top=�6�   $��24��6�   $	��327��6�   $��10(�6�   5      5      