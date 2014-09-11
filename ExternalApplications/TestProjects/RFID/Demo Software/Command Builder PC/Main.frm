VERSION 5.00
Object = "{648A5603-2C6E-101B-82B6-000000000014}#1.1#0"; "MSCOMM32.OCX"
Begin VB.Form frmMain 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "SkyeTek Protocol Command Builder (27 AUG 2003)"
   ClientHeight    =   9840
   ClientLeft      =   3750
   ClientTop       =   2490
   ClientWidth     =   11970
   Icon            =   "Main.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   ScaleHeight     =   9840
   ScaleWidth      =   11970
   StartUpPosition =   2  'CenterScreen
   WhatsThisHelp   =   -1  'True
   Begin VB.OptionButton Binary_btn 
      Caption         =   "Binary"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   195
      Left            =   3360
      TabIndex        =   42
      Top             =   6120
      Width           =   855
   End
   Begin VB.OptionButton ASCII_btn 
      Caption         =   "ASCII"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   195
      Left            =   2400
      TabIndex        =   41
      Top             =   6120
      Value           =   -1  'True
      Width           =   855
   End
   Begin VB.CommandButton Clear 
      Caption         =   "&Clear"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   375
      Left            =   9480
      TabIndex        =   40
      Top             =   9240
      Width           =   2055
   End
   Begin VB.TextBox Response_box 
      BeginProperty Font 
         Name            =   "Courier New"
         Size            =   12
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   1215
      Left            =   480
      MultiLine       =   -1  'True
      ScrollBars      =   2  'Vertical
      TabIndex        =   38
      Top             =   7800
      Width           =   11055
   End
   Begin VB.ComboBox TagType 
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   360
      ItemData        =   "Main.frx":0CF6
      Left            =   600
      List            =   "Main.frx":0D15
      Style           =   2  'Dropdown List
      TabIndex        =   33
      Top             =   3480
      Width           =   2295
   End
   Begin VB.Frame Frame11 
      Caption         =   "CRC"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   975
      Left            =   10560
      TabIndex        =   23
      Top             =   4560
      Width           =   975
      Begin VB.TextBox CRC_box 
         Enabled         =   0   'False
         BeginProperty Font 
            Name            =   "Courier New"
            Size            =   12
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   120
         MaxLength       =   4
         TabIndex        =   24
         Top             =   360
         Width           =   735
      End
   End
   Begin VB.Frame Frame10 
      Caption         =   "DATA"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   975
      Left            =   480
      TabIndex        =   21
      Top             =   4560
      Width           =   9735
      Begin VB.TextBox DATA_box 
         Enabled         =   0   'False
         BeginProperty Font 
            Name            =   "Courier New"
            Size            =   12
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   390
         Left            =   240
         MaxLength       =   8
         TabIndex        =   22
         Top             =   360
         Width           =   9255
      End
   End
   Begin VB.Frame Frame9 
      Caption         =   "NUMBER OF BLOCKS"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   975
      Left            =   9000
      TabIndex        =   19
      Top             =   3120
      Width           =   2535
      Begin VB.TextBox NUMBER_OF_BLOCKS_box 
         BeginProperty DataFormat 
            Type            =   0
            Format          =   "0"
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   1033
            SubFormatType   =   0
         EndProperty
         Enabled         =   0   'False
         BeginProperty Font 
            Name            =   "Courier New"
            Size            =   12
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   960
         MaxLength       =   2
         TabIndex        =   20
         Top             =   360
         Width           =   495
      End
   End
   Begin VB.Frame Frame8 
      Caption         =   "STARTING BLOCK"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   975
      Left            =   6600
      TabIndex        =   17
      Top             =   3120
      Width           =   2175
      Begin VB.TextBox STARTING_BLOCK_box 
         BeginProperty DataFormat 
            Type            =   0
            Format          =   "0"
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   1033
            SubFormatType   =   0
         EndProperty
         Enabled         =   0   'False
         BeginProperty Font 
            Name            =   "Courier New"
            Size            =   12
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   840
         MaxLength       =   2
         TabIndex        =   18
         Top             =   360
         Width           =   495
      End
   End
   Begin VB.Frame Frame7 
      Caption         =   "TID"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   975
      Left            =   3240
      TabIndex        =   15
      Top             =   3120
      Width           =   3015
      Begin VB.TextBox TID_box 
         Enabled         =   0   'False
         BeginProperty Font 
            Name            =   "Courier New"
            Size            =   12
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   240
         MaxLength       =   16
         TabIndex        =   16
         Top             =   360
         Width           =   2535
      End
   End
   Begin VB.Frame Frame6 
      Caption         =   "TAG TYPE"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   975
      Left            =   480
      TabIndex        =   14
      Top             =   3120
      Width           =   2535
   End
   Begin VB.Frame Frame5 
      Caption         =   "RID"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   1215
      Left            =   10320
      TabIndex        =   12
      Top             =   1440
      Width           =   1215
      Begin VB.TextBox RID_box 
         Enabled         =   0   'False
         BeginProperty Font 
            Name            =   "Courier New"
            Size            =   12
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   360
         MaxLength       =   2
         TabIndex        =   13
         Top             =   480
         Width           =   495
      End
   End
   Begin VB.Frame Frame2 
      Caption         =   "COMMAND"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   1215
      Left            =   480
      TabIndex        =   5
      Top             =   1440
      Width           =   9495
      Begin VB.TextBox COMMAND_box 
         Alignment       =   2  'Center
         BeginProperty Font 
            Name            =   "Courier New"
            Size            =   12
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   8520
         TabIndex        =   36
         Text            =   "14"
         Top             =   480
         Width           =   495
      End
      Begin VB.Frame Frame4 
         Caption         =   "Command_Target"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   9.75
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   735
         Left            =   4320
         TabIndex        =   7
         Top             =   240
         Width           =   3735
         Begin VB.OptionButton Sys_Bit 
            Caption         =   "System"
            BeginProperty Font 
               Name            =   "MS Sans Serif"
               Size            =   9.75
               Charset         =   0
               Weight          =   400
               Underline       =   0   'False
               Italic          =   0   'False
               Strikethrough   =   0   'False
            EndProperty
            Height          =   375
            Left            =   1200
            TabIndex        =   35
            Top             =   240
            Width           =   1095
         End
         Begin VB.OptionButton Tag_Bit 
            Caption         =   "Tag"
            BeginProperty Font 
               Name            =   "MS Sans Serif"
               Size            =   9.75
               Charset         =   0
               Weight          =   400
               Underline       =   0   'False
               Italic          =   0   'False
               Strikethrough   =   0   'False
            EndProperty
            Height          =   375
            Left            =   240
            TabIndex        =   11
            Top             =   240
            Value           =   -1  'True
            Width           =   855
         End
         Begin VB.OptionButton Mem_Bit 
            Caption         =   "Memory"
            BeginProperty Font 
               Name            =   "MS Sans Serif"
               Size            =   9.75
               Charset         =   0
               Weight          =   400
               Underline       =   0   'False
               Italic          =   0   'False
               Strikethrough   =   0   'False
            EndProperty
            Height          =   270
            Left            =   2400
            TabIndex        =   10
            Top             =   285
            Width           =   1095
         End
      End
      Begin VB.Frame Frame3 
         Caption         =   "Command_Type"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   9.75
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   735
         Left            =   600
         TabIndex        =   6
         Top             =   240
         Width           =   3375
         Begin VB.OptionButton Sel_Bit 
            Caption         =   "SELECT"
            Height          =   195
            Left            =   2160
            TabIndex        =   34
            Top             =   360
            Value           =   -1  'True
            Width           =   1095
         End
         Begin VB.OptionButton Write_Bit 
            Caption         =   "WRITE"
            Height          =   195
            Left            =   120
            TabIndex        =   9
            Top             =   360
            Width           =   855
         End
         Begin VB.OptionButton Read_Bit 
            Caption         =   "READ"
            Height          =   195
            Left            =   1200
            TabIndex        =   8
            Top             =   360
            Width           =   855
         End
      End
   End
   Begin VB.Frame Frame1 
      Caption         =   "FLAGS"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   855
      Left            =   480
      TabIndex        =   4
      Top             =   120
      Width           =   11055
      Begin VB.TextBox FLAGS_box 
         Alignment       =   2  'Center
         BeginProperty Font 
            Name            =   "Courier New"
            Size            =   12
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   10080
         TabIndex        =   37
         Text            =   "00"
         Top             =   360
         Width           =   495
      End
      Begin VB.CheckBox LOOP_F 
         Caption         =   "LOOP_F"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   9.75
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         ForeColor       =   &H00000012&
         Height          =   255
         Left            =   8520
         TabIndex        =   32
         Top             =   360
         Width           =   1335
      End
      Begin VB.CheckBox INV_F 
         Caption         =   "INV_F"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   9.75
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   7320
         TabIndex        =   31
         Top             =   360
         Width           =   975
      End
      Begin VB.CheckBox LOCK_F 
         Caption         =   "LOCK_F"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   9.75
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   6000
         TabIndex        =   30
         Top             =   360
         Width           =   1215
      End
      Begin VB.CheckBox RF_F 
         Caption         =   "RF_F"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   9.75
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   4800
         TabIndex        =   29
         Top             =   360
         Width           =   1095
      End
      Begin VB.CheckBox AFI_F 
         Caption         =   "AFI_F"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   9.75
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   3600
         TabIndex        =   28
         Top             =   360
         Width           =   1095
      End
      Begin VB.CheckBox CRC_F 
         Caption         =   "CRC_F"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   9.75
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   2400
         TabIndex        =   27
         Top             =   360
         Width           =   975
      End
      Begin VB.CheckBox TID_F 
         Caption         =   "TID_F"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   9.75
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   1320
         TabIndex        =   26
         Top             =   360
         Width           =   975
      End
      Begin VB.CheckBox RID_F 
         Caption         =   "RID_F"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   9.75
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   240
         TabIndex        =   25
         Top             =   360
         Width           =   975
      End
   End
   Begin VB.TextBox HostCommand_box 
      BeginProperty Font 
         Name            =   "Courier New"
         Size            =   12
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   375
      Left            =   480
      TabIndex        =   2
      Top             =   6360
      Width           =   11055
   End
   Begin VB.CommandButton btnExit 
      Caption         =   "E&xit"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   375
      Left            =   480
      TabIndex        =   1
      Top             =   9240
      Width           =   2235
   End
   Begin VB.CommandButton btnSendHostCommand 
      Caption         =   "&Send Host Command"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   495
      Left            =   8280
      TabIndex        =   0
      Top             =   7080
      Width           =   3195
   End
   Begin MSCommLib.MSComm MSComm1 
      Left            =   5520
      Top             =   9120
      _ExtentX        =   1005
      _ExtentY        =   1005
      _Version        =   393216
      DTREnable       =   0   'False
      InBufferSize    =   256
      InputLen        =   255
      RThreshold      =   1
   End
   Begin VB.Label Label1 
      BackStyle       =   0  'Transparent
      Caption         =   "Target Response:"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   480
      TabIndex        =   39
      Top             =   7560
      Width           =   2055
   End
   Begin VB.Label Label12 
      BackStyle       =   0  'Transparent
      Caption         =   "Host Request:"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   480
      TabIndex        =   3
      Top             =   6120
      Width           =   1695
   End
   Begin VB.Menu mnuFile 
      Caption         =   "&File"
      Begin VB.Menu mnuFileExit 
         Caption         =   "E&xit"
      End
   End
   Begin VB.Menu mnuBaudRate 
      Caption         =   "&System"
      Begin VB.Menu mnuBaudRate_show 
         Caption         =   "Detect Reader"
      End
      Begin VB.Menu mnuSystem_batt 
         Caption         =   "Read Battery Voltage"
      End
   End
End
Attribute VB_Name = "frmMain"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
'-------------------------------------------------------
'Declarations to Disable the 'X' button on the form menu
'-------------------------------------------------------
Private Const SC_CLOSE As Long = &HF060&
Private Const MF_BYCOMMAND = &H0&
Private Declare Function DeleteMenu Lib "user32.dll" (ByVal hMenu As Long, ByVal nPosition As Long, ByVal wFlags As Long) As Long
Private Declare Function GetSystemMenu Lib "user32.dll" (ByVal hWnd As Long, ByVal bRevert As Long) As Long
'-----------------------------------------------------------------------------------------------------------------------------------


Rem SkyeTek Protocol REQUEST fields
Dim response$
Dim flags%           ' FLAGS field
Dim command%         ' COMMAND field
Dim temp As String
Dim invalidF As Boolean
Dim tag_type As String
Dim secondLF As Boolean
Dim freezeF As Boolean
Dim protocolF As Boolean
Dim stxF As Boolean
Dim fwverH$
Dim fwverL$
Dim fwver$
Dim serNum$
Dim protocolv2 As Boolean

Public Function GetResponse() 'gets called from form 7 also
    Dim Start As Single
    
    ' wait .1 second to give the reader time to process the command before it responds
    Start = Timer
    Do
    Loop Until (Timer - Start) > 0.2

    response$ = ""
    Do While frmMain.MSComm1.InBufferCount > 0
        DoEvents
        response$ = response$ & frmMain.MSComm1.Input
        If Right$(response$, 2) = vbCrLf Then
            frmMain.MSComm1.InBufferCount = 0 ' clears the receive buffer
            Exit Do
        End If
    Loop
    
    If Not Right$(response$, 2) = vbCrLf Then
        ' unknown
        response$ = ""
        GetResponse = False
    ElseIf Not Left$(response$, 1) = vbLf Then
        ' unknown
        response$ = ""
        GetResponse = False
    Else
        GetResponse = True
    End If

End Function
Private Function IsNotHex(KeyAscii As Integer)

If KeyAscii > &H46 Then
    IsNotHex = True
End If

If KeyAscii < &H30 Then
    IsNotHex = True
    
    If KeyAscii = 8 Then
        IsNotHex = False
    End If
End If

If KeyAscii > &H39 Then
    If KeyAscii < &H41 Then
        IsNotHex = True
    End If
End If

End Function


Private Sub ASCII_btn_Click()
    protocolF = True
    Call refresh_HostCommand_box
    Response_box = ""
End Sub
Private Sub Binary_btn_Click()
    protocolF = False
    Call refresh_HostCommand_box
    Response_box = ""
End Sub
Private Sub Clear_Click()
    Response_box.Text = ""
    secondLF = False
End Sub
Private Function setcom() As Boolean
    
    On Error GoTo X:
    frmMain.MSComm1.PortOpen = True
    setcom = True
    'End Function
    
X:
    If frmMain.MSComm1.PortOpen = False Then
        setcom = False
    End If

End Function
Private Sub Form_Load()
 Dim result$

 'Disble 'X' button on form menu
 DeleteMenu GetSystemMenu(Me.hWnd, False), SC_CLOSE, MF_BYCOMMAND

   
 Call mnuBaudRate_show_Click
 
    ' Set default values
    flags% = 0
    command% = &H14
    secondLF = False
    freezeF = False
        
    NUMBER_OF_BLOCKS_box.Locked = False
    protocolF = True   ' ASCII mode
    stxF = False
    TagType.Text = TagType.List(0) ' Sets tag_type$ and forces call to refresh_HostCommand_box in TagType_Click
                
    'Update the Flags
    UpdateFlags
            
End Sub
Private Function detectReader()
 
 Dim Start As Single
 Dim baudrate As Integer
 Dim com As Integer
 Dim result$
 Dim X$ ' to clear unwanted responce characters
 
Do
    For com = 1 To 6
    'com$ = InputBox("Which COM port is the SkyeRead RFID reader attached to?", "Select COM Port", "")
        
        If frmMain.MSComm1.PortOpen = True Then
            frmMain.MSComm1.PortOpen = False
        End If
        
        frmMain.MSComm1.CommPort = com
        frmMain.MSComm1.InputLen = 1
        frmMain.MSComm1.RThreshold = 0  ' disble OnComm
        'frmMain.MSComm1.PortOpen = True
        If setcom() = True Then
            frmMain.MSComm1.InputMode = comInputModeText
        
            For baudrate = 0 To 3
                Select Case baudrate
                    Case 0
                        '9600
                        frmMain.MSComm1.Settings = "9600,N,8,1"
                
                    Case 1
                        ' 19200
                        frmMain.MSComm1.Settings = "19200,N,8,1"
                
                    Case 2
                        ' 38400
                        frmMain.MSComm1.Settings = "38400,N,8,1"
                
                    Case 3
                        ' 57600
                        frmMain.MSComm1.Settings = "57600,N,8,1"
                             
                End Select
                
                If frmMain.MSComm1.PortOpen = True Then
                    frmMain.MSComm1.Output = vbCr & "00220101" & vbCr  ' request the reader firmware version
                End If
                


                If GetResponse() = True Then
                    
                        fwverH$ = Mid$(response$, 4, 2)
                        fwverL$ = Mid$(response$, 6, 2)
                        
                        fwver$ = fwverH$
                        
                        If fwverL$ = vbCrLf Then
                            ' this must be firmware version 000D or lower so it has no serial number
                            fwver$ = "00" & fwverH$
                            serNum$ = "Not Available"
                            Exit Function
                        End If
                        
                        fwver$ = fwverH$ & fwverL$
            
                        If frmMain.MSComm1.PortOpen = True Then
                            frmMain.MSComm1.Output = vbCr & "00220001" & vbCr  ' get the reader serial number for fw versions > 0D
                        End If
                        
                        If GetResponse() = True Then
                            serNum$ = Mid$(response$, 4, 8)
                        Else
                            serNum$ = "Not Detected"
                        End If
    
                        Exit Function
                    
                    End If
                
            Next  ' baud rate loop
            
        End If
        
        If frmMain.MSComm1.PortOpen = True Then
            frmMain.MSComm1.PortOpen = False
        End If
              
    Next ' try the next com port
        
    result = MsgBox("No SkyeRead Device Detected ", vbAbortRetryIgnore, "SkyeRead Device")
Loop Until ((result = vbAbort) Or (result = vbIgnore))

If (result = vbAbort) Then
    End
End If

End Function


Private Sub mnuBaudRate_show_Click()
    Dim result
    Dim a$
    
    Call detectReader
    frmMain.MSComm1.RThreshold = 1  ' Enable OnComm again
    
    If frmMain.MSComm1.PortOpen = True Then
        result = MsgBox("Baud Rate = " & frmMain.MSComm1.Settings & vbCrLf & "Firmware Version = " & fwver$ & vbCrLf & "Serial Number = " & serNum$, vbOKOnly, "SkyeRead Device Information")
    Else
        result = MsgBox("No Reader Detected", vbOKOnly, "SkyeRead Device Information")
    End If
    
    If fwver$ > "001A" Then
        protocolv2 = True
    Else
        protocolv2 = False
    End If
        
End Sub

Private Sub MSComm1_OnComm()
Dim rx$
Dim H$
Dim Start As Single
        
    If protocolF = True Then
        If frmMain.MSComm1.InBufferCount Then
            rx$ = frmMain.MSComm1.Input
            
            If rx$ = vbCr Then
                
                ' let the follwing (expected) LF to come into the buffer before we look for it
                Start = Timer
                Do
                Loop Until (Timer - Start) > 0.001
                rx$ = frmMain.MSComm1.Input
                If rx$ = vbLf Then
                    ' end of response
                    Response_box.Text = (Response_box.Text + "<CR><LF>" + vbCrLf)
                End If
            Else
                If rx$ = vbLf Then ' Check for linefeed
                    ' LF detected so it's a new response coming in
                    If INV_F.Value <> 1 Then
                        Response_box.Text = "" ' Just clear
                    End If
                    Response_box.Text = Response_box.Text + "<LF>"  ' Accumulate inventory
                Else
                    Response_box.Text = Response_box.Text + rx$ ' Build up response
                End If
            End If
        End If
        ' here we test to see if the end of the sid poll has occurred by testing whether the end of the
        ' captured string equals "<LF>94<CR><LF>"
        If (INV_F.Value = 1) And (Len(Response_box.Text) > 30) Then
            If Right(Response_box.Text, 32) = ("<LF>94<CR><LF>" + vbCrLf + "<LF>94<CR><LF>" + vbCrLf) Then
                Response_box.Text = Left(Response_box.Text, Len(Response_box.Text) - 16)
                freezeF = False
            End If
        End If
    Else    ' Binary Mode
        If stxF = False Then   ' if this is the beginning of a STX response then display the ASCII STX
            If frmMain.MSComm1.InBufferCount Then
                stxF = True
                rx$ = frmMain.MSComm1.Input
                If rx$ = Chr$(2) Then
                    Response_box.Text = "<STX>"
                Else
                    Response_box.Text = "ERROR"
                End If
            End If
        Else
        ' stxF was true --> already got the STX so keep displaying incoming bytes (2 ascii chars per byte) until >20ms
            Start = Timer
            Do
                If frmMain.MSComm1.InBufferCount Then
                   Start = Timer
                    rx$ = frmMain.MSComm1.Input
                    H$ = Hex(Asc(rx$))
                    Response_box.Text = Response_box.Text & Left("00", 2 - Len(H$)) & H$
                End If
            Loop Until (Timer - Start) > 0.1
            stxF = False
        End If
    End If
End Sub



Private Sub RID_F_Click()
    If RID_F.Value = 1 Then
      flags% = flags% Or &H80     ' sets BIT7
      RID_box.Enabled = True
      RID_box.Text = "00"
    Else
      flags% = flags% And &H7F    ' clears BIT7
      RID_box.Enabled = False
      RID_box.Text = ""
    End If
    Call refresh_FLAGS_box
    Call refresh_HostCommand_box
End Sub
Private Sub TID_F_Click()
    If TID_F.Value = 1 Then
      flags% = flags% Or &H40     ' sets BIT6
      TID_box.Enabled = True
      
    Else
      flags% = flags% And &HBF    ' clears BIT6
      TID_box.Enabled = False
      TID_box.Text = ""
    End If
    
    Call CalcBlockLength
    Call refresh_FLAGS_box
    Call refresh_HostCommand_box
End Sub
Private Sub CRC_F_Click()
    If CRC_F.Value = 1 Then
      flags% = flags% Or &H20     ' sets BIT5
      CRC_box.Enabled = True
    Else
      flags% = flags% And &HDF    ' clears BIT5
      CRC_box.Enabled = False
      CRC_box.Text = ""
    End If
    
    Call refresh_FLAGS_box
    Call refresh_HostCommand_box
End Sub
Private Sub AFI_F_Click()
    If AFI_F.Value = 1 Then
        AFI_F.Value = 0
    End If
End Sub
Private Sub RF_F_Click()
    If RF_F.Value = 1 Then
      flags% = flags% Or &H8      ' sets BIT3
    Else
      flags% = flags% And &HF7    ' clears BIT3
    End If
    
   Call refresh_FLAGS_box
   Call refresh_HostCommand_box
End Sub
Private Sub LOCK_F_Click()
    If LOCK_F.Value = 1 Then
        flags% = flags% Or &H4   'sets BIT2
    Else
        flags% = flags% And &HFB 'clears BIT2
        'LOCK_F.Value = 0
    End If
    
    Call refresh_FLAGS_box
    Call refresh_HostCommand_box
    
'    If LOCK_F.Value = 1 Then
'        LOCK_F.Value = 0
'    End If

End Sub
Private Sub INV_F_Click()
    If Sel_Bit.Value = False Then
        INV_F.Value = 0
    End If
    
    If INV_F.Value = 1 Then
      flags% = flags% Or &H2       ' sets BIT1
    Else
      flags% = flags% And &HFD    ' clears BIT1
    End If
    
    Call refresh_FLAGS_box
    Call refresh_HostCommand_box
End Sub
Private Sub LOOP_F_Click()
    If LOOP_F.Value = 1 Then
      flags% = flags% Or &H1     ' sets BIT0
    Else
      flags% = flags% And &HFE    ' clears BIT0
    End If
    
   Call refresh_FLAGS_box
   Call refresh_HostCommand_box
End Sub
Private Sub Write_Bit_Click()

    'INV_F.Value = 0
    'INV_F.Enabled = False
    
    'flags% = flags% And &HFD    ' clears BIT1
    
    ' Update the Flags
    UpdateFlags
    
    STARTING_BLOCK_box.Enabled = True
    If STARTING_BLOCK_box.Text = "" Then
        STARTING_BLOCK_box.Text = "00"
    End If
    
    NUMBER_OF_BLOCKS_box.Enabled = True
    If NUMBER_OF_BLOCKS_box.Text = "" Then
        NUMBER_OF_BLOCKS_box.Text = "01"
    End If
        
    DATA_box.Enabled = True
    
    command% = command% And &HF  ' clears high nibble
    command% = command% Or &H40   ' sets high nibble to 0100
    
    Call CalcBlockLength
    Call refresh_COMMAND_box
    Call refresh_HostCommand_box
End Sub
Private Sub Read_Bit_Click()
    'INV_F.Value = 0
    'LOCK_F.Value = 0
    'flags% = flags% And &HFD    ' clears BIT1
    
    ' Update the flags
    UpdateFlags
    
    STARTING_BLOCK_box.Enabled = True
    If STARTING_BLOCK_box.Text = "" Then
        STARTING_BLOCK_box.Text = "00"
    End If
    
    NUMBER_OF_BLOCKS_box.Enabled = True
    If NUMBER_OF_BLOCKS_box.Text = "" Then
        NUMBER_OF_BLOCKS_box.Text = "01"
    End If
    
    If Sys_Bit.Value = True Then
        DATA_box.Text = ""
        DATA_box.Enabled = True
        DATA_box.MaxLength = 6
    Else
        DATA_box.Text = ""
        DATA_box.Enabled = False
    End If
    
    command% = command% And &HF  ' clears high nibble
    command% = command% Or &H20   ' sets high nibble to 0010
    
    Call CalcBlockLength
    Call refresh_COMMAND_box
    Call refresh_HostCommand_box
End Sub
Private Sub Sel_Bit_Click()
    'LOCK_F.Value = 0
    
    ' Update the Flags
    UpdateFlags
    
    STARTING_BLOCK_box.Enabled = False
    STARTING_BLOCK_box.Text = ""
    NUMBER_OF_BLOCKS_box.Enabled = False
    NUMBER_OF_BLOCKS_box.Text = ""
    
    If Tag_Bit.Value = True Then
        DATA_box.Enabled = False
        DATA_box.Text = ""
    End If
    
    command% = command% And &HF  ' clears high nibble
    command% = command% Or &H10   ' sets high nibble to 0001
    
    Call CalcBlockLength
    Call refresh_COMMAND_box
    Call refresh_HostCommand_box
End Sub
Private Sub Tag_Bit_Click()
    ' Update the Flags
    UpdateFlags
    
    TagType.Text = TagType.List(0)
    TagType.Enabled = True
    
    command% = command% And &HF0 ' clears low nibble
    command% = command% Or &H4   ' sets low nibble to 0100
    
    Call CalcBlockLength
    Call refresh_COMMAND_box
    Call refresh_HostCommand_box
End Sub
Private Sub Sys_Bit_Click()
    ' Update the Flags
    UpdateFlags
    
    TagType.Enabled = False
    
    command% = command% And &HF0 ' clears low nibble
    command% = command% Or &H2   ' sets low nibble to 0010
    
    Call refresh_COMMAND_box
    Call refresh_HostCommand_box
End Sub
Private Sub Mem_Bit_Click()
    ' Update the Flags
    UpdateFlags
    
    TagType.Enabled = False
    
    command% = command% And &HF0 ' clears low nibble
    command% = command% Or &H1   ' sets low nibble to 0001
    
    Call refresh_COMMAND_box
    Call refresh_HostCommand_box
End Sub
Private Sub RID_box_Change()
    If invalidF = True Then
        RID_box.Text = temp
    End If
    
    Call refresh_HostCommand_box
End Sub
Private Sub RID_box_KeyPress(KeyAscii As Integer)
    temp = RID_box.Text
    invalidF = IsNotHex(KeyAscii)
End Sub
Private Sub TagType_Change()
    Call refresh_HostCommand_box
End Sub
Private Sub TagType_Click()
    
    TID_box.Enabled = True
    
    Select Case TagType.Text
        Case "00  Auto-Detect"
            tag_type = "00"
            TID_box.Text = ""
            TID_box.Enabled = False
        Case "01  ISO15693"
            tag_type = "01"
            TID_box.MaxLength = 16
            TID_box.Text = ""
        Case "02  I·Code SL1"
            tag_type = "02"
            TID_box.MaxLength = 16
            TID_box.Text = ""
        Case "03  Tag-it HF"
            tag_type = "03"
            TID_box.MaxLength = 8
            TID_box.Text = ""
        Case "04  ISO14443A"
            tag_type = "04"
            TID_box.MaxLength = 8
            TID_box.Text = ""
        Case "06  PicoTag"
            tag_type = "06"
            TID_box.MaxLength = 16
            TID_box.Text = ""
        Case "08  GemWave C210"
            tag_type = "08"
            TID_box.MaxLength = 0
            TID_box.Text = ""
    End Select
    
Call CalcBlockLength
Call refresh_HostCommand_box
End Sub
Private Sub TID_box_Change()
    If invalidF = True Then
        TID_box.Text = temp
    End If
    
    Call CalcBlockLength
    Call refresh_HostCommand_box
End Sub
Private Sub TID_box_KeyPress(KeyAscii As Integer)
    temp = TID_box.Text
    invalidF = IsNotHex(KeyAscii)
End Sub
Private Sub STARTING_BLOCK_box_Change()
    If invalidF = True Then
        STARTING_BLOCK_box.Text = temp
    End If
    
    Call refresh_HostCommand_box
End Sub
Private Sub STARTING_BLOCK_box_KeyPress(KeyAscii As Integer)
    temp = STARTING_BLOCK_box.Text
    invalidF = IsNotHex(KeyAscii)
End Sub


Private Function CalcBlockLength()
' determine block length here
        CalcBlockLength = 2
             
        If Read_Bit.Value = True And Sys_Bit.Value = True Then
            CalcBlockLength = 6
        End If
        
        If Write_Bit.Value = True And Sys_Bit.Value = True Then
            If STARTING_BLOCK_box.Text = "0A" Or STARTING_BLOCK_box.Text = "0C" Then
                CalcBlockLength = 160
            End If
        End If
             
        Select Case TagType
            Case "03  Tag-it HF"
                CalcBlockLength = 8
        End Select

        If Len(TID_box.Text) = 16 Then
            CalcBlockLength = 8              ' 4 bytes per block
            If (Val("&H" & Mid(TID_box.Text, 3, 2)) = 5) Then
                CalcBlockLength = 16  ' 8 bytes per block for Infineon my-d tags
            End If
        End If
    
        If Len(TID_box.Text) = 8 Then
            CalcBlockLength = 8              ' 4 bytes per block
        End If
        
        If Len(NUMBER_OF_BLOCKS_box.Text) = 2 Then
            ' note the value (text) shown in the NUMBER_OF_BLOCKS_box must be interpreted as a hex value
            DATA_box.MaxLength = Val("&H" & Mid(NUMBER_OF_BLOCKS_box.Text, 1, 2)) * CalcBlockLength
        End If
        
    ' block length now determined
End Function

Private Sub NUMBER_OF_BLOCKS_box_Change()
    Dim BlockLength%
    
    If invalidF = True Then
        NUMBER_OF_BLOCKS_box.Text = temp
    End If
    
        
    Call CalcBlockLength
    Call refresh_HostCommand_box
End Sub
Private Sub NUMBER_OF_BLOCKS_box_KeyPress(KeyAscii As Integer)
    temp = NUMBER_OF_BLOCKS_box.Text
    invalidF = IsNotHex(KeyAscii)
End Sub
Private Sub DATA_box_Change()
    If invalidF = True Then
        DATA_box.Text = temp
    End If
    
    Call refresh_HostCommand_box
End Sub
Private Sub DATA_box_KeyPress(KeyAscii As Integer)
    temp = DATA_box.Text
    invalidF = IsNotHex(KeyAscii)
End Sub
Private Sub CRC_box_Change()
    If invalidF = True Then
        CRC_box.Text = temp
    End If
End Sub
Private Sub CRC_box_KeyPress(KeyAscii As Integer)
    temp = CRC_box.Text
    invalidF = IsNotHex(KeyAscii)
    Call refresh_HostCommand_box
End Sub
Private Sub btnExit_Click()
    mnuFileExit_Click
End Sub
Private Sub btnSendHostCommand_Click()
Dim i As Integer   'Loop Variable

    ' if the inventory comand is not yet complete (i.e. if it ends with <LF><94><LF>) then dont send another command
    If INV_F.Value = 1 Then
        freezeF = True
    End If
    
    Call refresh_HostCommand_box
    
    frmMain.MSComm1.InBufferCount = 0    ' clear buffer
                
    If protocolF = True Then
        ' Send Host command as ASCII
        If secondLF = False Then
            ' test here to see if the inventory command is complete
            ' i.e. if the host response box contains ends with either <LF>94<LF> in ascii mode
            ' if the host response box ends with <STX>94 in binary mode, then the inventory
            ' command (in which INV_F=1) is complete and thus it's ok to send this next command
            If Len(Response_box.Text) > 10 Then
                If Mid(Response_box.Text, (Len(Response_box.Text) - 11), 12) = ("<LF>94<LF>" + vbCrLf) Then
                    If INV_F.Value = 0 Then
                        Response_box.Text = ""
                    End If
                End If
            End If
        End If
    
        frmMain.MSComm1.Output = ""
        frmMain.MSComm1.Output = vbCr
        For i = 5 To (Len(HostCommand_box.Text) - 4)
            frmMain.MSComm1.Output = Mid(HostCommand_box.Text, i, 1)
        Next i
        frmMain.MSComm1.Output = vbCr
    Else
        If Len(Response_box.Text) > 7 Then
            If Mid(Response_box.Text, (Len(Response_box.Text) - 8), 9) = ("<STX>94" + vbCrLf) Then
                If INV_F.Value = 0 Then
                    Response_box.Text = ""
                End If
            End If
        End If
        ' Send Host command as Binary
        frmMain.MSComm1.Output = ""
        frmMain.MSComm1.Output = Chr$(2)
        
        For i = 6 To Len(HostCommand_box.Text) Step 2
            frmMain.MSComm1.Output = Chr$(Val("&H" & Mid(HostCommand_box.Text, i, 2)))
        Next i
    End If
End Sub
Private Sub mnuFileExit_Click()
    If (frmMain.MSComm1.PortOpen = True) Then
            frmMain.MSComm1.PortOpen = False
    End If
    Unload frmMain
End Sub
Sub refresh_COMMAND_box()
    Dim H$
    H$ = Hex(command%)
    COMMAND_box.Text = Left("00", 2 - Len(H$)) & H$
End Sub

Sub refresh_FLAGS_box()
    Dim H$
    H$ = Hex(flags%)
    FLAGS_box.Text = Left("00", 2 - Len(H$)) & H$
End Sub
Sub refresh_HostCommand_box()
    Dim byteCount As Integer
    Dim strByteCount As String

    If TID_F.Value = 0 Then
        TID_box.Text = ""
        TID_box.Enabled = False
    End If
    
    If COMMAND_box.Text = "14" Then
        STARTING_BLOCK_box.Text = ""
        NUMBER_OF_BLOCKS_box.Text = ""
        DATA_box.Text = ""
    End If
    
    If TagType.Enabled = False Then
        tag_type = ""
    Else
        TID_box.Enabled = True
        Select Case TagType.Text
            Case "00  Auto-Detect"
                tag_type = "00"
                TID_box.Text = ""
                TID_box.Enabled = False
            Case "01  ISO15693"
                tag_type = "01"
                TID_box.MaxLength = 16
            Case "02  I·Code SL1"
                tag_type = "02"
                TID_box.MaxLength = 16
            Case "03  Tag-it HF"
                tag_type = "03"
                TID_box.MaxLength = 8
            Case "04  ISO14443A"
                tag_type = "04"
                TID_box.MaxLength = 8
            Case "06  PicoTag"
                tag_type = "06"
                TID_box.MaxLength = 16
            Case "08  GemWave C210"
                tag_type = "08"
                TID_box.MaxLength = 0
        End Select
    End If
    
    If protocolF = True Then     ' ascii mode
        HostCommand_box.Text = "<CR>" + FLAGS_box.Text + COMMAND_box.Text + RID_box.Text + tag_type + TID_box.Text + STARTING_BLOCK_box.Text + NUMBER_OF_BLOCKS_box.Text + DATA_box.Text
        If CRC_F.Value = 1 Then
            Call refresh_CRC_box
        End If
        HostCommand_box.Text = HostCommand_box.Text + CRC_box.Text + "<CR>"
    End If
    
    ' october 26 work-around
    If TagType.Enabled = False Then
        tag_type = ""
    End If
    
    If protocolF = False Then     ' binary mode
                
        If protocolv2 = True Then
            byteCount = (Len(FLAGS_box.Text) + Len(COMMAND_box.Text) + Len(RID_box.Text) + Len(tag_type) + Len(TID_box.Text) + Len(STARTING_BLOCK_box.Text) + Len(NUMBER_OF_BLOCKS_box.Text) + Len(DATA_box.Text)) / 2
            
            If CRC_F.Value = 1 Then
            byteCount = byteCount + 2
            End If
            
            strByteCount = Hex(byteCount)
            
            If (byteCount < 16) Then
                strByteCount = "0" + strByteCount
            End If
        Else
            strByteCount = ""
        End If
        
        HostCommand_box.Text = "<STX>" + strByteCount + FLAGS_box.Text + COMMAND_box.Text + RID_box.Text + tag_type + TID_box.Text + STARTING_BLOCK_box.Text + NUMBER_OF_BLOCKS_box.Text + DATA_box.Text
        If CRC_F.Value = 1 Then
            Call refresh_CRC_box
        End If
        HostCommand_box.Text = HostCommand_box.Text + CRC_box.Text
    End If
End Sub

Sub refresh_CRC_box()
Dim H$
Dim X As Integer, byteHL%
Dim i As Integer, j As Integer 'Loop Variables
Dim CRC_16 As Long
Const Poly As Long = 33800   ' Cannot use &H8404 due to bug in VB6 relating to sign extension?
                                                'Temp fix to bug would be &H8408 Xor &HFFFF0000. However this will fail if bug fixed.
    CRC_16 = 0
    If protocolF = True Then ' ASCII mode
        X = 5  ' ASCII Mode
    Else
        X = 6  ' Binary Mode
    End If
    
    For i = X To Len(HostCommand_box.Text) Step 2
        byteHL% = Val("&H" & Mid(HostCommand_box.Text, i, 2))
        ' byteHL% is the next byte to CRC
        CRC_16 = CRC_16 Xor byteHL%
        For j = 1 To 8
            If (CRC_16 And 1) Then
                CRC_16 = CRC_16 \ 2
                CRC_16 = (CRC_16 Xor Poly)
            Else
                CRC_16 = CRC_16 \ 2
            End If
        Next j
    Next i
    H$ = Hex(CRC_16)
    CRC_box.Text = Left("0000", 4 - Len(H$)) & H$
End Sub

Sub UpdateFlags()

    If Sel_Bit.Value = True Then
        
        If Tag_Bit.Value = True Then
            TID_F.Enabled = True
            RF_F.Enabled = True
            INV_F.Enabled = True
            LOOP_F.Enabled = True
            AFI_F.Enabled = True
            LOCK_F.Value = 0
            LOCK_F.Enabled = False
            flags% = flags% And &HFB    ' clears BIT2
        Else
            TID_F.Value = 0
            TID_F.Enabled = False
            RF_F.Value = 0
            RF_F.Enabled = False
            INV_F.Value = 0
            INV_F.Enabled = False
            LOOP_F.Value = 0
            LOOP_F.Enabled = False
            AFI_F.Value = 0
            AFI_F.Enabled = False
            LOCK_F.Value = 0
            LOCK_F.Enabled = False
            flags% = flags% And &HA0    ' clears BITs 0,1,2,3,4 and 6
        End If
    End If
    
    If Read_Bit.Value = True Then
    
        If Tag_Bit.Value = True Then
            'TID_F.Value = 0
            TID_F.Enabled = True
            'RF_F.Value = 0
            RF_F.Enabled = True
            INV_F.Value = 0
            INV_F.Enabled = False
            LOOP_F.Value = 0
            LOOP_F.Enabled = False
            AFI_F.Value = 0
            AFI_F.Enabled = False
            LOCK_F.Value = 0
            LOCK_F.Enabled = False
            flags% = flags% And &HE8    ' clears BITs 0,1,2 and 4
        Else
            TID_F.Value = 0
            TID_F.Enabled = False
            RF_F.Value = 0
            RF_F.Enabled = False
            INV_F.Value = 0
            INV_F.Enabled = False
            LOOP_F.Value = 0
            LOOP_F.Enabled = False
            AFI_F.Value = 0
            AFI_F.Enabled = False
            LOCK_F.Value = 0
            LOCK_F.Enabled = False
            flags% = flags% And &HA0    ' clears BITs 0,1,2,3,4 and 6
        End If
    End If
    
    If Write_Bit.Value = True Then
    
        If Tag_Bit.Value = True Then
            'TID_F.Value = 0
            TID_F.Enabled = True
            'RF_F.Value = 0
            RF_F.Enabled = True
            INV_F.Value = 0
            INV_F.Enabled = False
            LOOP_F.Value = 0
            LOOP_F.Enabled = False
            AFI_F.Value = 0
            AFI_F.Enabled = False
            'LOCK_F.Value = 0
            LOCK_F.Enabled = True
            flags% = flags% And &HEC    ' clears BITs 0,1,2 and 4
        Else
            TID_F.Value = 0
            TID_F.Enabled = False
            RF_F.Value = 0
            RF_F.Enabled = False
            INV_F.Value = 0
            INV_F.Enabled = False
            LOOP_F.Value = 0
            LOOP_F.Enabled = False
            AFI_F.Value = 0
            AFI_F.Enabled = False
            LOCK_F.Value = 0
            LOCK_F.Enabled = False
            flags% = flags% And &HA0    ' clears BITs 0,1,2,3,4 and 6
        End If
    
    End If
    
End Sub

