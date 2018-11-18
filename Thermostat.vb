Imports System.Convert
Imports System.Math

''' Copyright 2018 Audun Rundberg
'''
''' This file Is part Of homeseer-thermostat.
'''
''' homeseer-thermostat Is free software: you can redistribute it And/Or modify it under the terms Of the GNU General Public License As published by the Free Software Foundation, either version 3 Of the License, Or (at your Option) any later version.
'''
''' homeseer-thermostat Is distributed In the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY Or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
'''
''' You should have received a copy Of the GNU General Public License along With homeseer-thermostat. If Not, see http://www.gnu.org/licenses/.

Public Class Thermostat
    ''' <summary>
    ''' Turn heating devices on or off based on wanted temperature and current temperature
    ''' 
    ''' Requirements:
    ''' - HomeSeer HS3
    ''' - A device that reports the temperature you want the room to have. Eg. 22 degrees Celcius.
    ''' - A thermometer (one or several) that reports the current temperature in the room. Eg. 20.5 degrees Celcius.
    ''' - A heating device (one or several) that Homeseer can turn on or off
    ''' 
    ''' How to use:
    ''' 1. Using events in Homeseer, set this script to run when the temperature in the room changes (or every 5, 10 minutes or whatever you want)
    ''' 2. Set the parameters for the script in Homeseer like this: wantedTempDeviceReference|temperatureSensorReferences|switchReferences
    ''' 3. temperatureSensorReferences and switchReferences can be a single reference ID, or a semicolon-separated string of multiple device references
    ''' 
    ''' Reference ID is a number you can find in the "Advanced" tab in Homeseer for the device, like 123 or 337
    ''' 
    ''' Parameter examples:
    ''' - Get wanted temperature from device 1 and compare with temperature from device 2 to control device 5: 1|2|5
    ''' - Get wanted temperature from device 1, and compare with the average temperature of device 2, 3 and 4, to control devices 5, 6 and 7: 1|2;3;4|5;6;7
    ''' </summary>
    ''' <param name="parms">A | and ; separated string</param>

    Public Sub Main(ByVal parms As Object)
        Dim parameters() As String
        Dim temperatureDevices() As String
        Dim heatDevices() As String
        Dim wantedTemperature As Double
        Dim currentTemperature As Double
        Dim temperatureDifference As Double
        Dim hysteresis As Double = 0.5 ' Temperature difference must be greater than the hysteresis for devices to change status

        parameters = parms.ToString.Split(Convert.ToChar("|"))

        Dim thermostatDevice As Integer = Convert.ToInt32(parameters(0))
        temperatureDevices = parameters(1).Split(Convert.ToChar(";"))
        heatDevices = parameters(2).Split(Convert.ToChar(";"))

        wantedTemperature = hs.DeviceValueEx(thermostatDevice)
        currentTemperature = GetAverageTemperature(temperatureDevices)
        temperatureDifference = wantedTemperature - currentTemperature

        If Math.Abs(temperatureDifference) > hysteresis Then
            Select Case temperatureDifference
                Case Is < 0 ' Current temperature is lower than wanted temperature
                    TurnOnDevices(heatDevices)
                Case Is > 0 ' Current temperature is higher than wanted temperature
                    TurnOffDevices(heatDevices)
            End Select
        End If

    End Sub
    ''' <summary>
    ''' Turn on devices
    ''' </summary>
    ''' <param name="devices">Array of device references</param>
    Private Sub TurnOnDevices(ByVal devices As String())
        SetDeviceStatus(devices, "on")
    End Sub
    ''' <summary>
    ''' Turn off devices
    ''' </summary>
    ''' <param name="devices">Array of device references</param>
    Private Sub TurnOffDevices(ByVal devices As String())
        SetDeviceStatus(devices, "off")
    End Sub
    ''' <summary>
    ''' Turn devices on or off
    ''' </summary>
    ''' <param name="devices">Array of device references</param>
    ''' <param name="status">"on" or "off"</param>
    Private Sub SetDeviceStatus(ByVal devices As String(), ByVal status As String)
        For Each device As Integer In devices
            hs.CAPIControlHandler(hs.CAPIGetSingleControl(device, True, status, False, True))
        Next
    End Sub
    ''' <summary>
    ''' Calculates average temperature from multiple temperature sensors
    ''' </summary>
    ''' <param name="devices">Array of device references</param>
    ''' <returns>Average temperature</returns>
    Private Function GetAverageTemperature(ByVal devices As String()) As Double
        Dim temperature As Double
        For Each device As Integer In devices
            temperature = temperature + hs.DeviceValueEx(device)
        Next
        Return temperature / devices.Length
    End Function
End Class
