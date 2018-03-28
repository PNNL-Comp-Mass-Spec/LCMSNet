//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 06/29/2010
//
//*********************************************************************************************************

using System;

namespace LcmsNetPlugins.Bruker
{
    /// <summary>
    /// Class to hold constants and enums required for communication with Bruker solarXcontrol
    /// </summary>
    public class classBrukerComConstants
    {
        #region "Enums"
        // Represents response from sXc
        public enum SxcReplies : short
        {
            PROCESSFTMSDATA = 2,
            FTMSCLOSED = 5,
            FTMSACQSTART = 11,
            FTMSACQSTOP = 12,
            MALDIACQSTART = 13,
            MALDIACQSTOP = 14,
            MSMETHODCHANGED = 15,
            ACTIVATEHYSTARWINDOW = 16,
            DISCSPACEWARNING = 17,
            FTMS_READY = 20,    // NOTE: Obtained by adding 20 to parameter for SOCKET_REQUEST_CHECKMSSTATUS parampeter value
            FTMS_NOTREADY = 21, // NOTE: Obtained by adding 20 to parameter for SOCKET_REQUEST_CHECKMSSTATUS parampeter value
            FTMS_ERROR = 22,    // NOTE: Obtained by adding 20 to parameter for SOCKET_REQUEST_CHECKMSSTATUS parampeter value
            FTMS_CRITICALERROR = 23,    // NOTE: Obtained by adding 20 to parameter for SOCKET_REQUEST_CHECKMSSTATUS parampeter value
            FTMS_RUN = 24,  // NOTE: Obtained by adding 20 to parameter for SOCKET_REQUEST_CHECKMSSTATUS parampeter value
            FTMS_SHUTDOWN = 25, // NOTE: Obtained by adding 20 to parameter for SOCKET_REQUEST_CHECKMSSTATUS parampeter value
            SXC_NOMESSAGE = 30,
            SXC_INVALID = 40
        }
        #endregion

        #region "Constants"
        // Commands sent to solarXcontrol
        public const short SOCKET_INITFTMS = 0;
        public const short SOCKET_EXITFTMS = 1;
        public const short SOCKET_SWITCHFTMS = 2;
        public const short SOCKET_STARTSTOPABORTFTMS = 3;
        public const short SOCKET_SETANALYSISMETHNAME = 5;
        public const short SOCKET_CHECKMSSTATUS = 6;
        public const short SOCKET_STARTACQUISITION = 7;
        public const short SOCKET_STARTMALDIRUN = 8;
        public const short SOCKET_SHOWFTMSAPPLICATION = 9;
        public const short SOCKET_PROCESSMALDISAMPLEINFORMATION = 15;
        public const short SOCKET_PROCESSLCMSSAMPLEINFORMATION = 16;
        public const short SOCKET_SWITCHVALVE = 19;
        public const short SOCKET_HYSTARSTATUS = 20;
        public const short SOCKET_NUMOFMEAS_INSEQUENCE = 21;

        // Responses received from solarXcontrol
        public const short SOCKET_REQUEST_PROCESSFTMSDATA = 2;
        public const short SOCKET_REQUEST_FTMSCLOSED = 5;
        public const short SOCKET_REQUEST_CHECKMSSTATUS = 9;    // This will be followed by a CHECKMSSTATUS param
        public const short SOCKET_REQUEST_FTMSACQSTART = 11;
        public const short SOCKET_REQUEST_FTMSACQSTOP = 12;
        public const short SOCKET_REQUEST_MALDIACQSTART = 13;
        public const short SOCKET_REQUEST_MALDIACQSTOP = 14;
        public const short SOCKET_REQUEST_MSMETHODCHANGED = 15;
        public const short SOCKET_REQUEST_ACTIVATEHYSTARWINDOW = 16;
        public const short SOCKET_REQUEST_DISCSPACEWARNING = 17;

        // INITFTMS params
        public const short PARAM_INITFTMS_ESI = 0;
        public const short PARAM_INITFTMS_MALDI = 1;

        // EXITFTMS params
        public const short PARAM_EXITFTMS_DEFAULT = 0;

        // SWITCHFTMS params
        public const short PARAM_SWITCHFTMS_SHUTDOWN = 0;
        public const short PARAM_SWITCHFTMS_STANDBY = 1;
        public const short PARAM_SWITCHFTMS_OPERATE = 3;

        // STARTSTOP params
        public const short PARAM_STARTSTOP_STOP = 2;
        public const short PARAM_STARTSTOP_ABORT = 3;
        public const short PARAM_STARTSTOP_PREPARE = 4;
        public const short PARAM_STARTSTOP_FINISH = 5;
        public const short PARAM_STARTSTOP_STARTTHRESHOLDING = 6;
        public const short PARAM_STARTSTOP_STOPTHRESHOLDING = 7;

        // SHOWAPPLICATION params
        public const short PARAM_SHOWAPP_HIDE = 0;
        public const short PARAM_SHOWAPP_SHOW = 1;

        // CHECKMSSTATUS params
        public const short PARAM_CHECKMSSTATUS_READY = 0;
        public const short PARAM_CHECKMSSTATUS_NOTREADY = 1;
        #endregion

        #region "Methods"
        public static SxcReplies ConvertShortToSxcReply(short inpVal)
        {
            if (Enum.IsDefined(typeof(SxcReplies), inpVal))
            {
                return (SxcReplies)inpVal;
            }
            else
            {
                return SxcReplies.SXC_INVALID;
            }
        }   
        #endregion
    }   
}
