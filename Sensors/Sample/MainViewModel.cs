﻿using System;
using System.Collections.Generic;
using System.Linq;
using Shiny;
using Shiny.Sensors;


namespace Sample
{
    public class MainViewModel
    {
        public List<ISensorViewModel> Sensors { get; }
        public bool HasSensors => this.Sensors.Any();


        public MainViewModel()
        {
            this.Sensors = new List<ISensorViewModel>();

            this.AddIf<IAccelerometer, MotionReading>("G");
            this.AddIf<IGyroscope, MotionReading>("G");
            this.AddIf<IMagnetometer, MotionReading>("M");
            this.AddIf<ICompass, CompassReading>("D");
            this.AddIf<IAmbientLight, double>("Light");
            this.AddIf<IBarometer, double>("Pressure");
            this.AddIf<IPedometer, int>("Steps");
            this.AddIf<IProximity, bool>("Near");
            this.AddIf<IHumidity, double>("Humidity");
            this.AddIf<ITemperature, double>("Temp");
        }


        void AddIf<T, U>(string measurement)
        {
            var sensor = ShinyHost.Resolve<T>() as ISensor<U>;
            if (sensor != null)
                this.Sensors.Add(new SensorViewModel<U>(sensor, measurement));
        }
    }
}
