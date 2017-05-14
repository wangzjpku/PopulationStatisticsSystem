Public Class Form2
    ' Declarations
    Const NUM_OF_SENSORS As Byte = 8
    Const LANES As Byte = 3
    Const ROWS As Byte = 2
    Const CHECKPOINTS As Byte = 6
    Const KEEPPERIOD As Integer = 50
    Const CALIBRATION_MODE As Byte = 1
    Const COUNTING_MODE As Byte = 2
    Const CALIBRATION_CYCLES As Byte = 60
    Const BARHEIGHT As Integer = 200
    Const KEEPHEIGHTLIFECNT = 20
    Const PIR_FILTER_LENGTH As Integer = 3

    Const NUM_OF_PIR_SENSORS As Integer = 8

    Const FLAG_ZERO As Byte = 0
    Const ROSE As Byte = 1
    Const FALLED As Byte = 2
    Const PASSED As Byte = 3

    Const CHECKDURATION As Byte = 12

    Const ULTRASONIC_235 As Byte = 1
    Const ULTRASONIC_40 As Byte = 2

    Public q(NUM_OF_SENSORS * 3) As Double
    Shared dTmean(NUM_OF_SENSORS - 1) As Double

    Shared iii As Integer
    Shared ii As Integer
    Shared iiold As Integer
    Shared dT_pos(LANES - 1, ROWS - 1) As Double
    Shared dT_neg(LANES - 1, ROWS - 1) As Double
    Shared iLastDetectionTime As Integer

    Shared dThresh1 As Double
    Shared dThresh2 As Double

    Shared dT(CHECKPOINTS - 1) As Double
    Shared dTold(CHECKPOINTS - 1) As Double
    Shared dTmax(LANES - 1, ROWS - 1) As Double
    Shared iTmax_time(LANES - 1, ROWS - 1) As Integer

    Shared bFlag1(LANES - 1, ROWS - 1) As Byte
    Shared bState As Byte
    Shared bLastState As Byte
    Shared bLane_State(LANES - 1, KEEPPERIOD - 1) As Byte
    Shared iDuration(LANES - 1, KEEPPERIOD - 1) As Integer
    Shared bLane_Tmax(LANES - 1, KEEPPERIOD - 1) As Byte

    Shared bMode As Byte
    Shared bCalibrationCnt As Byte

    ''' Double Type or Integer?
    Shared iCalSum(NUM_OF_SENSORS - 1) As Integer

    Public Shared iPeopleCnt_In As Integer
    Public Shared iPeopleCnt_Out As Integer
    Public Shared iFilterdPIRMax As Integer

    Shared iHighDuration(LANES - 1, ROWS - 1) As Integer
    Public Shared iKeepHeight As Integer
    Shared iKeepHeightLife As Integer

    Shared dQbuf(NUM_OF_SENSORS, PIR_FILTER_LENGTH - 1) As Double
    Shared bQbufIndex(NUM_OF_PIR_SENSORS - 1) As Byte

    Shared bCounterResetFlag As Byte
    Shared iPassCount As Integer

    Private Sub Form2_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ' Initialise
        '
        'iPeopleCnt = ExeCountProcess()  ' TEST 
    End Sub

    Public Sub CountProcessInit()
        For Sensor As Byte = 0 To NUM_OF_SENSORS - 1 Step 1
            iCalSum(Sensor) = 0

            ' init PIR filter buffer
            For fi As Byte = 0 To PIR_FILTER_LENGTH - 1
                dQbuf(Sensor, fi) = 0
            Next
        Next

        ' init PIR filter index buffer
        For fi As Byte = 0 To NUM_OF_PIR_SENSORS - 1
            bQbufIndex(fi) = 0
        Next

        iii = 0
        ii = 0
        'iiold = KEEPPERIOD - 20
        iiold = CHECKDURATION

        'dThresh1 = 22
        'dThresh2 = 21  ' must be bigger than 20, and less than thresh 1 
        dThresh1 = Form1.TextBox_Thresh1.Text
        dThresh2 = Form1.TextBox_Thresh2.Text
        If dThresh2 >= dThresh1 Then
            MsgBox("Thresh Value Error!")
        End If

        bState = 0
        bLastState = 0

        ' iT(CHECKPOINTS - 1) 
        ' iTmax(CHECKPOINTS - 1) 
        ' iTmax_time(CHECKPOINTS - 1)


        For lane As Byte = 0 To LANES - 1
            bFlag1(lane, 0) = 0
            bFlag1(lane, 1) = 0
            iHighDuration(lane, 0) = 0
            iHighDuration(lane, 1) = 0
            For time As Byte = 0 To KEEPPERIOD - 1
                bLane_State(lane, time) = 0
                iDuration(lane, time) = 0
                bLane_Tmax(lane, time) = 0
            Next
        Next

        bMode = CALIBRATION_MODE
        bCalibrationCnt = 0

        iKeepHeight = 0
        iKeepHeightLife = 0

        iPeopleCnt_In = 0
        iPeopleCnt_Out = 0

        iFilterdPIRMax = 0
        bCounterResetFlag = 0
    End Sub

    ' call side sample:
    ' Dim People(1) As Integer
    ' People =  Form2.ExeCountProcess()
    '
    Public Function ExeCountProcess(ByVal RxData() As Byte, ByVal count As Integer) As Integer()
        ' called by COM port Rx event 

        Dim iValue As Integer
        'Dim bValue As Byte
        Dim PIRmax As Integer = 0
        Dim HeightMax As Integer = 0
        Dim iPeopleCnt(3) As Integer

        If RxData(0) = 0 AndAlso RxData(1) = 0 Then
            ' Illegal, just Skip
            iPeopleCnt(0) = 0  ' for DEBUG break 
        Else
            ' Data store and management
            For Sensor As Byte = 0 To NUM_OF_SENSORS * 3 - 1
                'iValue = Form1.data(Sensor * 2) + Form1.data((Sensor * 2) + 1) * 256
                iValue = RxData(Sensor * 2) + RxData((Sensor * 2) + 1) * 256
                If Sensor < NUM_OF_SENSORS Then
                    ' PIR
                    q(Sensor) = 80 * (5 * iValue / 1024 - 1) - 18
                    ' keep MAX
                    'If q(Sensor) - dTmean(Sensor) + 20 > PIRmax Then
                    'PIRmax = q(Sensor)
                    'End If
                ElseIf Sensor < NUM_OF_SENSORS * 2 Then
                    ' EZ4 40kHz range reading
                    q(Sensor) = 2.54 * iValue / 2
                    If q(Sensor) > 200 Then
                        q(Sensor) = 200
                    End If
                Else
                    ' SRF 235kHz range reading
                    If iValue = 0 Then
                        q(Sensor) = 120
                    Else
                        q(Sensor) = iValue
                    End If
                    If Sensor = NUM_OF_SENSORS * 2 + 3 Then    ' Sensor No.3 malfunction so ignore 
                        q(Sensor) = 120
                    End If
                    If q(Sensor) > 120 Then
                        q(Sensor) = 120
                    End If
                    If BARHEIGHT - q(Sensor) > HeightMax Then
                        HeightMax = BARHEIGHT - q(Sensor)
                    End If
                End If
            Next

            ' Keep Max PIR

            ' Update Count by q() 
            If bMode = CALIBRATION_MODE Then
                Calibration()
            Else
                ' COUNTING MODE
                Counting(count)
            End If
        End If ' RxData

        iPeopleCnt(0) = iPeopleCnt_In
        iPeopleCnt(1) = iPeopleCnt_Out
        'iPeopleCnt(2) = PIRmax
        iPeopleCnt(2) = iFilterdPIRMax
        'iPeopleCnt(3) = HeightMax
        iPeopleCnt(3) = iKeepHeight    ' kept highest value at PIR rising edge (Life time = KEEPHEIGHTLIFECNT)
        'Form1.Label2.Text = "IN:" & Str(iPeopleCnt(0))
        'Form1.Label3.Text = "OUT:" & Str(iPeopleCnt(1))
        Return iPeopleCnt
    End Function

    Private Sub Calibration()
        '  UI notification of Calibration started 
        'Form1.Label1.Text = "Calibrating"
        Form1.Calibration_Started()

        For Sensor As Byte = 0 To NUM_OF_SENSORS - 1 Step 1
            ' iCalSum is Integer, q is Double, sum will auto rounding
            ' here lead to lost accuracy, but through test, Integer better than Double
            iCalSum(Sensor) = iCalSum(Sensor) + q(Sensor)
        Next

        If bCalibrationCnt = CALIBRATION_CYCLES - 1 Then
            Dim meansStr As String = ""

            For Sensor As Byte = 0 To NUM_OF_SENSORS - 1 Step 1
                dTmean(Sensor) = iCalSum(Sensor) / CALIBRATION_CYCLES
                meansStr = meansStr & dTmean(Sensor)
                If Sensor <> NUM_OF_SENSORS - 1 Then
                    meansStr = meansStr & ","
                End If
            Next
            Form1.m_Log.WriteLine(DateTime.Now.ToString() & ">" & "Calibration," & meansStr)

            ' CHECK
            For Sensor As Byte = 0 To NUM_OF_SENSORS - 1 Step 1
                If dTmean(Sensor) < 0 OrElse dTmean(Sensor) > 35 Then
                    MsgBox("Calibration ERROR, Please exit SW and reset HW")
                End If
            Next

            bMode = COUNTING_MODE
            'MsgBox("Calibration Finished")
            'Form1.Label1.Text = "Counting"
            Form1.Calibration_Ended()
        Else
            bCalibrationCnt = bCalibrationCnt + 1
        End If

        ' UI notification of Calibration finished  How??
    End Sub

    Private Function GetState(ByVal value As Byte) As String
        If value = FLAG_ZERO Then
            Return "FLAG_ZERO"
        ElseIf value = ROSE Then
            Return "ROSE"
        ElseIf value = FALLED Then
            Return "FALLED"
        Else
            Return "PASSED"
        End If
    End Function

    Private Sub SetBFlag1(ByVal count As Integer, ByVal lane As Byte, ByVal row As Byte, ByVal value As Byte)
        Dim oldValue, newValue As String
        Dim str As String

        oldValue = GetState(bFlag1(lane, row))
        newValue = GetState(value)

        str = "sample=" & count & ",bFlag1(" & lane & "," & row & "," & (lane * 2 + row + 1) & ") from " & oldValue & "->" & newValue
        Form1.m_Log.WriteLine(DateTime.Now.ToString() & ">" & "SetBFlag," & str)
        bFlag1(lane, row) = value
    End Sub

    Private Function CalcHeight_235(ByVal count As Integer) As Integer
        ' keep the ultra sonic data at while ROSE state
        Dim MaxUs As Integer = 0
        ' 235K
        For us As Byte = NUM_OF_SENSORS * 2 To NUM_OF_SENSORS * 3 - 1
            If BARHEIGHT - q(us) > MaxUs Then
                MaxUs = BARHEIGHT - q(us)
            End If
        Next
        If MaxUs = BARHEIGHT - 120 Then
            ' UltraSonic does not detect anything 
            MaxUs = 0
        End If
        'iKeepHeight = MaxUs  ' Max Value at this cycle 
        If MaxUs > iKeepHeight Then
            ' Update Max Height and renew counter
            iKeepHeight = MaxUs
            iKeepHeightLife = KEEPHEIGHTLIFECNT    ' extend the life even this rise is not with Max value
        End If
        Return iKeepHeight
    End Function

    Private Function CalcHeight_40(ByVal count As Integer) As Integer
        ' keep the ultra sonic data at while ROSE state
        Dim MaxUs As Integer = 0
        ' 235K
        For us As Byte = NUM_OF_SENSORS * 1 To NUM_OF_SENSORS * 2 - 5
            If BARHEIGHT - q(us) > MaxUs Then
                MaxUs = BARHEIGHT - q(us)
            End If
        Next
        If MaxUs = BARHEIGHT - 120 Then
            ' UltraSonic does not detect anything 
            MaxUs = 0
        End If
        'iKeepHeight = MaxUs  ' Max Value at this cycle
        If MaxUs > iKeepHeight Then
            ' Update Max Height and renew counter
            iKeepHeight = MaxUs
            iKeepHeightLife = KEEPHEIGHTLIFECNT    ' extend the life even this rise is not with Max value
        End If
        Return iKeepHeight
    End Function

    Private Function CalcHeight(ByVal count As Integer, ByVal sensor As Byte) As Integer
        ' Detection by Lane 
        If iKeepHeightLife > 0 Then
            iKeepHeightLife = iKeepHeightLife - 1
            If iKeepHeightLife = 0 Then
                iKeepHeight = 0
            End If
        End If

        If sensor = ULTRASONIC_235 Then
            Return CalcHeight_235(count)
        Else
            Return CalcHeight_40(count)
        End If
    End Function

    Private Sub Counting(ByVal count As Integer)
        Dim Point As Byte
        Dim row_other As Byte
        Dim kkk As Byte
        Dim bChan(LANES - 1) As Byte

        ii = ii + 1
        iii = iii + 1

        If count >= 541 And count <= 541 Then
            count = count
        End If

        If ii >= KEEPPERIOD Then
            HistoryShift()
        End If

        ' Height
        iKeepHeight = CalcHeight(count, ULTRASONIC_40)

        ' Temperature
        iFilterdPIRMax = 0
        For Sensor As Byte = 0 To NUM_OF_SENSORS - 1 Step 1
            q(Sensor) = q(Sensor) - dTmean(Sensor) + 20    ' q(1:8) = q(1:8)- m'+20;

            ' add filtering (2014/08/26)  moving mean value
            If (bQbufIndex(Sensor) >= PIR_FILTER_LENGTH) Then
                bQbufIndex(Sensor) = 0
            End If
            dQbuf(Sensor, bQbufIndex(Sensor)) = q(Sensor)
            bQbufIndex(Sensor) = bQbufIndex(Sensor) + 1

            Dim dTempSum As Double = 0
            For fi As Byte = 0 To PIR_FILTER_LENGTH - 1
                dTempSum = dTempSum + dQbuf(Sensor, fi)
            Next
            q(Sensor) = dTempSum / PIR_FILTER_LENGTH

            ' keep MAX
            If q(Sensor) > iFilterdPIRMax Then
                iFilterdPIRMax = q(Sensor)
            End If
        Next

        ' Turn 4 lanes into 3 
        For Point = 0 To CHECKPOINTS - 1
            dTold(Point) = dT(Point)
        Next

        dT(0) = GetMax2(q(0), q(2))
        dT(1) = GetMax2(q(1), q(3))

        dT(2) = GetMax2(q(2), q(4))
        dT(3) = GetMax2(q(3), q(5))

        dT(4) = GetMax2(q(4), q(6))
        dT(5) = GetMax2(q(5), q(7))

        For lane As Byte = 0 To LANES - 1
            For row As Byte = 0 To ROWS - 1
                Point = lane * 2 + row  ' 0-5

                ' Check for Thermal trigger 
                If bFlag1(lane, row) = FLAG_ZERO Then    ' in case of new detection 
                    If dT(Point) > dThresh1 Then
                        SetBFlag1(count, lane, row, ROSE)
                        dTmax(lane, row) = dT(Point)
                        iTmax_time(lane, row) = iii
                        dT_pos(lane, row) = iii    ' time stamp of rising 
                    End If
                End If

                ' check when temp falls below threshold
                If bFlag1(lane, row) > 0 Then
                    If dT(Point) > dTmax(lane, row) Then
                        dTmax(lane, row) = dT(Point)
                        iTmax_time(lane, row) = iii
                    End If

                    iHighDuration(lane, row) = iHighDuration(lane, row) + 1  ' DEBUG 

                    ' keep the ultra sonic data at while ROSE state
                    ' put outside ROSE state 
                    If dTold(Point) >= dThresh2 AndAlso dT(Point) < dThresh2 Then
                        SetBFlag1(count, lane, row, FALLED)
                        dT_neg(lane, row) = iii - 1 + (dThresh2 - dTold(Point)) / (dT(Point) - dTold(Point))  ' estimated (precise) time when drop below thresh2 
                        iHighDuration(lane, row) = 0
                    End If
                End If

                ' Get rid of detection if flag1=2 has been triggered for more than
                ' a certain amount of time
                ' think about this a bit more - 
                ' how to deal with it ?
                ' if one rises then falls, and the other hasn't risen within x
                ' amount of samples following th first ones demise, we reset to zero
                If row = 0 Then
                    row_other = 1
                ElseIf row = 1 Then
                    row_other = 0
                End If

                If bFlag1(lane, row) = FALLED AndAlso bFlag1(lane, row_other) = FLAG_ZERO Then
                    If iii - 20 > dT_neg(lane, row) Then
                        SetBFlag1(count, lane, row, FLAG_ZERO)
                    End If
                End If

                ' add T.Ito 2014/09/17  seen a bug of continued ROSE of one sensor
                If bFlag1(lane, row) = ROSE AndAlso bFlag1(lane, row_other) = FLAG_ZERO Then
                    If iii - 100 > dT_pos(lane, row) Then
                        SetBFlag1(count, lane, row, FLAG_ZERO)
                    End If
                End If

                ' add zheng 2014/11/03
                If bFlag1(lane, row) = ROSE AndAlso
                    (bFlag1(lane, row_other) = ROSE Or bFlag1(lane, row_other) = FALLED) Then
                    If iii - 100 > dT_pos(lane, row) Then
                        SetBFlag1(count, lane, row, FALLED)
                        SetBFlag1(count, lane, row_other, FALLED)

                        If (dT_pos(lane, row) < dT_pos(lane, row_other)) Then
                            dT_neg(lane, row) = iii - 1
                            dT_neg(lane, row_other) = iii
                        Else
                            dT_neg(lane, row) = iii
                            dT_neg(lane, row_other) = iii - 1
                        End If
                        iHighDuration(lane, row) = 0
                        iHighDuration(lane, row_other) = 0
                    End If
                End If
            Next 'row

            ' find when both are below thermal threshold
            If bFlag1(lane, 0) = FALLED AndAlso bFlag1(lane, 1) = FALLED Then
                If dT(Point) < dThresh2 AndAlso dT(Point - 1) < dThresh2 Then
                    SetBFlag1(count, lane, 0, PASSED)
                    SetBFlag1(count, lane, 1, PASSED)
                End If
            End If

            ' now go for a detection along the lane 
            If bFlag1(lane, 0) = PASSED AndAlso bFlag1(lane, 1) = PASSED Then
                Dim dT_Tpos, dT_Tneg As Double

                iPassCount = iPassCount + 1
                dT_Tpos = dT_pos(lane, 1) - dT_pos(lane, 0)
                dT_Tneg = dT_neg(lane, 1) - dT_neg(lane, 0)
                SetBFlag1(count, lane, 0, FLAG_ZERO)
                SetBFlag1(count, lane, 1, FLAG_ZERO)

                kkk = lane * 2 + 1
                Dim index1 As Integer
                Dim index2 As Integer
                index1 = Math.Round(GetMax2(dT_neg(lane, 0), dT_neg(lane, 1)))
                index2 = Math.Round(GetMin2(dT_pos(lane, 0), dT_pos(lane, 1)))
                ' index =  round(max([t_pos(lane,1) t_pos(lane,2) t_neg(lane,1) t_neg(lane,2)]));
                ' index2 = round(min([t_pos(lane,1) t_pos(lane,2) t_neg(lane,1) t_neg(lane,2)]));
                bLane_Tmax(lane, ii) = GetMax2(dTmax(lane, 0), dTmax(lane, 1))
                If index1 >= index2 Then
                    iDuration(lane, ii) = index1 - index2
                Else
                    MsgBox("Err, Please Stop")
                End If

                If dT_Tpos >= 0 AndAlso dT_Tneg > 0 Then ' row1 -> row2 direction 
                    bLane_State(lane, ii) = 2
                    ' IN
                ElseIf dT_Tpos <= 0 AndAlso dT_Tneg < 0 Then  ' row2 -> row1 direction
                    bLane_State(lane, ii) = 1
                    ' OUT
                Else
                    ' dunno TODO
                    'If iDuration(lane, ii) < 40 Then
                    kkk = lane * 2 + 1
                    Dim ind1, ind2 As Double
                    ind1 = iTmax_time(lane, 0)
                    ind2 = iTmax_time(lane, 1)
                    If ind1 > ind2 Then
                        bLane_State(lane, ii) = 1
                    ElseIf ind1 < ind2 Then
                        bLane_State(lane, ii) = 2
                    Else
                        bLane_State(lane, ii) = 0    ' lane_state(lane,ii) = -1 
                    End If
                    ' ABS(pos) <> ABS(neg)
                    'If Math.Abs(dT_Tpos) > Math.Abs(dT_Tneg) Then
                    '    If dT_Tpos > 0 Then
                    '        ' IN
                    '        bLane_State(lane, ii) = 2
                    '    Else ' OUT
                    '        bLane_State(lane, ii) = 1
                    '    End If
                    'ElseIf Math.Abs(dT_Tpos) < Math.Abs(dT_Tneg) Then
                    '    If dT_Tneg > 0 Then
                    '        ' IN
                    '        bLane_State(lane, ii) = 2
                    '    Else ' OUT
                    '        bLane_State(lane, ii) = 1
                    '    End If
                    'Else
                    '    bLane_State(lane, ii) = 0
                    'End If

                    'Else
                    '    ' duration is less than 1.5 seconds 
                    '    bLane_State(lane, ii) = 0    ' lane_state(lane,ii) = -1
                    'End If
                End If ' Direction 
            End If ' both PASSED
        Next  'lane

        ' Judgement
        Dim Flag2 As Byte
        Flag2 = 0

        If ii >= iiold Then
            For lane As Byte = 0 To LANES - 1
                If bLane_State(lane, ii - CHECKDURATION) > 0 Then
                    Flag2 = 1
                End If
            Next
        End If

        Dim chan(2) As Byte
        If Flag2 = 1 Then
            chan(0) = Search_LaneStateMax(0, ii - CHECKDURATION, ii)
            chan(1) = Search_LaneStateMax(1, ii - CHECKDURATION, ii)
            chan(2) = Search_LaneStateMax(2, ii - CHECKDURATION, ii)
            ' LANES
            bState = 0

            ' if only one channel is high we take it
            ' q = find(chan>0);
            Dim ActiveLaneCnt As Byte = 0
            For lane As Byte = 0 To LANES - 1
                If chan(lane) > 0 Then
                    ActiveLaneCnt = ActiveLaneCnt + 1
                End If
            Next

            If ActiveLaneCnt = 1 Then
                If chan(0) > 0 Then
                    bState = chan(0)
                ElseIf chan(1) > 0 Then
                    bState = chan(1)
                Else
                    bState = chan(2)
                End If
            End If

            If ActiveLaneCnt = 2 Then
                If chan(0) = 0 Then
                    If chan(1) = chan(2) Then
                        bState = chan(1)
                    Else
                        ' Opposit: PENDING
                        bState = 3
                    End If
                ElseIf chan(1) = 0 Then
                    If chan(0) = chan(2) Then
                        bState = chan(0)
                    Else
                        ' Opposit: PENDING
                        bState = 3
                    End If
                Else
                    If chan(0) = chan(1) Then
                        bState = chan(0)
                    Else
                        ' Opposit: PENDING
                        bState = 3
                    End If
                End If
            End If  ' ActiveLaneCnt = 2 

            Dim bTwoPeople As Boolean = False

            If ActiveLaneCnt = 3 Then
                If chan(0) = chan(1) AndAlso chan(1) = chan(2) Then
                    ' fat person going through
                    bState = chan(0)
                    'bTwoPeople = True
                    Form1.m_Log.WriteLine(DateTime.Now.ToString() & ">LaneSameDirection,sample=" & count)
                Else
                    If chan(0) = chan(2) Then
                        ' ignore 0 and 2
                        ' zheng san, 2014/11/04
                        bState = chan(1)
                    Else
                        ' two thin people opposite directions
                        bState = 3
                    End If
                End If
            End If

            'TODO'
            ' dd = max(max(duration(q,ii-w:ii)))    seems checking double count for long duration case
            Dim iMaxLaneDuration As Integer = 0
            For lane As Byte = 0 To LANES - 1
                If iDuration(lane, ii - CHECKDURATION) > iMaxLaneDuration Then
                    iMaxLaneDuration = iDuration(lane, ii - CHECKDURATION)
                    ActiveLaneCnt = lane
                End If
            Next
            If iMaxLaneDuration > 16 Then
                If iMaxLaneDuration > iii - iLastDetectionTime AndAlso bLastState = bState Then
                    bState = 0
                    ' if state is same as previous one and duration is more than last detection, ignore it 
                    Form1.m_Log.WriteLine(DateTime.Now.ToString() & ">MaxDuration,sample=" & count & ",lane=" & ActiveLaneCnt & ",duration=" & iMaxLaneDuration)
                End If
            End If

            ' Finally update the count
            If bState > 0 Then
                bLastState = bState
                iLastDetectionTime = iii
            End If

            ' COUNT
            If bState = 1 Then
                If bTwoPeople Then
                    iPeopleCnt_Out = iPeopleCnt_Out + 2
                Else
                    iPeopleCnt_Out = iPeopleCnt_Out + 1
                End If
            ElseIf bState = 2 Then
                If bTwoPeople Then
                    iPeopleCnt_In = iPeopleCnt_In + 2
                Else
                    iPeopleCnt_In = iPeopleCnt_In + 1
                End If
            ElseIf bState = 3 Then
                iPeopleCnt_Out = iPeopleCnt_Out + 1
                iPeopleCnt_In = iPeopleCnt_In + 1
            Else
                'Console.WriteLine(DateTime.Now.ToString() & ">" & "People Repeat," & "!!!!!!!!!!!!!!!!!!!!!, count=" & count)
            End If

            Flag2 = 0
            iiold = ii + CHECKDURATION + 1
            bState = 0
        End If ' Flag2=1 

        If bCounterResetFlag = 1 Then
            iPeopleCnt_In = 0
            iPeopleCnt_Out = 0
            bCounterResetFlag = 0
        End If
    End Sub

    Private Sub HistoryShift()
        ' loop counter go back to 20 
        ii = 20    ' 20 just 20 Plextek one (can be other)

        'If iiold >= (KEEPPERIOD - 20) Then
        iiold = iiold - (KEEPPERIOD - ii)
        'Else
        ' what can I do ??? 
        'End If

        For lane As Byte = 0 To LANES - 1
            For time As Byte = 0 To ii - 1
                bLane_State(lane, time) = bLane_State(lane, KEEPPERIOD - ii + time)
                iDuration(lane, time) = iDuration(lane, KEEPPERIOD - ii + time)
                bLane_Tmax(lane, time) = bLane_Tmax(lane, KEEPPERIOD - ii + time)
            Next
            For time As Byte = ii To KEEPPERIOD - 1
                bLane_State(lane, time) = 0
                iDuration(lane, time) = 0
                bLane_Tmax(lane, time) = 0
            Next
        Next
    End Sub

    Private Function GetMax2(ByVal Val1 As Double, ByVal Val2 As Double) As Double

        If Val1 >= Val2 Then
            Return Val1
        Else
            Return Val2
        End If

    End Function


    Private Function GetMin2(ByVal Val1 As Double, ByVal Val2 As Double) As Double

        If Val1 <= Val2 Then
            Return Val1
        Else
            Return Val2
        End If

    End Function


    Private Function Search_LaneStateMax(ByVal lane, ByVal t1, ByVal t2) As Byte

        Dim Max As Byte = 0

        For time As Byte = t1 To t2
            If bLane_State(lane, time) > Max Then
                Max = bLane_State(lane, time)
            End If
        Next

        Return Max

    End Function

    Public Sub SetCounterReset()
        bCounterResetFlag = 1
    End Sub

    Public Function GetMeanValue(ByVal SenNum As Byte) As Double
        Return dTmean(SenNum)
    End Function
End Class