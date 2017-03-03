
//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2011, Battelle Memorial Institute
// Created 03/17/2011
//
// Last modified 03/17/2011
//*********************************************************************************************************

namespace LcmsNet.Devices.Pumps
{
    /// <summary>
    /// Creates a control array for Isco display controls
    /// </summary>
    class classPumpIscoDisplayArray : System.Collections.CollectionBase
    {
        #region "Events"
            public event DelegateIscoPumpDisplaySetpointHandler SetpointChanged;
            public event DelegateIscoPumpDisplayHandler StartRefill;
            public event DelegateIscoPumpDisplayHandler StartPump;
            public event DelegateIscoPumpDisplayHandler StopPump;
        #endregion

        #region "Properties"
            /// <summary>
            /// Default property
            /// </summary>
            /// <param name="indx">Index of requested control</param>
            /// <returns>Requested control</returns>
            public controlPumpIscoDisplay this[int indx]
            {
                get { return (controlPumpIscoDisplay)this.List[indx]; }
            }
        #endregion

        #region "Constructors"
        #endregion

        #region "Methods"
            /// <summary>
            /// Adds a new display to the control array
            /// (Assumes control already exists in the parent container)
            /// </summary>
            /// <param name="newDisplay">Display control to be added</param>
            public void AddDisplayControl(controlPumpIscoDisplay newDisplay)
            {
                this.List.Add(newDisplay);
                newDisplay.Tag = this.Count;
                newDisplay.InitControl(this.Count - 1);

                newDisplay.SetpointChanged += new DelegateIscoPumpDisplaySetpointHandler(SetpointChange_Clicked);
                newDisplay.StartRefill += new DelegateIscoPumpDisplayHandler(Refill_Clicked);
                newDisplay.StartPump += new DelegateIscoPumpDisplayHandler(StartPump_Clicked);
                newDisplay.StopPump += new DelegateIscoPumpDisplayHandler(StopPump_Clicked);
            }   

            ///// <summary>
            ///// Gets the index of the input control
            ///// </summary>
            ///// <param name="inpControl">Control to search for</param>
            ///// <returns>Index of control if found; -1 otherwise</returns>
            //private int GetIndxForControl(controlPumpIscoDisplay inpControl)
            //{
            //   int retIndx = -1;
            //   for (int indx = 0; indx < this.Count; indx++)
            //   {
            //      if ((int)inpControl.Tag == (int)this[indx].Tag)
            //      {
            //         retIndx = indx;;
            //         break;
            //      }
            //   }

            //   return retIndx;
            //} 
        #endregion

        #region "Event handlers"
            /// <summary>
            /// Set Press or Set Flow button clicked
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void SetpointChange_Clicked(object sender, int pumpIndex, double newSetpoint)
            {
            SetpointChanged?.Invoke(sender, pumpIndex, newSetpoint);
        }   

            /// <summary>
            /// Refill button clicked
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void Refill_Clicked(object sender, int pumpIndex)
            {
            StartRefill?.Invoke(sender, pumpIndex);
        }   

            /// <summary>
            /// Start Pump button clicked
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void StartPump_Clicked(object sender, int pumpIndex)
            {
            StartPump?.Invoke(sender, pumpIndex);
        }   

            /// <summary>
            /// Stop Pump button clicked
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void StopPump_Clicked(object sender, int pumpIndex)
            {
            StopPump?.Invoke(sender, pumpIndex);
        }   
        #endregion
    }   
}
