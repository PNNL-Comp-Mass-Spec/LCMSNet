using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;
using FluidicsSDK.Managers;
using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices.Valves
{
    public sealed class SixPortInjectionFluidicsValve : TwoPositionValve
    {
        private const int NUMBER_OF_PORTS = 6;

        public SixPortInjectionFluidicsValve() :
            base(NUMBER_OF_PORTS, false, yOffset: 32) // Additional vertical offset for injection port drawing
        {
            // https://www.vici.com/support/app/2p_japp.php
            // 6-port Cheminert: A is 6-1, B is 1-2

            //add top "injection port"
            AddPrimitive(new FluidicsRectangle(new Point(Loc.X + Offset.X + (int)Size.Width / 4, Loc.Y + Offset.Y - 30), new Size((int)Size.Width / 2, 10), Colors.Black, Brushes.White));
            AddPrimitive(new FluidicsRectangle(new Point(Loc.X + Offset.X + (((int)Size.Width / 2) - 5), Loc.Y + Offset.Y - 20), new Size(10, 20), Colors.Black, Brushes.White));
            // add injection loop
            AddPrimitive(new FluidicsLine(m_portList[1].Center, m_portList[4].Center));
            AddPrimitive(new FluidicsRectangle(new Point(Center.X - 25, Center.Y - 15), new Size(50, 30), Colors.Black, Brushes.White, true, 1));
        }

        public override void Render(DrawingContext g, byte alpha, float scale = 1)
        {
            base.Render(g, alpha, scale);
            var stringScale = (int)Math.Round(scale < 1 ? -(1 / scale) : scale, 0, MidpointRounding.AwayFromZero);

            var font = new Typeface(new FontFamily("Calibri"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var volumeText = new FormattedText(Volume.ToString(CultureInfo.InvariantCulture) + " \u00b5" + "L", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, ElevenPoint * stringScale, Brushes.Black, FluidicsModerator.Moderator.DrawingScaleFactor);
            g.DrawText(volumeText, new Point((Center.X * scale - 20), (Center.Y * scale - 10)));
        }

        protected override void SetDevice(IDevice device)
        {
            var valve = device as ISixPortInjectionValve;
            SetBaseDevice(valve);
            try
            {
                if (valve != null)
                {
                    Volume = valve.InjectionVolume;
                    valve.InjectionVolumeChanged += ValveInjectionVolumeChanged;
                }
            }
            catch (Exception)
            {
                // MessageBox.Show("Null valve: " + ex.Message);
            }
        }

        private void ValveInjectionVolumeChanged(object sender, EventArgs e)
        {
            Volume = (GetBaseDevice() as ISixPortInjectionValve)?.InjectionVolume ?? 0;
            DeviceChanged?.Invoke(this, new FluidicsDevChangeEventArgs());
        }

        /// <summary>
        /// Take a list of tuples and use it to create the internal connections
        /// of the device.
        /// </summary>
        /// <param name="state">a list of tuples, each tuple represents a single internal connection</param>
        public override void ActivateState(List<Tuple<int, int>> state)
        {
            // Overridden to maintain the injection loop connection
            FluidicsModerator.Moderator.BeginModelSuspension();
            // Run the standard required changes, which also removes all internal connections
            ActivateStateWork(state);

            // add injection loop connection.
            ConnectionManager.GetConnectionManager.Connect(m_portList[1], m_portList[4], this);
            var injectionLoopConnection = ConnectionManager.GetConnectionManager.FindConnection(m_portList[1], m_portList[4]);
            injectionLoopConnection.Transparent = true;

            FluidicsModerator.Moderator.EndModelSuspension(true);
        }

        public double Volume { get; set; }

        public override event EventHandler<FluidicsDevChangeEventArgs> DeviceChanged;
    }
}
