using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using wagyourtail.JetpackBoosting.Settings;
using wagyourtail.JetpackBoosting.Settings.Elements;


namespace wagyourtail.JetpackBoosting
{
    public class Config : INotifyPropertyChanged
    {
        #region Options

        private int noBoostSpeed = 13;
        private int slowDownSpeed = 14;
        
        private bool slowWhenDampenersOff = false;
        
        private float hitSpeedMultiplier = .5f;

        #endregion

        #region User interface

        // TODO: Settings dialog title
        public readonly string Title = "Jetpack Boosting";

        // TODO: Settings dialog controls, one property for each configuration option


        [Slider(0f, 100f, 1f, SliderAttribute.SliderType.Integer, description: "Speed to go when not hitting the boost keybind")]
        public int NoBoostSpeed
        {
            get => noBoostSpeed;
            set => SetField(ref noBoostSpeed, value);
        }

        public int NoBoostSpeedSq
        {
            get => noBoostSpeed * NoBoostSpeed;
            set => NoBoostSpeed = (int)Math.Sqrt(value);
        }
        
        [Slider(0f, 100f, 1f, SliderAttribute.SliderType.Integer, description: "Speed to slow down to when releasing the boost keybind")]
        public int SlowDownSpeed
        {
            get => slowDownSpeed;
            set
            {
                value = Math.Max(value, NoBoostSpeed);
                SetField(ref slowDownSpeed, value);
            }
        }

        public int SlowDownSpeedSq
        {
            get => slowDownSpeed * SlowDownSpeed;
            set => SlowDownSpeed = (int)Math.Sqrt(value);
        }

        [Checkbox(description: "Slow down when dampeners off")]
        public bool SlowWhenDampenersOff
        {
            get => slowWhenDampenersOff;
            set => SetField(ref slowWhenDampenersOff, value);
        }
        
        [Slider(0f, 1f, .01f, SliderAttribute.SliderType.Float, description: "Hit Speed Multiplier")]
        public float HitSpeedMultiplier
        {
            get => hitSpeedMultiplier;
            set => SetField(ref hitSpeedMultiplier, value);
        }
        
        public float HitSpeedMultiplierSq
        {
            get => hitSpeedMultiplier * HitSpeedMultiplier;
            set => HitSpeedMultiplier = (float)Math.Sqrt(value);
        }

        #endregion

        #region Property change notification boilerplate

        public static readonly Config Default = new Config();
        public static readonly Config Current = ConfigStorage.Load();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}