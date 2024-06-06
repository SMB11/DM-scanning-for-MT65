Imports System.Text
Imports System.Runtime.InteropServices
Imports System.ComponentModel

Module modDeclares

    <DllImport("kernel32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)>
    Public Function SetDllDirectory(ByVal lpPathName As String) As Boolean
    End Function

    <DllImport("libnldevicemaster.dll", SetLastError:=True, CallingConvention:=CallingConvention.Cdecl)>
    Public Function nl_EnumDevices(ByRef deviceCount As Integer) As IntPtr
    End Function

    <DllImport("libnldevicemaster.dll", SetLastError:=True, CallingConvention:=CallingConvention.Cdecl)>
    Public Function nl_OpenDevice(ByVal hDeviceList As IntPtr, ByVal index As Int32, ByVal protocol As Integer) As IntPtr
    End Function

    <DllImport("libnldevicemaster.dll", SetLastError:=True, CallingConvention:=CallingConvention.Cdecl)>
    Public Function nl_GetDevStatus(ByVal hDeviceHandle As IntPtr) As Integer
    End Function

    <DllImport("libnldevicemaster.dll", SetLastError:=True, CallingConvention:=CallingConvention.Cdecl)>
    Public Sub nl_ReleaseDevices(ByRef hDeviceList As IntPtr)
    End Sub

    <DllImport("libnldevicemaster.dll", SetLastError:=True, CallingConvention:=CallingConvention.Cdecl)>
    Public Function nl_CloseDevice(ByVal hDeviceHandle As IntPtr) As Boolean
    End Function

    <DllImport("libnldevicemaster.dll", SetLastError:=True, CallingConvention:=CallingConvention.Cdecl)>
    Public Function nl_Write(ByVal hDeviceHandle As IntPtr, ByVal cData() As Char, ByVal nLen As Integer, ByVal isPacked As Boolean) As Boolean
    End Function

    <DllImport("libnldevicemaster.dll", SetLastError:=True, CallingConvention:=CallingConvention.Cdecl)>
    Public Function nl_Read(ByVal hDeviceHandle As IntPtr, ByVal cData() As Byte, nLen As Integer, ByVal timeout As Integer) As Integer
    End Function

    <DllImport("libnldevicemaster.dll", SetLastError:=True, CallingConvention:=CallingConvention.Cdecl)>
    Public Function nl_ReadDevCfgToXml(ByVal hDeviceHandle As IntPtr, ByVal cData As String) As Boolean
    End Function

    <DllImport("libnldevicemaster.dll", SetLastError:=True, CallingConvention:=CallingConvention.Cdecl)>
    Public Function nl_GetPicSize(ByVal hDeviceHandle As IntPtr, ByRef nWidth As Integer, ByRef nHeight As Integer) As Boolean
    End Function

    <DllImport("libnldevicemaster.dll", SetLastError:=True, CallingConvention:=CallingConvention.Cdecl)>
    Public Function nl_GetPicData(ByVal hDeviceHandle As IntPtr, ByVal cData() As Byte, ByVal iBufferLength As Integer) As Boolean
    End Function

    <DllImport("libnldevicemaster.dll", SetLastError:=True, CallingConvention:=CallingConvention.Cdecl)>
    Public Function nl_SavePicDataToFile(ByVal cBmpName As String, ByVal cImgData() As Byte, ByVal nWidth As Integer, ByVal nHeight As Integer, biBitCount As Integer) As Boolean
    End Function

    <DllImport("libnldevicemaster.dll", SetLastError:=True, CallingConvention:=CallingConvention.Cdecl)>
    Public Function nl_StopListener(ByVal hDeviceHandle As IntPtr) As Boolean
    End Function

    <DllImport("libnldevicemaster.dll", SetLastError:=True, CallingConvention:=CallingConvention.Cdecl)>
    Public Sub nl_SetListener(ByVal hDeviceHandle As IntPtr, ByVal callback As DataArrivalCallback)
    End Sub


    Public Delegate Sub DataArrivalCallback(ByVal pDevicePointer As IntPtr, ByVal cData() As Byte, ByVal dataLen As Integer)

    Enum T_ErrorType
        Success = 0
        UnknownError = 1
        NotExistError = 2
        NotOpenError = 3
        AlreadyOpenError = 4
        AccessDeniedError = 5
        NotInitializedError = 6
        InvalidParamsError = 8
        InvalidFileFormatError = 9
        FileNameExtError = 10
        CommunicationError = 11
        MallocError = 12
        UpdateFailedError = 13
        NoUpdateObjectError = 14
        FileNotExistError = 15
        BufferOverflowError = 16
        FileNotSuitableError = 17
        DeviceNotUniqueError = 18
    End Enum

    Enum T_DeviceStatus
        Opened = 0
        NotOpened
        Closed
        NotClosed
        Updating
        Updated
        Writing
        Written
        Reading
        ReadOK
        GettingPicData
        GetPicDataOK
        UnknownStatus
    End Enum

    Enum T_CommunicationResult
        SendError = 0
        Support
        Unsupport
        OutOfRange
        UnknownResult
    End Enum

    Enum T_Protocol
        Nlscan = 0
    End Enum


End Module


