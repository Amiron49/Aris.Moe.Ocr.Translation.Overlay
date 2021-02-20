using System;
using System.Collections.Generic;

namespace Aris.Moe.OverlayTranslate.Gui
{
    public class SettingsModel
    {
        public List<SingleSetting<object>> Properties { get; set; }

        public SettingsModel(List<SingleSetting<object>> properties)
        {
            Properties = properties;
        }
    }

    public abstract class SingleSetting
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public abstract object? Value { get; set; }

        public bool Valid { get; set; } = true;

        public SingleSetting(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }

    public class SingleSetting<T> : SingleSetting
    {
        public override object Value
        {
            get => _valueGetter();
            set => _valueSetter(value);
        }

        private Action<object> _valueSetter;

        private Func<T> _valueGetter;

        public SingleSetting(string name, string type) : base(name, type)
        {
        }
    }
}