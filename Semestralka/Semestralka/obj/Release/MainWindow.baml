   M S B A M L   `   `   `  ���� F  BSemestralka, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:SemestralkaSemestralka      Semestralka.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     +localclr-namespace:Semestralka       � Title$  
MainWindow��$	��531��$	��830�� � WindowStartupLocation$ CenterScreen=����	#FFF3F3F3 J� SnapsToDevicePixels .  � 
ResizeMode$ NoResize=� � SizeToContent$ WidthAndHeight=� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     Color$ 	#FFD32A2A��   
BlurRadius$ 10Q�.��� -   $��testicek��$��	0,0,670,0q�
���   �$	��160��$	��531�� � RowDefinitions �� $��0* ��� $
��258* ��� $
��263* �	 � ColumnDefinitions	 �� $	��41* ��� $	��12* ��� $	��29* �.+��� -   
 ��Name$
 button_graf��$��Graph��$��3��$��
0,151,1,56q�$��14(�$��Century Gothic)�
�������$��2��$	��159��$��56��   MyButton#��[  ��Z� ��Z� �� -    ��Name$ image��$,��&Arrioch-Office-Dock-Color-MS-Excel.ico�� J� RenderTransformOrigin$ 0.184,-1.359&�$��40��$��40��$��10,162,30,61q�$��2���� -   $
 button_export��$��Export��$��3��$��
-3,207,4,0q�$��14(�$��Century Gothic)�
�������$��2��$	��159��$��56��#��[  ��Z� ��Z� �� -   $ 
image_Copy��$2��,Custom-Icon-Design-Mono-General-2-Export.ico��$ 0.184,-1.359&�$��40��$��40��$��10,218,30,5q�$��2���� -   $
 button_count��$��	0,0,1,211q�$��14(�$��Century Gothic)�
�������$	��159��$��57��#��[  $��2��$��3��$��Count����Z� ��Z� �� -   $
 button_save��$
��Save��$��3��$��
0,95,2,112q�$��14(�$��Century Gothic)�
�������$��2��$	��158��$��55��#��[  ��Z� ��Z� �� -   $ image_Copy1��$1��+Custom-Icon-Design-Pretty-Office-7-Save.ico��$ 0.184,-1.359&�$��40��$��51��$��10,95,30,117q�$��2���� -	   $
 button_refresh��$��-1,54.8,2,156.2q�$��14(�$��Century Gothic)�
�������$	��159��$��57��#��[  $��2��$��3��$��Refresh����Z� ��Z� � $��2��$��85��$��53,0,-685,0q�$��2��$	��Top=�
���   �.+��� $
��Auto=�$
��Auto=�$��	0,0,113,0q�$	��576��.���� -
    ��Name$ sp_settings��$��1��$��5,5,5,10q��� -    ��Name$ 	lb_status��$��not searched��
���  ��$��18(�$��Center=�$��10,20,-25,202q�$��20,5,0,0q�$��2��$��Center=�$��3��$	��175��$��41��$
��Bold"��� -   $ label��$ ��Pecina, Souček, Špetlík��$
��Left=�$��30��$��
10,228,0,0q�$��2��$	��Top=�$	��146��$��3��
�������$��Times New Roman)�$��14(�&   System.Windows.Controls.DataGrid  -     Name$ dataGrid��!   EnableColumnVirtualization .     EnableRowVirtualization .  $��58,100,-675,9q�$��2��$��3��$	��674��$	��422��   
IsReadOnly . $ �� IsSynchronizedWithCurrentItem$ FalseY����	#FF340032
�������   HorizontalGridLinesBrush
 �   �   VerticalGridLinesBrush
 �   �$
��Bold"�$��14(���Z�    �� -   $ image_Copy2��$=��7Oxygen-Icons.org-Oxygen-Apps-accessories-calculator.ico��$ 0.184,-1.359&�$��40��$��40��$��10,10,30,218q�$��2���� -   $ image_Copy3��$&�� Graphicloads-100-Flat-Reload.ico��$ 0.184,-1.359&�$��40��$��40��$��10,62,30,166q�$��2���� -   $ lb_status_connect��$�� ��
��� � �$��22(�$��Center=�$��580,20,-675,0q�$��20,5,0,0q�$��2��$	��Top=�$	��152��$��65��$��2��$
��Bold"�