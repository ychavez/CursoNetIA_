


public 

// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
public static readonly DependencyProperty MyPropertyProperty =
    DependencyProperty.Register(nameof(MyProperty), typeof(int), typeof(ClassNamePlaceholder), new PropertyMetadata(0));
 MyProperty
{
    get { return (

// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
public static readonly DependencyProperty MyPropertyProperty =
    DependencyProperty.Register(nameof(MyProperty), typeof(int), typeof(ClassNamePlaceholder), new PropertyMetadata(0));
)GetValue(MyPropertyProperty); }
    set { SetValue(MyPropertyProperty, value); }
}

public static int GetMyProperty(DependencyObject obj)
{
    return (int)obj.GetValue(MyPropertyProperty);
}

public static void SetMyProperty(DependencyObject obj, int value)
{
    obj.SetValue(MyPropertyProperty, value);
}

// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
public static readonly DependencyProperty MyPropertyProperty =
    DependencyProperty.RegisterAttached("MyProperty", typeof(int), typeof(ClassNamePlaceholder), new PropertyMetadata(0));

