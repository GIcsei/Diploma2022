using System;
using System.Windows;

namespace PSI_Checker_2p0
{
    /// <summary>
    /// A base attached property to replace the vanilla WPF attached property
    /// </summary>
    /// <typeparam name="Parent">The parent class to be the attached property</typeparam>
    /// <typeparam name="Property">The type of this attached property</typeparam>
    public abstract class BaseAttachedProperty<Parent, Property>
        where Parent : BaseAttachedProperty<Parent, Property>, new()
    {
        #region Public Events
        public event Action<DependencyObject, DependencyPropertyChangedEventArgs> ValueChanged =
            (sender, e) => { };
        #endregion

        #region Public Properties
        public static Parent Instance { get; private set; } = new Parent();

        protected static string Name = "Value";
        #endregion

        #region Attached Property Definitions
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
            Name, typeof(Property), typeof(BaseAttachedProperty<Parent, Property>),
            new UIPropertyMetadata(new PropertyChangedCallback(OnValuePropertyChanged)));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Instance.OnValueChanged(d, e);
            Instance.ValueChanged(d, e);
        }

        public static Property GetValue(DependencyObject d) => (Property)d.GetValue(ValueProperty);

        public static void SetValue(DependencyObject d, Property value) => d.SetValue(ValueProperty, value);
        #endregion

        #region
        public virtual void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }

        #endregion
    }
}
