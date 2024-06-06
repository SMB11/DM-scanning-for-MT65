Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Window

Public Class frmMain
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim AssemblyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)
        AssemblyPath = Path.Combine(AssemblyPath, If(IntPtr.Size = 8, "x64", "x86"))
        Dim ok As Boolean = SetDllDirectory(AssemblyPath)
        If Not ok Then Throw New System.ComponentModel.Win32Exception()
    End Sub

    Private Sub ButtonStart_Click(sender As Object, e As EventArgs) Handles ButtonStart.Click

        Dim iDeviceCount As Integer = 0
        Dim pHandleListPointer As IntPtr = Nothing
        Dim pHandleDevicePointer As IntPtr = Nothing
        Dim aReceivedData(1024) As Byte
        Dim cTempOutputPath As String = Path.GetTempPath


        DataGridViewLog.Rows.Clear()

        ButtonStart.Enabled = False
        AddToLog("Application", "Start button pressed", "One moment....")

        pHandleListPointer = nl_EnumDevices(iDeviceCount)

        AddToLog("nl_EnumDevices", "Enumerate connected devices", "Devices found : " & iDeviceCount & " - Handle : " & pHandleListPointer.ToString)

        For i As Integer = 0 To iDeviceCount - 1
            pHandleDevicePointer = nl_OpenDevice(pHandleListPointer, i, T_Protocol.Nlscan)
            AddToLog("nl_OpenDevice", "Open device " & i + 1, "Device handle : " & pHandleDevicePointer.ToString)

            Dim devStatus As T_DeviceStatus = nl_GetDevStatus(pHandleDevicePointer)
            AddToLog("nl_GetDevStatus", "Get device " & i + 1 & " status", "Result : " & devStatus.ToString)

            AddToLog("nl_SetListener", "Please scan barcodes for 20 seconds ", "")
            nl_SetListener(pHandleDevicePointer, AddressOf CallbackSub)
            For cnt = 1 To 100
                Threading.Thread.Sleep(200)
                Application.DoEvents()
            Next

            nl_StopListener(pHandleDevicePointer)
            AddToLog("nl_StopListener", "Scan barcodes ended ", "")
            Dim aCommand() As Char = "QRYSYS".ToCharArray
            Dim bWriteResult As Boolean = nl_Write(pHandleDevicePointer, aCommand, aCommand.Length, True)
            AddToLog("nl_Write", "Write QRYSYS to device " & i + 1, "Result : " & bWriteResult.ToString)

            Dim iReadLength As Integer = nl_Read(pHandleDevicePointer, aReceivedData, aReceivedData.Length, 1000)
            AddToLog("nl_Read", "Read answer from device " & i + 1, "Answer length : " & iReadLength & " - Result : " & System.Text.Encoding.ASCII.GetString(aReceivedData))

            Dim cOutputFile As String = Path.Combine(Path.GetTempPath, "device" & (i + 1) & ".xml")
            Dim bXMLReadConfig As Boolean = nl_ReadDevCfgToXml(pHandleDevicePointer, cOutputFile)
            AddToLog("nl_ReadDevCfgToXml", "Read device config to XML ", bXMLReadConfig.ToString & " : " & cOutputFile)

            ' image

            Dim iImgWidth As Integer = 0 : Dim iImgHeight As Integer = 0

            Dim bIsGetPicSizeOK As Boolean = nl_GetPicSize(pHandleDevicePointer, iImgWidth, iImgHeight) 'Get image width And height
            AddToLog("nl_GetPicSize", "Get picture size", bIsGetPicSizeOK.ToString & " - Width : " & iImgWidth & " Height : " & iImgHeight)
            If (bIsGetPicSizeOK And iImgWidth > 0 And iImgHeight > 0) Then
                Dim iRecBufferSize As Integer = (iImgWidth * iImgHeight)
                Dim aPictureReceivedData(iRecBufferSize) As Byte
                Dim bGetPicData As Boolean = nl_GetPicData(pHandleDevicePointer, aPictureReceivedData, iRecBufferSize)
                AddToLog("nl_GetPicData", "Get picture data", bGetPicData.ToString & " - Width : " & iImgWidth & " Height : " & iImgHeight)

                If (bGetPicData) Then

                    Dim cPictureOutputFile As String = Path.Combine(Path.GetTempPath, "ImgDevice" & (i + 1) & ".bmp")
                    Dim bSavePicDataToFile As Boolean = nl_SavePicDataToFile(cPictureOutputFile, aPictureReceivedData, iImgWidth, iImgHeight, 8)
                    AddToLog("nl_SavePicDataToFile", "Save picture to file", bSavePicDataToFile.ToString & " : " & cPictureOutputFile)

                End If
            End If



            Dim bIsClosed As Boolean = nl_CloseDevice(pHandleDevicePointer)
            AddToLog("nl_CloseDevice", "Close device " & i + 1, bIsClosed.ToString)
        Next

        nl_ReleaseDevices(pHandleListPointer)
        AddToLog("nl_ReleaseDevices", "release devices", pHandleListPointer.ToString)

        ButtonStart.Enabled = True

        AddToLog("Application", "All done", "")

    End Sub



    Public Sub CallbackSub(ByVal pDevicePointer As IntPtr, ByVal cData() As Byte, ByVal dataLen As Integer)

        AddToLog("Barcode arrival", System.Text.Encoding.ASCII.GetString(cData), "")

    End Sub



    Delegate Sub _AddToLog(ByVal cAction As String, ByVal cInfo As String, ByVal cDescription As String)
    Sub AddToLog(cAction As String, cInfo As String, cDescription As String)

        If InvokeRequired Then
            Invoke(New _AddToLog(AddressOf AddToLog), New Object() {cAction, cInfo, cDescription})
        Else
            Dim iIndex As Integer = DataGridViewLog.Rows.Add(Format(Now, "HH:mm:ss.fff"), cAction, cInfo, cDescription)
            DataGridViewLog.Refresh()

        End If

    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LabelInfo.Text = "OS 64 bit = " & System.Environment.Is64BitOperatingSystem.ToString & " - 64 bit Process = " & System.Environment.Is64BitProcess.ToString
    End Sub

    Private Sub DataGridViewLog_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewLog.CellContentClick

    End Sub
End Class
