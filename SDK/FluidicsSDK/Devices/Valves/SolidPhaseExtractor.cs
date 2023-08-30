using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Graphic;
using FluidicsSDK.Managers;
using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices.Valves
{
    public sealed class SolidPhaseExtractor : TwoPositionValve
    {
        private const int NUMBER_OF_PORTS = 6;

        public SolidPhaseExtractor()
            : base(NUMBER_OF_PORTS, false)
        {
            // https://www.vici.com/support/app/2p_japp.php
            // 6-port Cheminert: A is 6-1, B is 1-2

            // add loop
            AddPrimitive(new FluidicsLine(m_portList[1].Center, m_portList[4].Center));
            AddPrimitive(new FluidicsRectangle(new Point(Center.X - 25, Center.Y - 15), new Size(50, 30), Colors.Black, Brushes.White, fill: true, atScale: 1));
        }

        public override void Render(DrawingContext g, byte alpha, float scale = 1)
        {
            base.Render(g, alpha, scale);
            var stringScale = (int)Math.Round(scale < 1 ? -(1 / scale) : scale, 0, MidpointRounding.AwayFromZero);

            var font = new Typeface(new FontFamily("Calibri"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var volumeText = new FormattedText("SPE", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, ElevenPoint * stringScale, Brushes.Black, FluidicsModerator.Moderator.DrawingScaleFactor);
            g.DrawText(volumeText, new Point(Center.X - 20, Center.Y - 10));
        }

        protected override void SetDevice(IDevice device)
        {
            var valve = device as ISolidPhaseExtractor;
            SetBaseDevice(valve);
        }

        /// <summary>
        /// Take a list of tuples and use it to create the internal connections
        /// of the device.
        /// </summary>
        /// <param name="state">a list of tuples, each tuple represents a single internal connection</param>
        public override void ActivateState(List<Tuple<int, int>> state)
        {
            // Overridden to maintain the SPE loop connection
            FluidicsModerator.Moderator.BeginModelSuspension();
            // Run the standard required changes, which also removes all internal connections
            ActivateStateWork(state);

            // add SPE loop connection.
            ConnectionManager.GetConnectionManager.Connect(m_portList[1], m_portList[4], this);
            var injectionLoopConnection = ConnectionManager.GetConnectionManager.FindConnection(m_portList[1], m_portList[4]);
            injectionLoopConnection.Transparent = true;

            FluidicsModerator.Moderator.EndModelSuspension(true);
        }
    }
}
