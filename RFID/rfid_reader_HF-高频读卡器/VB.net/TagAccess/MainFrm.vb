Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports System.Threading
Imports System.Collections
Public Class MainFrm
    Private hreader As UIntPtr = UIntPtr.Zero
    Private htag As UIntPtr = UIntPtr.Zero
    Private InvenThread As Thread = Nothing
    Private bInventoryThrdRun As Boolean = False

    Private Sub MainFrm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        RFIDLIB.rfidlib_reader.RDR_LoadReaderDrivers("\Drivers") ' XXXX Verzeichnis





        Dim nCount As UInt32
        nCount = RFIDLIB.rfidlib_reader.RDR_GetLoadedReaderDriverCount()
        Dim i As Integer
        For i = 0 To nCount - 1
            Dim nSize As UInt32
            Dim strName As New StringBuilder()
            strName.Append(ControlChars.NullChar, 64)
            nSize = CType(strName.Capacity, UInt32)
            RFIDLIB.rfidlib_reader.RDR_GetLoadedReaderDriverOpt(i, "NAME", strName, nSize)
            ComboBoxReader.Items.Add(strName.ToString())
        Next
        ComboBoxReader.SelectedIndex = 0

        ComboBoxComm.Items.Add("COM")
        ComboBoxComm.Items.Add("NET")
        ComboBoxComm.Items.Add("USB")
        ComboBoxComm.SelectedIndex = 0

        ComboBoxBaud.Items.Add("38400")
        ComboBoxBaud.SelectedIndex = 0

        ComboBoxFrame.Items.Add("8E1")
        ComboBoxFrame.Items.Add("8O1")
        ComboBoxFrame.Items.Add("8N1")
        ComboBoxFrame.SelectedIndex = 0

        nCount = RFIDLIB.rfidlib_reader.COMPort_Enum()
        If nCount > 0 Then
            For i = 0 To nCount - 1
                Dim nSize As UInt32
                Dim nameBuf As New StringBuilder()
                nameBuf.Append(ControlChars.NullChar, 64)
                nSize = CType(nameBuf.Capacity, UInt32)
                RFIDLIB.rfidlib_reader.COMPort_GetEnumItem(i, nameBuf, nSize)
                ComboBoxCom.Items.Add(nameBuf.ToString)
            Next
            ComboBoxCom.SelectedIndex = 0
        End If

        For i = 0 To 27
            ComboBoxBlockAddr.Items.Add(i)
            ComboBoxBlockNum.Items.Add(i + 1)
        Next
        ComboBoxBlockAddr.SelectedIndex = 0
        ComboBoxBlockNum.SelectedIndex = 0

        ComboBoxReader.Enabled = True
        ComboBoxComm.Enabled = True
        ButtonOpen.Enabled = True
        ButtonClose.Enabled = False
        ComboBoxCom.Enabled = True
        ComboBoxBaud.Enabled = True
        ComboBoxFrame.Enabled = True
        TextBoxIpAddress.Enabled = True
        TextBoxPort.Enabled = True
        TabControlFun.Enabled = False
        ButtonStart.Enabled = False
        ButtonStop.Enabled = True
        ButtonGetReaderInfo.Enabled = False

        CbxAddressModeM24LR04E.SelectedIndex = 1
        CbxPwdNum1.SelectedIndex = 0
        CbxPwdNum2.SelectedIndex = 0
        CbxSectorNum.SelectedIndex = 0
        CbxSectorPwdNum.SelectedIndex = 0
        CbxAccess.SelectedIndex = 0

        CbxStartAddressM24LR04E.SelectedIndex = 0
        CbxBlockNumM24LR04E.SelectedIndex = 0

        BtnSetLedOff.Enabled = False
        BtnSetLedOn.Enabled = False

    End Sub

    Private Sub ButtonStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonStart.Click
        If htag <> UIntPtr.Zero Then
            MessageBox.Show("Please disconnect the tag first.")
            Return
        End If
        ListViewUIDs.Items.Clear()
        ComboBoxUIDs.Items.Clear()
        CbxM24LR04EUIDs.Items.Clear()
        InvenThread = New Thread(AddressOf DoInventory)
        ButtonStart.Enabled = False
        ButtonStop.Enabled = True
        InvenThread.Start()
    End Sub

    Private Sub DoInventory()
        bInventoryThrdRun = True
        Dim iret As Integer
        Dim InvenParamSpecList As UIntPtr = UIntPtr.Zero
        Dim AntennaIDs As Byte() = New Byte(32) {}
        Dim AIType As Byte = RFIDLIB.rfidlib_def.AI_TYPE_NEW
        InvenParamSpecList = RFIDLIB.rfidlib_reader.RDR_CreateInvenParamSpecList()
        If InvenParamSpecList.ToUInt64() <> 0 Then
            'Enable ISO15693 protacol
            RFIDLIB.rfidlib_aip_iso15693.ISO15693_CreateInvenParam(InvenParamSpecList, 0, 0, 0, 0)
            'Enable ISO14443 protacol
            RFIDLIB.rfidlib_aip_iso14443A.ISO14443A_CreateInvenParam(InvenParamSpecList, 0)
            'Enable ISO18000p3m3 protacol
            RFIDLIB.rfidlib_aip_iso18000p3m3.ISO18000p3m3_CreateInvenParam(InvenParamSpecList, 0, 0, 0, RFIDLIB.rfidlib_def.ISO18000p6C_Dynamic_Q)
        End If
        While bInventoryThrdRun
            iret = RFIDLIB.rfidlib_reader.RDR_TagInventory(hreader, AIType, 0, AntennaIDs, InvenParamSpecList)
            If iret = 0 OrElse iret = -21 Then
                If iret = -21 Then
                    AIType = RFIDLIB.rfidlib_def.AI_TYPE_CONTINUE
                Else
                    AIType = RFIDLIB.rfidlib_def.AI_TYPE_NEW
                End If
                Dim TagDataReport As UIntPtr
                TagDataReport = CType(0, UIntPtr)
                TagDataReport = RFIDLIB.rfidlib_reader.RDR_GetTagDataReport(hreader, RFIDLIB.rfidlib_def.RFID_SEEK_FIRST)
                While TagDataReport.ToUInt64() > 0
                    Dim aip_id As UInt32 = 0
                    Dim tag_id As UInt32 = 0
                    Dim ant_id As UInt32 = 0
                    Dim dsfid As [Byte] = 0
                    Dim uidlen As [Byte] = 0
                    Dim uid As [Byte]() = New [Byte](32) {}
                    Dim cbTagReportHandle As delegate_tag_report_handle
                    cbTagReportHandle = New delegate_tag_report_handle(AddressOf dele_tag_report_handler)

                    'Try to parse the tag information by ISO15693
                    iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_ParseTagDataReport(TagDataReport, aip_id, tag_id, ant_id, dsfid, uid)
                    If iret = 0 Then
                        uidlen = 8
                        Dim pList As Object() = {aip_id, tag_id, ant_id, uid, CInt(uidlen)}
                        Invoke(cbTagReportHandle, pList)
                        'Go to next record
                        GoTo Next_tag
                    End If

                    'Try to parse the tag information by ISO14443A
                    iret = RFIDLIB.rfidlib_aip_iso14443A.ISO14443A_ParseTagDataReport(TagDataReport, aip_id, tag_id, ant_id, uid, uidlen)
                    If iret = 0 Then
                        Dim pList As Object() = {aip_id, tag_id, ant_id, uid, CInt(uidlen)}
                        Invoke(cbTagReportHandle, pList)
                        'Go to next record
                        GoTo Next_tag
                    End If

                    'Try to parse the tag information by ISO18000P3M3
                    Dim metaFlags As UInt32
                    Dim uidLenOf3m3 As UInt32
                    uidLenOf3m3 = uid.Length
                    iret = RFIDLIB.rfidlib_aip_iso18000p3m3.ISO18000p3m3_ParseTagDataReport(TagDataReport, aip_id, tag_id, ant_id, metaFlags, uid, uidLenOf3m3)
                    If iret = 0 Then
                        Dim pList As Object() = {aip_id, tag_id, ant_id, uid, CInt(uidLenOf3m3)}
                        Invoke(cbTagReportHandle, pList)
                        'Go to next record
                        GoTo Next_tag
                    End If
Next_tag:
                    TagDataReport = RFIDLIB.rfidlib_reader.RDR_GetTagDataReport(hreader, RFIDLIB.rfidlib_def.RFID_SEEK_NEXT)
                End While
            End If
        End While
        If InvenParamSpecList <> UIntPtr.Zero Then
            'Ddestroy the inventory parameter if it is not null.
            RFIDLIB.rfidlib_reader.DNODE_Destroy(InvenParamSpecList)
        End If
        RFIDLIB.rfidlib_reader.RDR_ResetCommuImmeTimeout(hreader)
        Dim cbThreadFinishHandle As delegate_thread_end_handle
        cbThreadFinishHandle = New delegate_thread_end_handle(AddressOf thread_end_handle)
        Invoke(cbThreadFinishHandle)
    End Sub
    Private Delegate Sub delegate_thread_end_handle()
    Private Sub thread_end_handle()
        ButtonStart.Enabled = True
        ButtonStop.Enabled = False
        InvenThread = Nothing
        If ComboBoxUIDs.Items.Count > 0 Then
            ComboBoxUIDs.SelectedIndex = 0
        End If
        If CbxM24LR04EUIDs.Items.Count > 0 Then
            CbxM24LR04EUIDs.SelectedIndex = 0
        End If
    End Sub

    Private Delegate Sub delegate_tag_report_handle(ByVal AIPType As UInt32, ByVal tagType As UInt32, ByVal antID As UInt32, ByVal uid As [Byte](), ByVal uidlen As Integer)
    Private Sub dele_tag_report_handler(ByVal AIPType As UInt32, ByVal tagType As UInt32, ByVal antID As UInt32, ByVal uid As [Byte](), ByVal uidlen As Integer)

        Dim strUid As [String]
        Dim iret As Integer
        Dim strAIPName As [String], strTagTypeName As [String]
        Dim sbAIPName As New StringBuilder()
        sbAIPName.Append(ControlChars.NullChar, 128)
        Dim nSize As UInt32 = CType(sbAIPName.Capacity, UInt32)
        iret = RFIDLIB.rfidlib_reader.RDR_GetAIPTypeName(hreader, AIPType, sbAIPName, nSize)
        If iret <> 0 Then
            strAIPName = "Unknown"
        Else
            strAIPName = sbAIPName.ToString()
        End If

        Dim sbTagName As New StringBuilder()
        sbTagName.Append(ControlChars.NullChar, 128)
        nSize = CType(sbTagName.Capacity, UInt32)
        iret = RFIDLIB.rfidlib_reader.RDR_GetTagTypeName(hreader, AIPType, tagType, sbTagName, nSize)
        If iret <> 0 Then
            strTagTypeName = "Unknown"
        Else
            strTagTypeName = sbTagName.ToString()
        End If

        strUid = BitConverter.ToString(uid, 0, CInt(uidlen)).Replace("-", String.Empty)

        Dim found As Boolean = False
        Dim i As Integer
        For i = 0 To ListViewUIDs.Items.Count - 1
            If ListViewUIDs.Items(i).SubItems(2).Text = strUid AndAlso ListViewUIDs.Items(i).SubItems(3).Text = antID.ToString() Then
                found = True
                Exit For

            End If
        Next
        If Not found Then
            Dim lvi As New ListViewItem()
            lvi.Text = strAIPName
            lvi.SubItems.Add(strTagTypeName)
            lvi.SubItems.Add(strUid)
            lvi.SubItems.Add(antID.ToString())
            lvi.SubItems.Add("1")
            ListViewUIDs.Items.Add(lvi)

            Dim ti As New tagInfo(strUid, tagType)
            ComboBoxUIDs.Items.Add(ti)
            CbxM24LR04EUIDs.Items.Add(ti)
        Else
            Dim strCounter As [String] = ListViewUIDs.Items(i).SubItems(4).Text
            Dim counter As Integer
            counter = Integer.Parse(strCounter)
            counter += 1
            If counter >= 100000 Then
                counter = 1
            End If
            ListViewUIDs.Items(i).SubItems(4).Text = counter.ToString()
        End If
        'label13.Text = listView1.Items.Count.ToString()


    End Sub
    Private Sub ButtonOpen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonOpen.Click
        Dim iret As Integer
        Dim commTypeIdx As Integer = ComboBoxComm.SelectedIndex
        Dim readerDriverName As String = ComboBoxReader.Text
        Dim connstr As String = ""
        If commTypeIdx = 0 Then
            connstr = (Convert.ToString(RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=") & readerDriverName) + ";" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_COM + ";" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMNAME + "=" + ComboBoxCom.Text + ";" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMBARUD + "=" + ComboBoxBaud.Text + ";" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMFRAME + "=" + ComboBoxFrame.Text + ";" + RFIDLIB.rfidlib_def.CONNSTR_NAME_BUSADDR + "=" + "255"
        ElseIf commTypeIdx = 1 Then
            Dim ipAddr As String
            Dim port As UInt16
            ipAddr = TextBoxIpAddress.Text
            port = CType(Integer.Parse(TextBoxPort.Text), UInt16)
            connstr = (Convert.ToString((Convert.ToString(RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=") & readerDriverName) + ";" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_NET + ";" + RFIDLIB.rfidlib_def.CONNSTR_NAME_REMOTEIP + "=") & ipAddr) + ";" + RFIDLIB.rfidlib_def.CONNSTR_NAME_REMOTEPORT + "=" + port.ToString() + ";" + RFIDLIB.rfidlib_def.CONNSTR_NAME_LOCALIP + "=" + ""
        ElseIf commTypeIdx = 2 Then
            connstr = (Convert.ToString(RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=") & readerDriverName) + ";" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_USB + ";" + RFIDLIB.rfidlib_def.CONNSTR_NAME_HIDADDRMODE + "=" + "0" + ";" + RFIDLIB.rfidlib_def.CONNSTR_NAME_HIDSERNUM + "=" + ""
        End If
        iret = RFIDLIB.rfidlib_reader.RDR_Open(connstr, hreader)
        If iret = 0 Then
            ComboBoxReader.Enabled = False
            ComboBoxComm.Enabled = False
            ButtonOpen.Enabled = False
            ButtonClose.Enabled = True
            ComboBoxCom.Enabled = False
            ComboBoxBaud.Enabled = False
            ComboBoxFrame.Enabled = False
            TextBoxIpAddress.Enabled = False
            TextBoxPort.Enabled = False
            TabControlFun.Enabled = True
            ButtonStart.Enabled = True
            ButtonStop.Enabled = False

            ButtonGetReaderInfo.Enabled = True
            ComboBoxUIDs.Enabled = True
            ButtonConnect.Enabled = True
            ButtonDisconnect.Enabled = False
            ButtonGetTagInfor.Enabled = False
            TextBoxBlockBuffer.Enabled = False
            ComboBoxBlockAddr.Enabled = False
            ComboBoxBlockNum.Enabled = False
            ButtonReadBlock.Enabled = False
            ButtonWriteBlock.Enabled = False
            ButtonLockBlock.Enabled = False
            TextBoxAFI.Enabled = False
            'ButtonReadAFI.Enabled = False
            ButtonWriteAFI.Enabled = False
            ButtonLockAFI.Enabled = False
            TextBoxDSFID.Enabled = False
            'ButtonReadDSFID.Enabled = False
            ButtonWriteDSFID.Enabled = False
            ButtonLockDSFID.Enabled = False
            ButtonEnableEAS.Enabled = False
            ButtonDisableEAS.Enabled = False
            ButtonCheckEAS.Enabled = False
            ButtonLockEAS.Enabled = False

            ButtonConnectM24LR04E.Enabled = True
            ButtonDisconnM24LR04E.Enabled = False

            ButtonPresentSectorPwd.Enabled = False
            ButtonReadBlocksM24LR04E.Enabled = False
            ButtonBlockSecurityStaM24LR04E.Enabled = False
            ButtonWriteAFIM24LR04E.Enabled = False
            ButtonWriteBlocksM24LR04E.Enabled = False

            ButtonWriteDSFIDM24LR04E.Enabled = False
            ButtonWritePWDM24LR04E.Enabled = False
            ButtonLockSector.Enabled = False







        Else
            MessageBox.Show("fail")
        End If
    End Sub

    Private Sub ButtonClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonClose.Click
        If hreader <> 0 Then
            If Not (InvenThread Is Nothing) Then
                MessageBox.Show("Please stop the inventory thread first.")
                Return
            End If
            If htag <> UIntPtr.Zero Then
                MessageBox.Show("Please disconnect the tag first.")
                Return
            End If
            RFIDLIB.rfidlib_reader.RDR_Close(hreader)
            hreader = UIntPtr.Zero
            ComboBoxReader.Enabled = True
            ComboBoxComm.Enabled = True
            ButtonOpen.Enabled = True
            ButtonClose.Enabled = False
            ComboBoxCom.Enabled = True
            ComboBoxBaud.Enabled = True
            ComboBoxFrame.Enabled = True
            TextBoxIpAddress.Enabled = True
            TextBoxPort.Enabled = True
            TabControlFun.Enabled = False
            ButtonStart.Enabled = False
            ButtonStop.Enabled = True
            ButtonGetReaderInfo.Enabled = False
        End If
    End Sub

    Private Sub ButtonStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonStop.Click
        ButtonStart.Enabled = False
        bInventoryThrdRun = False
        RFIDLIB.rfidlib_reader.RDR_SetCommuImmeTimeout(hreader)
    End Sub

    Private Sub ButtonGetReaderInfo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonGetReaderInfo.Click
        Dim info As New StringBuilder()
        Dim nSize As UInt32
        Dim iret As Integer
        info.Append(ControlChars.NullChar, 64)
        nSize = CType(info.Capacity, UInt32)
        iret = RFIDLIB.rfidlib_reader.RDR_GetReaderInfor(hreader, 0, info, nSize)
        If iret = 0 Then
            MessageBox.Show(info.ToString())
        End If

    End Sub
    Public Shared Function StringToByteArrayFastest(ByVal hex As String) As Byte()
        If hex.Length Mod 2 = 1 Then
            Throw New Exception("The binary key cannot have an odd number of digits")
        End If

        Dim len As Integer = hex.Length >> 1
        Dim arr As Byte() = New Byte(len - 1) {}

        For i As Integer = 0 To len - 1
            arr(i) = CByte((GetHexVal(hex(i << 1)) << 4) + (GetHexVal(hex((i << 1) + 1))))
        Next

        Return arr
    End Function

    Public Shared Function GetHexVal(ByVal hex As Char) As Integer
        Dim data As Integer = Val(hex.ToString)
        Dim str As String = hex.ToString

        If str = "0" Then
            data = 0
        ElseIf str = "1" Then
            data = 1
        ElseIf str = "2" Then
            data = 2
        ElseIf str = "3" Then
            data = 3
        ElseIf str = "4" Then
            data = 4
        ElseIf str = "5" Then
            data = 5
        ElseIf str = "6" Then
            data = 6
        ElseIf str = "7" Then
            data = 7
        ElseIf str = "8" Then
            data = 8
        ElseIf str = "9" Then
            data = 9
        ElseIf str = "a" Or str = "A" Then
            data = 10
        ElseIf str = "b" Or str = "B" Then
            data = 11
        ElseIf str = "c" Or str = "C" Then
            data = 12
        ElseIf str = "d" Or str = "D" Then
            data = 13
        ElseIf str = "e" Or str = "E" Then
            data = 14
        ElseIf str = "f" Or str = "F" Then
            data = 15
        Else
            data = -1
        End If
        Return data
    End Function
    Private Sub ButtonConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonConnect.Click
        If Not (InvenThread Is Nothing) Then
            MessageBox.Show("Please stop inventory thread first.")
            Return
        End If
        Dim iret As Integer
        Dim suid As String
        suid = ComboBoxUIDs.Text
        If suid = "" Then
            MessageBox.Show("Please inventory first.")
            Return
        End If
        Dim uid As Byte() = StringToByteArrayFastest(suid)
        Dim tagType As UInt32 = RFIDLIB.rfidlib_def.RFID_ISO15693_PICC_ICODE_SLI_ID
        If ComboBoxUIDs.SelectedIndex <> -1 Then
            tagType = TryCast(ComboBoxUIDs.SelectedItem, tagInfo).m_tagType
        End If
        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_Connect(hreader, tagType, 1, uid, htag)
        If iret = 0 Then
            ComboBoxUIDs.Enabled = False
            ButtonConnect.Enabled = False
            ButtonDisconnect.Enabled = True
            ButtonGetTagInfor.Enabled = True
            TextBoxBlockBuffer.Enabled = True
            ComboBoxBlockAddr.Enabled = True
            ComboBoxBlockNum.Enabled = True
            ButtonReadBlock.Enabled = True
            ButtonWriteBlock.Enabled = True
            ButtonLockBlock.Enabled = True
            TextBoxAFI.Enabled = True
            'ButtonReadAFI.Enabled = True
            ButtonWriteAFI.Enabled = True
            ButtonLockAFI.Enabled = True
            TextBoxDSFID.Enabled = True
            'ButtonReadDSFID.Enabled = True
            ButtonWriteDSFID.Enabled = True
            ButtonLockDSFID.Enabled = True
            ButtonEnableEAS.Enabled = True
            ButtonDisableEAS.Enabled = True
            ButtonCheckEAS.Enabled = True
            ButtonLockEAS.Enabled = True
        End If
    End Sub

    Private Sub ButtonDisconnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonDisconnect.Click
        If htag <> UIntPtr.Zero Then
            Dim iret As Integer
            iret = RFIDLIB.rfidlib_reader.RDR_TagDisconnect(hreader, htag)
            ComboBoxUIDs.Enabled = True
            ButtonConnect.Enabled = True
            ButtonDisconnect.Enabled = False
            ButtonGetTagInfor.Enabled = False
            TextBoxBlockBuffer.Enabled = False
            ComboBoxBlockAddr.Enabled = False
            ComboBoxBlockNum.Enabled = False
            ButtonReadBlock.Enabled = False
            ButtonWriteBlock.Enabled = False
            ButtonLockBlock.Enabled = False
            TextBoxAFI.Enabled = False
            'ButtonReadAFI.Enabled = False
            ButtonWriteAFI.Enabled = False
            ButtonLockAFI.Enabled = False
            TextBoxDSFID.Enabled = False
            'ButtonReadDSFID.Enabled = False
            ButtonWriteDSFID.Enabled = False
            ButtonLockDSFID.Enabled = False
            ButtonEnableEAS.Enabled = False
            ButtonDisableEAS.Enabled = False
            ButtonCheckEAS.Enabled = False
            ButtonLockEAS.Enabled = False
        End If
        htag = UIntPtr.Zero

    End Sub

    Private Sub ButtonGetTagInfor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonGetTagInfor.Click
        Dim iret As Integer
        Dim uid As Byte() = New Byte(8) {}
        Dim dsfid As Byte = 0
        Dim afi As Byte = 0
        Dim blkSize As UInt32 = 0
        Dim numOfBloks As UInt32 = 0
        Dim icRef As Byte = 0

        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_GetSystemInfo(hreader, htag, uid, dsfid, afi, blkSize, numOfBloks, icRef)
        If iret = 0 Then
            Dim info As String
            info = String.Format("DSFID:&H{0:X2};AFI:&H{1:X2};Block Size:{2};Block number:{3}", dsfid, afi, blkSize, numOfBloks)
            MessageBox.Show(info)
        Else
            MessageBox.Show("Failed")
        End If
    End Sub

    Private Sub ButtonReadBlock_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonReadBlock.Click
        Dim iret As Integer
        Dim idx As Integer
        Dim blockAddr As UInt32
        Dim blockToRead As UInt32
        Dim blocksRead As UInt32 = 0
        idx = ComboBoxBlockAddr.SelectedIndex
        If idx < 0 Then
            MessageBox.Show("please select block address")
            Return
        End If
        blockAddr = CType(idx, UInt32)
        idx = ComboBoxBlockNum.SelectedIndex
        If idx < 0 Then
            MessageBox.Show("please select number of blocks")
            Return
        End If
        blockToRead = CType(idx + 1, UInt32)
        Dim nSize As UInt32
        Dim BlockBuffer As [Byte]() = New [Byte](blockToRead * 4) {}

        nSize = CType(BlockBuffer.GetLength(0), UInt32)
        Dim bytesRead As UInt32 = 0
        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_ReadMultiBlocks(hreader, htag, 0, blockAddr, blockToRead, blocksRead, BlockBuffer, nSize, bytesRead)
        If iret = 0 Then
            TextBoxBlockBuffer.Text = BitConverter.ToString(BlockBuffer, 0, CInt(bytesRead)).Replace("-", String.Empty)
            MessageBox.Show("ok")
        Else
            MessageBox.Show("fail")
        End If
    End Sub

    Private Sub ButtonWriteBlock_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonWriteBlock.Click
        Dim iret As Integer
        Dim idx As Integer
        Dim blkAddr As UInt32
        Dim numOfBlks As UInt32
        idx = ComboBoxBlockAddr.SelectedIndex
        If idx < 0 Then
            MessageBox.Show("please select block address")
            Return
        End If
        blkAddr = CType(idx, UInt32)
        idx = ComboBoxBlockNum.SelectedIndex
        If idx < 0 Then
            MessageBox.Show("please select number of blocks")
            Return
        End If
        numOfBlks = CType(idx + 1, UInt32)
        Dim newBlksData As Byte() = StringToByteArrayFastest(TextBoxBlockBuffer.Text)
        If (newBlksData Is Nothing) Or newBlksData.Length = 0 Then
            MessageBox.Show("The buffer can not be null.")
            Return
        End If

        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_WriteMultipleBlocks(hreader, htag, blkAddr, numOfBlks, newBlksData, CUInt(newBlksData.Length))
        If iret = 0 Then
            MessageBox.Show("ok.")
        Else
            MessageBox.Show("failed!err = " + iret)
        End If
    End Sub

    Private Sub ButtonLockBlock_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonLockBlock.Click
        Dim iret As Integer
        Dim idx As Integer
        Dim blkAddr As UInt32
        Dim numOfBlks As UInt32
        idx = ComboBoxBlockAddr.SelectedIndex
        If idx < 0 Then
            MessageBox.Show("please select block address")
            Return
        End If
        blkAddr = CType(idx, UInt32)
        idx = ComboBoxBlockNum.SelectedIndex
        If idx < 0 Then
            MessageBox.Show("please select number of blocks")
            Return
        End If
        numOfBlks = CType(idx + 1, UInt32)

        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_LockMultipleBlocks(hreader, htag, blkAddr, numOfBlks)
        If iret = 0 Then
            MessageBox.Show("ok")
        Else
            MessageBox.Show("fail")
        End If
    End Sub

    Private Sub ButtonWriteAFI_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonWriteAFI.Click
        Dim iret As Integer
        Dim afi As Byte()
        afi = StringToByteArrayFastest(TextBoxAFI.Text)
        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_WriteAFI(hreader, htag, afi(0))
        If iret = 0 Then
            MessageBox.Show("ok")
        Else
            MessageBox.Show("fail")
        End If

    End Sub

    Private Sub ButtonLockAFI_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonLockAFI.Click
        Dim iret As Integer
        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_LockAFI(hreader, htag)
        If iret = 0 Then
            MessageBox.Show("ok")
        Else
            MessageBox.Show("fail")
        End If
    End Sub

    Private Sub ButtonWriteDSFID_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonWriteDSFID.Click
        Dim iret As Integer
        Dim dsfid As Byte()
        dsfid = StringToByteArrayFastest(TextBoxDSFID.Text)
        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_WriteDSFID(hreader, htag, dsfid(0))
        If iret = 0 Then
            MessageBox.Show("ok")
        Else
            MessageBox.Show("fail")
        End If
    End Sub

    Private Sub ButtonLockDSFID_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonLockDSFID.Click
        Dim iret As Integer
        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_LockDSFID(hreader, htag)
        If iret = 0 Then
            MessageBox.Show("ok")
        Else
            MessageBox.Show("fail")
        End If
    End Sub

    Private Sub ButtonEnableEAS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonEnableEAS.Click
        Dim iret As Integer
        iret = RFIDLIB.rfidlib_aip_iso15693.NXPICODESLI_EableEAS(hreader, htag)
        If iret = 0 Then
            MessageBox.Show("ok")
        Else
            MessageBox.Show("fail")
        End If
    End Sub

    Private Sub ButtonDisableEAS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonDisableEAS.Click
        Dim iret As Integer
        iret = RFIDLIB.rfidlib_aip_iso15693.NXPICODESLI_DisableEAS(hreader, htag)
        If iret = 0 Then
            MessageBox.Show("ok")
        Else
            MessageBox.Show("fail")
        End If
    End Sub

    Private Sub ButtonCheckEAS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonCheckEAS.Click
        Dim iret As Integer
        Dim EASStatus As Byte
        iret = RFIDLIB.rfidlib_aip_iso15693.NXPICODESLI_EASCheck(hreader, htag, EASStatus)
        If iret = 0 Then
            If EASStatus = 0 Then
                MessageBox.Show("EAS is disenabled.")
            Else
                MessageBox.Show("EAS is enabled.")
            End If
        Else
            MessageBox.Show("fail")
        End If
    End Sub

    Private Sub ButtonLockEAS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonLockEAS.Click
        Dim iret As Integer
        iret = RFIDLIB.rfidlib_aip_iso15693.NXPICODESLI_LockEAS(hreader, htag)
        If iret = 0 Then
            MessageBox.Show("ok")
        Else
            MessageBox.Show("fail")
        End If
    End Sub

    Private Sub ButtonReadBlocksM24LR04E_Click(sender As Object, e As EventArgs) Handles ButtonReadBlocksM24LR04E.Click
        Dim iret As Integer
        Dim idx As Integer
        Dim blockAddr As UInt32 = 0
        Dim blockToRead As UInt32 = 0
        Dim blocksRead As UInt32 = 0

        idx = CbxStartAddressM24LR04E.SelectedIndex

        If idx < 0 Then
            MessageBox.Show("please select block address")

            Return
        End If

        blockAddr = idx

        idx = CbxBlockNumM24LR04E.SelectedIndex

        If (idx < 0) Then
            MessageBox.Show("please select number of blocks")
            Return
        End If

        blockToRead = idx + 1



        Dim BlockBuffer As Byte() = New Byte(40) {}
        Dim nSize As UInt32 = BlockBuffer.Length
        Dim numOfBlksRead As UInteger = 0
        Dim bytesBlkDatRead As UInteger = 0


        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_ReadMultiBlocks(hreader, htag, 0, blockAddr, blockToRead, numOfBlksRead, BlockBuffer, nSize, bytesBlkDatRead)

        If iret = 0 Then

            TbxBlockData.Text = BitConverter.ToString(BlockBuffer, 0, CInt(bytesBlkDatRead)).Replace("-", String.Empty)
            MessageBox.Show("ok")


        Else
            MessageBox.Show("Fail")
        End If


    End Sub

    Private Sub ButtonBlockSecurityStaM24LR04E_Click(sender As Object, e As EventArgs) Handles ButtonBlockSecurityStaM24LR04E.Click

        Dim iret As Integer
        Dim idx As Integer
        Dim blockAddr As UInt32 = 0
        Dim blockToRead As UInt32 = 0
        Dim blocksRead As UInt32 = 0

        idx = CbxStartAddressM24LR04E.SelectedIndex

        If idx < 0 Then
            MessageBox.Show("please select block address")
        End If

        blockAddr = idx

        idx = CbxBlockNumM24LR04E.SelectedIndex

        If (idx < 0) Then
            MessageBox.Show("please select number of blocks")
            Return
        End If

        blockToRead = idx + 1

        Dim nSize As UInt32 = 0

        Dim BlockBuffer As Byte() = New Byte(40) {}

        Dim bytesSecRead As UInteger = 0


        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_GetBlockSecStatus(hreader, htag, blockAddr, blockToRead, BlockBuffer, nSize, bytesSecRead)

        If iret = 0 Then
            TbxBlockData.Text = BitConverter.ToString(BlockBuffer, 0, CInt(bytesSecRead)).Replace("-", String.Empty)
            MessageBox.Show("ok")

        Else
            MessageBox.Show("Fail")
        End If


    End Sub

    Private Sub ButtonConnectM24LR04E_Click(sender As Object, e As EventArgs) Handles ButtonConnectM24LR04E.Click
        If Not (InvenThread Is Nothing) Then
            MessageBox.Show("Please stop inventory thread first.")
            Return
        End If
        Dim iret As Integer
        Dim suid As String
        suid = CbxM24LR04EUIDs.Text
        If suid = "" Then
            MessageBox.Show("Please inventory first.")
            Return
        End If
        Dim uid As Byte() = StringToByteArrayFastest(suid)
        Dim tagType As UInt32 = RFIDLIB.rfidlib_def.RFID_ISO15693_PICC_ST_M24LR04E_ID


        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_Connect(hreader, tagType, CbxAddressModeM24LR04E.SelectedIndex, uid, htag)
        If iret = 0 Then
            ButtonConnectM24LR04E.Enabled = False
            ButtonDisconnM24LR04E.Enabled = True
            ButtonPresentSectorPwd.Enabled = True
            ButtonReadBlocksM24LR04E.Enabled = True
            ButtonBlockSecurityStaM24LR04E.Enabled = True
            ButtonWriteAFIM24LR04E.Enabled = True
            ButtonWriteBlocksM24LR04E.Enabled = True
            ButtonWriteDSFIDM24LR04E.Enabled = True
            ButtonWritePWDM24LR04E.Enabled = True
            ButtonLockSector.Enabled = True
            CbxM24LR04EUIDs.Enabled = False

            CbxPwdNum1.SelectedIndex = 0

            BtnSetLedOff.Enabled = True
            BtnSetLedOn.Enabled = True
        End If
    End Sub

    Private Sub ButtonDisconnM24LR04E_Click(sender As Object, e As EventArgs) Handles ButtonDisconnM24LR04E.Click
        If htag <> UIntPtr.Zero Then
            Dim iret As Integer
            iret = RFIDLIB.rfidlib_reader.RDR_TagDisconnect(hreader, htag)
            ButtonConnectM24LR04E.Enabled = True
            ButtonDisconnM24LR04E.Enabled = False

            ButtonPresentSectorPwd.Enabled = False
            ButtonReadBlocksM24LR04E.Enabled = False
            ButtonBlockSecurityStaM24LR04E.Enabled = False
            ButtonWriteAFIM24LR04E.Enabled = False
            ButtonWriteBlocksM24LR04E.Enabled = False

            ButtonWriteDSFIDM24LR04E.Enabled = False
            ButtonWritePWDM24LR04E.Enabled = False
            ButtonLockSector.Enabled = False
            CbxM24LR04EUIDs.Enabled = True


            BtnSetLedOff.Enabled = False
            BtnSetLedOn.Enabled = False
        End If
        htag = UIntPtr.Zero
    End Sub

    Private Sub ButtonWriteAFIM24LR04E_Click(sender As Object, e As EventArgs) Handles ButtonWriteAFIM24LR04E.Click
        Dim safi As String
        safi = TbxAFI.Text
        If safi = "" Then
            MessageBox.Show("Please input afi.")
            Return
        End If
        Dim afi As Byte() = StringToByteArrayFastest(safi)

        Dim iret As Integer = RFIDLIB.rfidlib_aip_iso15693.ISO15693_WriteAFI(hreader, htag, afi(0))

        If iret <> 0 Then
            MessageBox.Show("Fail")
        Else
            MessageBox.Show("ok")
        End If


    End Sub

    Private Sub ButtonWriteDSFIDM24LR04E_Click(sender As Object, e As EventArgs) Handles ButtonWriteDSFIDM24LR04E.Click
        Dim iret As Integer
        Dim dsfid As Byte()
        dsfid = StringToByteArrayFastest(TbxDSFID.Text)
        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_WriteDSFID(hreader, htag, dsfid(0))
        If iret = 0 Then
            MessageBox.Show("ok")
        Else
            MessageBox.Show("fail")
        End If
    End Sub

    Private Sub ButtonWritePWDM24LR04E_Click(sender As Object, e As EventArgs) Handles ButtonWritePWDM24LR04E.Click
        Dim pwdnum As Byte
        Dim iret As Integer
        Dim newpwd As UInt32

        pwdnum = CbxPwdNum2.SelectedIndex

        If pwdnum < 0 Then
            MessageBox.Show("please select password number")
            Return
        End If

        pwdnum = pwdnum + 1
        If TbxPwd2.Text.Length <> 8 Then
            MessageBox.Show("Invalid password format")
            Return
        End If

        Dim pwds As Byte() = StringToByteArrayFastest(TbxPwd2.Text)

        newpwd = pwds(0) Or pwds(1) << 8 Or pwds(1) << 16 Or pwds(2) << 24

        iret = RFIDLIB.rfidlib_aip_iso15693.STM24LR_WriteSectorPassword(hreader, htag, pwdnum, newpwd)

        If iret <> 0 Then
            MessageBox.Show("Fail")
        Else
            MessageBox.Show("ok")
        End If

    End Sub

    Private Sub ButtonLockSector_Click(sender As Object, e As EventArgs) Handles ButtonLockSector.Click

        Dim pwdnum As Byte
        Dim iret As Integer
        Dim Sector As Integer
        Dim SectorNum As Integer
        pwdnum = CbxSectorPwdNum.SelectedIndex

        If pwdnum < 0 Then
            MessageBox.Show("please select password number")
            Return
        End If
        pwdnum = pwdnum + 1
        SectorNum = CbxSectorNum.SelectedIndex

        If SectorNum < 0 Then
            MessageBox.Show("please select sector number")
            Return
        End If


        Dim access As Byte = CbxAccess.SelectedIndex


        If access < 0 Then
            MessageBox.Show("please select access")
            Return
        End If

        iret = RFIDLIB.rfidlib_aip_iso15693.STM24LR_LockSector(hreader, htag, CByte(SectorNum), CByte(access), CByte(pwdnum))

        If iret <> 0 Then
            MessageBox.Show("Fail")
        Else
            MessageBox.Show("ok")
        End If

    End Sub

    Private Sub ButtonPresentSectorPwd_Click(sender As Object, e As EventArgs) Handles ButtonPresentSectorPwd.Click
        Dim pwdnum As Byte
        Dim iret As Integer
        Dim pwd As UInt32

        pwdnum = CbxPwdNum1.SelectedIndex

        If pwdnum < 0 Then
            MessageBox.Show("please select password number")
            Return
        End If

        pwdnum = pwdnum + 1
        If TbxPWD1.Text.Length <> 8 Then
            MessageBox.Show("Invalid password format")
            Return
        End If

        Dim pwds As Byte() = StringToByteArrayFastest(TbxPWD1.Text)

        pwd = pwds(0) Or pwds(1) << 8 Or pwds(1) << 16 Or pwds(2) << 24

        iret = RFIDLIB.rfidlib_aip_iso15693.STM24LR_PresentSectorPassword(hreader, htag, pwdnum, pwd)

        If iret = 0 Then
            MessageBox.Show("ok")
        Else
            MessageBox.Show("fail")
        End If



    End Sub

    Private Sub ButtonWriteBlocksM24LR04E_Click(sender As Object, e As EventArgs) Handles ButtonWriteBlocksM24LR04E.Click
        Dim iret As Integer
        Dim idx As Integer
        Dim blockAddr As UInt32 = 0
        Dim numOfBlks As UInt32 = 0
        Dim blocksRead As UInt32 = 0

        idx = CbxStartAddressM24LR04E.SelectedIndex

        If idx < 0 Then
            MessageBox.Show("please select block address")
        End If

        blockAddr = idx

        idx = CbxBlockNumM24LR04E.SelectedIndex

        If (idx < 0) Then
            MessageBox.Show("please select number of blocks")
            Return
        End If

        numOfBlks = idx + 1

        Dim BlockBuffer As Byte() = StringToByteArrayFastest(TbxBlockData.Text)

        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_WriteMultipleBlocks(hreader, htag, blockAddr, numOfBlks, BlockBuffer, CUInt(BlockBuffer.Length))
        If iret = 0 Then
            MessageBox.Show("ok.")
        Else
            MessageBox.Show("failed!err = " + iret)
        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles BtnSetLedOn.Click



        Dim uid As Byte() = StringToByteArrayFastest(CbxM24LR04EUIDs.SelectedItem.ToString())
        Array.Reverse(uid)

        Dim cmd As Byte() = New Byte(4 + uid.Length - 1) {}

        cmd(0) = &H23
        cmd(1) = &HA2
        cmd(2) = &H2
        System.Array.Copy(uid, 0, cmd, 3, uid.Length)
        cmd(cmd.Length - 1) = 1
        Dim reqData As Byte() = New Byte(4) {}
        Dim nSize As UInteger = reqData.Length
        Dim iret As Integer = RFIDLIB.rfidlib_aip_iso15693.ISO15693_TransparentTransceive(hreader, 0, 1, 1, cmd, CUInt(cmd.Length), reqData.Length, reqData, nSize, 0)
        If iret = 0 Then
            MessageBox.Show("ok.")
        Else
            MessageBox.Show("failed!err = " + iret)
        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles BtnSetLedOff.Click

        Dim uid As Byte() = StringToByteArrayFastest(CbxM24LR04EUIDs.SelectedItem.ToString())
        Array.Reverse(uid)

        Dim cmd As Byte() = New Byte(4 + uid.Length - 1) {}

        cmd(0) = &H23
        cmd(1) = &HA2
        cmd(2) = &H2
        System.Array.Copy(uid, 0, cmd, 3, uid.Length)
        cmd(cmd.Length - 1) = 0
        Dim reqData As Byte() = New Byte(4) {}
        Dim nSize As UInteger = 0
        Dim iret As Integer = RFIDLIB.rfidlib_aip_iso15693.ISO15693_TransparentTransceive(hreader, 0, 1, 1, cmd, cmd.Length, reqData.Length, reqData, nSize, 0)

        If iret = 0 Then
            MessageBox.Show("ok.")
        Else
            MessageBox.Show("failed!err = " + iret)
        End If

    End Sub
End Class


Public Class tagInfo
    Public m_uid As String
    Public m_tagType As UInt32
    Public Sub New(ByVal uid As String, ByVal tagType As UInt32)
        m_uid = uid
        m_tagType = tagType
    End Sub
    Public Overrides Function ToString() As String
        Return m_uid.ToString()
    End Function

End Class

Public Class CSupportedAirProtocol
    Public m_ID As UInt32
    Public m_name As String
    Public m_en As [Boolean]
End Class
