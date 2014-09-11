
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2011, Battelle Memorial Institute
// Created 03/24/2011
//
// Last modified 03/24/2011
//*********************************************************************************************************

namespace LcmsNet.Devices.Pumps
{
	public class classPumpIscoSetpointLimits
	{
		//*********************************************************************************************************
		// Holds instrument setpoint ranges
		//**********************************************************************************************************

		#region "Properties"
			/// <summary>
			/// Min flow SP
			/// </summary>
			public double MinFlowSp { get; set; }

			/// <summary>
			/// Max flow SP
			/// </summary>
			public double MaxFlowSp { get; set; }

			/// <summary>
			/// Maximum flow limit
			/// </summary>
			public double MaxFlowLimit { get; set; }

			/// <summary>
			/// Min pressure SP
			/// </summary>
			public double MinPressSp { get; set; }

			/// <summary>
			/// Max pressure SP
			/// </summary>
			public double MaxPressSp { get; set; }

			/// <summary>
			/// Max refill rate SP
			/// </summary>
			public double MaxRefillRateSp { get; set; }
		#endregion

		#region "Constructors"
			/// <summary>
			/// Default constructor
			/// </summary>
			public classPumpIscoSetpointLimits()
			{
				MinFlowSp = 0.0010D;
				MaxFlowSp = 25D;
				MinPressSp = 10D;
				MaxPressSp = 10000D;
				MaxRefillRateSp = 30D;
			}
		#endregion
	}	 
}	
