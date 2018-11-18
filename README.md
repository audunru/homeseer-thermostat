# homeseer-thermostat
Turn heating devices on or off based on wanted temperature and current temperature

## Setup in Homeseer

After you've added the script to your scripts folder, set up a new event.

In this example, I trigger the script when the temperature in the room changes OR the thermostat changes

The parameters I use here are separated with | and ;

345: Thermostat. This device returns the temperature I want the room to have.

334: Thermometer. This device returns the temperature the room currently has. You can use ; to specify multiple thermometers. In that case, the average temperature will be used.

248;234;111: Heaters. You can use one or more, separate with ;

I've set the script to not run again for at least 5 minutes to avoid heaters switching on/off rapidly.

![alt text](https://raw.githubusercontent.com/audunru/homeseer-thermostat/master/event-setup.png)
