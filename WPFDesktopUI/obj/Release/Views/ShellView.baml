   M S B A M L   `   `   `  ���� a  ]mpolewaczyk.InteractiveDataDisplay.WPF, Version=1.1.0.0, Culture=neutral, PublicKeyToken=nullwXclr-namespace:InteractiveDataDisplay.WPF;assembly=mpolewaczyk.InteractiveDataDisplay.WPFInteractiveDataDisplay.WPF   TrajectoryPredictor"   WPFDesktopUI.Views.ShellView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     hwpfXclr-namespace:InteractiveDataDisplay.WPF;assembly=mpolewaczyk.InteractiveDataDisplay.WPF       � WindowStartupLocation$  CenterScreen=� � Title$ Trajectory predictor��$	��500��$	��800��
�������.���  � RowDefinitions �� $��10 ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $��* ��� $��10 � � ColumnDefinitions �� $��20 ��� $
��auto ��� $
��auto ��� $��* ��� $��20 �.+��� $��1��$��1��$��2�� �� Orientation$ Vertical=�.+��� $��	Function:��]� $
��Left=�$��5q�.+��� $
��x = ���� $	��150��$	��200������  �� Path$ UiModel.FunctionX� �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�]� $
��Left=�$��5q�.+��� $
��y = ���� $	��150��$	��200������ $ UiModel.FunctionY�$ TwoWay=�$ PropertyChanged=��� $��1��$��3��$ Vertical=�$��5q�.+��� $��Draw in range:��]� $
��Left=�$��5q�.+��� $
��Left=� �� TextAlignment$ Center=�$��50������ $ UiModel.FirstPoint�$ TwoWay=�$ PropertyChanged=��� $��-��$��	10,0,10,0q��� $
��Left=�$ Center=�$��50������ $ UiModel.LastPoint�$ TwoWay=�$ PropertyChanged=�]� .+��� $��Points in range: ���� $
��Left=�$ Center=�$��50������ $ UiModel.DrawRange�$ TwoWay=�$ PropertyChanged=��� $��1��$��4��$ 
Horizontal=�$��5q�.+��� $��Input layer points: ���� $
��Left=�$ Center=�$��40������ $ UiModel.InputLayerPoints�$ TwoWay=�$ PropertyChanged=��� $��1��$��5��$ Vertical=�$��5q�.+��� $��Hidden layers:��]� .+��� $��#1��$
��Left=�$��10(�$��22,0,0,0q��� $��#2��$
��Left=�$��10(�$��40,0,0,0q��� $��#3��$
��Left=�$��10(�$��37,0,0,0q��� $��#4��$
��Left=�$��10(�$��40,0,0,0q�]� $��5q�.+��� $ Center=�$
��Left=�$
��Left=�$��10,5,0,0q�	 �� 	MaxLength$	 2��$��30������ $ HiddenLayers[0]�$ TwoWay=�$ PropertyChanged=��� $ Center=�$
��Left=�$
��Left=�$��20,5,0,0q�$	 2��$��30������ $ HiddenLayers[1]�$ TwoWay=�$ PropertyChanged=��� $ Center=�$
��Left=�$
��Left=�$��20,5,0,0q�$	 2��$��30������ $ HiddenLayers[2]�$ TwoWay=�$ PropertyChanged=��� $ Center=�$
��Left=�$
��Left=�$��20,5,0,0q�$	 2��$��30������ $ HiddenLayers[3]�$ TwoWay=�$ PropertyChanged=��� $��1��$��6��$ Vertical=�.+�]� $��5q�.+��� $��Learning rate: ���� $
��Left=�$ Center=�$��40��$	 4��$��15,0,0,0q����� $ UiModel.LearningRate�$ TwoWay=�$ PropertyChanged=�]� $��5q�.+��� $��
Momentum: ���� $
��Left=�$ Center=�$��40��$	 4��$��23,0,0,0q����� $ UiModel.Momentum�$ TwoWay=�$ PropertyChanged=�]� $��5q�.+��� $��Steepness alpha: ���� $
��Left=�$ Center=�$��40��$	 4������ $ UiModel.SteepnessAlpha�$ TwoWay=�$ PropertyChanged=��� $��1��$��2��$��7��$	��200��$
��Wrap=�
����������� *TextBlockMessage+�� $��3��$��2��$��1��.+��� -   
 ��Name$
 LearnedNetworks��$��5q� �� AlternationCount$ 2�� �� SelectedItem �� *SelectedModel+���� .R��� $ 
Horizontal=�.+��� ���� *
FullName+]� $��3��$��2��$��2��$��5q�.+��� -    ��Name$ StartLearning��$��5q�$��5q�$
��Left=�.��Start Learning�� -   $
 Stop��$��5q�$��5q�$
��Left=�.��Stop�� -   $ ShowProgress��$��	150,0,0,0q�$��5q�$��Center=�.����  �� TextAlignment$ Center=�Show Learning�� 
Progress�� -   $ Predict��$��5q�$��5q�$��Right=�.��	Predict� $��3��$��2��$��3��$��6��
�������$��0,10,0,0q�.+�&    InteractiveDataDisplay.WPF.Chart  $��3��$��2��$��3��$��6��
�������.��� -    �Name$ Lines��.+�*   $InteractiveDataDisplay.WPF.LineGraph     Description$ Correct��   Stroke
 � � �   StrokeThickness$	 0.8Q�%   InteractiveDataDisplay.WPF.Plot   Points �� *CorrectPointsGraph+$ TwoWay=�$ PropertyChanged=�  $ Trained��
 �  ��$	 0.8Q� �� *TrainedPointsGraph+$ TwoWay=�$ PropertyChanged=�  $ 	Predicted��
 ��  �$	 0.8Q� �� *PredictedPointsGraph+$ TwoWay=�$ PropertyChanged=�   LegendContent 1   +InteractiveDataDisplay.WPF.LegendItemsPanel   � 	Resources  (  $InteractiveDataDisplay.WPF.LineGraph d� 	IsChecked   VisibilityToCheckedConverter �� 	Converter	 �� X1	 �� Y1	 �� X2	 �� Y2��%�   &	        7  �� .R��� $ 
Horizontal=�.+���  �� $ 
Visibility�8   $ TwoWay=��� $��15��$��15��$	 -10��$	 -10��$ 15��$ 15��$��2������ $ Stroke��� $��5,0,0,0q����� $ Description�